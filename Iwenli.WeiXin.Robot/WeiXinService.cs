using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Iwenli.WeiXin.Robot.Utility;
using Iwenli.WeiXin.Robot.Handlers;

namespace Iwenli.WeiXin.Robot
{
    public class WeiXinService
    {

        #region 属性
        private HttpRequest Request { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request"></param>
        public WeiXinService(HttpRequest request)
        {
            this.Request = request;
        }
        #endregion

        #region public方法
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <returns></returns>
        public string Response()
        {
            string method = Request.HttpMethod.ToLower();
            switch (method)
            {
                case Common.GET:  //验证签名
                    return CheckSignature() ? Request.QueryString[Common.ECHOSTR] : Common.ERROR;
                case Common.POST: //处理消息
                    return CheckSignature() ? ResponseMsg() : Common.OTHER;
                default:
                    return Common.OTHER;
            }
        }
        #endregion

        #region private方法
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <returns></returns>
        private string ResponseMsg()
        {
            string requestXml = Common.ReadRequest(this.Request); 
            IHandler handler = HandlerFactory.CreateHandler(requestXml);
            return (handler == null) ? string.Empty : handler.HandleRequest();
        }
        /// <summary>
        /// 检查签名是否正确
        /// </summary>
        /// <returns></returns>
        private bool CheckSignature()
        {
            string signature = Request.QueryString[Common.SIGNATURE];
            string timestamp = Request.QueryString[Common.TIMESTAMP];
            string nonce = Request.QueryString[Common.NONCE];

            List<string> list = new List<string>();
            list.Add(Common.TOKIN);
            list.Add(timestamp);
            list.Add(nonce);
            //排序
            list.Sort();
            //拼串
            string input = string.Empty;
            foreach (var item in list)
            {
                input += item;
            }
            //加密
            string new_signature = SecurityUtility.SHA1Encrypt(input);
            //返回验证结果
            return new_signature.Equals(signature);
        }
        #endregion 
    }
}
