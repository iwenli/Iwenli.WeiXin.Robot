using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Iwenli.WeiXin.Robot.Web
{
    public class Open : IHttpHandler
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此处理程序 
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            context.Response.Charset = "UTF-8";

            //在此处写入您的处理程序实现。
            string data = context.Request["data"];
            if (string.IsNullOrEmpty(data))
            {
                context.Response.Write(GetReturnJson(1, "参数data不能为空"));
                return;
            }
            var list = JsonConvert.DeserializeObject<List<TenderModel>>(data);
            if (list.Count < 3)
            {
                context.Response.Write(GetReturnJson(2, "计算公司个数不能少于3个"));
                return;
            }
            else
            {
                TenderBLl tender = new TenderBLl();
                tender.AddRange(list);
                tender.CalcTender();
                context.Response.Write(GetReturnJson(0, "ok", JsonConvert.SerializeObject(tender.Tenders.OrderByDescending(m => m.Scroe))));
            }

            //TenderModel m1 = new TenderModel()
            //{
            //    Id = 0,
            //    Company = "北京中睿昊天信息科技有限公司",
            //    Price = 24156043.5M,
            //    Scroe = 0
            //};
            //TenderModel m2 = new TenderModel()
            //{
            //    Id = 1,
            //    Company = "青岛鼎信通讯股份有限公司",
            //    Price = 22850974.9M,
            //    Scroe = 0
            //};
            //TenderModel m3 = new TenderModel()
            //{
            //    Id = 2,
            //    Company = "瑞斯康微电子(深圳)有限公司",
            //    Price = 23241874.74M,
            //    Scroe = 0
            //};
            //tender.Add(m1);
            //tender.Add(m2);
            //tender.Add(m3);

            //http://www.docin.com/p-833388640.html&model=2905
            //http://localhost:60640/api/open.axd?data=1050.7068,1039.7055,1030.6296,1015.5200,730.5012,717.2000,657.7848,376.3729
            //if (string.IsNullOrEmpty(data))
            //{
            //    data = "100,22,99,11,55.5100,99,1,999,234,234,234,234,567,57,567,567,567,1,44234,1";
            //}
            //if (data.IndexOf(",,") > -1)
            //{
            //    context.Response.Write(GetReturnJson(2, "参数data有空数据"));
            //}
            //else if (new Regex(@"^([\d.]*[,])+[\d\.]*$").IsMatch(data))
            //{
            //    //转换类型
            //    decimal[] money = Array.ConvertAll<string, decimal>(data.Split(','), s => decimal.Parse(s));
            //    //执行计算
            //    CalcTender(money, context);

            //    context.Response.Write(GetReturnJson());
            //}
            //else
            //{
            //    context.Response.Write(GetReturnJson(1, "参数data错误"));

            //}
            context.Response.End();
        }

        #endregion

        public string GetReturnJson(int code = 0, string msg = "ok", string data = null)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append(@"{");
            jsonBuilder.AppendFormat("\"errcode\":{0},\"errmsg\":\"{1}\"", code, msg);
            if (!string.IsNullOrEmpty(data))
            {
                jsonBuilder.AppendFormat(",\"data\":{0}", data);
            }
            jsonBuilder.Append(@"}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 评标的核心算法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public decimal CalcTender(decimal[] data, HttpContext context)
        {
            #region 核心算法
            /*
                    M为进入详评的投标人数
                    若 M < 10 不去掉任何一个报价   -0
                    若 10 <= M < 20 , 去掉1个最高评标价 和 1个最低评标价,如果出现并列最高或最低的情况，在计算基准价时只去其一，以下类推  -2
                    若 20 <= M < 30 , 去掉2个最高评标价 和 1个最低评标价  -3
                    若 M >= 30 , 去掉3个最高评标价 和 2个最低评标价     -5

                    N剩余投标人
                    计算剩余N个人的算术平均值A1

                    然后在剔除（只为计算，并不是废除）评标价与算术平均值A1偏差超过 [-20%,10%] 区间的报价
                    剔除后投标人P
                    计算剩余P个人的算术平均值A2

                    计算A2 和 P个中最低投标价 的平均值A3

                    以A3为基准价
                    如果N个评标价均在算术平均值A1 [-20%,10%] 区间外，则所有进入详评的投标人M评标价的算术平均值A4作为基准价

                    价格部分得分 = 100 - 100 * n * m * | 投标人的评标总价 - 基准价 | / 基准价

                    当投标人的评标总价 >= 基准价 ， m = 1
                    当投标人的评标总价 < 基准价 ， m = [0.3,0.8]

                    计算出的价格部分得分 < 0  按0分计

                */
            #endregion
            decimal top = new decimal(0.1);
            decimal bottom = new decimal(-0.2);
            decimal[] beforSortData = new decimal[data.Length];
            Array.Copy(data, beforSortData, data.Length);
            decimal A = new decimal(0);  //基准价


            //排序
            Array.Sort(data);
            int M = data.Length;  //总投标人数

            #region 第1步筛选
            //第1步筛选
            decimal[] data1 = null;  //剩余报价
            if (M < 10)
            {
                data1 = new decimal[M];
                Array.Copy(data, data1, M);
            }
            if (M >= 10 && M < 20)
            {
                data1 = new decimal[M - 2];
                Array.Copy(data, 1, data1, 0, M - 2);
            }
            else if (M >= 20 && M < 30)
            {
                data1 = new decimal[M - 3];
                Array.Copy(data, 1, data1, 0, M - 3);
            }
            else if (M >= 30)
            {
                data1 = new decimal[M - 5];
                Array.Copy(data, 2, data1, 0, M - 5);
            }

            int N = data1.Length; //剩余投标人
            decimal A1 = GetAvg(data1);
            #endregion

            #region 第2步筛选 - 剔除
            //第2步筛选 - 剔除
            decimal[] data2 = data1.Where(a => (1 - decimal.Divide(A1, a)) <= top && (1 - decimal.Divide(A1, a)) > bottom).ToArray<decimal>();
            int P = data2.Length; //剔除后投标人
            decimal A2 = GetAvg(data2);
            #endregion

            #region 第3步 得出基准价   计算A2 和 P个中最低投标价 的平均值A3 
            if (P > 0)
            {
                decimal A3 = (A2 + data2[0]) / 2;
                A = A3;
            }
            else//如果N个评标价均在算术平均值A1 [-20%,10%] 区间外，则所有进入详评的投标人M评标价的算术平均值A4作为基准价
            {
                A = GetAvg(data1);
            }
            #endregion







            #region Log
            StringBuilder sbFromat = new StringBuilder();
            sbFromat.AppendFormat("总人数：{0}", M.ToString());
            sbFromat.AppendLine();
            sbFromat.AppendFormat("元数据：{0}", string.Join(",", Array.ConvertAll<decimal, string>(beforSortData, s => s.ToString())));
            sbFromat.AppendLine();
            sbFromat.AppendFormat("排序后：{0}", string.Join(",", Array.ConvertAll<decimal, string>(data, s => s.ToString())));
            sbFromat.AppendLine();
            sbFromat.AppendFormat("第1步筛选后人数：{0}", N.ToString());
            sbFromat.AppendLine();
            sbFromat.AppendFormat("第1步筛选后数据：{0}", string.Join(",", Array.ConvertAll<decimal, string>(data1, s => s.ToString())));
            sbFromat.AppendLine();
            sbFromat.AppendFormat("第1步筛选后平均值：{0}", A1);
            sbFromat.AppendLine();
            sbFromat.AppendFormat("第2步筛选后人数：{0}", P.ToString());
            sbFromat.AppendLine();
            sbFromat.AppendFormat("第2步筛选后数据：{0}", string.Join(",", Array.ConvertAll<decimal, string>(data2, s => s.ToString())));
            sbFromat.AppendLine();
            sbFromat.AppendFormat("第2步筛选后平均值：{0}", A2);
            sbFromat.AppendLine();
            sbFromat.AppendFormat("基准价：{0}", A);
            sbFromat.AppendLine();
            sbFromat.AppendLine();
            sbFromat.AppendFormat("得分计算如下:");
            sbFromat.AppendLine();
            sbFromat.AppendFormat("---------------------------------------------------------------");
            sbFromat.AppendLine();

            foreach (var item in beforSortData)
            {
                sbFromat.AppendFormat("出价：{0}  的得分为：{1}", item, CalcScroe(item, A));
                sbFromat.AppendLine();
            }
            sbFromat.AppendLine();
            sbFromat.AppendFormat("---------------------------------------------------------------");
            #endregion


            sbFromat.AppendLine();
            context.Response.Write(sbFromat.ToString());
            return 0;
        }

        /// <summary>
        /// 求数组的算术平均数
        /// </summary>
        /// <param name="array">带计算数组</param>
        /// <returns></returns>
        public decimal GetAvg(decimal[] array)
        {
            decimal sum = 0;
            if (array.Length == 0 || array == null)
            {
                return 0;
            }
            foreach (var item in array)
            {
                sum += item;
            }
            return sum / array.Length;
        }

        /// <summary>
        /// 计算得分
        /// </summary>
        /// <param name="price"></param>
        /// <param name="basePrice"></param>
        /// <returns></returns>
        public decimal CalcScroe(decimal price, decimal basePrice)
        {
            //价格部分得分 = 100 - 100 * n * m * | 投标人的评标总价 - 基准价 | / 基准价

            //当投标人的评标总价 >= 基准价 ， m = 1
            //当投标人的评标总价 < 基准价 ， m = [0.3, 0.8]
            decimal n = new decimal(1);
            decimal m = new decimal(1);
            if (price < basePrice)
            {
                m = new decimal(0.5);// GetRandom(3, 8);
            }

            decimal retValue = new decimal(100) - (new decimal(100) * n * m * Math.Abs(decimal.Subtract(basePrice, price))) / basePrice;
            return retValue > 0 ? retValue : 0;
        }

        public decimal GetRandom(int min, int max)
        {
            Random r = new Random();
            return new decimal((double)r.Next(min, max) / 10);
        }
    }


    public class TenderBLl
    {
        #region 核心算法
        /*
                M为进入详评的投标人数
                若 M < 10 不去掉任何一个报价   -0
                若 10 <= M < 20 , 去掉1个最高评标价 和 1个最低评标价,如果出现并列最高或最低的情况，在计算基准价时只去其一，以下类推  -2
                若 20 <= M < 30 , 去掉2个最高评标价 和 1个最低评标价  -3
                若 M >= 30 , 去掉3个最高评标价 和 2个最低评标价     -5

                N剩余投标人
                计算剩余N个人的算术平均值A1

                然后在剔除（只为计算，并不是废除）评标价与算术平均值A1偏差超过 [-20%,10%] 区间的报价
                剔除后投标人P
                计算剩余P个人的算术平均值A2

                计算A2 和 P个中最低投标价 的平均值A3

                以A3为基准价
                如果N个评标价均在算术平均值A1 [-20%,10%] 区间外，则所有进入详评的投标人M评标价的算术平均值A4作为基准价

                价格部分得分 = 100 - 100 * n * m * | 投标人的评标总价 - 基准价 | / 基准价

                当投标人的评标总价 >= 基准价 ， m = 1
                当投标人的评标总价 < 基准价 ， m = [0.3,0.8]

                计算出的价格部分得分 < 0  按0分计

            */
        #endregion

        private List<TenderModel> m_tenders = new List<TenderModel>();
        private decimal m_top = new decimal(0.1);
        private decimal m_bottom = new decimal(-0.2);
        private decimal m_n = new decimal(1);
        private decimal m_basePrice = new decimal(0);

        #region 属性
        public decimal BasePrice
        {
            get { return m_basePrice; }

        }
        /// <summary>
        /// 区间上限值
        /// </summary>
        public decimal Top
        {
            set { m_top = value; }
            get { return m_top; }
        }
        /// <summary>
        /// 区间下限值
        /// </summary>
        public decimal Bottom
        {
            set { m_bottom = value; }
            get { return m_bottom; }
        }

        /// <summary>
        /// 获取投标集合
        /// </summary>
        public List<TenderModel> Tenders
        {
            get
            {
                if (m_tenders.Count > 0)
                {
                    return m_tenders;
                }
                else { return null; }
            }
        }
        /// <summary>
        /// 投标个数
        /// </summary>
        public int Count
        {
            get
            {
                return m_tenders.Count;
            }
        }
        #endregion

        #region Public方法
        /// <summary>
        /// 添加一个投标对象
        /// </summary>
        /// <param name="tender"></param>
        public void Add(TenderModel tender)
        {
            m_tenders.Add(tender);
        }
        /// <summary>
        /// 添加一组投标对象
        /// </summary>
        /// <param name="tender"></param>
        public void AddRange(IEnumerable<TenderModel> tenders)
        {
            m_tenders.AddRange(tenders);
        }
        /// <summary>
        /// 计算评标价
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void CalcTender()
        {

            List<TenderModel> list = new List<TenderModel>(m_tenders);  //复制集合 保留元数据 在新数据上操作
            //排序
            list.Sort((x, y) => x.Price.CompareTo(y.Price));


            #region 第1步筛选
            //第1步筛选
            List<TenderModel> list1 = new List<TenderModel>(list);  //复制集合 保留元数据 在新数据上操作  //剩余报价

            if (Count >= 10 && Count < 20)
            {
                list1.RemoveAt(list1.Count);
                list1.RemoveAt(0);
            }
            else if (Count >= 20 && Count < 30)
            {
                list1.RemoveAt(list1.Count);
                list1.RemoveAt(list1.Count);
                list1.RemoveAt(0);
            }
            else if (Count >= 30)
            {
                list1.RemoveAt(list1.Count);
                list1.RemoveAt(list1.Count);
                list1.RemoveAt(list1.Count);
                list1.RemoveAt(0);
                list1.RemoveAt(0);
            }
            int N = list1.Count; //剩余投标人
            decimal A1 = GetAvg(list1);
            #endregion

            #region 第2步筛选 - 剔除
            //第2步筛选 - 剔除
            List<TenderModel> list2 = list1.Where(a => (1 - decimal.Divide(A1, a.Price)) <= m_top && (1 - decimal.Divide(A1, a.Price)) > m_bottom).ToList();
            int P = list2.Count; //剔除后投标人
            decimal A2 = GetAvg(list2);
            #endregion

            #region 第3步 得出基准价   计算A2 和 P个中最低投标价 的平均值A3 
            if (P > 0)
            {
                decimal A3 = (A2 + list2[0].Price) / 2;
                m_basePrice = A3;
            }
            else//如果N个评标价均在算术平均值A1 [-20%,10%] 区间外，则所有进入详评的投标人M评标价的算术平均值A4作为基准价
            {
                m_basePrice = GetAvg(list1);
            }
            #endregion

            #region 计算得分
            for (int i = 0; i < Count; i++)
            {
                m_tenders[i].Scroe = CalcScroe(m_tenders[i].Price, m_basePrice);
            }
            #endregion

        }

        #endregion

        #region Private 方法
        /// <summary>
        /// 计算得分
        /// </summary>
        /// <param name="price"></param>
        /// <param name="basePrice"></param>
        /// <returns></returns>
        private decimal CalcScroe(decimal price, decimal basePrice)
        {
            //价格部分得分 = 100 - 100 * n * m * | 投标人的评标总价 - 基准价 | / 基准价

            //当投标人的评标总价 >= 基准价 ， m = 1
            //当投标人的评标总价 < 基准价 ， m = [0.3, 0.8]
            decimal n = new decimal(1);
            decimal m = new decimal(1);
            if (price < basePrice)
            {
                m = new decimal(0.5);// GetRandom(3, 8);
            }

            decimal retValue = new decimal(100) - (new decimal(100) * n * m * Math.Abs(decimal.Subtract(basePrice, price))) / basePrice;
            return retValue > 0 ? retValue : 0;
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="min">随机数下限*10</param>
        /// <param name="max">随机数上限*10</param>
        /// <returns></returns>
        private decimal GetRandom(int min, int max)
        {
            Random r = new Random();
            return new decimal((double)r.Next(min, max) / 10);
        }
        /// 求算术平均数
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        private decimal GetAvg(List<TenderModel> list)
        {
            if (list.Count == 0 || list == null)
            {
                return 0;
            }
            return list.Sum(m => m.Price) / list.Count;
        }
        #endregion 
    }

    public class TenderModel
    {
        /// <summary>
        /// 计算前编号
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { set; get; }
        /// <summary>
        /// 投标出价
        /// </summary>
        public decimal Price { set; get; }
        /// <summary>
        /// 得分
        /// </summary>
        public decimal Scroe { set; get; }
    }
}
