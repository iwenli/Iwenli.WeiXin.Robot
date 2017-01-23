using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSLib.Network.Http;
using Iwenli.WeiXin.Robot.Utility;

namespace Iwenli.WeiXin.Robot.Api
{
    /// <summary>
    /// 查快递
    /// </summary>
    public class KuaiDi100
    {
        /// <summary>
        /// 快递单号
        /// </summary>
        public string KuaidiNumber { set; get; }
        /// <summary>
        /// 公司编码 
        /// </summary>
        private string CompanyCode { set; get; }
        /// <summary>
        /// 验证条件  默认为1   2：需要验证码   13 || 14 :快递公司没有选择 
        /// </summary>
        private string QueryType { set; get; }
        /// <summary>
        /// 验证码   可以为空
        /// </summary>
        private string ValiCode { set; get; }
        /// <summary>
        /// 获取快递公司编码url
        /// </summary>
        private string AutoComNumUrl = "http://www.kuaidi100.com/autonumber/autoComNum?text={0}";
        /// <summary>
        /// 获取快递物流url
        /// </summary>
        private string QueryUrl = "http://www.kuaidi100.com/query?type={0}&postid={1}&id={2}&valicode={3}&temp={4}";
        /// <summary>
        /// Http请求对象
        /// </summary>
        private HttpClient client = new HttpClient();

        public KuaiDi100(string num)
        {
            this.QueryType = "1";
            this.KuaidiNumber = num;
        }

        public string GetResult()
        {
            string ret = string.Empty;
            this.AutoComNumUrl = string.Format(AutoComNumUrl, this.KuaidiNumber);
            try
            {
                //更新快递公司编码
                CompanyType ct = client.Create<CompanyType>(HttpMethod.Get, AutoComNumUrl).Send().Result;
                if (ct.auto == null)
                {
                    ret = "抱歉，暂无查询记录.";
                }
                else
                {
                    this.CompanyCode = ct.auto[0].comCode;

                    //更新查询地址
                    this.QueryUrl = string.Format(QueryUrl, this.CompanyCode, this.KuaidiNumber, this.QueryType, this.ValiCode, new Random().NextDouble());
                    QueryResult qr = client.Create<QueryResult>(HttpMethod.Post, this.QueryUrl).Send().Result;
                    if (qr.message.Equals("ok"))
                    {
                        ret = "单号【" + this.KuaidiNumber + "】记录如下：\n";
                        foreach (DataContent d in qr.data)
                        {
                            ret = ret + "【时间：" + d.ftime + "  " + d.context + "】\n";
                        }
                    }
                    else
                    {
                        ret = "抱歉，暂无查询记录.";
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.CreateEerrorLogTxt(ex, "【获取物流信息失败：】" + ex.Message);
            }

            return ret;
        }
        #region 快递公司结果
        internal class CompanyType
        {
            public string comCode { set; get; }
            public string num { set; get; }
            public List<AutoNum> auto { set; get; }
        }
        internal class AutoNum
        {
            public string comCode { set; get; }
            public string id { set; get; }
            public string noCount { set; get; }
            public string noPre { set; get; }
            public string startTime { set; get; }
        }
        #endregion

        #region 快递结果
        internal class QueryResult
        {
            public string message { get; set; }
            public string nu { get; set; }
            public string ischeck { get; set; }
            public string condition { get; set; }
            public string com { get; set; }
            public string status { get; set; }
            public string state { get; set; }
            public List<DataContent> data { get; set; }

        }
        internal class DataContent
        {
            public string time { get; set; }
            public string ftime { get; set; }
            public string context { get; set; }
            public string location { get; set; }
        }
        #endregion

    }
}
