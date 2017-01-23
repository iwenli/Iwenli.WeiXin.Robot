using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iwenli.WeiXin.Robot.Messages;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Handlers
{
    class PicHandler : IHandler
    {
        private string RequestXml { set; get; }

        public PicHandler(string requestXml)
        {
            this.RequestXml = requestXml;
        }

        public string HandleRequest()
        {
            LogHelper.CreateLogTxt("图片请求： " + RequestXml);
            string response = string.Empty;
            //原图发回  具体业务具体处理
            PicMessage pm = PicMessage.LoadFromXml(this.RequestXml);
            string temp = pm.FromUserName;
            pm.FromUserName = pm.ToUserName;
            pm.ToUserName = temp; 
            response = pm.GenerateContent();

            LogHelper.CreateLogTxt("图片响应： " + response);
            return response;
        }
    }
}
