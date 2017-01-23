using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Messages
{
    class EventMessage : Message
    {
        
        ///// <summary>
        ///// 
        ///// </summary>
        //private static string mTemplate;

        ///// <summary>
        ///// 模板
        ///// </summary>
        //public override string Template
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(mTemplate))
        //        {
        //            mTemplate = @"<xml>
        //                        <ToUserName><![CDATA[{0}]]></ToUserName>
        //                        <FromUserName><![CDATA[{1}]]></FromUserName>
        //                        <CreateTime>{2}</CreateTime>
        //                        <MsgType><![CDATA[event]]></MsgType>
        //                        <Event><![CDATA[{3}]]></Event>
        //                        <EventKey>{4}</EventKey>
        //                    </xml>";
        //        }

        //        return mTemplate;
        //    }
        //}
        /// <summary>
        /// 事件类型
        /// </summary>
        public EventType? Event { get; set; }
        /// <summary>
        /// 事件KEY值，与自定义菜单接口中KEY值对应
        /// </summary>
        public string EventKey { get; set; }
        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 地理位置精度
        /// </summary>
        public string Precision { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EventMessage()
        {
            this.MsgType = MessageType.EVENT;
        }

        /// <summary>
        /// 从xml数据加载文本消息
        /// </summary>
        /// <param name="xml"></param>
        public static EventMessage LoadFromXml(string xml)
        {
            EventMessage em = null;

            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    XElement element = XElement.Parse(xml);

                    if (element != null)
                    {
                        em = new EventMessage();
                        em.FromUserName = element.Element(Common.FROM_USERNAME).Value;
                        em.ToUserName = element.Element(Common.TO_USERNAME).Value;
                        em.CreateTime = element.Element(Common.CREATE_TIME).Value;
                        em.Event = (EventType)Enum.Parse(typeof(EventType), element.Element(Common.EVENT).Value);
                        if (element.Element(Common.EVENT_KEY) != null)
                        {
                            em.EventKey = element.Element(Common.EVENT_KEY).Value;
                        }
                        if (element.Element(Common.TICKET) != null)
                        {
                            em.Ticket = element.Element(Common.TICKET).Value;
                        }
                        if (element.Element(Common.LATITUDE) != null)
                        {
                            em.Latitude = element.Element(Common.LATITUDE).Value;
                            em.Longitude = element.Element(Common.LONGITUDE).Value;
                            em.Precision = element.Element(Common.PRECISION).Value;
                        }
                    }
                }
                catch (Exception e)
                {

                    LogHelper.CreateLogTxt("错误：" + "   " + xml);
                }

            }

            return em;
        }
    }
}
