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
        public const string Version = "3.0";
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
                if (now - lastUpdateTime >= REFRESH_TIME || info == null)
                {
                    //超过更新时间开始更新
                    double lastFlow = info.HasValue ? info.Value.flow : 0;
                    info = GetInfo();
                    if (lastFlow > 0 && lastUpdateTime > 0 && info.HasValue)
                    {
                        //计算流量速度
                        AccountInfo _info = info.Value;
                        _info.speed = (info.Value.flow - lastFlow) * 10000000.0 / (now - lastUpdateTime);
                        info = _info;
                    }
                    lastUpdateTime = now;
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
                if (String.IsNullOrEmpty(value) || value.Trim(trimChars) == DEFAULT_HOST)
                {
                    //津南网关
                    hostlist = new List<string>(1);
                    hostlist.Add(DEFAULT_HOST);
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
        /// 默认网关
        /// </summary>
        public const string DEFAULT_HOST = "http://202.113.18.106";


        const string query_path = "/";
        const string login_path = @":801/eportal/?c=ACSetting&a=Login";
        const string logout_path = @":801/eportal/?c=ACSetting&a=Logout&iTermType=1";
        const string sucess_title = @"<title>登录成功</title>";
        private const long REFRESH_TIME = 60 * 10000000;//信息刷新最短时间1分钟
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
            if (proxy == null)
            {
                req.Proxy = null;
            }
            else if (proxy.ToLower() != "default")
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
                        DefaultHost = host;
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
        /// 查询登录信息
        /// </summary>
        /// <returns></returns>
        static public AccountInfo? GetInfo()
        {
            AccountInfo? _info = null;
            foreach (var host in HostList)
            {
                _info = Query(host + query_path);
                if (_info.HasValue)
                {
                    return _info;
                }
            }
            return null;
        }


        /// <summary>
        /// 注销网关登陆
        /// 尝试列表中所有网关进行注销
        /// </summary>
        /// <returns>异常返回false</returns>
        static public bool Logout()
        {
            lastUpdateTime = 0;
            var path = logout_path + "&wlanuserip" + (info.HasValue ? info.Value.Ip : "null");
            info = null;
            pwd = null;
            uid = null;
            try
            {
                var request = CreateRequest(DefaultHost + path, "POST");
                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
                sr.ReadToEnd();
                sr.Close();
                var i = GetInfo();
                if (i == null)
                {
                    return true;
                }
                foreach (var host in HostList)
                {
                    string url = host + path;
                    request = CreateRequest(url, "POST");
                    sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
                    var s = sr.ReadToEnd();
                    //Console.Write(s);
                    sr.Close();
                }
                info = null;
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
                Double.TryParse(time, out info.time);

                //流量，换算单位
                string s_Flow = s.Remove(0, s.IndexOf(@"flow='") + 6);
                s_Flow = s_Flow.Remove(s_Flow.IndexOf(@"'")).Trim();
                if (Double.TryParse(s_Flow, out info.flow))
                {
                    info.flow /= 1024;
                }

                //余额
                string fee = s.Remove(0, s.IndexOf(@"fee='") + 5);
                fee = fee.Remove(fee.IndexOf(@"'")).Trim();
                if (Double.TryParse(fee, out info.fee))
                {
                    info.fee /= 10000;
                }

                //IP
                string ip = s.Remove(0, s.IndexOf(@"v4ip='") + 6);
                ip = ip.Remove(ip.IndexOf(@"'")).Trim();
                info.Ip = ip;
                return info;
            }
            catch
            {
#if DEBUG
                Console.WriteLine("query err");
#endif
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
}