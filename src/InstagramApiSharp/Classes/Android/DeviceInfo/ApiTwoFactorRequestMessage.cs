using InstagramApiSharp.API;
using InstagramApiSharp.Helpers;
using Newtonsoft.Json;
using InstagramApiSharp.API.Versions;
#pragma warning disable IDE1006
namespace InstagramApiSharp.Classes.Android.DeviceInfo
{
    internal class ApiTwoFactorRequestMessage
    {
        internal ApiTwoFactorRequestMessage(string verificationCode, string username, string deviceId,
            string twoFactorIdentifier, string csrftoken, string deviceGuid, int trustThisDevice = 0, int verificationMethod = 1,
            string fbAccessToken = null)
        {
            verification_code = verificationCode;
            this.username = username;
            device_id = deviceId;
            two_factor_identifier = twoFactorIdentifier;
            _csrftoken = csrftoken;
            guid = deviceGuid;
            trust_this_device = trustThisDevice.ToString(); // 0 no, 1 yes
            verification_method = verificationMethod.ToString(); // 1 text
            fb_access_token = fbAccessToken;
        }

        public string verification_code { get; set; }
        public string _csrftoken { get; set; }
        public string two_factor_identifier { get; set; }
        public string username { get; set; }
        public string trust_this_device { get; set; }
        public string guid { get; set; }
        public string device_id { get; set; }
        public string verification_method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fb_access_token { get; set; }
        internal string GenerateSignature(InstaApiVersion apiVersion, string signatureKey)
        {
            if (string.IsNullOrEmpty(signatureKey))
                signatureKey = apiVersion.SignatureKey;
            return CryptoHelper.CalculateHash(signatureKey,
                JsonConvert.SerializeObject(this));
        }

        internal string GetMessageString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}