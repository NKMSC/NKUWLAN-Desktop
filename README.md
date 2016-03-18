# GatewayClient-Desktop

南开网关登录客户端桌面版 NKU Gateway Client for Windows Desktop

### 特性(features)

* 网关登录 【done】 
* 网关注销 【done】
* 流量查询 【done】
* 保存账号 【done】
* 自动登录 【done】
* 托盘程序 【done】
* 开机启动 【done】
* 界面美化 【doing】
* 自动刷新 【doing】
* 关机注销
* 配置管理
* 流量监控
* 后台服务
* 账户管理

### 截图 

* 登录 ![登录](login.png)
* 信息 ![信息](info.png)
* 自动登录托盘显示 

### 引用

根据[NKU-Gateway](https://github.com/NewFuture/NKU-Gateway)
网关接口修改而成

登录界面修改自[QQIdea](https://github.com/JsonRuby/QQIdea)

图标 修改自网络南开表情包![](GatewayClient-Desktop/images/logo_256.png)

### 目录结构
```
__
│
│  GatewayClient-Desktop.sln [VS 2015解决方案]
│  LICENSE
│  README.md
│
└─GatewayClient-Desktop
    │
	│  App.xaml.cs  [APP 入口]
	│  AutoStart.cs  [开机启动]
    │  Config.cs     [配置读写]
    │  Gateway.cs    [网关接口]
    │  InfoWindow.xaml     [信息界面]
    │  InfoWindow.xaml.cs  [界面响应]
	│  LoginWindow.xaml     [登录界面]
    │  LoginWindow.xaml.cs  [界面响应]
    │	TrayNotify.cs	[托盘通知]
	└─images 图片资源

```

### 作者

@NewFuture

### 协议和授权
Apache2 License 开源协议
