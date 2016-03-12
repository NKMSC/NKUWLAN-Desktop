using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using GatewayClient;

namespace Desktop_GUI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Gateway.Timeout = 500;
            var info = Gateway.Info;

            if (info != null || GatewayClient.Gateway.Login() == true)
            {
                Gateway.Timeout = Config.Timeout;
                Current.StartupUri = new Uri("InfoWindow.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                Gateway.Timeout = Config.Timeout;
                Current.StartupUri = new Uri("LoginWindow.xaml", UriKind.RelativeOrAbsolute);
            }
        }
    }
}
