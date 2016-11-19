using System;
using YahooSports.OAuthLib.Core;

namespace YahooSports.OAuthLib.Providers
{
    public interface IOAuthProvider
    {
        AccessToken Authenticate(Func<string, string> userAuthCallback);
        AccessToken RefreshToken(AccessToken oldToken);
    }
}