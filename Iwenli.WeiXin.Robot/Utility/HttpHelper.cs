using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Linq;
using System.Net.Cache;

namespace Iwenli.WeiXin.Robot.Utility
{

    /// <summary>
    /// 模拟post get   已被他替代   FSLib.Network
    /// </summary>
    public class HttpHelper
    {
        private string _CookieHeader = string.Empty;
        /// <summary> 
        /// 网站Cookies 
        /// </summary> 
        public string CookieHeader
        {
            get
            {
                return _CookieHeader;
            }
            set
            {
                _CookieHeader = value;
            }
        }
        private Encoding _Encoding = Encoding.UTF8;
        /// <summary> 
        /// 网站编码  默认utf-8
        /// </summary> 
        public Encoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }
        string _Method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 100000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 30000;
        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        Boolean _KeepAlive = true;
        /// <summary>
        ///  获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接默认为true。
        /// </summary>
        public Boolean KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        string _Accept = "text/html, application/xhtml+xml, */*";
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "text/html";
        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }

        private HttpWebRequest myHttpWebRequest = null;


        #region private method
        /// <summary>
        /// 初始化HttpWebRequest对象
        /// </summary>
        /// <param name="url">请求地址</param>
        private void initHttpWebRequest(string url)
        {
            myHttpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            myHttpWebRequest.KeepAlive = _KeepAlive;
            myHttpWebRequest.Accept = _Accept;
            myHttpWebRequest.UserAgent = _UserAgent;
            myHttpWebRequest.ContentType = _ContentType;
            myHttpWebRequest.Method = _Method;
            myHttpWebRequest.Timeout = _Timeout;
            myHttpWebRequest.ReadWriteTimeout = _ReadWriteTimeout;
            myHttpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

            if (myHttpWebRequest.CookieContainer == null)
            {
                myHttpWebRequest.CookieContainer = new CookieContainer();
            }
            if (this._CookieHeader.Length > 0)
            {
                myHttpWebRequest.Headers.Add("cookie:" + this._CookieHeader);
                myHttpWebRequest.CookieContainer.SetCookies(new Uri(url), this._CookieHeader);
            }
        }
        /// <summary>
        /// 根据请求结果设置Cookie
        /// </summary>
        /// <param name="url">请求地址</param>
        private void SetCookie(string url)
        {
            if (myHttpWebRequest.CookieContainer != null)
            {
                this._CookieHeader = myHttpWebRequest.CookieContainer.GetCookieHeader(new Uri(url));
            }
        }
        /// <summary>
        /// Post方式发送数据
        /// </summary>
        /// <param name="strPostData">待发送的数据</param>
        private void PostData(string strPostData)
        {
            byte[] postData = _Encoding.GetBytes(strPostData);
            myHttpWebRequest.ContentLength = postData.Length;
            Stream PostStream = myHttpWebRequest.GetRequestStream();
            PostStream.Write(postData, 0, postData.Length);
            PostStream.Close();
        }
        /// <summary>
        /// 提交请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        private string ResponseResult(string url)
        {
            string strResult = string.Empty;
            HttpWebResponse response = null;
            System.IO.StreamReader sr = null;
            response = (HttpWebResponse)myHttpWebRequest.GetResponse();

            SetCookie(url);

            Stream streamReceive;
            string gzip = response.ContentEncoding;

            if (string.IsNullOrEmpty(gzip) || gzip.ToLower() != "gzip")
            {
                streamReceive = response.GetResponseStream();
            }
            else
            {
                streamReceive = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
            }

            sr = new System.IO.StreamReader(streamReceive, _Encoding);

            if (response.ContentLength > 1)
            {
                strResult = sr.ReadToEnd();
            }
            else
            {
                char[] buffer = new char[256];
                int count = 0;
                StringBuilder sb = new StringBuilder();
                while ((count = sr.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(new string(buffer));
                }
                strResult = sb.ToString();
            }
            sr.Close();
            response.Close();
            return strResult;
        }

        #endregion


        /// <summary>
        /// post方式请求数据  方法1： strUrl = url?key1=value1&key2=value2... , postData = ""   方法2：strUrl = url , postData = key1=value1&key2=value2...
        /// </summary> 
        /// <param name="strUrl">请求地址</param>
        /// <param name="postData">post的数据</param>
        /// <param name="strReferer">引用地址,模拟登陆页面一般需要</param>
        /// <returns></returns>
        public string PostRequest(string strUrl, string postData = "", string strReferer = "")
        {
            string strResult = string.Empty;
            try
            {
                //初始化对象
                initHttpWebRequest(strUrl);
                myHttpWebRequest.Method = "POST";
                if (!strReferer.Equals(string.Empty))
                {
                    myHttpWebRequest.Referer = strReferer;
                }
                if (!postData.Equals(string.Empty))
                {
                    PostData(postData);
                }
                strResult = ResponseResult(strUrl);
            }
            catch (Exception ex)
            {
                LogHelper.CreateLogTxt(string.Format("【Post异常】ex.Message=>{0}  strUrl=>{1}  postData=>{2}", ex.Message, strUrl, postData));
            }
            return strResult;
        }

        /// <summary>
        /// get方式请求数据
        /// </summary>
        /// <param name="strUrl">请求地址 url?key1=value1&key2=value2...</param>
        /// <returns></returns>
        public string GetRequest(string strUrl)
        {
            string strResult = string.Empty;
            try
            {
                //初始化对象
                initHttpWebRequest(strUrl);
                strResult = ResponseResult(strUrl);
            }
            catch (Exception ex)
            {
                LogHelper.CreateLogTxt(string.Format("【Get异常】ex.Message=>{0}  strUrl=>{1}", ex.Message, strUrl));
            }
            return strResult;
        }


    }

}