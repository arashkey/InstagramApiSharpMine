/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaAudioUpload
    {
        public string Uri { get; set; }

        public TimeSpan Duration { get; set; }

        public List<float> WaveformData { get; set; } = new List<float>();

        public int WaveformSamplingFrequencyHz { get; set; } = 10;

        internal string UploadId { get; set; }

        /// <summary>
        /// This is only for .NET core apps like UWP(Windows 10) apps
        /// </summary>
        public byte[] VoiceBytes { get; set; }
    }
}
