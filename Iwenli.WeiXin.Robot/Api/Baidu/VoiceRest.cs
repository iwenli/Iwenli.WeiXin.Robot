using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using FSLib.Network.Http;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Api.Baidu
{
    /// <summary>
    /// 百度语音识别接口
    /// </summary>
    public class VoiceRest
    {
        private const string ApiUrl2Text = "http://vop.baidu.com/server_api";
        private const string ApiUrl2Voice = "http://tsn.baidu.com/text2audio";
        private const string Cuid = "iwenli_9205973";
        private static HttpClient client = new HttpClient();

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
            try
            {
                var send = client.Create<VoiceToTextResult>(HttpMethod.Post, ApiUrl2Text, data: new
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

        /// <summary>
        /// 文字合成语言
        /// </summary>
        /// <param name="text">待合成的文字</param>
        /// <param name="lan">选填 | >中文（zh）、粤语（ct）、英文（en），默认为zh</param>
        /// <param name="spd">选填 | 语速，取值0 - 9，默认为5中语速</param>
        /// <param name="pit">选填 | 音调，取值0 - 9，默认为5中语调</param>
        /// <param name="vol">选填 | 音量，取值0 - 9，默认为5中音量</param>
        /// <param name="per">选填 | 发音人选择，取值0 - 1, 0为女声，1为男声，默认为女声</param
        /// <returns></returns>
        public static byte[] TextToVoice(string text, string lan = "zh", int spd = 5, int pit = 5, int vol = 5, int per = 0)
        {
            byte[] result = null;
            string data = string.Format("tex={0}&lan={1}&cuid={2}&ctp={3}&tok={4}&spd={5}&pit={6}&vol={7}&per={8}", text, lan, Cuid, 1, Context.AccessToken, spd, pit, vol, per);
            var context = client.Create<byte[]>(HttpMethod.Post, ApiUrl2Voice, data: data, contentType: ContentType.Json);
            context.Send();
            if (context.Response.Headers.Get("Content-Type").Equals("audio/mp3"))
            {
                //成功
                result = context.Result;
            }
            else
            {
                LogHelper.CreateLogTxt("调用百度接口转语音  -  失败");
                //失败
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
