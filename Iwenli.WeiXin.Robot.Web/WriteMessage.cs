using System;
using System.IO;
using System.Text;
using System.Web;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Web
{
    public class WriteMessage : IHttpHandler
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
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            context.Response.Charset = "UTF-8";

            string request = context.Request.Params["msg"];
            if (request != null)
            {
                string pathFormat = "Data\\Log\\{0}\\{1}\\{2}\\{3}\\{4}.log";
                string path = string.Format(pathFormat, "qq", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.ToString("HH"));
                StringBuilder writeSB = new StringBuilder();
                writeSB.Append("\r\n");
                writeSB.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]\r\n");
                writeSB.Append(request.Replace("local-host", "\r\n"));
                writeSB.Append("\r\n");
                FileHelper.WriteFile(path, writeSB.ToString());
                context.Response.Write("{ \"Statu\" : \"1\" }");
            }
            else
            {
                context.Response.Redirect("http://www.baidu.com");
            }
            context.Response.End();
        }

        #endregion
    }
}
