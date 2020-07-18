using System;
using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace xXML.Benchmarks
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private const int BufferSize = 20;

        [Benchmark]
        public void ReadOnlyElementWrite()
        {
            var body = new ReadOnlyElement("Body", new ReadOnlyAttr("xmlns", "http://tempuri.org/"))
            {
                ["Origin"] = "THR",
                ["Destination"] = "MHD",
            };

            var buffer = new ArrayBufferWriter<byte>();

            body.WriteTo(buffer);

            var xml = Encoding.UTF8.GetString(buffer.WrittenSpan);
        }

        [Benchmark]
        public void ReadOnlyElementWriteWithSpanBuffer()
        {
            var body = new ReadOnlyElement("Body", new ReadOnlyAttr("xmlns", "http://tempuri.org/"))
            {
                ["Origin"] = "THR",
                ["Destination"] = "MHD",
            };

            byte[]? bufferToReturnToPool = null;

            Span<byte> buffer = BufferSize <= 128
                ? stackalloc byte[BufferSize]
                : bufferToReturnToPool = ArrayPool<byte>.Shared.Rent(BufferSize);

            if (bufferToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(bufferToReturnToPool);
            }

            body.WriteTo(in buffer);

            var xml = Encoding.UTF8.GetString(buffer);
        }

        [Benchmark]
        public void XElementWrite()
        {
            var stream = new MemoryStream();
            XNamespace xln = "http://tempuri.org/";

            var body = new XElement(xln + "Body",
                new XElement(xln + "Origin", "THR"),
                new XElement(xln + "Destination", "MHD"),
                new XAttribute("xmlns", "http://tempuri.org/"));

            body.Save(stream, SaveOptions.DisableFormatting);

            stream.Position = 0;

            var xml = new StreamReader(stream).ReadToEnd();
        }
    }
}