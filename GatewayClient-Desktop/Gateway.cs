/*********************************************************************
 * NetWork.cs提供登陆注销网关等相关函数
 * 由NewFuture 
 * 于2013-2-24创建
 *        2-25修改login()
 *        3-3修改
 * *******************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 提供登录相关方法
/// </summary>
namespace GatewayClient_Desktop
{
   public struct Info
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Uid;
        /// <summary>
        /// 流量MB
        /// </summary>
        public double Flow;
        /// <summary>
        /// 余额￥
        /// </summary>
        public double Fee;
        /// <summary>
        /// 时间s
        /// </summary>
        public double Time;
        public Info(string uid=null, double flow=0,double fee=0,double time=0)
        {
            Uid = uid;
            Flow = flow;
            Fee = fee;
            Time = time;
        }
    
    }
    static class Gateway
    {
        /// <summary>
        /// host
        /// </summary>
        public const string HOST1 = "http://202.113.18.110";
        public const string HOST2 = "http://202.113.18.210";
        public static int TimeOut = 2500;
        const string sucess_title = @"<title>登录成功</title>";
        const string query_path = "/";
        const string login_path = ":801/eportal/?c=ACSetting&a=Login";
        const string logout_path = ":801/eportal/?c=ACSetting&a=Logout";

        //    /// <summary>
        //    /// i网关地址
        //    /// </summary>
        //    public static string ipv4_url = "http://202.113.18.188/";
        ///// <summary>
        ///// ipv6网关地址
        ///// </summary>
        //static public readonly string ipv6_url = "http://ip6.nku.cn/";

        static string pwd;
        static string uid;

        static public bool Login(string name, string password)
        {
         
            uid = name;
            pwd = password;
            string login_url = HOST1 + login_path;
            var s = postLogin(login_url);
            if(s.IndexOf(sucess_title)>0)
            {
                return true;
            }
            else
            {
                s = postLogin(HOST2 + login_path);
                return s.IndexOf(sucess_title) > 0;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="login_url"></param>
        /// <returns></returns>
        static private string postLogin(string login_url)
        {
            try
            {
                System.Net.WebRequest post = System.Net.HttpWebRequest.Create(login_url);
                post.Timeout = TimeOut;
                post.Method = "POST";
                post.ContentType = "application/x-www-form-urlencoded";
                byte[] toout = System.Text.Encoding.UTF8.GetBytes("DDDDD=" + uid + "&upass=" + pwd);

                post.ContentLength = toout.Length;
                var requeststream = post.GetRequestStream();
                requeststream.Write(toout, 0, toout.Length);

                var result = (new System.IO.StreamReader(post.GetResponse().GetResponseStream(),
                                                                  System.Text.Encoding.GetEncoding("GB2312"))).ReadToEnd();
                return result;
            }
            catch (Exception)
            {

                return "";
            }      
        }

        //static public object Query()
        //{

        //}
        ///// <summary>
        ///// 后台登录
        ///// </summary>
        ///// <param name="isIpv4"></param>
        ///// <returns></returns>
        //static public string LoginBackground(bool isIpv4)
        //{
        //    //return Login(usrnm, psswrd);
        //}

        /// <summary>
        /// 注销网关登陆
        /// </summary>
        /// <param name="isIpv4">true表示ipv4，false表示ipv6</param>
        /// <returns>异常返回false</returns>
        static public bool Logout()
        {

            try
            {
                string url = HOST1 + logout_path;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = TimeOut;
                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
                {
                    sr.ReadToEnd();
                    sr.Close();
                }
                request = (HttpWebRequest)WebRequest.Create(HOST2+logout_path);
                sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
                {
                    sr.ReadToEnd();
                    sr.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        ///// <summary>
        ///// 判断是否是未登陆状态
        ///// </summary>
        ///// <param name="isIpv4">true为ipv4查询，false为ipv6查询</param>
        ///// <returns>未登陆返回true，已登陆返回false,链接失败抛出异常</returns>
        //static bool IsNotLogon()
        //{

        //    //建立ipv4或者ipv6链接请求
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create();
        //    req.Timeout = 3000;
        //    try
        //    {
        //        //从网关获取信息
        //        StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
        //        string result = sr.ReadToEnd();
        //        sr.Close();
        //        return result.Contains("密码");//含有 密码 即未登陆返回true
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}


        
        static public Info Query()
        {
            return AccountInfo(HOST1);
        }

        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <param name="url">获取途径</param>
        /// <returns></returns>
        static private Info AccountInfo(string url)
        {
           
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 3000;
            Info info=new Info();
            try
            {
                StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
               string s = sr.ReadToEnd();

                string uid = s.Remove(0, s.IndexOf(@"uid='") + 5);
                uid = uid.Remove(uid.IndexOf(@"'")).Trim();
                info.Uid = uid;

                string time = s.Remove(0, s.IndexOf(@"time='") + 6);
                time = time.Remove(time.IndexOf(@"'")).Trim();
                Double.TryParse(time, out info.Time);

                string s_Flow = s.Remove(0, s.IndexOf(@"flow='") + 6);
                s_Flow = s_Flow.Remove(s_Flow.IndexOf(@"'")).Trim();
               if( Double.TryParse(s_Flow, out info.Flow))
                {
                    info.Flow /= 1024;
                }

                string fee = s.Remove(0, s.IndexOf(@"fee='") + 5);
                fee = fee.Remove(fee.IndexOf(@"'")).Trim();
               if( Double.TryParse(fee, out info.Fee))
                {
                    info.Fee /= 10000;
                }

                sr.Close();
                return info;
            }
            catch
            {
                return info;
            }
        }
    }
}
