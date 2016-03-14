/**********************************************************
 * Conifg.cs 配置读取库
 * 读取和生成配置 xml
 * 2016-03-11 Created by NewFuture
 * *******************************************************/
using System;
using System.IO;
using System.Xml;

namespace GatewayClient
{
    /// <summary>
    /// 配置读写
    /// </summary>
    public static class Config
    {
        public const String VERSION = "2.0";
        private const String FILE_NAME = "NKU_Gateway.config.xml";
        private static String _path = null;
        private static String PATH
        {
            get
            {
                //如果当前文件不存在则存于用户目录中
                if (_path == null)
                {
                    _path = File.Exists(FILE_NAME) ? FILE_NAME :
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + FILE_NAME;
                }
                return _path;
            }
        }
        /// <summary>
        /// 默认网关
        /// </summary>
        public static String DefaultHost
        {
            get
            {
                return GET("nku/hosts", "defaut");
            }
            set
            {
                SET("nku/hosts", value, "defaut");
            }
        }
        /// <summary>
        /// 超时时间限制
        /// </summary>
        public static int Timeout
        {
            get
            {
                string time = GET("nku/hosts", "timeout");
                return time == null ? Gateway.Timeout : int.Parse(time);
            }
            set
            {
                if (value > 0) SET("nku/hosts", value.ToString(), "defaut");
            }
        }
        /// <summary>
        /// 账号ID
        /// </summary>
        public static String UID
        {
            get
            {
                return GET("nku/account/uid");
            }
            set
            {
                SET("nku/account/uid", value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public static String PWD
        {
            get
            {
                return GET("nku/account/pwd");
            }
            set
            {
                SET("nku/account/pwd", value);
            }
        }

        /// <summary>
        /// 其他选项
        /// Conifg.Options["anything"]
        /// </summary>
        public static Setting Options = new Setting();

        /// <summary>
        /// 获取配置
        /// </summary>
        private static XmlDocument ConfigXml
        {
            get
            {
                if (_config == null)
                {
                    try
                    {
                        _config = new XmlDocument();
                        _config.Load(PATH);
                    }
                    catch
                    {//载入异常重新生成
                        Init();
                    }
                }
                return _config;
            }
        }
        private static XmlDocument _config = null;

        /// <summary>
        /// 初始化配置
        /// </summary>
        private static XmlDocument Init()
        {
            try
            {
                _config = new XmlDocument();
                //创建xml的根节点
                XmlElement rootElement = _config.CreateElement("nku");
                rootElement.SetAttribute("version", VERSION);
                rootElement.SetAttribute("getway", Gateway.Version);
                rootElement.SetAttribute("author", "NewFuture");
                _config.AppendChild(rootElement);

                //初始化第一层的第一个子节点hosts
                XmlElement hostsInfoElement = _config.CreateElement("hosts");
                hostsInfoElement.SetAttribute("default", Gateway.DefaultHost);
                hostsInfoElement.SetAttribute("timeout", Gateway.Timeout.ToString());
                rootElement.AppendChild(hostsInfoElement);

                //初始化第而层的第一个子节点账户信息
                XmlElement accountInfoElement = _config.CreateElement("account");
                accountInfoElement.SetAttribute("auto", "1");
                rootElement.AppendChild(accountInfoElement);

                XmlElement uidElement = _config.CreateElement("uid");
                accountInfoElement.AppendChild(uidElement);
                XmlElement pwdElement = _config.CreateElement("pwd");
                accountInfoElement.AppendChild(pwdElement);

                _config.Save(PATH);
                return _config;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="path">配置的路径</param>
        /// <param name="attr">配置的属性，为null时读取对应的inner值</param>
        /// <returns></returns>
        private static string GET(string path, string attr = null)
        {
            try
            {
                var node = ConfigXml.SelectSingleNode(path);
                return String.IsNullOrEmpty(attr) ? node.InnerText.Trim() : node.Attributes[attr].Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="value">值</param>
        /// <param name="attr">属性</param>
        private static void SET(string path, string value, string attr = null)
        {
            try
            {
                var node = ConfigXml.SelectSingleNode(path);
                if (String.IsNullOrEmpty(attr))
                {
                    node.InnerText = value;
                }
                else
                {
                    node.Attributes[attr].Value = value;
                }
                ConfigXml.Save(PATH);
            }
            catch { }
        }

        public class Setting
        {
            public String this[String key]
            {
                get
                {
                    return Config.GET("option/" + key);
                }
                set
                {
                    Config.SET("option/" + key, value);
                }
            }
        }
    }
}