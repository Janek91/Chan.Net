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

    public class BannedException : Exception
    {
        public override string Message
        {
            get { return "You are banned!"; }
        }
    }

    public class ThreadNotFoundException : Exception
    {
        public override string Message
        {
            get { return "404: Thread was not found!"; }
        }
    }

    public class UnsupportedFiletypeException : Exception
    {
        public string FileType { get; private set; }

        public UnsupportedFiletypeException(string filetype)
        {
            FileType = filetype;
        }

        public override string Message
        {
            get { return "Unsupported file type: " + FileType; }
        }
    }

    public class UnknownPostException : Exception
    {
        public override string Message
        {
            get { return "Something went wrong that we didn't account for. Please tell a developer!"; }
        }
    }
}