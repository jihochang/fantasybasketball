using System;
using YahooSports.Api.Sports.Models;

namespace YahooSports.Api.Providers
{
    public interface IApiProvider
    {
        /// <summary>
        /// Do a synchronous request to the api.
        /// </summary>
        /// <param name="uri">Uri of the request</param>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <returns>A deserialized POCO representation of the web service resource.</returns>
        /// <exception cref="System.InvalidOperationException">If the response cannot be deserialized.</exception>
        T ExecuteRequest<T>(string uri) where T : new();
    }

}