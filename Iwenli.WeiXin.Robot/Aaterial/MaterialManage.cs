using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSLib.Network.Http;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Aaterial
{
    /// <summary>
    /// 素材管理
    /// </summary>
    public class MaterialManage
    {
        private static HttpClient client = new HttpClient();


        public MaterialManage()
        {

        }

        /// <summary>
        /// 获取临时素材
        /// </summary>
        /// <param name="media_id">素材id</param>
        /// <returns></returns>
        public static byte[] GetTemporaryMaterial(string media_id)
        {
            LogHelper.CreateLogTxt("开始获取素材");
            string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", Context.AccessToken, media_id);
            LogHelper.CreateLogTxt("素材url=" + url);
            byte[] ret = null;
            try
            {
                var response = client.Create<byte[]>(HttpMethod.Get, url).Send();
                if (response.Response.Headers.Get("Content-Type") == "audio/amr")
                {
                    ret = response.Result;
                    LogHelper.CreateLogTxt("【获取临时素材成功】");
                }
            }
            catch (Exception)
            {
                LogHelper.CreateLogTxt("获取临时素材失败。");
            }
            return ret;
        }
    }
}
