#if !WINDOWS_UWP && !NETSTANDARD

/*
 * Credit Ramtin Jokar
 * Github: https://github.com/ramtinak/FFmpegFa/
 * 
 */

namespace InstagramApiSharp.FFmpegFa
{
    public class VideoSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public VideoSize() : this(0, 0) { }
        public VideoSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
#endif