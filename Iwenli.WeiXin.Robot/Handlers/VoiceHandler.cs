using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iwenli.WeiXin.Robot.Aaterial;
using Iwenli.WeiXin.Robot.Messages;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Handlers
{
    class VoiceHandler : IHandler
    {
        private string RequestXml { set; get; }
        public VoiceHandler(string requestXml)
        {
            this.RequestXml = requestXml;
        }

        public string HandleRequest()
        {
            LogHelper.CreateLogTxt("语音请求： " + RequestXml);
            string response = string.Empty;
            VoiceMessage vm = VoiceMessage.LoadFromXml(this.RequestXml);
            /*
             * 还原说话
            
            string temp = vm.FromUserName;
            vm.FromUserName = vm.ToUserName;
            vm.ToUserName = temp; 
            response =  vm.GenerateContent();
            LogHelper.CreateLogTxt("语音响应： " + response);
            */

            TextMessage tm = new TextMessage();
            tm.FromUserName = vm.ToUserName;
            tm.ToUserName = vm.FromUserName;
            tm.CreateTime = Common.GetTimeStamp();
            //转文字   
            byte[] speech = MaterialManage.GetTemporaryMaterial(vm.MediaId);
            List<string> textRreult = Api.Baidu.VoiceRest.VoiceToText(speech, vm.Format);
            if (textRreult.Count > 0)
            {
                tm.Content = textRreult[0]; //把第一个返回去，其他的就算了吧
            }
            else
            {
                tm.Content = "哎妈呀，你说的话太难听了，我完全听不懂呐O(∩_∩)O~";
            }
            response = tm.GenerateContent();
            LogHelper.CreateLogTxt("语音识别后返回文字： " + response);
            return response;
        }
    }
}
