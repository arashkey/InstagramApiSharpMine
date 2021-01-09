using Ionic.Zlib;
using System.IO;

namespace InstagramApiSharp.Helpers
{
    public static class ZlibHelper
    {
        public static byte[] Compress(byte[] bytes)
        {
            byte[] compressed;
            using (var compressedStream = new MemoryStream(bytes.Length))
            {
                using (var zlibStream = new ZlibStream(compressedStream, CompressionMode.Compress, CompressionLevel.Level9, true))
                    zlibStream.Write(bytes, 0, bytes.Length);
                compressed = new byte[compressedStream.Length];
                compressedStream.Position = 0;
                compressedStream.Read(compressed, 0, compressed.Length);

                compressedStream.Dispose();
            }
            return compressed;
        }
        public static byte[] Decompress(byte[] bytes)
        {
            byte[] compressed;
            using (var compressedStream = new MemoryStream(bytes.Length))
            {
                using (var zlibStream = new ZlibStream(compressedStream, CompressionMode.Decompress, true))
                    zlibStream.Write(bytes, 0, bytes.Length);
                compressed = new byte[compressedStream.Length];
                compressedStream.Position = 0;  
                compressedStream.Read(compressed, 0, compressed.Length);
            }
            return compressed;
        }
    }
}
