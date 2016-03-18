/**********************************************************
 * Gateway.cs 南开网关接口库
 * 提供网关登录 查询 注销 等相关接口
 * 2016-03-10 Created by NewFuture
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace GatewayClient
{

    /// <summary>
    /// 提供登录相关方法
    /// </summary>
    public static class Gateway
    {
        /// <summary>
        /// 版本
        /// </summary>
        public const string Version = "2.1";
        /// <summary>
        /// 超时时间
        /// </summary>
        public static int Timeout = 2500;


        /// <summary>
        /// 获取登录状态和账号信息
        /// 未登录或在无法获取时返回null
        /// </summary>
        public static AccountInfo? Info
        {
            get
            {
                long now = DateTime.Now.Ticks;
                if (lastUpdateTime == 0 || now - lastUpdateTime < REFRESH_TIME)
                {
                    lastUpdateTime = now;
                    AccountInfo? _info = null;
                    foreach (var host in HostList)
                    {
                        _info = Query(host + query_path);
                        if (_info != null)
                        {
                            info = _info;
                            return info;
                        }
                    }
                }

                return info;
            }
        }
        public static AccountInfo? info;


        /// <summary>
        /// 获取和设置默认网关
        /// </summary>
        public static string DefaultHost
        {
            get { return HostList[0]; }
            set
            {
                char[] trimChars = { '\\', '/', ' ', '\n' };
                if (String.IsNullOrEmpty(value) || value.Trim(trimChars) == HOST_JINNAN)
                {
                    //津南网关
                    hostlist = new List<string>(2);
                    hostlist.Add(HOST_JINNAN);
                    hostlist.Add(HOST_BALITAI);
                }
                else if (value.Trim(trimChars) == HOST_BALITAI)
                {
                    //八里台网关
                    hostlist = new List<string>(2);
                    hostlist.Add(HOST_BALITAI);
                    hostlist.Add(HOST_JINNAN);

                }
                else
                {
                    //其他
                    HostList.Insert(0, value.Trim());
                }
            }
        }

        /// <summary>
        /// 网关列表，越靠前优先级越高
        /// </summary>
        private static List<string> HostList
        {
            get
            {
                if (hostlist == null)
                {
                    DefaultHost = null;
                }
                return hostlist;
            }
        }
        private static List<string> hostlist = null;

        /// <summary>
        /// 八里台网关
        /// </summary>
        public const string HOST_BALITAI = "http://202.113.18.110";
        /// <summary>
        /// 津南网关
        /// </summary>
        public const string HOST_JINNAN = "http://202.113.18.210";


        const string query_path = "/";
        const string login_path = ":801/eportal/?c=ACSetting&a=Login";
        const string logout_path = ":801/eportal/?c=ACSetting&a=Logout";
        const string sucess_title = @"<title>登录成功</title>";
        private const long REFRESH_TIME = 10 * 10000000;//信息刷新最短时间
        private static long lastUpdateTime = 0;

        private static string pwd = null;
        private static string uid = null;

        /// <summary>
        /// 登录网关
        /// 会遍历主机列表
        /// </summary>
        /// <param name="name">账号</param>
        /// <param name="password">密码</param>
        /// <returns>返回登录结果</returns>
        static public bool? Login(string name, string password)
        {
            if (!String.IsNullOrEmpty(name))
            {
                uid = name;
            }
            if (!String.IsNullOrEmpty(password))
            {
                pwd = password;
            }
            return Login();
        }

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static WebRequest CreateRequest(string url, string method = "GET")
        {
            WebRequest req = WebRequest.Create(url);
            var proxy = Config.Options["proxy"];
            //代理设置
            if(proxy==null)
            {
                req.Proxy=null;
            }else if(proxy.ToLower()!="default")
            {
                req.Proxy = new WebProxy(proxy, true);
            }
            
            req.Method = method;
            req.Timeout = Timeout;
            return req;

        }
        /// <summary>
        /// 再次登陆
        /// </summary>
        /// <returns>
        /// True 登录成功
        /// False登录失败
        /// Null 网络错误
        /// </returns>
        public static bool? Login()
        {
            //未指定读取配置
            if (uid == null && pwd == null)
            {
                uid = Config.UID;
                pwd = Config.PWD;
            }
            bool isNetworkErr = true;
            if (uid != null && pwd != null)
            {
                foreach (var host in HostList)
                {
                    var s = postLogin(host + login_path);
                    if (s == null)
                    {
                        continue;
                    }
                    else if (s.IndexOf(sucess_title) > 0)
                    {
                        //登录成功更新时间
                        lastUpdateTime = 0;
                        return true;
                    }
                    else
                    {
                        isNetworkErr = false;
                    }
                }
            }
            if (isNetworkErr)
            {
                return null;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 注销网关登陆
        /// 尝试列表中所有网关进行注销
        /// </summary>
        /// <returns>异常返回false</returns>
        static public bool Logout()
        {
            lastUpdateTime = 0;
            info = null;
            pwd = null;
            uid = null;
            try
            {
                foreach (var host in HostList)
                {
                    string url = host + logout_path;
                    var request = CreateRequest(url);
                    StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
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

        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <param name="url">获取途径</param>
        /// <returns>查询成功返回账号信息，失败返回null</returns>
        static private AccountInfo? Query(string url)
        {

            try
            {
                var req = CreateRequest(url);
                StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
                string s = sr.ReadToEnd();
                sr.Close();

                //查询UID，如果不存在则登录失败
                int index = s.IndexOf(@"uid='");
                if (index < 0)
                {
                    return null;
                }
                AccountInfo info = new AccountInfo();
                string uid = s.Remove(0, index + 5);
                uid = uid.Remove(uid.IndexOf(@"'")).Trim();
                info.Uid = uid;

                //解析登录时长
                string time = s.Remove(0, s.IndexOf(@"time='") + 6);
                time = time.Remove(time.IndexOf(@"'")).Trim();
                Double.TryParse(time, out info.Time);

                //流量，换算单位
                string s_Flow = s.Remove(0, s.IndexOf(@"flow='") + 6);
                s_Flow = s_Flow.Remove(s_Flow.IndexOf(@"'")).Trim();
                if (Double.TryParse(s_Flow, out info.Flow))
                {
                    info.Flow /= 1024;
                }

                //余额
                string fee = s.Remove(0, s.IndexOf(@"fee='") + 5);
                fee = fee.Remove(fee.IndexOf(@"'")).Trim();
                if (Double.TryParse(fee, out info.Fee))
                {
                    info.Fee /= 10000;
                }
                return info;
            }
            catch
            {
                return null;
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
                //提交表单
                var post = CreateRequest(login_url, "POST");
                post.ContentType = "application/x-www-form-urlencoded";
                byte[] toout = System.Text.Encoding.UTF8.GetBytes("DDDDD=" + uid + "&upass=" + pwd);
                post.ContentLength = toout.Length;
                var requeststream = post.GetRequestStream();
                requeststream.Write(toout, 0, toout.Length);

                var result = (new System.IO.StreamReader(post.GetResponse().GetResponseStream(), System.Text.Encoding.GetEncoding("GB2312"))).ReadToEnd();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 账号信息
    /// </summary>
    public struct AccountInfo
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Uid;
        /// <summary>
        /// 流量 单位MB
        /// </summary>
        public double Flow;
        /// <summary>
        /// 余额 单位￥
        /// </summary>
        public double Fee;
        /// <summary>
        /// 时间 单位s
        /// </summary>
        public double Time;
        public AccountInfo(string uid = null, double flow = 0, double fee = 0, double time = 0)
        {
            Uid = uid;
            Flow = flow;
            Fee = fee;
            Time = time;
        }
    }
}