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
            string response = string.Empty;
            TextMessage tm = TextMessage.LoadFromXml(RequestXml);
            string content = tm.Content.Trim();
            if (string.IsNullOrEmpty(content))
            {
                response = "您什么都没输入，没法帮您啊，%>_<%。";
            }
            else if (content.Contains("kd") || content.Contains("ckd") || content.Contains("快递") || content.Contains("查快递"))
            {
                string kuaidiNum = content.Replace("kd", "").Replace("c", "").Replace("快递", "").Replace("查", "").Trim();
                if (!string.IsNullOrEmpty(kuaidiNum) && Regex.IsMatch(kuaidiNum, @"^[+-]?\d*[.]?\d*$"))
                {
                    KuaiDi100 kd = new KuaiDi100(kuaidiNum);
                    response = kd.GetResult();
                }
                else
                { response = "请输入正确的快递单号！"; }
            }
            else if (content == "创建菜单")
            {
                Menu.MenuManage.CreateMenu(Menu.MenuManage.LoadMenu());
                response = "创建好了哦。";
            }
            else if (content == "删除菜单")
            {
                Menu.MenuManage.DeleteMenu();
                response = "删除好了哦。";
            }
            else if (content == "获取菜单")
            {
                response = Menu.MenuManage.GetMenu();
            }
            else
            {
                response = TuLingRobot.Say(tm.FromUserName, content);
            }
            tm.Content = response;

            if (content == "iwenli")
            {
                string res = @"<xml>
                                  <ToUserName><![CDATA[oWqQMwsAintKpK13GVFhktlF-e6c]]></ToUserName>
                                  <FromUserName><![CDATA[gh_b07eda6c5e91]]></FromUserName>
                                  <CreateTime>1484553262</CreateTime>
                                  <MsgType><![CDATA[event]]></MsgType>
                                  <Event><![CDATA[unsubscribe]]></Event>
                                  <EventKey><![CDATA[]]></EventKey>
                                </xml>";
                //进行发送者接受者转换  
                return res;
            }

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
