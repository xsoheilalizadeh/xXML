using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace xXML.Benchmarks
{
    [MemoryDiagnoser]
    [ShortRunJob]
    public class Benchmarks
    {
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