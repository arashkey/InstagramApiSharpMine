using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaSendDirectMessageResponse : BaseStatusResponse
    {
        public List<InstaDirectInboxThreadResponse> Threads { get; set; } = new List<InstaDirectInboxThreadResponse>();
    }
}