using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace NLog.LineNotifyKit.Models
{
    public class LineNotifyRequest
    {
        /// <summary>
        /// Message of LineNotify.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// the Sticker package id of LineNotify.
        /// </summary>
        public string StickerPackageId { get; set; }
        /// <summary>
        /// the Sticker identifier of LineNotify.
        /// </summary>
        public string StickerId { get; set; }
        /// <summary>
        /// the Image thumbnail URL.
        /// </summary>
        public string ImageThumbnailUrl { get; set; }
        /// <summary>
        /// The image fullsize URL.
        /// </summary>
        public string ImageFullsizeUrl { get; set; }
        /// <summary>
        /// the Imamge file full path.
        /// </summary>
        public string ImageFileFullPath { get; set; }

    }
}
