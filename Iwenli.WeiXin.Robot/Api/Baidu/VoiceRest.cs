using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSLib.Network.Http; 
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Api.Baidu
{
    /// <summary>
    /// 百度语音识别接口
    /// </summary>
    public class VoiceRest
    {
        private const string ApiUrl = "http://vop.baidu.com/server_api";
        private const string Cuid = "iwenli_9205973";

        public VoiceRest() { }

        /// <summary>
        /// 语音转文字
        /// </summary>
        /// <param name="voice">语音字节流</param>
        /// <param name="format">语音格式</param>
        /// <returns>识别结果数组，提供1-5 个候选结果，string 类型为识别的字符串， utf-8 编码</returns>
        public static List<string> VoiceToText(byte[] voice, string format = "amr")
        {
            List<string> result = null;
            HttpClient client = new HttpClient();

            try
            {
                var send = client.Create<VoiceToTextResult>(HttpMethod.Post, ApiUrl, data: new
                {
                    format = format,    //语音压缩的格式，请填写上述格式之一，不区分大小写
                    rate = 8000,        //采样率，支持 8000 或者 16000
                    channel = 1,        //声道数，仅支持单声道，请填写 1
                    token = Context.AccessToken,
                    cuid = Cuid,        //用户唯一标识，用来区分用户，填写机器 MAC 地址或 IMEI 码，长度为60以内
                    len = voice.Length,       //选填  原始语音长度，单位字节
                    speech = Convert.ToBase64String(voice)      //选填  真实的语音数据 ，需要进行base64 编码
                }, contentType: ContentType.Json).Send();

                VoiceToTextResult temp = send.Result;
                if (temp.err_msg.Equals("success."))  //识别成功
                {
                    result = temp.result;
                    LogHelper.CreateLogTxt("百度语言识别成功，结果如下：\r\n" + result.Join("\r\n"));
                }
                else
                {
                    //识别失败
                    result = new List<string>();
                    result.Add("你说的话太难听了，小图听不出来呢O(∩_∩)O.");
                    //记录异常
                    StringBuilder sb = new StringBuilder();
                    sb.Append("百度语言识别无结果.");
                    sb.Append("\r\nerr_no = " + temp.err_no);
                    sb.Append("\r\nerr_msg = " + temp.err_msg);
                    sb.Append("\r\nsn = " + temp.sn);
                    LogHelper.CreateLogTxt(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LogHelper.CreateEerrorLogTxt(ex, "百度语言识别异常啦.");
            }
            return result;
        }

        #region 内部类
        public class VoiceToTextResult
        {
            public string corpus_no { get; set; }
            public string err_msg { get; set; }
            public int err_no { get; set; }
            public List<string> result { get; set; }
            public string sn { get; set; }
        }
        #endregion
    }
}
