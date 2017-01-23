using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iwenli.WeiXin.Robot.Messages;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Handlers
{
    class EventHandler : IHandler
    {
        /// <summary>
        /// 请求的xml
        /// </summary>
        private string RequestXml { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requestXml"></param>
        public EventHandler(string requestXml)
        {
            this.RequestXml = requestXml;
        }
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <returns></returns>
        public string HandleRequest()
        {
            LogHelper.CreateLogTxt("事件请求： " + RequestXml);

            string response = string.Empty;
            EventMessage em = EventMessage.LoadFromXml(RequestXml);

            switch (em.Event)
            {
                case EventType.SUBSCRIBE:
                    response = SubscribeEventHandler(em);
                    break;
                case EventType.CLICK:
                    response = ClickEventHandler(em);
                    break;
                default:
                    break;
            }

            return response;

        }
        /// <summary>
        /// 处理点击事件
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        private string ClickEventHandler(EventMessage em)
        {
            string result = string.Empty;
            if (em != null && em.EventKey != null)
            {
                switch (em.EventKey.ToUpper())
                {
                    case "BIN_GOOD":
                        result = btnClickTextMessage(em, @"感谢您的支持！");
                        break;
                    case "BIN_HELP":
                        result = btnClickTextMessage(em, @"查询快递,输入 查快递\快递\ckd\kd 快递单号！");
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 回复文本信息
        /// </summary>
        /// <param name="em">事件对象</param>
        /// <param name="message">回复的内容</param>
        /// <returns></returns>
        private string btnClickTextMessage(EventMessage em, string message)
        {
            string response = string.Empty;
            TextMessage tm = new TextMessage();
            tm.ToUserName = em.FromUserName;
            tm.FromUserName = em.ToUserName;
            tm.CreateTime = Common.GetTimeStamp();
            tm.Content = message;
            response = tm.GenerateContent();
            LogHelper.CreateLogTxt("事件响应文本： " + response);

            return response;
        }

        /// <summary>
        /// 关注事件
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        private string SubscribeEventHandler(EventMessage em)
        {
            //回复欢迎消息
            return btnClickTextMessage(em,@"欢迎您关注龙哥的机器人小图。。。\n\n我是百事通，有事就问我，哈哈！");
        }
    }
}
