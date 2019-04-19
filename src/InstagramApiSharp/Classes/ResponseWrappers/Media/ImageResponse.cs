﻿using Newtonsoft.Json;
using System;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class ImageResponse
    {
        public Uri Uri => new Uri(Url);
        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("width")] public string Width { get; set; }

        [JsonProperty("height")] public string Height { get; set; }
    }
}