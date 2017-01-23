using Iwenli.WeiXin.Robot.Utility;
using Newtonsoft.Json;
using FSLib.Network.Http;
using System.Collections.Generic;

namespace Iwenli.WeiXin.Robot.Api
{
    public class TuLingRobot
    {
        private const string TULIN_API_KEY = "b6bd12a330bb4d3b8f776a7998865333";
        private const string TULING_API_URL = "http://www.tuling123.com/openapi/api";
        private const string TULING_API_SECRET = "06b06c2ec0749851";   //开启发送数据需要加密

        private static HttpClient client = new HttpClient();

        public TuLingRobot() { }

        public static string Say(string userid, string info)
        {
            return Say(userid, info, string.Empty);
        }
        public static string Say(string userid, string info, string loc)
        {
            string retMsg = string.Empty;
            userid = SecurityUtility.Md5Encode(userid); //对userid进行MD5 去除特殊字符
            string strArgs = EncryptionInfo(JsonConvert.SerializeObject(new Info(TULIN_API_KEY, info, loc, userid)));

            ResponseJsonData responseJson = client.Create<ResponseJsonData>(HttpMethod.Post, TULING_API_URL, data: strArgs,
                contentType: ContentType.Json).Send().Result;
            switch (responseJson.code)
            {
                case "100000":// 文本类
                    {
                        //code 链接类标识码
                        //text 提示语
                        retMsg = responseJson.text;
                        break;
                    }
                case "200000":// 链接类
                    {
                        //code 链接类标识码
                        //text 提示语
                        //url 链接地址
                        retMsg = responseJson.text + responseJson.url;
                        break;
                    }
                case "302000":// 新闻类
                    {
                        retMsg = responseJson.text;
                        //code 新闻类标识码
                        //text 提示语
                        //list 信息列表
                        //article 新闻标题
                        //source 新闻来源
                        //icon 新闻图片
                        //detailurl 新闻详情链接
                        break;
                    }
                case "308000":// 菜谱类
                    {
                        retMsg = responseJson.text;
                        //code 菜谱类标识码
                        //text 提示语
                        //list
                        //name 菜名
                        //info 菜谱信息
                        //detailurl 详情链接
                        //icon 信息图标
                        break;
                    }
                default:
                    break;
            }
            return retMsg;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="jsonParam">待传输信息的json字符串</param>
        /// <returns>返回json可传送数据</returns>
        private static string EncryptionInfo(string jsonParam)
        {
            string secret = TULING_API_SECRET;
            string timestamp = Common.GetTimeStamp(false);
            string apiKey = TULIN_API_KEY;

            string keyParam = secret + timestamp + apiKey;
            string aesKey = SecurityUtility.Md5Encode(keyParam);
            string data = SecurityUtility.EncryptAES(jsonParam, aesKey);
            return JsonConvert.SerializeObject(new PostJosnData(apiKey, timestamp, data));
        }

        #region 内部类

        /// <summary>
        /// 请求信息内部类
        /// </summary>
        internal class Info
        {
            public string key { get; set; }
            public string info { get; set; }
            public string loc { get; set; }
            public string userid { get; set; }
            public Info(string key, string info, string loc, string userid)
            {
                this.key = key;
                this.info = info;
                this.loc = loc;
                this.userid = userid;
            }
            public Info(string key, string info, string userid) : this(key, info, string.Empty, userid)
            { }
            public Info(string key, string info) : this(key, info, string.Empty, string.Empty)
            { }
            public Info() { }
        }
        /// <summary>
        /// 加密请求数据内部类
        /// </summary>
        internal class PostJosnData
        {
            public string key { get; set; }
            public string timestamp { get; set; }
            public string data { get; set; }
            public PostJosnData(string key, string timestamp, string data)
            {
                this.key = key;
                this.timestamp = timestamp;
                this.data = data;
            }
        }
        /// <summary>
        /// 返回结果内部类
        /// </summary>
        internal class ResponseJsonData
        {
            /// <summary>
            /// 标识码
            /// </summary>
            public string code { set; get; }
            /// <summary>
            /// 提示信息
            /// </summary>
            public string text { set; get; }
            /// <summary>
            /// 连接地址
            /// </summary>
            public string url { set; get; }
            /// <summary>
            /// 新闻类和菜谱类List
            /// </summary>
            public ResponseJsonDataListAttr list { set; get; }
            public ResponseJsonData() { }
        }
        /// <summary>
        /// 菜谱 和  新闻集合字段
        /// </summary>
        internal class ResponseJsonDataListAttr
        {
            /// <summary>
            /// 新闻标题
            /// </summary>
            public string article { set; get; }
            /// <summary>
            /// 新闻来源
            /// </summary>
            public string source { set; get; }
            /// <summary>
            /// 菜名
            /// </summary>
            public string name { set; get; }
            /// <summary>
            /// 菜谱信息
            /// </summary>
            public string info { set; get; }
            /// <summary>
            /// 新闻图片 | 菜谱信息图标
            /// </summary>
            public string icon { set; get; }
            /// <summary>
            /// 新闻详情链接 | 菜谱详情链接
            /// </summary>
            public string detailurl { set; get; }
            public ResponseJsonDataListAttr() { }
        }
        #endregion

    }
}
