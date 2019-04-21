using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models.Business;
using InstagramApiSharp.Classes.ResponseWrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace InstagramApiSharp.Converters.Json
{
    //InstaUserStoriesFeedsResponse
    internal class InstaUserStoriesFeedsDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(InstaUserStoriesFeedsResponse);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var reel = token.ToObject<InstaUserStoriesFeedsResponse>();

            var t = token["reels"];
            foreach(var item in t)
            {
                var r = item.First.ToObject<InstaReelFeedResponse>();
                reel.Items.Add(r);
            }
            return reel;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
