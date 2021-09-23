/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.Helpers
{
    internal class InstaCookiesToAuthorizationHelper
    {
        private const string Bearer = "Bearer IGT:2:";
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

            return Bearer + CryptoHelper.Base64Encode(data.ToString(Formatting.None));
        }

        public static string ConvertFromAuthorization(string authorization)
        {
            if (authorization.IsEmpty())
                return null;

            authorization = authorization.Substring(authorization.IndexOf(Bearer) + Bearer.Length);
            
            var json = CryptoHelper.Base64Decode(authorization);

            var data = JsonConvert.DeserializeObject<JObject>(json);

            return data["sessionid"].Value<string>();
        }
    }
}
