using System;
using System.Collections.Generic;
using System.Text;
using InstagramApiSharp.Helpers;
namespace InstagramApiSharp.Classes
{
    internal static class GiphyUrls
    {
        //GET /v1/gifs/trending?api_key=hOMPjlOPV6vrfmsZzupLcBY9RH9IGwYN&limit=50&rating=pg HTTP/1.1
        ///v1/gifs/search?api_key=hOMPjlOPV6vrfmsZzupLcBY9RH9IGwYN&limit=50&rating=pg&q=iran
        private const string ApiKey = "tf5253BtnmB6If04j0SF9pLvkbBQuErB";
        private static readonly Uri BaseAddress = new Uri("https://api.giphy.com/");

        public static Uri GetTrendingUri(int count = 100)
        {
            if (!Uri.TryCreate(BaseAddress, "/v1/gifs/trending", out var instaUri))
                throw new Exception("Cant create URI for giphy trending");

            return instaUri
                .AddQueryParameter("api_key", ApiKey)
                .AddQueryParameter("limit", count.ToString())
                .AddQueryParameter("rating", "pg");
        }

        public static Uri GetSearchUri(string query, int count = 100)
        {
            if (!Uri.TryCreate(BaseAddress, "/v1/gifs/search", out var instaUri))
                throw new Exception("Cant create URI for giphy search");

            return instaUri
                .AddQueryParameter("api_key", ApiKey)
                .AddQueryParameter("limit", count.ToString())
                .AddQueryParameter("rating", "pg")
                .AddQueryParameter("q", query);
        }
    }
}
