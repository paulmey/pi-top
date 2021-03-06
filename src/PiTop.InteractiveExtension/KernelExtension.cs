﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using SixLabors.ImageSharp;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;
using Display = PiTop.Abstractions.Display;

namespace PiTop.InteractiveExtension
{
    public class KernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            Formatter.Register(typeof(Display), (d, writer) =>
            {
                switch (d)
                {
                    case Display display:
                        {
                            var id = Guid.NewGuid().ToString("N");
                            using var stream = new MemoryStream();
                            using var bitmapImage = display.Capture();
                            bitmapImage.SaveAsPng(stream);
                            stream.Flush();
                            var data = stream.ToArray();
                            var imgTag = CreateImgTag(data, id, bitmapImage.Height, bitmapImage.Width);
                        }
                        break;
                }
            }, HtmlFormatter.MimeType);
            return Task.CompletedTask;
        }

        private static PocketView CreateImgTag(byte[] data, string id, int height, int width)
        {
            var imageSource = $"data:image/png;base64, {Convert.ToBase64String(data)}";
            PocketView imgTag = img[id: id, src: imageSource, height: height, width: width]();
            return imgTag;
        }

    }
}
