using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoMods.BBDom.EncodingAwareStrings
{
    /// <summary>
    /// Represents a String in UTF8 encoding
    /// </summary>
    public sealed class UTF8String : EncodingAwareString
    {
        private static Encoding _encoding = new UTF8Encoding(false, false);

        public UTF8String(String s)
        { 
            Content.Clear();
            Content.Append(s);
        }

        public UTF8String(byte[] b) : this(_encoding.GetString(b)) { }

        public static implicit operator UTF8String(String s) => new UTF8String(s);
        public static implicit operator UTF8String(byte[] b) => new UTF8String(b);

        public override byte[] GetBytes() => _encoding.GetBytes(Content.ToString());
    }

    /// <summary>
    /// Represents a String in Unicode/UTF16 encoding
    /// </summary>
    public sealed class UnicodeString : EncodingAwareString 
    {
        private static Encoding _encoding = new UnicodeEncoding(false, false, true);

        public UnicodeString(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public UnicodeString(byte[] b) : this(_encoding.GetString(b)) { }

        public static implicit operator UnicodeString(String s) => new UnicodeString(s);
        public static implicit operator UnicodeString(byte[] b) => new UnicodeString(b);

        public override byte[] GetBytes() => _encoding.GetBytes(Content.ToString());
    }

    /// <summary>
    /// Represents a String in UTF32 encoding
    /// </summary>
    public sealed class UTF32String : EncodingAwareString
    {
        private static Encoding _encoding = new UTF32Encoding(true, false, true);

        public UTF32String(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public UTF32String(byte[] b) : this(_encoding.GetString(b)) { }

        public static implicit operator UTF32String(String s) => new UTF32String(s);
        public static implicit operator UTF32String(byte[] b) => new UTF32String(b);

        public override byte[] GetBytes() => _encoding.GetBytes(Content.ToString());
    }

    /// <summary>
    /// Represents a String in ASCII encoding
    /// </summary>
    [Obsolete("ASCII does not support character validation! Use UTF8 instead")]
    public sealed class ASCIIString : EncodingAwareString
    {
        private static Encoding _encoding = new ASCIIEncoding();

        public ASCIIString(String s)
        {
            Content.Clear();
            Content.Append(s);
        }

        public ASCIIString(byte[] b) : this(_encoding.GetString(b)) { }

        public static implicit operator ASCIIString(String s) => new ASCIIString(s);
        public static implicit operator ASCIIString(byte[] b) => new ASCIIString(b);

        public override byte[] GetBytes() => _encoding.GetBytes(Content.ToString());
    }
}
