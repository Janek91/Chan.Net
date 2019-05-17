using System;

namespace Chan.Net
{
    public class PostedImage
    {
        public PostedImage(string board, long tim, string extension)
        {
            this.tim = tim;
            this.board = board;
            this.extension = extension;
        }

        private long tim;
        private string extension;
        private string board;

        public int Width { get; set; }
        public int Height { get; set; }
        public string Filename { get; set; }
        public string Md5Hash { get; set; }
        public int Filesize { get; set; }

        public Uri Image
        {
            get
            {
                return new Uri(string.Format("https://i.4cdn.org/{0}/{1}{2}", board, tim, extension));
            }
        }

        public Uri Thumbnail
        {
            get
            {
                return new Uri(string.Format("https://i.4cdn.org/{0}/{1}s{2}", board, tim, ".jpg"));
            }
        }
    }
}
