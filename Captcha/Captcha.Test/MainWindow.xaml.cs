using Captcha.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Captcha.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(!int.TryParse(TxtCaptchaCodeLength.Text, out int length))
            {
                length = 4;
            }

            var captchaCode = CaptchaHelper.GenerateCaptchaCode(length);
            TxtCaptchaCode.Text = captchaCode;

            if(!int.TryParse(TxtImageWidth.Text, out int imageWidth))
            {
                imageWidth = 200;
            }
            if(!int.TryParse(TxtImageHeight.Text, out int imageHeight))
            {
                imageHeight = 50;
            }

            var captcha = CaptchaHelper.GenerateCaptcha(captchaCode, imageWidth, imageHeight);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = new MemoryStream(captcha.ByteData);
            bmp.EndInit();
            ImgCaptcha.Source = bmp;
            ImgCaptcha.Width = imageWidth;
            ImgCaptcha.Height = imageHeight;
        }
    }
}
