#if !WINDOWS_UWP && !NETSTANDARD
/*
 * Credit Ramtin Jokar
 * Github: https://github.com/ramtinak/FFmpegFa/
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.FFmpegFa
{
    public class FFmpegProgress
    {
        //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x    
        public int Percent { get; set; } = 0;
        public TimeSpan CurrentTime { get; set; } = TimeSpan.FromMilliseconds(0);
        public int CurrentFrame { get; set; } = 0;
        public double CurrentFrameRate { get; set; } = 0;
        public string CurrentFileSize { get; set; } = string.Empty;
        public string CurrentBitRate { get; set; } = string.Empty;
        public string CurrentSpeed { get; set; } = string.Empty;
        public FFmpegInfo InputFileInfo { get; set; }
    }

    public delegate void FFmpegProgressChanged(FFmpeg sender, FFmpegProgress ffmpegProgress);
}

#endif
