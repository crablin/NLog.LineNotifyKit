using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace NLog.LineNotifyKit
{
    [Target("LineNotify")]
    public class LineNotifyTarget : TargetWithLayout
    {
        private readonly int _currentProcessId = Process.GetCurrentProcess().Id;

        /// <summary>
        /// 發送 LineNotify 所需要的 通行代碼
        /// </summary>
        [RequiredParameter]
        public string AccessToken { get; set; }

        protected override void InitializeTarget()
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
            {
                throw new ArgumentOutOfRangeException("AccessToken", "Line Notify AccessToken cannot be empty.");
            }

            base.InitializeTarget();
        }

        protected override async void Write(AsyncLogEventInfo info)
        {
            try
            {
                var dataContent = GenerateLineNotifyRequest(info);

                if (!LineNotifyLogQueue.Counter.ContainsKey(_currentProcessId))
                {
                    LineNotifyLogQueue.Counter.TryAdd(_currentProcessId, new StrongBox<int>(0));
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                    
                    Interlocked.Increment(ref LineNotifyLogQueue.Counter[_currentProcessId].Value);

                    var response = await client.PostAsync("https://notify-api.line.me/api/notify", dataContent);
                    
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new HttpRequestException(await response.Content.ReadAsStringAsync());
                    }

                    Interlocked.Decrement(ref LineNotifyLogQueue.Counter[_currentProcessId].Value);
                }
            }
            catch (Exception e)
            {
                info.Continuation(e);
            }
        }

        private MultipartFormDataContent GenerateLineNotifyRequest(AsyncLogEventInfo info)
        {
            string message = Layout.Render(info.LogEvent);

            var result = new MultipartFormDataContent();

            if (info.LogEvent.Parameters != null)
            {
                foreach (var param in info.LogEvent.Parameters)
                {
                    if (param is ILineNotifyLogger lineNotifyLogger)
                    {
                        var request = lineNotifyLogger.ToLineNotifyRequest(info.LogEvent);

                        message = string.Concat(message, request.Message);

                        if (!string.IsNullOrEmpty(request.StickerPackageId))
                        {
                            result.Add(new StringContent(request.StickerPackageId), "stickerPackageId");
                        }

                        if (!string.IsNullOrEmpty(request.StickerId))
                        {
                            result.Add(new StringContent(request.StickerId), "stickerId");
                        }

                        if (!string.IsNullOrEmpty(request.ImageThumbnailUrl))
                        {
                            result.Add(new StringContent(request.ImageThumbnailUrl), "imageThumbnail");
                        }

                        if (!string.IsNullOrEmpty(request.ImageFullsizeUrl))
                        {
                            result.Add(new StringContent(request.ImageFullsizeUrl), "imageFullsize");
                        }

                        if (!string.IsNullOrEmpty(request.ImageFileFullPath))
                        {
                            using (var fileStream = new StreamContent(File.Open(request.ImageFileFullPath, FileMode.Open, FileAccess.Read)))
                            {
                                result.Add(fileStream, "imageFile", Path.GetFileName(request.ImageFileFullPath));
                            }
                        }
                    }
                }
            }

            result.Add(new StringContent(message), "message");

            return result;
        }
    }
}