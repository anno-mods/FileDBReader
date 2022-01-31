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

        public static implicit operator UTF8String(String s)
        {
            return new UTF8String(s);
        }

        public override byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(Content.ToString());
        }
    }

    public sealed class UnicodeString : EncodingAwareString 
    {
        public UnicodeString(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public static implicit operator UnicodeString(String s)
        {
            return new UnicodeString(s);
        }

        public override byte[] GetBytes()
        {
            return Encoding.Unicode.GetBytes(Content.ToString());
        }
    }

    public sealed class UTF32String : EncodingAwareString
    {
        public UTF32String(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public static implicit operator UTF32String(String s)
        {
            return new UTF32String(s);
        }

        public override byte[] GetBytes()
        {
            return Encoding.UTF32.GetBytes(Content.ToString());
        }
    }

    public sealed class ASCIIString : EncodingAwareString
    {
        public ASCIIString(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public static implicit operator ASCIIString(String s)
        {
            return new ASCIIString(s);
        }

        public override byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(Content.ToString());
        }
    }
}
