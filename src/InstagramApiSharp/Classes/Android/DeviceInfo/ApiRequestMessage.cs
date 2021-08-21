﻿using InstagramApiSharp.API.Versions;
using InstagramApiSharp.Helpers;
using Newtonsoft.Json;
using System;

namespace InstagramApiSharp.Classes.Android.DeviceInfo
{
    internal class ApiRequestChallengeMessage : ApiRequestMessage
    {
    }
    public class ApiRequestMessage
    {
        private const string _countryCodeUIgViaPhoneId = "[{\"country_code\":\"$COUNTRYCODE$\",\"source\":[\"default\"]},{\"country_code\":\"$COUNTRYCODE$\",\"source\":[\"uig_via_phone_id\"]}]";
        private string _phoneId;
        readonly static Random Rnd = new Random();
        [JsonProperty("jazoest")]
        public string Jazoest { get; set; }
        [JsonProperty("country_codes")]
        public string CountryCodes { get; set; } = "[{\"country_code\":\"$COUNTRYCODE$\",\"source\":[\"default\"]}]";
        [JsonProperty("phone_id")]
        public string PhoneId { get { return _phoneId; } set { _phoneId = value; Jazoest = ExtensionHelper.GenerateJazoest(value); } }
        [JsonProperty("enc_password")]
        public string EncPassword { get; set; }

        [JsonProperty("_csrftoken")]
        public string CsrfToken { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("adid")]
        public string AdId { get; set; }
        [JsonProperty("guid")]
        public Guid Guid { get; set; }
        [JsonProperty("device_id")]
        public string DeviceId { get; set; }
        //[JsonProperty("_uuid")]
        //public string Uuid => Guid.ToString();
        [JsonProperty("google_tokens")]
        public string GoogleTokens { get; set; } = "[]";
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("login_attempt_count")]
        public string LoginAttemptCount { get; set; } = "0";
        [JsonIgnore()]
        public uint StartupCountryCode { get; set; }
        [JsonIgnore()]
        public bool UIgViaPhoneId = false; //uig_via_phone_id
        public static ApiRequestMessage CurrentDevice { get; private set; }
        internal string GetMessageString(bool isNewerApi)
        {
            var pass = Password;
            if (isNewerApi)
                Password = null;

            if (UIgViaPhoneId)
                CountryCodes = _countryCodeUIgViaPhoneId;

            var json = JsonConvert.SerializeObject(this,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            Password = pass;
            return json
                .Replace("$COUNTRYCODE$", StartupCountryCode.ToString());
        }
        internal string GenerateSignature(InstaApiVersion apiVersion, string signatureKey, bool isNewerApi, out string deviceid)
        {
            if (string.IsNullOrEmpty(signatureKey))
                signatureKey = apiVersion.SignatureKey;
            var res = CryptoHelper.CalculateHash(signatureKey,
                GetMessageString(isNewerApi));
            deviceid = DeviceId;
            return res;
        }
        internal bool IsEmpty()
        {
            if (string.IsNullOrEmpty(PhoneId)) return true;
            if (string.IsNullOrEmpty(DeviceId)) return true;
            if (Guid.Empty == Guid) return true;
            return false;
        }

        internal static string GenerateDeviceId()
        {
            return GenerateDeviceIdFromGuid(Guid.NewGuid());
        }

        internal static string GenerateUploadId()
        {
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            var uploadId = (long)timeSpan.TotalSeconds;
            return uploadId.ToString();
        }
        //internal static string GenerateRandomUploadIdOLD()
        //{
        //    var total = GenerateUploadId();
        //    var uploadId = total + rnd.Next(11111, 99999).ToString("D5");
        //    return uploadId;
        //}
        internal static string GenerateRandomUploadId()
        {
            return DateTime.UtcNow.ToUnixTimeMiliSeconds().ToString();
        }
        internal static string GenerateUnknownUploadId()
        {
            var mil = DateTime.UtcNow.ToUnixTimeMiliSeconds();
            var sec = DateTime.UtcNow.ToUnixTime();
            var s = mil + sec;
            s += s;
            s -= Rnd.Next(10000, 999999);
            s += Rnd.Next(1000, 9999);
            return s + Rnd.Next(123, 987).ToString();
        }
        public static ApiRequestMessage FromDevice(AndroidDevice device)
        {
            var requestMessage = new ApiRequestMessage
            {
                PhoneId = device.PhoneGuid.ToString(),
                Guid = device.DeviceGuid,
                DeviceId = device.DeviceId
            };
            return requestMessage;
        }

        public static string GenerateDeviceIdFromGuid(Guid guid)
        {
            var hashedGuid = CryptoHelper.CalculateMd5(guid.ToString());
            return $"android-{hashedGuid.Substring(0, 16)}";
        }
        //public static string GenerateDeviceIdFromGuid(Guid guid)
        //{
        //    var unixMiliSec = Math.Round((double)DateTime.UtcNow.ToUnixTimeMiliSeconds()).ToString() + Rnd.Next(6789, 9999).ToString();
        //    var hashedGuid = CryptoHelper.CalculateMd5(unixMiliSec);
        //    return $"android-{hashedGuid.Substring(0, 16)}";
        //}
    }
}
