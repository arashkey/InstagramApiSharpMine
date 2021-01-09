#if NETSTANDARD

/*
 * Credit Ramtin Jokar
 * Github: https://github.com/ramtinak/FFmpegFa/
 * 
 */

namespace InstagramApiSharp.FFmpegFa
{
    public class ImageSize : VideoSize
    {
        public ImageSize() : base() { }
        public ImageSize(int width, int height) : base(width, height) { }
    }
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