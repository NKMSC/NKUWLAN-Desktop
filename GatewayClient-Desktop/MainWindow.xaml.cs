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
              Login();
            }
        }

        private bool Login()
        {
            string uid = UserNameBox.Text.Trim();
            string pwd = PwdBox.Password.Trim();
            if( Gateway.Login(uid, pwd))
            {
               var info= Gateway.Query();
                InfoBox.Text = "账号:" + info.Uid + "\n"
                    + "流量:" + info.Flow + "MB\n"
                    + "余额:" + info.Fee + "￥\n"
                    + "时间:" + info.Time + "s\n";
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
    }
}
