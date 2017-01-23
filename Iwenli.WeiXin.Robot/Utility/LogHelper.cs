using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Iwenli.WeiXin.Robot.Utility
{
    public class LogHelper
    {
        /// <summary>
        /// 格式化时间前缀
        /// </summary>
        /// <returns></returns>
        private static string GetTimePrefix
        {
            get
            {
                return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]  ";
            }
        }


        #region  创建日志
        ///-----------------------------------------------------------------------------
        /// <summary>创建日志</summary>
        /// <param name="message">记录信息</param> 
        ///-----------------------------------------------------------------------------
        public static void CreateLogTxt(string message)
        {
            //string defaultLogPath = Assembly.GetExecutingAssembly().Location.ToString(); //winform
            //defaultLogPath = defaultLogPath.Substring(0, defaultLogPath.LastIndexOf("\\")); //winform 
            string defaultLogPath = System.Web.HttpRuntime.AppDomainAppPath.ToString();//web 
            CreateLogTxt(message, defaultLogPath);
        }



        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="message">记录信息</param>
        /// <param name="path">文件路径</param>
        public static void CreateLogTxt(string message, string path)
        {
            string strPath = path; //文件的路径
            DateTime dt = DateTime.Now;
            try
            {
                strPath = strPath + "\\Log\\" + dt.ToString("yyyy");

                if (Directory.Exists(strPath) == false) //目录是否存在,不存在则没有此目录
                {
                    Directory.CreateDirectory(strPath);
                }
                strPath = strPath + "\\" + dt.Year.ToString() + "-" + dt.Month.ToString() + "-" + dt.Day.ToString() + ".txt";

                WriteTxt(message, strPath);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        public static void CreateEerrorLogTxt(Exception e, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(message);
            sb.Append("\r\nError.Message = " + e.Message);
            sb.Append("\r\nError.Source = " + e.Source);
            sb.Append("\r\nError.TargetSite = " + e.TargetSite);
            CreateLogTxt(sb.ToString());
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="message">内容</param>
        /// <param name="filepath">文件全路径</param>
        private static void WriteTxt(string message, string filepath)
        {
            StreamWriter FileWriter = new StreamWriter(filepath, true);           //创建日志文件
            FileWriter.WriteLine(GetTimePrefix + message);
            FileWriter.Close();     //关闭StreamWriter对象
        }

        #endregion
    }
}
