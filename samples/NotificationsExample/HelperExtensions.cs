using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsExample
{
    static class HelperExtensions
    {
        internal static async Task<long> GetUserId(this IInstaApi api, string sourceUserId, string username = null)
        {
            long.TryParse(sourceUserId, out long userPk);
            if (userPk <= 0)
            {
                if (string.IsNullOrEmpty(username)) return -1;
                var userResult = await api.UserProcessor.GetUserAsync(username);
                if (!userResult.Succeeded) return -1;
                userPk = userResult.Value.Pk;
            }
            return userPk;
        }
        internal static string GetValueIfPossible(this Dictionary<string, string> keyValuePairs, string key)
        {
            if (keyValuePairs.Any(x => x.Key == key))
                return keyValuePairs[key];
            else
                return null;
        }
    }
}
