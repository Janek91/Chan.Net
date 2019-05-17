using System.IO;

namespace Chan.Net
{
    public class OptionalPostArgs
    {
        public OptionalPostArgs()
        {
            Name = "Anonymous";
        }

        public string Name;
        public string Options;
        public string Password;

        public Stream File;
        public string Filename;

        public static OptionalPostArgs Default
        {
            get { return new OptionalPostArgs(); }
        }
    }
}