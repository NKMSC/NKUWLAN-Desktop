using System;
using System.Reflection;
using System.Windows;
using IWshRuntimeLibrary;

namespace Desktop_GUI
{
    /// <summary>
    /// 自动启动
    /// </summary>
    static class AutoStart
    {

        /// <summary>
        /// 是否开启自启动
        /// </summary>
        public static bool Enable
        {
            set
            {
                if (value)
                {

                    CreateLink(StartupPath, LinkPath, "南开网关自动登录客户端");
                }
                else
                {
                    Delete(LinkPath);
                }
            }
            get
            {
                //判断快捷方式是否存在
                if (!System.IO.File.Exists(LinkPath))
                {
                    return false;
                }
                else
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut;
                    shortcut = (IWshShortcut)shell.CreateShortcut(LinkPath);
                    if (shortcut.TargetPath != StartupPath)
                    {
                        //快捷方式路径不一致时更新路径
                        shortcut.TargetPath = StartupPath;
                        shortcut.Save();
                    }
                    shell = null;
                    shortcut = null;
                    return true;
                }
            }
        }

        /// <summary>
        /// 链接路径
        /// </summary>
        private static string LinkPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\NKU网关.lnk";
            }
        }

        private static string StartupPath = Assembly.GetEntryAssembly().Location;

        /// <summary>
        /// 将文件放到启动文件夹中开机启动
        /// </summary>
        /// <param name="setupPath">启动程序</param>
        /// <param name="linkPath">快捷方式位置</param>
        /// <param name="description">描述</param>
        private static void CreateLink(string setupPath, string linkPath, string description)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut;
            try
            {
                shortcut = (IWshShortcut)shell.CreateShortcut(linkPath);
                shortcut.TargetPath = setupPath;//程序路径
                shortcut.Description = description;//描述
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(setupPath);//程序所在目录
                shortcut.IconLocation = setupPath;//图标   
                shortcut.WindowStyle = 1;
                shortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "快捷方式创建出错");
            }
            finally
            {
                shell = null;
                shortcut = null;
            }
        }

        /// <summary>
        /// 删除快捷方式
        /// </summary>
        /// <param name="linkPath"></param>
        private static void Delete(string linkPath)
        {
            if (System.IO.File.Exists(linkPath))
                System.IO.File.Delete(linkPath);
        }
    }
}
