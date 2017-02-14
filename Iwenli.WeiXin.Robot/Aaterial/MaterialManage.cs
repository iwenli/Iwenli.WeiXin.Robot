using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using FSLib.Network.Http;
using Iwenli.WeiXin.Robot.Utility;
using Newtonsoft.Json;

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



        /// <summary>
        /// 上传临时素材的格式、大小限制与公众平台官网一致。
        ///  图片（image）: 2M，支持PNG\JPEG\JPG\GIF格式
        ///  语音（voice）：2M，播放长度不超过60s，支持AMR\MP3格式
        ///  视频（video）：10MB，支持MP4格式
        ///  缩略图（thumb）：64KB，支持JPG格式
        /// </summary>
        /// <param name="mateialFile">上传文件流</param>
        /// <param name="mt">文件类型</param>
        /// <returns>成功返回media_id,失败返回String.Empty</returns>
        public static string UploadTemporaryMaterial(byte[] mateialFile, MaterialType mt)
        {
            string media_id = string.Empty;
            string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", Context.AccessToken, mt.ToString());

            string filename = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            switch (mt)
            {
                case MaterialType.image:
                    filename += "-imgage.jpg";
                    break;
                case MaterialType.voice:
                    filename += "-voice.amr";
                    break;
                case MaterialType.video:
                    filename += "-video.mp4";
                    break;
                case MaterialType.thumb:
                    filename += "-thumb.jpg";
                    break;
                default:
                    break;
            }
            /*
             * method 1 
            WebClient wxUpload = new WebClient();
            string path = System.Web.HttpRuntime.AppDomainAppPath.ToString() + "temp\\1.jpg";
            //API所需的媒体信息
            //wxUpload.Headers.Add("Content-Type", "application/octet-stream");
            //wxUpload.Headers.Add("filename", "1.jpg");
            //wxUpload.Headers.Add("filelength", mateialFile.Length.ToString());
            byte[] result =
                wxUpload.UploadFile(
                    new Uri(url), path);
            string resultjson = Encoding.UTF8.GetString(result); //在这里获取json数据，获得图片URL
            */

            /*
             * method 2
             */
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            //请求头部信息
            StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", filename));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            Stream postStream = request.GetRequestStream();
            //postStream.Write(b, 0, b.Length);
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(mateialFile, 0, mateialFile.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream instream = response.GetResponseStream();
            StreamReader sr = new StreamReader(instream, Encoding.UTF8);
            //返回结果网页（html）代码

            string content = sr.ReadToEnd();
            UploadTemporaryMaterialResult utm = JsonConvert.DeserializeObject<UploadTemporaryMaterialResult>(content);
            if (string.IsNullOrEmpty(utm.errcode))
            {
                //成功
                media_id = utm.media_id;

                StringBuilder sb = new StringBuilder();
                sb.Append("上传临时素材成功.");
                sb.Append("\r\ntype = " + utm.type);
                sb.Append("\r\nmedia_id = " + utm.media_id);
                sb.Append("\r\ncreated_at = " + utm.created_at);
                LogHelper.CreateLogTxt(sb.ToString());
            }
            else
            {
                //失败 
                //记录异常
                StringBuilder sb = new StringBuilder();
                sb.Append("上传临时素材失败.");
                sb.Append("\r\nerrcode = " + utm.errcode);
                sb.Append("\r\nerrmsg = " + utm.errmsg);
                LogHelper.CreateLogTxt(sb.ToString());
            }

            return media_id;
        }


    }

    internal class UploadTemporaryMaterialResult
    {
        public string type { get; set; }
        public string media_id { get; set; }
        public string created_at { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }
}
