using System;
using System.Net;
using YahooSports.Api.Constants;
using YahooSports.Api.Exceptions;
using YahooSports.Api.Providers;
using YahooSports.OAuthLib.Core;
using YahooSports.OAuthLib.Providers;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace YahooSports.Api
{
    public class SportsProvider : IApiProvider
    {
        public string AppUrl { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public AccessToken Token { get; private set; }

        internal string ConsumerKey { get; private set; }
        internal string ConsumerSecret { get; private set; }

        private readonly IOAuthProvider oauthProvider;

        private const SignatureMethod SignatureMethodType = SignatureMethod.HMACSHA1;

        public SportsProvider(string appUrl, string consumerKey, string consumerSecret, AccessToken token = null, string oauthCallback = OAuthConstants.DEFAULT_CALLBACK)
        {
            AppUrl = appUrl;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            Token = token;
            IsAuthenticated = token != null;
            oauthProvider = new YahooOAuthProvider(consumerKey, consumerSecret, appUrl, oauthCallback);
        }

        private readonly object authLock = new object();
        private bool isAuthenticating;

        #region Asynchronous Authentication

        public void AuthenticateAsync(Func<string, string> userAuthCallback, Action<Action> completeCallback)
        {
            lock (authLock)
            {
                if (isAuthenticating)
                    throw new InvalidOperationException("SportsClient is currently authenticating...");
                isAuthenticating = true;

                AuthenticateDelegate dlgt = Authenticate;
                dlgt.BeginInvoke(userAuthCallback, ar => completeCallback(() =>
                                                                              {
                                                                                  ((AuthenticateDelegate)ar.AsyncState)
                                                                                      .EndInvoke(ar);
                                                                                  IsAuthenticated = true;
                                                                                  isAuthenticating = false;
                                                                              }), dlgt);
            }
        }

        #endregion

        #region IApiProvider Implementation

        public T ExecuteRequest<T>(string uri) where T : new()
        {
            int retryCount = 0;
            bool retry;

            do
            {
                retry = false;

                // Create WebRequest, and add Authorization Header for authenticated requests
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                AddAuthHeaders(webRequest, uri);

                // Start receiving the response
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T));

                            return (T)serializer.Deserialize(stream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Thread.Sleep(500);

                    if (retryCount > 3)
                    {
                        throw ConvertException(ex);
                    }

                    retry = true;
                    retryCount++;

                }
            } while (retry);

            throw new Exception("Internal Jiho Error");
        }

        #endregion

        private readonly object refreshTokenLock = new object();

        public AccessToken RefreshToken()
        {
            lock (refreshTokenLock)
            {
                try
                {
                    Token = oauthProvider.RefreshToken(Token);
                    IsAuthenticated = true; //just in case (ex. this failed previously (sets to false), but we retry later with same token.)
                    return Token;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to refresh Token: " + ex.Message);
                    IsAuthenticated = false;
                    throw; // rethrow the exception
                }
            }
        }

        private delegate AccessToken AuthenticateDelegate(Func<string, string> userAuthCallback);

        public AccessToken Authenticate(Func<string, string> userAuthCallback)
        {
            return Token = oauthProvider.Authenticate(userAuthCallback);
        }

        private static SportsApiException ConvertException(Exception ex)
        {
            if (ex is OAuthProtocolException)
            {
                return new SportsApiException(SportsApiException.ApiException.GenericOAuth, innerException: ex);
            }
            if (ex is WebException)
            {
                HttpWebResponse response = (ex as WebException).Response as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string message = reader.ReadToEnd();
                        if (
                            message.IndexOf("expired", 0, message.Length,
                                            StringComparison.InvariantCultureIgnoreCase) >
                            -1)
                        {
                            return new SportsApiException(SportsApiException.ApiException.OAuthExpiredToken, innerException: ex);
                        }
                    }
                }
                return new SportsApiException(SportsApiException.ApiException.WebException, innerException: ex);
            }
            return new SportsApiException(SportsApiException.ApiException.Unknown, innerException: ex);
        }

        #region Private Request Helpers

        private string GetAuthHeaders(string uri, string method = "GET")
        {
            return new OAuthUtils().GetUserInfoAuthorizationHeader(uri, AppUrl, ConsumerKey,
                                                          ConsumerSecret, Token.Token, Token.TokenSecret,
                                                          SignatureMethodType, method).ToString();
        }

        private void AddAuthHeaders(WebRequest request, string uri)
        {
            request.Headers.Set("Authorization", GetAuthHeaders(uri, request.Method));
        }

        #endregion
    }
}