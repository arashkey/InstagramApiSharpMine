#if WINDOWS_UWP

/*
 * UWP Port of https://github.com/advancedrei/HttpClient.Compression/
 */
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace InstagramApiSharp.Helpers.GZip
{
    public sealed class DeflateHttpContent : HttpContent
    {

#region Private Members

        private readonly Ionic.Zlib.DeflateStream m_stream;

#endregion

#region Methods

        public DeflateHttpContent(Stream stream)
        {
            m_stream = new Ionic.Zlib.DeflateStream(stream, Ionic.Zlib.CompressionMode.Decompress);
        }

        public DeflateHttpContent(Stream stream, HttpContentHeaders headers) : this(stream)
        {
            foreach (var pair in headers)
            {
                Headers.TryAddWithoutValidation(pair.Key, pair.Value);
            }
        }

        protected override Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context)
        {
            return m_stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            m_stream.Dispose();
            base.Dispose(disposing);
        }

#endregion

    }
}

#endif