using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Iwenli.WeiXin.Robot.Utility
{
    public class AudioConvertToAmr
    {
        private static string _RootPath = System.Web.HttpRuntime.AppDomainAppPath.ToString();
        private static string _ApplicationPath = "app\\";
        /// <summary>
        /// ffmeg.exe文件路径，默认根目录下的app中
        /// </summary>
        public static string ApplicationPath
        {
            set { _ApplicationPath = value; }
            get { return _ApplicationPath; }
        }

        /// <summary>
        /// 将音频转成Amr手机音频
        /// </summary> 
        /// <param name="fileName">常见Audio文件的路径(带文件名)|相对目录</param>
        /// <param name="targetFilName">生成目前amr文件路径（带文件名）|相对目录</param>
        public static void ConvertToAmr(string fileName, string targetFilName)
        {
            string c = _RootPath + _ApplicationPath + "ffmpeg.exe -y -i " + _RootPath + fileName + " -ar 8000 -ab 12.2k -ac 1 " + _RootPath + targetFilName;
            Cmd(c);
        }
        /// <summary>
        /// 执行Cmd命令
        /// </summary>
        private static void Cmd(string c)
        {
            Process process = new System.Diagnostics.Process();
            try
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.Start();
                process.StandardInput.WriteLine(c); 
                process.StandardInput.WriteLine("exit"); 
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //process.WaitForExit();
                //process.Kill();
                process.Close();
            } 
        }
    }
}
