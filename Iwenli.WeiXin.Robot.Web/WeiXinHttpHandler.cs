using System.IO;
using Iwenli.WeiXin.Robot.Handlers;
using Iwenli.WeiXin.Robot.Utility;

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

            //if (context.Request.QueryString.Count > 0)
            //{
            //    string text = context.Request.Params["text"].ToString();
            //    text = string.IsNullOrEmpty(text) ? "明天你好。我是张玉龙。" : text;
            //    byte[] b = Api.Baidu.VoiceRest.TextToVoice(text, per: 1);

            //    string mp3Path = string.Format("temp\\{0}.mp3", text);
            //    string amrPath = string.Format("temp\\{0}.amr", text);
            //    Robot.Utility.FileHelper.WriteFile(mp3Path, b);
            //    AudioConvertToAmr.ConvertToAmr(mp3Path, amrPath);

            //    byte[] p = FileHelper.ReadFileBytes(amrPath);
            //    //string p = System.Web.HttpRuntime.AppDomainAppPath.ToString() + "temp\\1.amr"; 
            //    //byte[] b = File.ReadAllBytes(p);
            //    Iwenli.WeiXin.Robot.Aaterial.MaterialManage.UploadTemporaryMaterial(p, Aaterial.MaterialType.voice);
            //}
             
             

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
