using System;
using YahooSports.OAuthLib.Core;

namespace YahooSports.OAuthLib.Providers {
    /// <summary>
    /// Provides functionality purely for OAuth authentication mechanism.
    /// </summary>
    public class YahooOAuthProvider : OAuthConsumer, IOAuthProvider {

        private const string REQUEST_TOKEN_URL = "https://api.login.yahoo.com/oauth/v2/get_request_token";
        private const string USER_AUTH_URL = "https://api.login.yahoo.com/oauth/v2/request_auth";
        private const string REQUEST_ACCESS_URL = "https://api.login.yahoo.com/oauth/v2/get_token"; // Same for Refresh


        private readonly string consumerKey;
        private readonly string consumerSecret;
        private readonly string appUrl;
        private readonly string authCallback;

        public YahooOAuthProvider(string consumerKey, string consumerSecret, string appUrl, string authCallback) {
            this.consumerKey = consumerKey;
            this.appUrl = appUrl;
            this.consumerSecret = consumerSecret;
            this.authCallback = authCallback;
        }

        #region Yahoo Specific OAuth (Refresh Token)

        protected AuthorizeHeader GetRefreshTokenAuthorizationHeader(string url, string realm, string consumerKey, string consumerSecret, string token, string sessionHandle, string tokenSecret, SignatureMethod signatureMethod = SignatureMethod.HMACSHA1, string httpMethod = "POST") {

            var timestamp = OAuthUtils.GenerateTimeStamp();
            var nounce = OAuthUtils.GenerateNonce(timestamp);

            var protocolParameters = OAuthUtils.ExtractQueryStrings(url);
            protocolParameters.Add(new OAuthUtils.ProtocolParameter(OAuthProtocolParameter.ConsumerKey.GetStringValue(), consumerKey));
            protocolParameters.Add(new OAuthUtils.ProtocolParameter(OAuthProtocolParameter.SignatureMethod.GetStringValue(), signatureMethod.GetStringValue()));
            protocolParameters.Add(new OAuthUtils.ProtocolParameter(OAuthProtocolParameter.Timestamp.GetStringValue(), timestamp));
            protocolParameters.Add(new OAuthUtils.ProtocolParameter(OAuthProtocolParameter.Nounce.GetStringValue(), nounce));
            protocolParameters.Add(new OAuthUtils.ProtocolParameter(OAuthProtocolParameter.Version.GetStringValue(), OAuthUtils.OAuthVersion));
            protocolParameters.Add(new OAuthUtils.ProtocolParameter(OAuthProtocolParameter.Token.GetStringValue(), token));
            protocolParameters.Add(new OAuthUtils.ProtocolParameter(OAuthProtocolParameter.SessionHandle.GetStringValue(), sessionHandle));

            string signatureBaseString = OAuthUtils.GenerateSignatureBaseString(url, httpMethod, protocolParameters);
            System.Diagnostics.Debug.WriteLine(signatureBaseString);

            var signature = OAuthUtils.GenerateSignature(consumerSecret, signatureMethod, signatureBaseString, tokenSecret);
            return new AuthorizeHeader(realm, consumerKey, signatureMethod.GetStringValue(), signature, timestamp, nounce, OAuthUtils.OAuthVersion, token, null, sessionHandle);
        }

        #endregion

        #region IOAuthProvider Members

        public AccessToken RefreshToken(AccessToken oldToken) {
            var oAuthUtils = new OAuthUtils();
            var authorizationHeader = GetRefreshTokenAuthorizationHeader(REQUEST_ACCESS_URL, "", consumerKey, consumerSecret, oldToken.Token, oldToken.AddtionalProperties[OAuthProtocolParameter.SessionHandle.GetStringValue()], oldToken.TokenSecret);
            return MakeRequest<AccessToken>(REQUEST_ACCESS_URL, authorizationHeader);
        }

        /// <summary>
        /// Authenticates against Yahoo service using OAuth authentication, and returns AccessToken.
        /// </summary>
        /// <exception cref="OAuthProtocolException">If authentication fails.</exception>
        /// <param name="userAuthCallback">Callback to run when user needs to allow access to their account.</param>
        /// <returns>AccessToken</returns>
        public AccessToken Authenticate(Func<string, string> userAuthCallback) {
            RequestToken requestToken = GetOAuthRequestToken(REQUEST_TOKEN_URL, appUrl, consumerKey, consumerSecret, authCallback, SignatureMethod.HMACSHA1);
            string verifier = userAuthCallback.Invoke(USER_AUTH_URL + "?oauth_token=" + requestToken.Token);
            AccessToken accessToken = GetOAuthAccessToken(REQUEST_ACCESS_URL, appUrl, consumerKey, consumerSecret, requestToken.Token, verifier, requestToken.TokenSecret, SignatureMethod.HMACSHA1);

            return accessToken;
        }

        #endregion
    }
}
