using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace xXML
{

    public readonly struct ReadOnlyElement
    {
        private readonly ReadOnlyMemory<byte> _name;
        private readonly ReadOnlyMemory<byte> _value;

        private readonly List<ReadOnlyElement> _children;
        private readonly List<ReadOnlyAttr> _attributes;

        public ReadOnlyElement(string name)
        {
            _name = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes(name));
            _value = ReadOnlyMemory<byte>.Empty;
            _children = new List<ReadOnlyElement>();
            _attributes = new List<ReadOnlyAttr>();
        }

        public ReadOnlyElement(string name, string? value) : this(name)
        {
            _value = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(value));
        }


        public ReadOnlyElement(string name, string value, ReadOnlyAttr attr) : this(name, attr)
        {
            _value = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(value));
        }


        public ReadOnlyElement(string name, ReadOnlyAttr attr) : this(name)
        {
            _attributes.Add(attr);    
        }

        public ReadOnlyElement(string name, ReadOnlyElement element) : this(name)
        {
            _children.Add(element);
        }


        private static ReadOnlySpan<byte> LessThanSign => new[] { (byte)'<' };
        private static ReadOnlySpan<byte> GreaterThanSign => new[] { (byte)'>' };
        private static ReadOnlySpan<byte> CloseElementSign => new[] { (byte)'<', (byte)'/' };
        private static ReadOnlySpan<byte> EqualSign => new[] { (byte)'=' };
        private static ReadOnlySpan<byte> DoubleQuote => new[] { (byte)'\"' };
        private static ReadOnlySpan<byte> WhiteSpace => new[] { (byte)' ' };


        public ReadOnlySpan<byte> Name => _name.Span;

        public ReadOnlySpan<byte> Value => _value.Span;

        public IReadOnlyList<ReadOnlyElement> Children => _children;

        public IReadOnlyList<ReadOnlyAttr> Attributes => _attributes;

        public object this[string name]
        {
            set => _children.Add(new ReadOnlyElement(name, value.ToString()));
        }

        public ReadOnlyElement this[int index] => _children[index];

        public int Count => _children.Count;

        public void WriteTo(in IBufferWriter<byte> writer)
        {
            WriteFullElement(in writer, this);
        }

        public void WriteTo(in Span<byte> buffer)
        {
            WriteFullElement(in buffer, this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteFullElement(in Span<byte> buffer, in ReadOnlyElement child)
        {
            WriteElement(in buffer, child.Name, child.Value, child._attributes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteFullElement(in IBufferWriter<byte> writer, in ReadOnlyElement child)
        {
            WriteElement(writer, child.Name, child.Value, child._attributes);

            for (int i = 0; i < child.Count; i++)
            {
                WriteFullElement(writer, child[i]);
            }

            EndElement(writer, child.Name);
        }

        private void WriteElement(in Span<byte> buffer, in ReadOnlySpan<byte> name,
            in ReadOnlySpan<byte> value, in List<ReadOnlyAttr> attributes)
        {
            LessThanSign.CopyTo(buffer);
            name.CopyTo(buffer);
            
            for (int i = 0; i < attributes.Count; i++)
            {
                var attr = attributes[i];
                
                WhiteSpace.CopyTo(buffer);
                attr.Name.CopyTo(buffer);
                EqualSign.CopyTo(buffer);
                
                
                DoubleQuote.CopyTo(buffer);
                attr.Value.CopyTo(buffer);
                DoubleQuote.CopyTo(buffer);
            }
            
            GreaterThanSign.CopyTo(buffer);
            value.CopyTo(buffer);
        }

        private void WriteElement(in IBufferWriter<byte> writer, in ReadOnlySpan<byte> name,
            in ReadOnlySpan<byte> value, in List<ReadOnlyAttr> attributes)
        {
            writer.Write(LessThanSign);
            writer.Write(name);

            for (int i = 0; i < attributes.Count; i++)
            {
                var attr = attributes[i];

                writer.Write(WhiteSpace);
                writer.Write(attr.Name);
                writer.Write(EqualSign);

                writer.Write(DoubleQuote);
                writer.Write(attr.Value);
                writer.Write(DoubleQuote);
            }

            writer.Write(GreaterThanSign);

            writer.Write(value);
        }

        private void EndElement(in Span<byte> buffer, in ReadOnlySpan<byte> name)
        {
            CloseElementSign.CopyTo(buffer);
            name.CopyTo(buffer);
            GreaterThanSign.CopyTo(buffer);
        }

        private void EndElement(in IBufferWriter<byte> writer, in ReadOnlySpan<byte> name)
        {
            writer.Write(CloseElementSign);
            writer.Write(name);
            writer.Write(GreaterThanSign);
        }
    }
}
