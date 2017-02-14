using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Iwenli.WeiXin.Robot.Api;

namespace Iwenli.WeiXin.Robot.Handlers
{
    public class HandleCommon
    {
        /// <summary>
        /// 自动回复 文本2文本
        /// </summary>
        /// <param name="username">调用者FromUserName</param>
        /// <param name="content">查的内容</param>
        /// <returns>回复的内容</returns>
        public static string AutoResponseText(string username, string content)
        {
            string response = string.Empty;
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
                response = TuLingRobot.Say(username, content);
            }

            return response;
        }
    }
}
