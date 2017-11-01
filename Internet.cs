using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chan.Net
{
    public static class Internet
    {
        public static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";

        public static async Task<string> DownloadString(string uri)
        {
            HttpWebRequest req =
                    WebRequest.Create(uri) as HttpWebRequest;

#if NET45
            req.UserAgent = Internet.UserAgent;
#else
            req.Headers["User-Agent"] = UserAgent;
#endif

            HttpWebResponse resp = await req.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;

            using (var stream = new StreamReader(resp.GetResponseStream()))
            {
                return await stream.ReadToEndAsync();
            }
        }

        public static async Task<HttpWebResponse> PostFormData(string uri, Dictionary<string, object> postData)
        {
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            byte[] formData = GetMultipartFormData(postData, formDataBoundary);

            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + formDataBoundary;
            request.CookieContainer = new CookieContainer();
#if NET45
            request.UserAgent = Internet.UserAgent;
            request.ContentLength = formData.Length;
#else
            request.Headers["Content-Length"] = formData.Length.ToString();
            request.Headers["User-Agent"] = UserAgent;
#endif

            using (Stream requeStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
            {
                requeStream.Write(formData, 0, formData.Length);
            }

            return await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;
        }

        public static string ReadWebResponse(HttpWebResponse resp)
        {
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        internal static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Dispose();

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }

        private static readonly Encoding encoding = Encoding.UTF8;

       
    }
}