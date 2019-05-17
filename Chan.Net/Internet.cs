using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chan.Net
{
    public static class Internet
    {
        public static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";

        public static async Task<string> DownloadString(string uri)
        {
            var req =
                    WebRequest.Create(uri) as HttpWebRequest;

            req.Headers["User-Agent"] = UserAgent;

            var resp = await req.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;

            using (var stream = new StreamReader(resp.GetResponseStream()))
            {
                return await stream.ReadToEndAsync();
            }
        }

        public static async Task<HttpWebResponse> PostFormData(string uri, Dictionary<string, object> postData)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;

            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            byte[] formData = GetMultipartFormData(postData, formDataBoundary);

            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + formDataBoundary;
            request.CookieContainer = new CookieContainer();
            request.Headers["Content-Length"] = formData.Length.ToString();
            request.Headers["User-Agent"] = UserAgent;

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
            Stream formDataStream = new MemoryStream();
            var needsCLRF = false;

            foreach (KeyValuePair<string, object> param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                {
                    formDataStream.Write(Encoding.GetBytes("\r\n"), 0, Encoding.GetByteCount("\r\n"));
                }

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    var fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(Encoding.GetBytes(header), 0, Encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(Encoding.GetBytes(postData), 0, Encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(Encoding.GetBytes(footer), 0, Encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            var formData = new byte[formDataStream.Length];
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

        private static readonly Encoding Encoding = Encoding.UTF8;

    }
}
