using Newtonsoft.Json;
using System;

namespace InstagramApiSharp.Classes
{
    [Serializable]
    public class InstaTwoFactorLoginInfo
    {
        [JsonProperty("obfuscated_phone_number")]
        public string ObfuscatedPhoneNumber { get; set; }

        [JsonProperty("show_messenger_code_option")]
        public bool? ShowMessengerCodeOption { get; set; }

        [JsonProperty("two_factor_identifier")]
        public string TwoFactorIdentifier { get; set; }

        [JsonProperty("username")] public string Username { get; set; }

        [JsonProperty("phone_verification_settings")]
        public InstaPhoneVerificationSettings PhoneVerificationSettings { get; set; }
        [JsonProperty("sms_two_factor_on")]
        public bool? SmsTwoFactorOn { get; set; }
        [JsonProperty("totp_two_factor_on")]
        public bool? ToTpTwoFactorOn { get; set; }
        [JsonProperty("show_new_login_screen")]
        public bool? ShowNewLoginScreen { get; set; }
        [JsonProperty("show_trusted_device_option")]
        public bool? ShowTrustedDeviceOption { get; set; }

        [JsonProperty("whatsapp_two_factor_on")]
        public bool? WhatsappTwoFactorOn { get; set; }

        [JsonProperty("should_opt_in_trusted_device_option")]
        public bool? ShouldOptInTrustedDeviceOption { get; set; }

        [JsonProperty("pending_trusted_notification")]
        public bool? PendingTrustedNotification { get; set; }

        [JsonProperty("sms_not_allowed_reason")]
        public object SmsNotAllowedReason { get; set; } // always is null, but let us have it

        public static InstaTwoFactorLoginInfo Empty => new InstaTwoFactorLoginInfo();
    }
}