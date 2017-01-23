using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Iwenli.WeiXin.Robot.Messages
{
    /// <summary>
    /// 图片处理
    /// </summary>
    class PicMessage : Message
    {
        private static string _Template = string.Empty;
        public override string Template
        {
            get
            {
                if (String.IsNullOrEmpty(_Template))
                {
                    /*
                     * 接受到的XML格式
                     * _Template = @"<xml>
                                     <ToUserName><![CDATA[{0}]]></ToUserName>
                                     <FromUserName><![CDATA[{1}]]></FromUserName>
                                     <CreateTime>{2}</CreateTime>
                                     <MsgType><![CDATA[{3}]]></MsgType>
                                     <PicUrl><![CDATA[{4}]]></PicUrl>
                                     <MediaId><![CDATA[{5}]]></MediaId>
                                     <MsgId>{6}</MsgId>
                                     </xml>";*/

                    _Template = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[{3}]]></MsgType>
                                    <Image>
                                    <MediaId><![CDATA[{4}]]></MediaId>
                                    </Image>
                                    </xml>";
                }
                return _Template;
            }
        }
        public string PicUrl { get; set; }
        public string MediaId { get; set; }
        public string MsgId { get; set; }

        public PicMessage()
        {
            this.MsgType = MessageType.IMAGE;
        }

        /// <summary>
        /// 从xml数据加载文本信息
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static PicMessage LoadFromXml(string xml)
        {
            PicMessage pm = null;
            if (!string.IsNullOrEmpty(xml))
            {
                XElement element = XElement.Parse(xml);
                if (element != null)
                {
                    pm = new PicMessage();
                    pm.FromUserName = element.Element(Common.FROM_USERNAME).Value;
                    pm.ToUserName = element.Element(Common.TO_USERNAME).Value;
                    pm.CreateTime = element.Element(Common.CREATE_TIME).Value;
                    pm.PicUrl = element.Element(Common.PICURL).Value;
                    pm.MediaId = element.Element(Common.MEDIAID).Value;
                    pm.MsgId = element.Element(Common.MSG_ID).Value;
                }
            }
            return pm;
        }

        /// <summary>
        ///   生成内容
        /// </summary>
        /// <returns></returns>
        public override string GenerateContent()
        {
            this.CreateTime = Common.GetTimeStamp();
            return string.Format(this.Template, this.ToUserName, this.FromUserName, this.CreateTime,
                this.MsgType.ToString().ToLower(), this.MediaId);
        }
    }
}
