using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSLib.Network.Http;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot
{
    class Context
    {
        private static HttpClient client = new HttpClient(); 
        private static DateTime GetAccessToken_Time;
        private static int Expires_Period = 7200;
        private static string _AccessToken = string.Empty;
        private static string appID = "wxfbd56d884d96297c";
        private static string appsecret = "a92a1aeb7f16d336dfb74473fcd0f3b5";

        /// <summary>
        /// 微信access_token，2小时更新一次
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
            string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appID, appsecret);

            AccessTokenStruct responseJson = client.Create<AccessTokenStruct>(HttpMethod.Get, url).Send().Result;
            if (!string.IsNullOrEmpty(responseJson.access_token))
            {
                GetAccessToken_Time = DateTime.Now;
                Expires_Period = int.Parse(responseJson.expires_in);
                LogHelper.CreateLogTxt("【请求access_token成功】：access_token=" + responseJson.access_token);
                return responseJson.access_token;
            }
            else
            {
                GetAccessToken_Time = DateTime.MinValue;
                LogHelper.CreateLogTxt("【请求access_token失败】：errmsg=" + responseJson.errmsg + "  errcode=" + responseJson.errcode);
                return string.Empty;
            }
        } 

        #region 内部类
        /// <summary>
        /// access_token数据json类
        /// </summary>
        class AccessTokenStruct
        {
            public string access_token { set; get; }
            public string expires_in { set; get; }
            public string errcode { set; get; }
            public string errmsg { set; get; }
        }
        #endregion
    }
}
