using System;
using System.Text;
using System.Web;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Web
{
    public class VIPVideo : IHttpHandler
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
            string api = @"https://api.47ks.com/webcloud/?v=";
            string request = context.Request.Params["_"];
            if (request != null)
            {
                string response = HttpUtility.UrlEncode(api + request);
                string requestMsg = context.Request.Params["msg"];
                if (requestMsg != null)
                {
                    string pathFormat = "Data\\Log\\{0}\\{1}\\{2}\\{3}\\{4}.log";
                    string path = string.Format(pathFormat, "vipAPI", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                        DateTime.Now.ToString("HH"));

                    StringBuilder writeSB = new StringBuilder();
                    writeSB.Append("\r\n");
                    writeSB.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]\r\n");
                    writeSB.Append(string.Format("请求地址:{0}\r\n", response));
                    writeSB.Append(requestMsg.Replace("local-host", "\r\n"));
                    writeSB.Append("\r\n");
                    FileHelper.WriteFile(path, writeSB.ToString());
                }

                context.Response.Clear();
                context.Response.Charset = "UTF-8";
                context.Response.Write(response);
                context.Response.End();
            }

        }
        #endregion
    }
}
