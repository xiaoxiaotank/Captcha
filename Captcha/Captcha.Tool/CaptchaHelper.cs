using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Captcha.Tool
{
    /// <summary>
    /// 验证码辅助类
    /// </summary>
    public class CaptchaHelper
    {
        /// <summary>
        /// 可用于充当验证码的字符
        /// </summary>
        private const string Letters = "2346789ABCDEFGHJKLMNPRTUVWXYZ";

        /// <summary>
        /// 生成字符码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateCaptchaCode(int length)
        {
            var random = new Random();
            var maxValue = Letters.Length - 1;
            var sb = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                sb.Append(Letters[random.Next(maxValue)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="captchaCode"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        public static Captcha GenerateCaptcha(string captchaCode, int imageWidth, int imageHeight)
        {
            using(var bitmap = new Bitmap(imageWidth, imageHeight))
            using(var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(GetRandomLightColor());
                DrawCaptchaCode(graphics, captchaCode, imageWidth, imageHeight);
                DrawDiscordLine(graphics, imageWidth, imageHeight);
                AdjustRippleEffect(bitmap);
                
                using(var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return new Captcha()
                    {
                        Code = captchaCode,
                        ByteData = ms.ToArray(),
                        Timestamp = DateTime.Now
                    };
                }
            }
        }

        /// <summary>
        /// 获取随机亮色
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        private static Color GetRandomLightColor()
        {
            var random = new Random();
            var low = LightColorConsts.Low;
            var high = LightColorConsts.High;

            int red = random.Next(high) % (high - low) + low;
            int green = random.Next(high) % (high - low) + low;
            int blue = random.Next(high) % (high - low) + low;

            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// 获取随机深色
        /// </summary>
        /// <returns></returns>
        private static Color GetRandomDeepColor()
        {
            var random = new Random();
            return Color.FromArgb(
                random.Next(DeepColorConsts.RedLow), 
                random.Next(DeepColorConsts.GreenLow), 
                random.Next(DeepColorConsts.BlueLow)
            );
        }
        
        /// <summary>
        /// 绘制验证码
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="captchaCode"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHight"></param>
        private static void DrawCaptchaCode(Graphics graphics, string captchaCode, int imageWidth, int imageHight)
        {
            using (var solidBrush = new SolidBrush(GetRandomDeepColor()))
            {
                var fontSize = GetFontSize(captchaCode.Length, imageWidth);
                using (var font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    for (int i = 0; i < captchaCode.Length; i++)
                    {
                        var random = new Random();
                        var shiftPixel = fontSize / 6;
                        var x = fontSize * i + random.Next(-shiftPixel, shiftPixel) + random.Next(-shiftPixel, shiftPixel);
                        var maxY = imageHight - fontSize;
                        if (maxY < 0)
                        {
                            maxY = 0;
                        }
                        var y = random.Next(0, maxY);
                        graphics.DrawString(captchaCode[i].ToString(), font, solidBrush, x, y);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制干扰线
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHight"></param>
        private static void DrawDiscordLine(Graphics graphics, int imageWidth, int imageHight)
        {
            using (var pen = new Pen(new SolidBrush(Color.Black), 1))
            {
                var random = new Random();
                var minLineCount = imageHight / 10;
                var maxLineCount = imageHight / 5;
                if(maxLineCount == 0)
                {
                    maxLineCount = 2;
                }

                for (int i = 0; i < random.Next(minLineCount, maxLineCount); i++)
                {
                    pen.Color = GetRandomDeepColor();
                    var startPoint = new Point(random.Next(0, imageWidth), random.Next(0, imageHight));
                    var stopPoint = new Point(random.Next(0, imageWidth), random.Next(0, imageHight));
                    graphics.DrawLine(pen, startPoint, stopPoint);

                    var bezierPoint1 = new Point(random.Next(0, imageWidth), random.Next(0, imageHight));
                    var bezierPoint2 = new Point(random.Next(0, imageWidth), random.Next(0, imageHight));
                    graphics.DrawBezier(pen, startPoint, bezierPoint1, bezierPoint2, stopPoint);
                }
            }
        }

        /// <summary>
        /// 调整波纹效果
        /// </summary>
        /// <param name="bitmap"></param>
        private static void AdjustRippleEffect(Bitmap bitmap)
        {
            short wave = 6;
            var width = bitmap.Width;
            var height = bitmap.Height;
            Point[,] points = new Point[width, height];

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var xOffset = wave * Math.Sin(2d * 3.1415 * y / 128d);
                    var yOffset = wave * Math.Cos(2d * 3.1415 * x / 128d);
                    var newX = x + xOffset;
                    var newY = y + yOffset;

                    if (newX > 0 && newX < width)
                    {
                        points[x, y].X = (int)newX;
                    }
                    else
                    {
                        points[x, y].X = 0;
                    }

                    if (newY > 0 && newY < height)
                    {
                        points[x, y].Y = (int)newY;
                    }
                    else
                    {
                        points[x, y].Y = 0;
                    }
                }
            }

            var bitmpClone = (Bitmap)bitmap.Clone();
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            var bitmapCloneData = bitmpClone.LockBits(new Rectangle(0, 0, bitmpClone.Width, bitmpClone.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            var scanline = bitmapData.Stride;
            var scan0 = bitmapData.Scan0;
            var srcScan0 = bitmapCloneData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;
                byte* pSrc = (byte*)(void*)srcScan0;

                var offset = bitmapData.Stride - bitmap.Width * 3;
                for (var y = 0; y < height; ++y)
                {
                    for (var x = 0; x < width; ++x)
                    {
                        var xOffset = points[x, y].X;
                        var yOffset = points[x, y].Y;

                        if (yOffset >= 0 && yOffset < height && xOffset >= 0 && xOffset < width)
                        {
                            if (pSrc != null)
                            {
                                p[0] = pSrc[yOffset * scanline + xOffset * 3];
                                p[1] = pSrc[yOffset * scanline + xOffset * 3 + 1];
                                p[2] = pSrc[yOffset * scanline + xOffset * 3 + 2];
                            }
                        }
                        p += 3;
                    }
                    p += offset;
                }
            }

            bitmap.UnlockBits(bitmapData);
            bitmpClone.UnlockBits(bitmapCloneData);
            bitmpClone.Dispose();
        }

        /// <summary>
        /// 获取字体大小
        /// </summary>
        /// <param name="captchCodeLength"></param>
        /// <param name="imageWidth"></param>
        /// <returns></returns>
        private static int GetFontSize(int captchCodeLength, int imageWidth)
        {
            var avgSize = imageWidth / captchCodeLength;
            return Convert.ToInt32(avgSize);
        }

        private static class LightColorConsts
        {
            public const int Low = 180;
            public const int High = 255;
        }

        private static class DeepColorConsts
        {
            public const int RedLow = 160;
            public const int GreenLow = 100;
            public const int BlueLow = 160;
        }
    }
}
