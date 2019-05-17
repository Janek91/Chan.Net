using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Chan.Net.Captchas
{
    public class Pass : ICaptcha
    {
        public string PassId { get; set; }

        public static async Task<Pass> LoginAsync(string token, string pin)
        {
            const string uri = "https://sys.4chan.org/auth";

            var postData = new Dictionary<string, object>();
            postData.Add("act", "do_login");
            postData.Add("id", token);
            postData.Add("pin", pin);

            HttpWebResponse response = await Internet.PostFormData(uri, postData);
            string text = Internet.ReadWebResponse(response);

            if (text.Contains("This Pass is already in use by another IP"))
            {
                throw new PassAlreadyInUseException();
            }

            if (text.Contains("Incorrect Token or PIN.") || text.Contains("Error: You have left one or more fields blank."))
            {
                throw new IncorrectCredentialsException();
            }

            if (response.Cookies["pass_id"] == null)
            {
                throw new PassException("Cookie was not set");
            }

            var newPass = new Pass();

            foreach (Cookie cookie in response.Cookies)
            {
                if (cookie.Name == "pass_id" && !string.IsNullOrEmpty(cookie.Value) && cookie.Value.Length > 10)
                {
                    newPass.PassId = cookie.Value;
                    break;
                }
            }

            return newPass;
        }

        public static Pass Login(string token, string pin)
        {
            return LoginAsync(token, pin).GetAwaiter().GetResult();
        }

        bool ICaptcha.Solved
        {
            get { return !string.IsNullOrEmpty(PassId); }
        }

        void ICaptcha.Authenticate(HttpWebRequest req, Dictionary<string, object> postData)
        {
            req.CookieContainer.Add(new Uri("https://4chan.org"), new Cookie("pass_id", PassId, "/", ".4chan.org"));
            req.CookieContainer.Add(new Uri("https://4chan.org"), new Cookie("pass_enabled", 1.ToString(), "/", ".4chan.org"));
        }
    }
}
