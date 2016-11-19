using System;

namespace YahooSports.Api.Exceptions
{
    public class SportsApiException : Exception
    {
        public enum ApiException
        {
            OAuthExpiredToken,
            GenericOAuth,
            WebException,
            Unknown
        }

        public ApiException ExceptionType { get; set; }

        public SportsApiException(ApiException type, string message = null, Exception innerException = null) : base(
            message, innerException)
        {
            ExceptionType = type;
        }

    }
}
