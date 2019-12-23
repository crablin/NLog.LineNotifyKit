using NLog.Config;
using NLog.Targets;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        protected override void Write(LogEventInfo info)
        {
            var dataContent = GenerateLineNotifyRequest(info);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                var response = client.PostAsync("https://notify-api.line.me/api/notify", dataContent).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }


            }

        }

        private MultipartFormDataContent GenerateLineNotifyRequest(LogEventInfo info)
        {
            string message = Layout.Render(info);

            var result = new MultipartFormDataContent();

            if (info.Parameters != null)
            {
                foreach (var param in info.Parameters)
                {
                    if (param is ILineNotifyLogger lineNotifyLogger)
                    {
                        var request = lineNotifyLogger.ToLineNotifyRequest(info);

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