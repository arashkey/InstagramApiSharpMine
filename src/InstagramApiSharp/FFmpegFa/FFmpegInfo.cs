#if !WINDOWS_UWP && !NETSTANDARD

/*
 * Credit Ramtin Jokar
 * Github: https://github.com/ramtinak/FFmpegFa/
 * 
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InstagramApiSharp.FFmpegFa
{
    public class FFmpegInfo
    {
        public string Encoder { get; set; }
        public string Name { get; set; }
        public string BitRate { get; set; }
        public TimeSpan Duration { get; set; }
        public List<AudioStreamInfo> AudioStreams { get; internal set; } = new List<AudioStreamInfo>();
        public List<VideoStreamInfo> VideoStreams { get; internal set; } = new List<VideoStreamInfo>();
        internal FFmpegInfo(string content, string name)
        {
            if (string.IsNullOrEmpty(content))
                return;
            try
            {
                Name = name;
                var list = new List<string>(content.Split(new string[]
                {
                        Environment.NewLine,"\r\n","\n"
                }, StringSplitOptions.RemoveEmptyEntries));

                foreach (var item in list)
                {
                    if (item.Contains("encoder"))
                    {
                        try
                        {
                            //encoder         : Lavf56.1.100
                            var encoder = item.Substring(item.IndexOf("encoder"));
                            encoder = encoder.Substring(encoder.IndexOf(":") + 1);
                            Encoder = encoder.Trim();
                        }
                        catch { }
                    }
                    else if (item.Contains("Duration"))
                    {
                        try
                        {
                            //  Duration: 00:04:01.26, start: 0.000000, bitrate: 3239 kb/s
                            var split = item.Split(',');
                            var dur = split[0].Substring(split[0].IndexOf("Duration:") + "Duration:".Length).Trim().TrimStart().TrimEnd();

                            Duration = TimeSpan.Parse(dur);



                            foreach (var it in split)
                                if (it.Contains("kb"))
                                {
                                    BitRate = it.Substring(it.IndexOf("bitrate:") + "bitrate:".Length).Trim();
                                    break;
                                }
                        }
                        catch { }

                    }
                }

                var streams = new List<string>(content.Split(new string[]
                {
                   "Stream #"
                }, StringSplitOptions.RemoveEmptyEntries));
                if (streams.Count > 0)
                    streams.RemoveAt(0);

                VideoStreams = new List<VideoStreamInfo>();
                AudioStreams = new List<AudioStreamInfo>();
                int ix = 0;
                foreach (var item in streams)
                {
                    if (item.Contains("Video"))
                    {
                        VideoStreams.Add(new VideoStreamInfo(item, ix));
                    }
                    else if (item.Contains("Audio"))
                    {
                        AudioStreams.Add(new AudioStreamInfo(item, ix));
                    }
                    ix++;
                }
            }
            catch { }
        }
    }
    public class AudioStreamInfo
    {
        //Stream #0:1(und): Audio: aac (LC) (mp4a / 0x6134706D), 44100 Hz, stereo, fltp, 125 kb/s (default)
        //Stream #0:1:      Audio: aac (LC), 44100 Hz, stereo, fltp (default)

        public string Name { get; private set; }
        public string CodecName { get; private set; }
        public string BitRate { get; private set; }
        public string SamplingRate { get; private set; }

        internal AudioStreamInfo(string content, int id)
        {
            if (string.IsNullOrEmpty(content))
                return;
            try
            {
                if (content.ToLower().Contains("audio"))
                {
                    //Stream #0:1(und): Audio: aac (LC) (mp4a / 0x6134706D), 44100 Hz, stereo, fltp, 125 kb/s (default)
                    //Stream #0:1:      Audio: aac (LC), 44100 Hz, stereo, fltp (default)
                    content = content.Trim();
                    MatchCollection reg = Regex.Matches(content, @"\(([^)]*)\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (reg.Count > 0)
                    {
                        foreach (Capture item in reg)
                            content = content.Replace(item.Value, item.Value.Replace(",", "%$"));
                    }
                    var split = content.Split(',');
                    Name = $"Stream #{id} -Audio";
                    CodecName = split[0].Substring(split[0].IndexOf("Audio") + 6).Trim().TrimStart().TrimEnd().Replace("%$", ",");
                    foreach (var item in split)
                    {
                        if (item.Contains("Hz"))
                        {
                            SamplingRate = item.Trim();
                            SamplingRate = SamplingRate.Substring(0, SamplingRate.IndexOf(" "));
                            float i = float.Parse(SamplingRate.Trim());
                            i = i / 1000;
                            SamplingRate = i + " kHz";
                        }
                        else if (item.Contains("kb"))
                        {
                            BitRate = item.Substring(0, item.IndexOf("\r\n")).Trim().TrimStart().TrimEnd().Replace("%$", ",");
                        }
                    }
                }
            }
            catch { }
        }
    }
    public class VideoStreamInfo
    {
        //Stream #0:0(und): Video: h264 (High) (avc1 / 0x31637661), yuv420p(tv, bt709), 1920x1080 [SAR 1:1 DAR 16:9], 3107 kb/s, 23.98 fps, 23.98 tbr, 90k tbn, 47.95 tbc (default)
        //Video: h264 (High), yuv420p(tv, bt709, progressive), 1920x1080 [SAR 1:1 DAR 16:9], 25 fps, 25 tbr, 1k tbn, 50 tbc (default)
        public string Name { get; private set; }
        public string CodecName { get; private set; }
        public string ColorSpace { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public string FrameRate { get; private set; }
        public string BitRate { get; private set; }
        internal VideoStreamInfo(string content, int id)
        {
            if (string.IsNullOrEmpty(content))
                return;
            try
            {
                if (content.ToLower().Contains("video"))
                {
                    //Stream #0:0(und): Video: h264 (High) (avc1 / 0x31637661), yuv420p(tv, bt709), 1920x1080 [SAR 1:1 DAR 16:9], 3107 kb/s, 23.98 fps, 23.98 tbr, 90k tbn, 47.95 tbc (default)
                    //Video: h264 (High), yuv420p(tv, bt709, progressive), 1920x1080 [SAR 1:1 DAR 16:9], 25 fps, 25 tbr, 1k tbn, 50 tbc (default)

                    content = content.Trim();
                    MatchCollection reg = Regex.Matches(content, @"\(([^)]*)\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (reg.Count > 0)
                    {
                        foreach (Capture item in reg)
                            content = content.Replace(item.Value, item.Value.Replace(",", "%$"));
                    }

                    //var split = content.Split(',');
                    var split = content.Split(',');
                    Name = $"Stream #{id} -Video";

                    CodecName = split[0].Substring(split[0].IndexOf("Video:") + 6).Trim().TrimStart().TrimEnd().Replace("%$", ",");
                    if (split[1].Contains("("))
                        ColorSpace = split[1].Substring(0, split[1].IndexOf("(")).Trim().TrimStart().TrimEnd().Replace("%$", ",").ToUpper();
                    else
                        ColorSpace = split[1].Trim().TrimStart().TrimEnd().Replace("%$", ",").ToUpper();
                    var heightWidth = "";
                    split[2] = split[2].Trim().TrimStart().TrimEnd();
                    if (split[2].Contains(" "))
                        heightWidth = split[2].Substring(0, split[2].IndexOf(" ")).Trim().TrimStart().TrimEnd();
                    else heightWidth = split[2];
                    PixelWidth = int.Parse(heightWidth.Split('x')[0]);
                    PixelHeight = int.Parse(heightWidth.Split('x')[1]);

                    foreach (var item in split)
                    {
                        if (item.Contains("fps"))
                        {
                            FrameRate = item.Trim().TrimStart().TrimEnd().Replace("%$", ",").ToUpper();
                        }
                        else if (item.Contains("kb"))
                        {
                            BitRate = item.Trim().TrimStart().TrimEnd().Replace("%$", ",");
                        }
                    }
                }
            }
            catch { }
        }

    }
}

#endif
