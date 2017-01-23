using Iwenli.WeiXin.Robot.Handlers;

namespace Iwenli.WeiXin.Robot.Web
{
    public class WeiXinHttpHandler : System.Web.IHttpHandler
    {
        /// <summary>
        /// 您将需要在您网站的 web.config 文件中配置此处理程序，
        /// 并向 IIS 注册此处理程序，然后才能进行使用。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(System.Web.HttpContext context)
        {
            //VoiceHandler vh = new VoiceHandler(@"<xml><ToUserName><![CDATA[gh_b07eda6c5e91]]></ToUserName>
            //                                        <FromUserName><![CDATA[oWqQMwsAintKpK13GVFhktlF-e6c]]></FromUserName>
            //                                        <CreateTime>1485084344</CreateTime>
            //                                        <MsgType><![CDATA[voice]]></MsgType>
            //                                        <MediaId><![CDATA[RIZ9I3DAkCmYO4QTZSamQo4jMnZYWyrix-aXrrIUChjRtl8L_9tmX2QYVLrSQlnH]]></MediaId>
            //                                        <Format><![CDATA[amr]]></Format>
            //                                        <MsgId>6378388689691872444</MsgId>
            //                                        <Recognition><![CDATA[]]></Recognition>
            //                                        </xml>");
            //vh.HandleRequest();

            //string MediaId = "vNMOpzylGw52bJm2VrpEIa0x_Y-NKkqev3XUEunpAB505NOGQd207z4spUi-J7yh";
            //byte[] speech = Aaterial.MaterialManage.GetTemporaryMaterial(MediaId);
            //System.Collections.Generic.List<string> textRreult = Api.Baidu.VoiceRest.VoiceToText(speech);
            

            //由微信服务接受请求，处理具体请求
            WeiXinService wxService = new WeiXinService(context.Request);
            string responseMsg = wxService.Response();
            context.Response.Clear();
            context.Response.Charset = "UTF-8";
            context.Response.Write(responseMsg);
            context.Response.End();
        }

        #endregion
    }
}
