using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing.EncodingAwareStrings
{
    public abstract class EncodingAwareString
    {
        /// <summary>
        /// Gets the Byte Representation of the Strings Content in the Strings Encoding.
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetBytes();

        public StringBuilder Content { get; protected set; } = new StringBuilder();

        public override String ToString() { return Content.ToString(); }

        public static implicit operator string(EncodingAwareString s)
        {
            if(s is not null)
                return s.ToString();
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The current</returns>
        public UTF8String ToUTF8() => new UTF8String(this.ToString());
        public UnicodeString ToUnicode() => new UnicodeString(this.ToString());
        public UTF32String ToUTF32() => new UTF32String(this.ToString());
        public ASCIIString ToAscii() => new ASCIIString(this.ToString());
    }
}
