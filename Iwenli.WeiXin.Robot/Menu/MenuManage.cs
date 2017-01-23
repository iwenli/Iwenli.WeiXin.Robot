using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSLib.Network.Http;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Menu
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    public class MenuManage
    {
        /// <summary>
        /// 菜单文件路径
        /// </summary>
        private static readonly string Menu_Data_Path = System.AppDomain.CurrentDomain.BaseDirectory + "Data\\menu.json"; 
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public static string GetMenu() {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", Context.AccessToken);
            string menuJson=  client.Create<string>(HttpMethod.Get, url).Send().Result;
            LogHelper.CreateLogTxt("【获取菜单】：" + menuJson);
            return menuJson;
        }
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menu">菜单json</param>
        public static void CreateMenu(string menu) {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", Context.AccessToken);
            LogHelper.CreateLogTxt("【创建菜单】：" + menu);
            client.Create<string>(HttpMethod.Post, url,data:menu,contentType:ContentType.Json).Send();
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        public static void DeleteMenu()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", Context.AccessToken);
            LogHelper.CreateLogTxt("【删除菜单】");
            client.Create<string>(HttpMethod.Get, url).Send();
        }
        /// <summary>
        /// 加载菜单
        /// </summary>
        /// <returns></returns>
        public static string LoadMenu()
        {
            return FileHelper.ReadFile(Menu_Data_Path);
        }

        public static void TestWrite(string data) {
            FileHelper.WriteFile(Menu_Data_Path, data);
        }
    }
}
