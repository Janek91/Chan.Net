using System.Collections.Generic;
using System.Net;

namespace Chan.Net.Captchas
{
    public interface ICaptcha
    {
        bool Solved { get; }
        void Authenticate(HttpWebRequest req, Dictionary<string, object> nameValueCollection);
    }
}