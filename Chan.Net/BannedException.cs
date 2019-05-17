using System;

namespace Chan.Net
{
    public class BannedException : Exception
    {
        public override string Message
        {
            get { return "You are banned!"; }
        }
    }
}