using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using YahooSports.Api.Sports;
using YahooSports.Api.Yql;

namespace YahooSports.Api.Providers
{
    public enum ApiProvider
    {
        Yql, Sports
    }

    public class ApiProviderFactory
    {
        public static IApiProvider CreateProvider(ApiProvider provider, Action<HttpWebRequest, string> addAuthHeaders = null)
        {
            switch (provider)
            {
                case ApiProvider.Yql:
                    return new YqlProvider(addAuthHeaders);
                //case ApiProvider.Sports:
                //    return new SportsProvider(addAuthHeaders);
                default:
                    throw new ArgumentException("Provider specified does not exist!", "provider");
            }
        }
    }
}
