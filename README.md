NLog.LineNotifyKit
=========

[![NuGet](https://img.shields.io/nuget/v/NLog.LineNotifyKit.svg)](https://www.nuget.org/packages/NLog.LineNotifyKit/)

## Overview

Let Nlog use [Line Notify](https://notify-bot.line.me) to post message on your LINE.

Support .NET Core 2.0 and .NET Framework 4.6

## Features
- Post message to LINE Notify
- Async to send without missing any log.

## Usage
1. Go to [Line Notify](https://notify-bot.line.me), Login in your account
2. Go to [My Page](https://notify-bot.line.me/my/)
3. Click **Generate token**, input a displaed name and select your notification group.
4. Copy **AsscessToken** to NLog target setting.
5. Use LINE App, Add Line Notify to your notification group.

### nlog.config
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

  <extensions>
    <add assembly="NLog.LineNotifyKit" />
  </extensions>

  <targets>
    <targets>
    <target xsi:type="LineNotify"
            name="Line"
            layout="${message}"
            AccessToken="XXX"/>
  </targets>

  <rules>
    <logger name="*" minlevel="Debug" writeTo="Line" />
  </rules>
</nlog>
```

- **AccessToken** : the token has permission on Line notify post.


### Extension Assembly

- Don't forget config extension 

``` xml
<extensions>
    <add assembly="NLog.LineNotifyKit" />
</extensions>
```

## Async on Console

Avoid post slack api is closed when thread is end. you must be code that.

``` C#
var logger = _factory.GetCurrentClassLogger();

logger.Info("message");

LineNotifyLogQueue.WaitAsyncCompleted().Wait();

```

## Async Exception on .NET 4.5

If you get this exception message on website:

`InvalidOperationException: An asynchronous operation cannot be started at this time.`

Refer to [MSDN Blog](https://blogs.msdn.microsoft.com/webdev/2012/11/19/all-about-httpruntime-targetframework/)

You must be add `aspnet:UseTaskFriendlySynchronizationContext` on `web.config` appSetting 

``` xml
<configuration>
  <appSettings>
    <add key="aspnet:UseTaskFriendlySynchronizationContext" value="true" />
    <!-- other values -->
  </appSettings>
</configuration>
```

## License

Copyright (c) 2018 Crab Lin