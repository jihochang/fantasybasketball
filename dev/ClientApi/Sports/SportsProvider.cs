using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using YahooSports.Api.Providers;
using YahooSports.Api.Sports.Models;
using YahooSports.OAuthLib.Core;

namespace YahooSports.Api.Sports
{
    public class SportsProvider : AuthenticatedProvider
    {

        public SportsProvider(Action<HttpWebRequest, string> addAuthHeaders) : base(addAuthHeaders)
        {
        }

        public override T ExecuteRequest<T>(string uri)
        {
            // Create WebRequest, and add Authorization Header for authenticated requests
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            addAuthHeaders(webRequest, uri);

            // Start receiving the response
            try {
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse()) {
                    using (Stream str = response.GetResponseStream()) {
                        //XmlSerializer serializer = new XmlSerializer(typeof(YahooSports.Api.Sports.Models.League));

                        //StringWriter s = new StringWriter();
                        //serializer.Serialize(s, new YahooSports.Api.Sports.Models.League { CurrentWeek = "hello" });
                        //Console.WriteLine(s.ToString());
                        //Console.Out.Flush();

                        XmlSerializer serializer = new XmlSerializer(typeof(FantasyContent));

                        return (T)serializer.Deserialize(str);
                    }
                }
            }
            catch (WebException ex) {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized) {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream())) {
                        throw new OAuthProtocolException(reader.ReadToEnd(), ex);
                    }
                }
                else {
                    throw;
                }
            }
        }
    }
}
