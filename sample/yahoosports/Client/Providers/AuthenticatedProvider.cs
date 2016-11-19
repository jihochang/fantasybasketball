using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using YahooSports.Api.Sports.Models;

namespace YahooSports.Api.Providers
{
    public abstract class AuthenticatedProvider : IApiProvider
    {
        protected readonly Action<HttpWebRequest, string> addAuthHeaders;

        protected AuthenticatedProvider(Action<HttpWebRequest, string> addAuthHeaders)
        {
            this.addAuthHeaders = addAuthHeaders;
        }

        public abstract T ExecuteRequest<T>(string uri) where T : new();
    }
}
