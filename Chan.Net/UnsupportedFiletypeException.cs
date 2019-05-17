using System;

namespace Chan.Net
{
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
}