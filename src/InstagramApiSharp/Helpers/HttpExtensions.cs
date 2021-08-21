using InstagramApiSharp.API;
using System;
using System.Net.Http.Headers;

namespace InstagramApiSharp.Helpers
{
    public static class HttpExtensions
    {
        public static Uri AddQueryParameter(this Uri uri, string name, string value, bool dontCheck = false)
        {
            if (!dontCheck)
                if (value == null || value == "" || value == "[]") return uri;

            if (value == null)
                value = "";
            var httpValueCollection = HttpUtility.ParseQueryString(uri);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri);
            var q = "";
            foreach (var item in httpValueCollection)
            {
                if (q == "") q += $"{item.Key}={item.Value}";
                else q += $"&{item.Key}={item.Value}";
            }
            ub.Query = q;
            return ub.Uri;
        }

        public static Uri AddQueryParameterIfNotEmpty(this Uri uri, string name, string value)
        {
            if (value == null || value == "" || value == "[]") return uri;

            var httpValueCollection = HttpUtility.ParseQueryString(uri);
            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);
            var ub = new UriBuilder(uri);
            var q = "";
            foreach (var item in httpValueCollection)
            {
                if (q == "") q += $"{item.Key}={item.Value}";
                else q += $"&{item.Key}={item.Value}";
            }
            ub.Query = q;
            return ub.Uri;
        }
        internal static void AddHeader(this HttpRequestHeaders headers,
            string name,
            string value,
            IInstaApi instaApi,
            bool removeHeader = false)
        {
            var currentCulture = HttpHelper.GetCurrentCulture();
            System.Globalization.CultureInfo.CurrentCulture = HttpHelper.EnglishCulture;

            if (removeHeader)
                headers.Remove(name);

            headers.Add(name, value);

            System.Globalization.CultureInfo.CurrentCulture = currentCulture;
        }
        internal static void AddHeader(this HttpContentHeaders headers, string name, string value, IInstaApi instaApi,
            bool removeHeader = false)
        {
            var currentCulture = HttpHelper.GetCurrentCulture();
            System.Globalization.CultureInfo.CurrentCulture = HttpHelper.EnglishCulture;

            if (removeHeader)
                headers.Remove(name);

            headers.Add(name, value);

            System.Globalization.CultureInfo.CurrentCulture = currentCulture;
        }
        internal static void AppendPriorityHeader(this HttpRequestHeaders headers, IInstaApi instaApi)
        {
            AppendPriorityHeader(headers, InstaApiConstants.HEADER_PRIORITY_VALUE_3, instaApi);
        }
        internal static void AppendPriorityHeader(this HttpRequestHeaders headers, string priority, IInstaApi instaApi)
        {
            if (instaApi.GetApiVersionType() > Enums.InstaApiVersionType.Version180)
                AddHeader(headers, InstaApiConstants.HEADER_PRIORITY, priority, instaApi, true);
        }
        internal static void AppendPriorityHeader(this HttpContentHeaders headers, IInstaApi instaApi)
        {
            AppendPriorityHeader(headers, InstaApiConstants.HEADER_PRIORITY_VALUE_3, instaApi);
        }
        internal static void AppendPriorityHeader(this HttpContentHeaders headers, string priority, IInstaApi instaApi)
        {
            if (instaApi.GetApiVersionType() > Enums.InstaApiVersionType.Version180)
                AddHeader(headers, InstaApiConstants.HEADER_PRIORITY, priority, instaApi, true);
        }
    }
}