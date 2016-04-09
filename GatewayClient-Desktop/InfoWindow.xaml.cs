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
        /// <summary>
        /// 账号信息
        /// </summary>
        AccountInfo Info
        {
            set
            {
                UIDText.Text = value.Uid;
                FeeText.Text = value.fee < 100 ? value.fee.ToString("0.00") + "元" : value.fee.ToString("0.0");
                FlowText.Text = value.flow < 1000 ? value.flow.ToString("0.00") + "兆" : value.flow.ToString("0.0");
                TimeText.Text = value.time.ToString() + "秒";
            }
        }

        public InfoWindow()
        {
            InitializeComponent();
        }

        public bool Logout()
        {
            if (Gateway.Logout())
            {
                App.Current.MainWindow = new LoginWindow();
                this.Close();
                App.Current.MainWindow.Show();
                return true;
            }
            else
            {
                tipText.Text = "网关连接异常";
                MessageBox.Show("注销异常，可能已经断网!", "网络异常");
                return false;
            }
        }
        /// <summary>
        /// 更新信息
        /// </summary>
        /// <returns></returns>
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
            Logout();
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void hideBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            TrayNotify.Start();
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
