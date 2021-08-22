/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.Helpers
{
    internal class InstaCookiesToAuthorizationHelper
    {
        public static string ConvertToAuthorization(string user, string sessionId)
        {
            if (user.IsEmpty() || sessionId.IsEmpty())
                return null;

            var data = new JObject
            {
                {"ds_user_id", user},
                {"sessionid", sessionId},
                {"should_use_header_over_cookies", true},
            };

            return "Bearer IGT:2:" + CryptoHelper.Base64Encode(data.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}
