using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using GatewayClient;
namespace Desktop_GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            UidBox.Text = Config.UID;
            PwdBox.Password = Config.PWD;
        }

        /// <summary>
        /// 登录
        /// </summary>
        private void Login()
        {
            string uid = UidBox.Text.Trim();
            string pwd = PwdBox.Password.Trim();
            var result = Gateway.Login(uid, pwd);
            if (result == true)
            {
                if (saveCheckBox.IsChecked == true)
                {
                    Config.UID = uid;
                    Config.PWD = pwd;
                }
                else
                {
                    Config.PWD = null;
                    Config.UID = null;
                }
                //InfoWindow infoWindow = autoCheckBox.IsChecked == true ? new InfoWindow(uid, pwd) : new InfoWindow();
                App.Current.MainWindow = new InfoWindow();
                this.Close();
                App.Current.MainWindow.Show();
            }
            else
            {
                errBlock.Text = result == null ? "网络异常，无法连接到网关" : "密码错误或余额不足";
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
                Login();
            }
        }
        private void UidBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (String.IsNullOrEmpty(PwdBox.Password))
                {
                    PwdBox.Focus();
                }
                else
                {
                    Login();
                }
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        /// <summary>
        /// 退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            errBlock.Text = "";
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// 动画效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sbd = Resources["leafLeave"] as Storyboard;
            sbd.Begin();

            Storyboard baiyun = Resources["cloudMove"] as Storyboard;
            baiyun.Begin();
        }
    }
}
