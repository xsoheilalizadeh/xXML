## xXML (Prototype) 🕸


### What is xXML?

It's an efficient XML writer/reader for .NET, and an alternative for `XElement`.

#### Features

- Write ✔
- Read ❌
- Indented Format ❌
- Serialization/Deserialization ❌

```c#
var body = new ReadOnlyElement("Body", new ReadOnlyAttr("xmlns", "http://tempuri.org/"))
{
    ["Origin"] = "THR",
    ["Destination"] = "MHD",
};

var buffer = new ArrayBufferWriter<byte>();

body.WriteTo(buffer);

var xml = Encoding.UTF8.GetString(buffer.WrittenSpan);
```

#### Benchmark

```

|               Method |     Mean |    Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |---------:|---------:|----------:|-------:|------:|------:|----------:|
| ReadOnlyElementWrite | 1.830 μs | 2.163 μs | 0.1186 μs | 0.4215 |     - |     - |    1.3 KB |
|        XElementWrite | 4.577 μs | 8.461 μs | 0.4638 μs | 3.9139 |     - |     - |  12.01 KB |
```
