using System;

namespace Chan.Net
{
    public class UnknownPostException : Exception
    {
        public override string Message
        {
            get { return "Something went wrong that we didn't account for. Please tell a developer!"; }
        }
    }
}