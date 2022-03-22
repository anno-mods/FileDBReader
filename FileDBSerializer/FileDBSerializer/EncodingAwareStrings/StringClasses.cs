using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer.EncodingAwareStrings
{
    public sealed class UTF8String : EncodingAwareString
    {
        public UTF8String(String s)
        { 
            Content.Clear();
            Content.Append(s);
        }

        public UTF8String(byte[] b) : this(Encoding.UTF8.GetString(b)) { }

        public static implicit operator UTF8String(String s) => new UTF8String(s);
        public static implicit operator UTF8String(byte[] b) => new UTF8String(b);

        public override byte[] GetBytes() => Encoding.UTF8.GetBytes(Content.ToString());
    }

    public sealed class UnicodeString : EncodingAwareString 
    {
        public UnicodeString(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public UnicodeString(byte[] b) : this(Encoding.Unicode.GetString(b)) { }

        public static implicit operator UnicodeString(String s) => new UnicodeString(s);
        public static implicit operator UnicodeString(byte[] b) => new UnicodeString(b);

        public override byte[] GetBytes() => Encoding.Unicode.GetBytes(Content.ToString());
    }

    public sealed class UTF32String : EncodingAwareString
    {
        public UTF32String(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public UTF32String(byte[] b) : this(Encoding.UTF32.GetString(b)) { }

        public static implicit operator UTF32String(String s) => new UTF32String(s);
        public static implicit operator UTF32String(byte[] b) => new UTF32String(b);

        public override byte[] GetBytes() => Encoding.UTF32.GetBytes(Content.ToString());
    }

    public sealed class ASCIIString : EncodingAwareString
    {
        public ASCIIString(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public ASCIIString(byte[] b) : this(Encoding.ASCII.GetString(b)) { }

        public static implicit operator ASCIIString(String s) => new ASCIIString(s);
        public static implicit operator ASCIIString(byte[] b) => new ASCIIString(b);

        public override byte[] GetBytes() => Encoding.ASCII.GetBytes(Content.ToString());
    }
}
