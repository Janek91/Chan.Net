using System;

namespace Chan.Net.Captchas
{
    public class PassException : Exception
    {
        public PassException() : this(null)
        {
        }

        public PassException(string msg)
        {
            Message = msg;
        }

        public override string Message { get; }
    }
}