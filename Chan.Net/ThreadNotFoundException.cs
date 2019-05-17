using System;

namespace Chan.Net
{
    public class ThreadNotFoundException : Exception
    {
        public override string Message
        {
            get { return "404: Thread was not found!"; }
        }
    }
}