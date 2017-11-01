using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chan.Net.Captchas
{
    public class LegacyCaptcha : ICaptcha
    {
        public string CaptchaKey { get; set; }
        public string Solution { get; set; }

        bool ICaptcha.Solved
        {
            get { return !string.IsNullOrEmpty(Solution); }
        }

        public Uri Image
        {
            get
            {
                return new Uri(@"http://www.google.com/recaptcha/api/image?c=" + CaptchaKey);
            }
        }

        void ICaptcha.Authenticate(HttpWebRequest req, Dictionary<string, object> nameValueCollection)
        {
            nameValueCollection.Add("recaptcha_challenge_field", CaptchaKey);
            nameValueCollection.Add("recaptcha_response_field", Solution);
        }

        public static async Task<LegacyCaptcha> RequestAsync()
        {
            const string googleUri = @"http://www.google.com/recaptcha/api/challenge?k=6Ldp2bsSAAAAAAJ5uyx_lx34lJeEpTLVkP5k04qc";
            string keyOut = String.Empty;
            string javascriptResp = String.Empty;

            javascriptResp = await Internet.DownloadString(googleUri);

            Regex rx = new Regex(@"challenge\s*:\s*'(.*)',", RegexOptions.IgnoreCase);
            var match = rx.Match(javascriptResp);

            if (match.Success)
                keyOut = match.Groups[1].Value;

            return new LegacyCaptcha()
            {
                CaptchaKey = keyOut
            };
        }

        public static LegacyCaptcha Request()
        {
            return RequestAsync().GetAwaiter().GetResult();
        }
    }
}