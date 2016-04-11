using System;
using System.Threading;
using System.Windows;
using GatewayClient;

namespace Desktop_GUI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static Mutex mutex = new Mutex(true, "nku_gateway_client_desktop");

        public App()
        {
            CheckRunning();
            Gateway.Timeout = 500;
            var info = Gateway.GetInfo();
            if (info != null)
            {
                if (String.IsNullOrEmpty(Config.PWD))
                {
                    //已经登录，但是为设定账号，显示信息窗
                    Current.StartupUri = new Uri("InfoWindow.xaml", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    //未设定账号，登录过
                    TrayNotify.Start(info.Value.Uid + "已经登录网关了~");
                }
            }
            else if (GatewayClient.Gateway.Login() == true)
            {
                //自动登录成功直接托盘
                Gateway.Timeout = Config.Timeout;
                TrayNotify.Start(Config.UID + "登录成功，可以上网啦~");
            }
            else
            {
                Gateway.Timeout = Config.Timeout;
                Current.StartupUri = new Uri("LoginWindow.xaml", UriKind.RelativeOrAbsolute);
            }
        }

        /// <summary>
        /// 检查运行状态,放在重复运行
        /// </summary>
        private static void CheckRunning()
        {
            if (App.mutex.WaitOne(TimeSpan.Zero, true))
            {
                App.mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("客户端已在运行");
                Environment.Exit(1);
            }
        }
    }

}
