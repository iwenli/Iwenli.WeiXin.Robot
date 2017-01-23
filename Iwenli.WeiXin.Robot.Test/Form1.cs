using System;
using System.Windows.Forms;
using Iwenli.WeiXin.Robot.Api;
using Iwenli.WeiXin.Robot.Handlers;

namespace Iwenli.WeiXin.Robot.Test
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            //MessageBox.Show(Common.GetTimeStamp(false));

            //txtBoxResult.Text = SecurityUtility.DecryptAES("TwPFGlIQk/yl2qDbNyuSQg9JMeV6aLdCS7yo6lT5Ia0=", "912194e51267870e9283e9a035360a78");
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            //Common.SetAccessToken();
            //KuaiDi100 kd = new KuaiDi100("883866537840423409");
            //txtBoxResult.Text = kd.GetResult();
            TextHandler th = new TextHandler(@"<xml><ToUserName><![CDATA[gh_b07eda6c5e91]]></ToUserName>
<FromUserName><![CDATA[oWqQMwsAintKpK13GVFhktlF-e6c]]></FromUserName>
<CreateTime>1484814002</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[查快递883866537840423409]]></Content>
<MsgId>6377227579643100370</MsgId>
</xml>");
            string s = th.HandleRequest();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtBoxResult.Text = TuLingRobot.Say("iwenli", txtBoxInput.Text.Trim()); //ConnectTuLing(txtBoxInput.Text.Trim());
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            txtBoxInput.Text = "你好";
        }


    }
}
