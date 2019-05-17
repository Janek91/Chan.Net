using System;

namespace Chan.Net
{
    public class CaptchaException : Exception
    {
        public override string Message
        {
            get { return "The captcha was incorrect!"; }
        }
    }
}
