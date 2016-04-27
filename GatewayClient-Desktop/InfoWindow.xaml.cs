using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GatewayClient;
using System.Windows.Forms;

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
                RFlowText.Text = value.RFlow;
                FlowText.Text = value.Flow;
                TimeText.Text = value.Time;
                SpeedText.Text = value.Speed;
                //TipText.Text = value.Tip;
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
                System.Windows.MessageBox.Show("注销异常，可能已经断网!", "网络异常");
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
                    tipText.Text = DateTime.Now.ToString("HH:mm:ss") + "网关连接正常";
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

        //取消手动刷新
        /*private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo();
        }*/

        //每秒更新
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
            var timer = new System.Timers.Timer(1001);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;

            System.Windows.Forms.Timer StopRectTimer = new System.Windows.Forms.Timer();
            StopRectTimer.Tick += new EventHandler(timer_Tick);
            StopRectTimer.Interval = 1001;
            StopRectTimer.Enabled = true;
            //this.TopMost = true;
            StopRectTimer.Start();
        }

        //定时更新信息
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(UpdateInfo));
        }

        //点击跳转至网费充值网页
        /*private void popweb_Click(object sender, RoutedEventArgs e)
        {
            //this.Process.Start("http://http://ecard.nankai.edu.cn/");
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "iexplore.exe";
            process.StartInfo.Arguments = "http://ecard.nankai.edu.cn/";
            process.Start();
        }*/

        /// <summary>
        /// 总是置顶
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void topBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
            topBtn.Content = Topmost ? "取消" : "置顶";
        }*/


        //private void Form1_Load(object sender, EventArgs e)
        //{


        //}

        //记录鼠标是否在窗体内
        private bool mouseinform = false;

        //鼠标在窗体内
        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseinform = true;
        }

        //鼠标在窗体外
        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseinform = false;
        }

        //利用Timer控件根据窗体位置进行隐藏和展示，如果在边缘则缩进，鼠标再次移到改位置则展示
        private void timer_Tick(object sender, EventArgs e)
        {
            if (mouseinform)
            {
                switch (this.StopAanhor)
                {
                    case AnchorStyles.Top:
                        {
                            this.Top = 0;
                            break;
                        }
                    case AnchorStyles.Left:
                        {
                            this.Left = 0;
                            break;
                        }
                    case AnchorStyles.Right:
                        {
                            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width;
                            break;
                        }
                    case AnchorStyles.Bottom:
                        {
                            this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height;
                            break;
                        }
                }
            }
            else
            {
                switch (this.StopAanhor)
                {
                    case AnchorStyles.Top:
                        {
                            this.Top = (this.Height - 8) * (-1);
                            break;
                        }
                    case AnchorStyles.Left:
                        {
                            this.Left = (-1) * (this.Width - 8);
                            break;
                        }
                    case AnchorStyles.Right:
                        {
                            this.Left = Screen.PrimaryScreen.Bounds.Width - 8;
                            break;
                        }
                    case AnchorStyles.Bottom:
                        {
                            this.Top = (Screen.PrimaryScreen.Bounds.Height - 8);
                            break;
                        }
                }
            }

        }

        //判断窗体位置
        internal AnchorStyles StopAanhor = AnchorStyles.None;
        private void mStopAnhor()
        {
            if (this.Top <= 0 && this.Left <= 0)
            {
                StopAanhor = AnchorStyles.None;
            }
            else if (this.Top <= 0)
            {
                StopAanhor = AnchorStyles.Top;
            }
            else if (this.Left <= 0)
            {
                StopAanhor = AnchorStyles.Left;
            }
            else if (this.Left >= Screen.PrimaryScreen.Bounds.Width - this.Width)
            {
                StopAanhor = AnchorStyles.Right;
            }
            else if (this.Top >= Screen.PrimaryScreen.Bounds.Height - this.Height)
            {
                StopAanhor = AnchorStyles.Bottom;
            }
            else
            {
                StopAanhor = AnchorStyles.None;
            }
        }

        private void hide_LocationChanged(object sender, EventArgs e)
        {
            this.mStopAnhor();
        }
    }
}
