using FileDBSerializer;
using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializer
{
    public static class CreateNodesExtension
    {
        public static Tag CreateTag(this IBBDocument doc, String Name)
        {
            var newTagId = doc.TagSection.GetOrCreateTagId(Name);
            return new Tag() { ID = newTagId, ParentDoc = doc as BBDocument };
        }

        public static Attrib CreateAttrib(this IBBDocument doc, String Name)
        {
            var newAttribId = doc.TagSection.GetOrCreateAttribId(Name);
            return new Attrib() { ID = newAttribId, ParentDoc = doc as BBDocument };
        }

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, byte[] content)
        {
            var attrib = CreateAttrib(doc, Name);
            attrib.Content = content;
            return attrib; 
        }

        #region Numerics 
        public static Attrib CreateAttrib(this IBBDocument doc, String Name, bool b) => 
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(b));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, byte b) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(b));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, sbyte b) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(b));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, short s) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(s));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, ushort u) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(u));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, int i) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(i));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, uint i) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(i));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, long i) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(i));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, ulong i) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(i));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, float i) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(i));

        public static Attrib CreateAttrib(this IBBDocument doc, String Name, double i) =>
            CreateAttrib(doc, Name, PrimitiveTypeConverter.GetBytes(i));

        #endregion
        public static Attrib CreateAttrib(this IBBDocument doc, String Name, String content, Encoding encoding)
            => CreateAttrib(doc, Name, encoding.GetBytes(content));
    }
}
