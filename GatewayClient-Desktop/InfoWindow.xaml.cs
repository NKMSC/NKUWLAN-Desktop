using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
                FeeText.Text = value.Fee;
                FlowText.Text = value.Flow;
                TimeText.Text = value.Time;
                SpeedText.Text = value.Speed;
            }
        }
        private delegate bool TimerDispatcherDelegate();

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
                var login = Gateway.Login();
                if (login == true)
                {
                    tipText.Text = DateTime.Now.ToString("HH:mm:ss") + "已重新登录";
                    return true;
                 
                }else if (login==false)
                {
                    tipText.Text = DateTime.Now.ToString("HH:mm:ss") + "登录失败";
                }
                else
                {
                    tipText.Text = DateTime.Now.ToString("HH:mm:ss") + "网关连接常";
                }
                return false;
            }
            else
            {
                Info = info.Value;
                tipText.Text = DateTime.Now.ToString("HH:mm:ss") + " 更新";
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
            var timer = new Timer(60001);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        //定时更新信息
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(UpdateInfo));
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void topBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
            topBtn.Content = Topmost ? "取消" : "置顶";
        }
    }
}
