using System.Collections.Generic;
using Iwenli.WeiXin.Robot.Aaterial;
using Iwenli.WeiXin.Robot.Api.Baidu;
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

            string temp = vm.FromUserName;
            vm.FromUserName = vm.ToUserName;
            vm.ToUserName = temp;

            /*
             * 他说哈 回复啥 
            response =  vm.GenerateContent();
            */

            /*
             * 他说哈 查到结果后回复他
             */
            string requestText = string.Empty;
            string responseText = string.Empty;
            //转文字   
            byte[] speech = MaterialManage.GetTemporaryMaterial(vm.MediaId);
            List<string> textRreult = Api.Baidu.VoiceRest.VoiceToText(speech, vm.Format);
            if (textRreult.Count > 0)
            {
                requestText = textRreult[0]; //把第一个返回去，其他的就算了吧
                //识别出来之后调用图灵机器人 
                responseText = HandleCommon.AutoResponseText(vm.ToUserName, requestText);
            }
            else
            {
                responseText = "哎妈呀，你说的话太难听了，我完全听不懂呐O(∩_∩)O~";
            } 
            //调用百度接口转语音
            byte[] b = VoiceRest.TextToVoice(responseText, per: 0);
            if (b == null)
            { 
                //失败
            }
            else
            {
                string filePath = string.Format("temp\\{0}.amr", responseText);
                Robot.Utility.FileHelper.WriteFile(filePath, b);
                 
                //上传临时素材
                string media_id = MaterialManage.UploadTemporaryMaterial(b, MaterialType.voice);
                if (media_id.Equals(string.Empty))
                { 
                    //失败
                }
                else
                { 
                    vm.MediaId = media_id;
                }
            }

            response = vm.GenerateContent();
            LogHelper.CreateLogTxt("语音响应： " + response);

            return response;
        }
    }
}
