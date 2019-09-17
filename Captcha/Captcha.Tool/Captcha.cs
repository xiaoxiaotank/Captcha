using System;
using System.Collections.Generic;
using System.Text;

namespace Captcha.Tool
{
    public class Captcha
    {
        private byte[] _byteData;

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 二进制数据
        /// </summary>
        public byte[] ByteData
        {
            get => _byteData;
            set
            {
                _byteData = value ?? throw new ArgumentNullException(nameof(ByteData));
                Base64Data = "data:image/png;base64," + Convert.ToBase64String(_byteData);
            }
        }

        /// <summary>
        /// Base64数据
        /// </summary>
        public string Base64Data { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
