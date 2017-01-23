using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Iwenli.WeiXin.Robot.Messages
{
    /// <summary>
    /// 文本处理
    /// </summary>
    public class TextMessage : Message
    {
        private static string m_Template;
        public string Content { get; set; }
        public string MsgId { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        public override string Template
        {
            get
            {
                if (string.IsNullOrEmpty(m_Template))
                {
                    LoadTemplate();
                }
                return m_Template;
            }
        }

        public TextMessage()
        {
            this.MsgType =  MessageType.TEXT;
        }

        /// <summary>
        /// 从xml数据加载文本信息
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static TextMessage LoadFromXml(string xml)
        {
            TextMessage tm = null;
            if (!string.IsNullOrEmpty(xml))
            {
                XElement element = XElement.Parse(xml);
                if (element != null)
                {
                    tm = new TextMessage();
                    tm.FromUserName = element.Element(Common.FROM_USERNAME).Value;
                    tm.ToUserName = element.Element(Common.TO_USERNAME).Value;
                    tm.CreateTime = element.Element(Common.CREATE_TIME).Value;
                    tm.Content = element.Element(Common.CONTENT).Value;
                    tm.MsgId = element.Element(Common.MSG_ID).Value;
                }
            }
            return tm;
        } 
        
        /// <summary>
        ///   生成内容
        /// </summary>
        /// <returns></returns>
        public override string GenerateContent()
        {
            this.CreateTime = Common.GetTimeStamp();
            return string.Format(this.Template, this.ToUserName, this.FromUserName, this.CreateTime, this.MsgType.ToString().ToLower(), this.Content, this.MsgId);
        }
        /// <summary>
        /// 加载模板
        /// </summary>
        private static void LoadTemplate()
        {
            m_Template = @"<xml>
                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                <CreateTime>{2}</CreateTime>
                                <MsgType><![CDATA[{3}]]></MsgType>
                                <Content><![CDATA[{4}]]></Content>
                                <MsgId>{5}</MsgId>
                            </xml>";
        }
    }
}
