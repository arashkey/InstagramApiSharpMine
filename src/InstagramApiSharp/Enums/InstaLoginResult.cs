using System.Runtime.Serialization;

namespace InstagramApiSharp.Classes
{
    public enum InstaLoginResult
    {
        [EnumMember(Value = "Success")]
        Success,
        [EnumMember(Value = "BadPassword")]
        BadPassword,
        [EnumMember(Value = "InvalidUser")]
        InvalidUser,
        [EnumMember(Value = "TwoFactorRequired")]
        TwoFactorRequired,
        [EnumMember(Value = "Exception")]
        Exception,
        [EnumMember(Value = "ChallengeRequired")]
        ChallengeRequired,
        [EnumMember(Value = "LimitError")]
        LimitError,
        [EnumMember(Value = "InactiveUser")]
        InactiveUser,
        [EnumMember(Value = "CheckpointLoggedOut")]
        CheckpointLoggedOut,
        [EnumMember(Value = "UnusablePassword")]
        UnusablePassword
    }
}