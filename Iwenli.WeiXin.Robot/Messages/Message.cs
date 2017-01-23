using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iwenli.WeiXin.Robot.Messages
{
    /// <summary>
    /// 消息处理基类
    /// </summary>
    public class Message : ITemplate
    {
        #region 属性
        /// <summary>
        /// 发送方账号
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 接收方账号
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MsgType { get; protected set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        public virtual string Template
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region 构造函数
        public Message()
        {
        }
        #endregion

        /// <summary>
        /// 生成内容
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateContent()
        {
            throw new NotImplementedException();
        }
    }
}
