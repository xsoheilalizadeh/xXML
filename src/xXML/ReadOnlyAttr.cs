using System;
using System.Text;

namespace xXML
{
    public readonly struct ReadOnlyAttr
    {
        private readonly ReadOnlyMemory<byte> _name;
        private readonly ReadOnlyMemory<byte> _value;

        public ReadOnlyAttr(string name, string value)
        {
            _name = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes(name));
            _value = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes(value));
        }

        public ReadOnlySpan<byte> Name => _name.Span;

        public ReadOnlySpan<byte> Value => _value.Span;
    }
}
