using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading;
using Newtonsoft.Json;
using Iwenli.WeiXin.Robot.Utility;
using FSLib.Network.Http;
using System.Xml.Linq;

namespace Iwenli.WeiXin.Robot
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 事件信息
        /// </summary>
        EVENT = 0,
        /// <summary>
        /// 文本信息
        /// </summary>
        TEXT = 1,
        /// <summary>
        /// 图片信息
        /// </summary>
        IMAGE = 2,
        /// <summary>
        /// 语音信息
        /// </summary>
        VOICE = 3,
        /// <summary>
        /// 视频信息
        /// </summary>
        VIDEO = 4,
        /// <summary>
        /// 小视频信息
        /// </summary>
        SHORTVIDEO = 5,
        /// <summary>
        /// 地理位置消息
        /// </summary>
        LOCATION = 6,
        /// <summary>
        /// 链接消息
        /// </summary>
        LINK = 7
    }

    public enum EventType
    {
        /// <summary>
        /// 订阅事件 | 扫描带参数二维码事件-用户未关注时，进行关注后的事件推送
        /// </summary>
        SUBSCRIBE = 1,
        /// <summary>
        /// 取消订阅
        /// </summary>
        UNSUBSCRIBE = 2,
        /// <summary>
        /// 扫描带参数二维码事件-用户已关注时的事件推送
        /// </summary>
        SCAN = 3,
        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        LOCATION = 4,
        /// <summary>
        /// 自定义菜单事件-点击菜单拉取消息时的事件推送
        /// </summary>
        CLICK = 5,
        /// <summary>
        /// 自定义菜单事件-点击菜单跳转链接时的事件推送
        /// </summary>
        VIEW = 6
    }

    /// <summary>
    /// 公共功能
    /// </summary>
    public class Common
    {
        #region 常量
        public const string GET = "get";
        public const string POST = "post";
        public const string SIGNATURE = "signature";
        public const string TIMESTAMP = "timestamp";
        public const string NONCE = "nonce";
        public const string ECHOSTR = "echostr";

        //公共
        public const string FROM_USERNAME = "FromUserName";
        public const string TO_USERNAME = "ToUserName";
        public const string CREATE_TIME = "CreateTime";
        public const string MSG_TYPE = "MsgType"; 
        public const string MSG_ID = "MsgId"; //除Event公用
        //Text
        public const string CONTENT = "Content";
        //Event
        public const string EVENT = "Event";
        public const string EVENT_KEY = "EventKey";
        public const string TICKET = "Ticket";
        public const string LATITUDE = "Latitude";      //地理位置纬度
        public const string LONGITUDE = "Longitude";    //地理位置经度
        public const string PRECISION = "Precision";    //地理位置精度
        //Pic
        public const string PICURL = "PicUrl";          //图片链接（由系统生成）
        public const string MEDIAID = "MediaId";        //图片消息媒体id，可以调用多媒体文件下载接口拉取数据。 
        //voice
        public const string FORNAT = "Format";          //语音格式：amr
        #endregion

        #region XML初始化变量
        public static string TOKIN = "iwenli";
        public static string ERROR = "error";
        public static string OTHER = "无法处理";
        #endregion

       

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <param name="bflag">为真时获取10位时间戳（秒级），为假时获取13位时间戳（毫秒级），默认为真</param>
        /// <returns></returns>
        public static string GetTimeStamp(bool bflag = true)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            string ret = string.Empty;
            if (bflag) { ret = Convert.ToInt64(ts.TotalSeconds).ToString(); }
            else { ret = Convert.ToInt64(ts.TotalMilliseconds).ToString(); }
            return ret;
        }

        /// <summary>
        /// 读取请求对象的内容
        /// 只能读一次
        /// </summary>
        /// <param name="request">HttpRequest对象</param>
        /// <returns></returns>
        public static string ReadRequest(HttpRequest request)
        {
            string reqStr = string.Empty;
            using (Stream s = request.InputStream)
            {
                using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
                {

                    reqStr = reader.ReadToEnd();
                }
            }
            return reqStr;
        }
         
    }
}