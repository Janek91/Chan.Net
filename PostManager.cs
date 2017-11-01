using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chan.Net.Captchas;

namespace Chan.Net
{
    public static class PostManager
    {
        public static string CleanPostMessage(string com)
        {
            if (string.IsNullOrEmpty(com)) return com;
            com = com.Replace("<br>", "\n");
            com = Regex.Replace(com, "<.+?>", "");
            com = WebUtility.HtmlDecode(com).Trim();
            return com;
        }

        public static async Task<uint> CreatePost(Board board, Thread thread, string message, ICaptcha captcha, OptionalPostArgs args)
        {
            string url = string.Format("https://sys.4chan.org/{0}/post", board.BoardId);

            var postData = new Dictionary<string, object>();

            postData.Add("MAX_FILE_SIZE", "4194304");
            postData.Add("mode", "regist");
            postData.Add("name", args.Name);
            postData.Add("com", message);

            if (thread != null)
                postData.Add("resto", thread.PostNumber.ToString());

            if (!string.IsNullOrEmpty(args.Options))
                postData.Add("email", args.Options);
             
            if (!string.IsNullOrEmpty(args.Password))
                postData.Add("pwd", args.Password);
            else
            {
                StringBuilder sb = new StringBuilder();
                Random r = new Random();

                for (int i = 0; i < 15; i++)
                {
                    sb.Append((char) (r.Next('a', 'z')));
                }
            
                postData.Add("pwd", sb.ToString());
            }
            
            if (!captcha.Solved)
                throw new CaptchaException();
            
            if (args.File != null)
            {
                string filename = "file";
                if (!string.IsNullOrEmpty(args.Filename))
                    filename = args.Filename;

                if (!mappings.ContainsKey(Path.GetExtension(filename).ToLower()))
                    throw new UnsupportedFiletypeException(Path.GetExtension(filename));

                byte[] file = new byte[args.File.Length];

                await args.File.ReadAsync(file, 0, file.Length).ConfigureAwait(false);

                postData.Add("upfile",
                    new Internet.FileParameter(file, filename,
                    mappings[Path.GetExtension(filename).ToLower()]));
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = new CookieContainer();

            
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            byte[] formData = Internet.GetMultipartFormData(postData, formDataBoundary);

            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + formDataBoundary;

#if NET45
            request.ContentLength = formData.Length;
            request.UserAgent = Internet.UserAgent;
#else
            request.Headers["Content-Length"] = formData.Length.ToString();
            request.Headers["User-Agent"] = Internet.UserAgent;
#endif

            captcha.Authenticate(request, postData);

            using (Stream requeStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
            {
                requeStream.Write(formData, 0, formData.Length);
            }

            var response = await request.GetResponseAsync() as HttpWebResponse;

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new ThreadNotFoundException();

            using (var respStream = new StreamReader(response.GetResponseStream()))
            {
                string text = respStream.ReadToEnd();

                if (text.Contains("CAPTCHA"))
                    throw new CaptchaException();

                if (text.Contains("banned") || text.Contains("warn"))
                    throw new BannedException();

                try
                {
                    var postIdMatch = Regex.Match(text, "no:(\\d+)");
                    string postIdStr = postIdMatch.Groups[1].Captures[0].Value;

                    return uint.Parse(postIdStr);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new UnknownPostException();
                }
            }
        }

        private static IDictionary<string, string> mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {".bmp", "image/bmp"},
                {".fla", "application/octet-stream"},
                {".gif", "image/gif"},
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".pdf", "application/pdf"},
                {".pfb", "application/octet-stream"},
                {".png", "image/png"},
                {".pnm", "image/x-portable-anymap"},
                {".pnz", "image/png"},
                {".webm", "video/webm"},
            };
    }

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