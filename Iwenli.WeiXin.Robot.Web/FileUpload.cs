using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Iwenli.WeiXin.Robot.Web
{
    public class FileUpload : IHttpHandler
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此处理程序 
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //在此处写入您的处理程序实现。

            //接口内容换行用####实现，返回值{"errcode":0,"errmsg":"ok"}  正确时返回值errcode为0
            //请求示例：www.iwenli.org/api/fileUpload.axd?file_directory=vip_userInfo

            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            context.Response.Charset = "UTF-8";
            if (context.Request.Files.Count < 1)
            {
                context.Response.Write(GetReturnJson(1, "文件不能为空."));
            }
            else
            {
                //上传文件
                HttpPostedFile file = context.Request.Files[0];
                //获取文件名字
                string fileName = Path.GetFileName(file.FileName);
                //获取文件的扩展名
                string fileExt = Path.GetExtension(fileName);

                string fileDirectory = "default";
                if (context.Request.Params["file_directory"] != null)
                {
                    fileDirectory = context.Request.Params["file_directory"];
                }
                string rootPathpath = System.Web.HttpRuntime.AppDomainAppPath.ToString();
                string pathFormat = "Data\\Upload\\{0}\\{1}\\{2}\\{3}\\";
                string path = rootPathpath + string.Format(pathFormat, fileDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                List<string> ExtList = new List<string>(new string[] { ".txt" });
                if (ExtList.Contains(fileExt))
                {
                    Directory.CreateDirectory(path);
                    string newfileName = Guid.NewGuid().ToString() + "_" + fileName;
                    string fullDir = path + newfileName;
                    file.SaveAs(fullDir);
                    context.Response.Write(GetReturnJson());
                }
                else
                {
                    context.Response.Write(GetReturnJson(2, "文件上传类型不支持."));
                }
            }
            context.Response.End();
        }

        public string GetReturnJson(int code = 0, string msg = "ok")
        {
            return "{\"errcode\":" + code.ToString() + ",\"errmsg\":\"" + msg + "\"}";
        }

        #endregion
    }
}