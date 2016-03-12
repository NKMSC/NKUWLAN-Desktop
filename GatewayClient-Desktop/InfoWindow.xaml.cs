using System;
using System.Windows;
using System.Windows.Input;
using GatewayClient;

namespace Desktop_GUI
{
    /// <summary>
    /// 信息显示窗口
    /// Info.xaml 的交互逻辑
    /// </summary>
    public partial class InfoWindow : Window
    {
        String UID;
        String PWD;
        AccountInfo Info
        {
            set
            {
                UIDText.Text = value.Uid;
                FeeText.Text = value.Fee < 100 ? value.Fee.ToString("0.00") + "元" : value.Fee.ToString("0.0");
                FlowText.Text = value.Flow < 1000 ? value.Flow.ToString("0.00") + "兆" : value.Flow.ToString("0.0");
                TimeText.Text = value.Time.ToString() + "秒";
            }
        }

        public InfoWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 自动登录
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        public InfoWindow(String uid, String pwd = null)
        {
            InitializeComponent();
            Title += "-[" + uid + " 自动重连]";
            UID = uid;
            PWD = pwd;
        }


        bool UpdateInfo()
        {
            var info = Gateway.Info;
            if (info == null)
            {
                if (Gateway.Login() == null)
                {

                }
                return false;
            }
            else
            {
                Info = info.Value;
                return true;
            }
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Gateway.Logout())
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                tipText.Text = "网关连接异常";
                MessageBox.Show("注销异常，可能已经断网!", "网络异常");
            }

        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void hideBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo();
            tipText.Text = DateTime.Now.ToString("HH:mm:ss") + " 更新";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }
    }
}
