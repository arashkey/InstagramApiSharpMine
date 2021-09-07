#if WINDOWS_UWP
using System;
using System.Net;

namespace InstagramApiSharp.Helpers
{
    internal class WebProxy : IWebProxy
    {
        public Uri Address { get; set; }
        public ICredentials Credentials { get; set; }
        public WebProxy() { }
        public WebProxy(Uri address) => Address = address;
        public WebProxy(Uri address, string user, string password) : this(address)
        {
            Credentials = new NetworkCredential(user, password);
        }

        public Uri GetProxy(Uri destination)
        {
            return destination is null ? throw new ArgumentNullException(nameof(destination)) : Address;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
#endif