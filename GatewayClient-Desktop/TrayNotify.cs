using System;
using System.Windows.Forms;
using GatewayClient;

namespace Desktop_GUI
{
    public static class TrayNotify
    {
        //public static TrayNotify Instance = null;
        private static NotifyIcon Notify
        {
            get
            {
                if (_notify == null)
                {
                    InitialTray();
                }
                return _notify;
            }
        }
        private static NotifyIcon _notify = null;

        public static void Start(string msg = "单击此处可以隐藏和显示主界面")
        {
            Notify.Visible = true;
            ShowTips("网关客户端已经隐藏到托盘", msg, 2000);
        }

        private static void InitialTray()
        {
            _notify = new NotifyIcon();
            _notify.Text = "NKU网关客户端\n单击显示\n右键提示\n悬浮查看流量";
            _notify.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            _notify.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            //_notify.MouseMove += _notify_MouseMove;
            //注销菜单项
            MenuItem Logout = new MenuItem("注销[Logout]", Logout_Click);
            //登录菜单项
            //MenuItem Login = new MenuItem("登录", Login_Click);

            //设置
            //MenuItem Set = new MenuItem("设置", Set_Click);
            MenuItem View = new MenuItem("显示[Display]", View_Click);

            //退出菜单项
            MenuItem Close = new MenuItem("关闭[Close]", Close_Click);
            //关于
            MenuItem Ecard = new MenuItem("在线交网费", Ecard_Click);
            //关于
            MenuItem About = new MenuItem("关于[About]", About_Click);
            //
            MenuItem LogoutExit = new MenuItem("注销并退出", Logout_Exit_Click);

            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { Logout, View, Ecard,About, Close, LogoutExit };
            _notify.ContextMenu = new ContextMenu(childen);

        }


        /// <summary>
        /// 显示气球消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="time"></param>
        /// <param name="icon"></param>
        public static void ShowTips(string title, string text, int time = 2500, ToolTipIcon icon = ToolTipIcon.Info)
        {
            Notify.ShowBalloonTip(time, title, text, icon);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        //private static void Login_Click(object s, EventArgs e)
        //{
        //    if (Gateway.Login()!=null)
        //    {
        //        ShowTips("登录成功", "已经连上网关，可以正常上网了~", 1000);
        //    }
        //    else
        //    {
        //        ShowTips("登录失败", "目前无法接到网关", 2500, ToolTipIcon.Error);
        //    }
        //}

        /// <summary>
        ///显示窗口 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void View_Click(object sender, EventArgs e)
        {
            if (App.Current.MainWindow == null)
            {
                App.Current.MainWindow = new InfoWindow();
                App.Current.MainWindow.Show();
            }
            else if (App.Current.MainWindow.IsVisible)
            {
                App.Current.MainWindow.Hide();
            }
            else
            {
                App.Current.MainWindow.Show();
                App.Current.MainWindow.Activate();
            }
        }

        private static void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            var info = Gateway.Info;
            if (info == null)
            {
                _notify.Text = "当前离线";
            }
            else
            {
                _notify.Text = "账号:" + info.Value.Uid
                    + "\n流量:" + info.Value.Flow
                    + "\n余额:" + info.Value.Fee
                + "\n速度:" + info.Value.Speed;
            }
            if (e.Button == MouseButtons.Left)
            {
                View_Click(null, null);
            }
        }
        /// <summary>
        /// 退出选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Close_Click(object sender, EventArgs e)
        {
            ShowTips("Bye~", "See you next time!");
            Environment.Exit(0);
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Logout_Click(object sender, EventArgs e)
        {
            if (App.Current.MainWindow is InfoWindow)
            {
                (App.Current.MainWindow as InfoWindow).Logout();
            }
            else if (Gateway.Logout())
            {
                App.Current.MainWindow = new LoginWindow();
                App.Current.MainWindow.Show();
                ShowTips("注销成功", "已经断开网关,内网流量已经停止", 1000);
            }
            else
            {
                ShowTips("注销失败", "网络异常，当前无法连接到网关", 2500, ToolTipIcon.Error);
            }
        }

        /// <summary>
        /// 关于选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void About_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/NKMSC/NKUWLAN-Desktop");
            }
            catch { }
        }


        /// <summary>
        /// 注销并关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Logout_Exit_Click(object sender, EventArgs e)
        {
            if (Gateway.Logout())
            {
                ShowTips("已注销正在退出~", "See you next time!");
            }
            else
            {
                ShowTips("网络异常~", "已退出!");
            }

            Environment.Exit(0);
        }


        //点击跳转至网费充值网页
        private static void Ecard_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://ecard.nankai.edu.cn/");
            }
            catch { }
        }
    }
}
