using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSLib.Network.Http;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Api.Baidu
{
    /// <summary>
    /// 百度语言apiContext
    /// </summary>
    class Context
    {
        private static HttpClient client = new HttpClient();
        private static DateTime GetAccessToken_Time;
        private static int Expires_Period = 86400;
        private static string _AccessToken = string.Empty;
        //private static string AppID = "9205973";
        private static string ApiKey = "wce81vMLGMVLLMKKw8Mgw97Q";
        private static string SecretKey = "3d21293ef8df5c6af919e7c46dc42f41";
        //private static string GetAccessTokenurl = @"https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}";
        private static string GetAccessTokenurl = @"https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}";



        /// <summary>
        /// 百度语言API
        /// access_token，有效期30天
        /// </summary>
        public static string AccessToken
        {
            get
            {
                //如果为空  或者 过期  需要重新设置
                if (string.IsNullOrEmpty(_AccessToken) || HasExpired())
                {
                    _AccessToken = GetAccessToken();
                }
                return _AccessToken;
            }
        }

        /// <summary>
        /// 判断是否过期
        /// </summary>
        /// <returns></returns>
        private static bool HasExpired()
        {
            bool ret = false;
            if (GetAccessToken_Time != null)
            {
                //过期时间，允许有一定的误差。获取时间消耗
                if (DateTime.Now > GetAccessToken_Time.AddSeconds(Expires_Period - 60))
                {
                    ret = true;
                }
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetAccessToken()
        {
            AccessTokenStruct responseJson = client.Create<AccessTokenStruct>(HttpMethod.Post, string.Format(GetAccessTokenurl,ApiKey,SecretKey)).Send().Result;
            if (!string.IsNullOrEmpty(responseJson.access_token))
            {
                GetAccessToken_Time = DateTime.Now;
                Expires_Period = int.Parse(responseJson.expires_in);
                LogHelper.CreateLogTxt("【百度access_token成功】：access_token=" + responseJson.access_token);
                return responseJson.access_token;
            }
            else
            {
                GetAccessToken_Time = DateTime.MinValue;
                LogHelper.CreateLogTxt("百度access_token获取失败。"); 
                return string.Empty;
            }
        }

        #region 内部类
        /// <summary>
        /// access_token结果类
        /// </summary>
        internal class AccessTokenStruct
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string session_key { get; set; }
            public string session_secret { get; set; }
            public string error { get; set; }
            public string error_description { get; set; }
        }
        #endregion
    }
}
