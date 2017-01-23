using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Messages
{
    class VoiceMessage : Message
    {
        private static string _Template;
        public override string Template
        {
            /* 
             * 接受到的xml 
            <xml>
            <ToUserName><![CDATA[gh_b07eda6c5e91]]></ToUserName>
            <FromUserName><![CDATA[oWqQMwsAintKpK13GVFhktlF-e6c]]></FromUserName>
            <CreateTime>1485076170</CreateTime>
            <MsgType><![CDATA[voice]]></MsgType>
            <MediaId><![CDATA[LrBlYVC1ATsFvIl9d7Ddhun1VWKrKeTq0OcARWODauf18RP-SXTHzp2k5qyXVjvT]]></MediaId>
            <Format><![CDATA[amr]]></Format>
            <MsgId>6378353582629192027</MsgId>
            <Recognition><![CDATA[]]></Recognition>
            </xml>
            */
            get
            {
                if (String.IsNullOrEmpty(_Template))
                {
                    _Template = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[{3}]]></MsgType>
                                    <Voice>
                                    <MediaId><![CDATA[{4}]]></MediaId>
                                    </Voice>
                                    </xml>";
                }
                return _Template;
            }
        }
        public string MediaId { set; get; }
        public string Format { set; get; }
        public string MsgId { get; set; }

        public VoiceMessage()
        {
            this.MsgType = MessageType.VOICE;
        }
        public static VoiceMessage LoadFromXml(string xml)
        {
            VoiceMessage vm = null;
            try
            {
                if (!string.IsNullOrEmpty(xml))
                {
                    XElement element = XElement.Parse(xml);
                    if (element != null)
                    {
                        vm = new VoiceMessage();
                        vm.FromUserName = element.Element(Common.FROM_USERNAME).Value;
                        vm.ToUserName = element.Element(Common.TO_USERNAME).Value;
                        vm.CreateTime = element.Element(Common.CREATE_TIME).Value;
                        vm.Format = element.Element(Common.FORNAT).Value;
                        vm.MediaId = element.Element(Common.MEDIAID).Value;
                        vm.MsgId = element.Element(Common.MSG_ID).Value; 
                    }
                }
            }
            catch (Exception)
            {
                LogHelper.CreateLogTxt("语音xml转对象失败");
            }

            return vm;
        }
        public override string GenerateContent()
        {
            this.CreateTime = Common.GetTimeStamp();
            return string.Format(this.Template, this.ToUserName, this.FromUserName, this.CreateTime,
                this.MsgType.ToString().ToLower(), this.MediaId);
        }
    }
}
