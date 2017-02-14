using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iwenli.WeiXin.Robot.Messages;
using Iwenli.WeiXin.Robot.Api;
using Iwenli.WeiXin.Robot.Utility;
using System.Text.RegularExpressions;

namespace Iwenli.WeiXin.Robot.Handlers
{
    /// <summary>
    /// 文本信息处理类
    /// </summary>
    public class TextHandler : IHandler
    {
        #region 属性
        private string RequestXml { get; set; }
        #endregion

        #region 构造函数
        public TextHandler(string requestXml)
        {
            this.RequestXml = requestXml;
        }
        #endregion

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <returns></returns>
        public string HandleRequest()
        {
            LogHelper.CreateLogTxt("文本请求： " + RequestXml);
            TextMessage tm = TextMessage.LoadFromXml(RequestXml);
            string response = string.Empty;

            string content = tm.Content.Trim(); 
            tm.Content = HandleCommon.AutoResponseText(tm.FromUserName, content); 

            //进行发送者接受者转换
            string temp = tm.ToUserName;
            tm.ToUserName = tm.FromUserName;
            tm.FromUserName = temp;
            response = tm.GenerateContent();
            LogHelper.CreateLogTxt("文本响应： " + response);
            return response;
        }
    }
}
