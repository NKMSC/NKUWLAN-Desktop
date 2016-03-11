using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GatewayClient;
namespace GatewayClient_Desktop
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Gateway.DefaultHost = Config.DefaultHost;
            Gateway.Timeout = Config.Timeout;
            
            getStatus();
            string uid= Config.UID;
            string pwd = Config.PWD;

            UserNameBox.Text=uid;
            PwdBox.Password =pwd;
            if(!String.IsNullOrEmpty(uid)&&!String.IsNullOrEmpty(pwd))
            {
                Login();
            }
          
        }

        /// <summary>
        /// 按下enter键登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PwdBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PwdBox.Password = PwdBox.Password.Trim();
                if(Login())
                {
                    Config.PWD = PwdBox.Password.Trim();
                    Config.UID = UserNameBox.Text.Trim();
                }
            }
        }

        private bool Login()
        {
            string uid = UserNameBox.Text.Trim();
            string pwd = PwdBox.Password.Trim();
            if (Gateway.Login(uid, pwd))
            {
                getStatus();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Gateway.Logout();
            InfoBox.Text = "已注销";
        }

        private bool getStatus()
        {
            var info = Gateway.Info;
            if (info != null)
            {
                InfoBox.Text = "账号:" + ((AccountInfo)info).Uid + "\n"
                    + "流量:" + ((AccountInfo)info).Flow.ToString("0.0000") + " MB\n"
                    + "余额:" + ((AccountInfo)info).Fee.ToString("0.00") + " ￥\n"
                    + "时间:" + ((AccountInfo)info).Time + "s\n";
                return true;
            }
            else
            {
                InfoBox.Text = "离线[未登录]";
                return false;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Config.UID = null;           
            Config.PWD = null;
            PwdBox.Password = null;
            UserNameBox.Text = null;
        }
    }
}
