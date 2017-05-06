using Iwenli.WeiXin.Robot.Utility;
using System;
using System.Text;
using System.Web;

namespace Iwenli.WeiXin.Robot.Web
{
    public class WriteLog : IHttpHandler
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
            //请求示例：www.iwenli.org/api/log.axd?file_directory=vip_video&content=hello
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            context.Response.Charset = "UTF-8";

            string fileDirectory = "default";
            if (context.Request.Params["file_directory"] != null)
            {
                fileDirectory = context.Request.Params["file_directory"];
            }

            string request = context.Request.Params["content"];
            if (request != null)
            {
                string pathFormat = "Data\\Log\\{0}\\{1}\\{2}\\{3}\\{4}.log";
                string path = string.Format(pathFormat, fileDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.ToString("HH"));
                StringBuilder writeSB = new StringBuilder();
                writeSB.Append("\r\n");
                writeSB.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]\r\n");
                writeSB.Append(request.Replace("####", "\r\n"));
                writeSB.Append("\r\n");
                FileHelper.WriteFile(path, writeSB.ToString());
                context.Response.Write(GetReturnJson());
            }
            else
            {
                context.Response.Write(GetReturnJson(1, "content不能为空."));
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
