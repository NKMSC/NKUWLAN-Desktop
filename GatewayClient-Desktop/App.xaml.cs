using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
        static Mutex mutex = new Mutex(true,"nku_gateway_client_desktop");

        public App()
        {
            CheckRunning();
            Gateway.Timeout = 500;
            var info = Gateway.Info;
            if (info != null || GatewayClient.Gateway.Login() == true)
            {
                Gateway.Timeout = Config.Timeout;
                TrayNotify.Start("网关已经登录成功，可以上网啦~");
                //Current.StartupUri = new Uri("InfoWindow.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                Gateway.Timeout = Config.Timeout;
                Current.StartupUri = new Uri("LoginWindow.xaml", UriKind.RelativeOrAbsolute);
            }

        }

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
