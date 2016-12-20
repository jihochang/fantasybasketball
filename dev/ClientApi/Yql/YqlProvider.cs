using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;
using System.Web;
using System.Xml.Serialization;
using YahooSports.Api.Providers;
using YahooSports.OAuthLib;
using YahooSports.OAuthLib.Core;

namespace YahooSports.Api.Yql
{

    /// <summary>
    /// This class represents a YQL API provider.
    /// </summary>
    public class YqlProvider : IApiProvider
    {
        public string Uri { get; private set; }
        private readonly Action<HttpWebRequest, string> addAuthHeaders;

        private const string YqlUrl = "http://fantasysports.yahooapis.com/fantasy/v2/game/";//"http://query.yahooapis.com/v1/yql?q=";

        public YqlProvider(Action<HttpWebRequest, string> addAuthHeaders = null)
        {
            this.addAuthHeaders = addAuthHeaders;
        }

        /// <summary>
        /// Do a synchronous request to the api.
        /// </summary>
        /// <param name="uri">Uri of the request</param>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <returns>A (XML) deserialized POCO representation of the web service resource.</returns>
        /// <exception cref="System.InvalidOperationException">If the response cannot be deserialized.</exception>
        public T ExecuteRequest<T>(string uri) where T : new()
        {
            Uri = uri = YqlUrl + uri;//uri.Replace("=", "%3D");// +HttpUtility.UrlPathEncode("&format=xml&diagnostics=true");

            // Create WebRequest, and (optionally) add Authorization Header for authenticated requests
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            AddAuthHeaders(webRequest, uri);

            // Start receiving the response
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            {
                using (Stream str = response.GetResponseStream())
                {
                    Console.WriteLine(new StreamReader(str).ReadToEnd());
                    return new T();
                    //XmlSerializer serializer = new XmlSerializer(typeof(T));

                    //return (T) serializer.Deserialize(str);
                }
            }
        }

        private void AddAuthHeaders(WebRequest request, string uri)
        {
            if (addAuthHeaders != null)
                addAuthHeaders((HttpWebRequest) request, uri);
        }
    }
}