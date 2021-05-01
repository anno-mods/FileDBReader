using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FileDBReader
{
    /// <summary>
    /// converts all text in an xml file into hex strings using conversion rules set up in an external xml file
    /// </summary>
    class XmlExporter
    {
        Dictionary<Type, Func<String, Encoding, byte[]>> ConversionRules = new Dictionary<Type, Func<String, Encoding, byte[]>>
            {
                { typeof(bool),     (s, Encoding)   => BitConverter.GetBytes(bool.Parse(s))},
                { typeof(byte),     (s, Encoding)   => BitConverter.GetBytes(byte.Parse(s))},
                { typeof(sbyte),    (s, Encoding)   => BitConverter.GetBytes(sbyte.Parse(s))},
                { typeof(short),    (s, Encoding)   => BitConverter.GetBytes(short.Parse(s))},
                { typeof(ushort),   (s, Encoding)   => BitConverter.GetBytes(ushort.Parse(s))},
                { typeof(int),      (s, Encoding)   => BitConverter.GetBytes((int.Parse(s)))},
                { typeof(uint),     (s, Encoding)   => BitConverter.GetBytes((uint.Parse(s)))},
                { typeof(long),     (s, Encoding)   => BitConverter.GetBytes((long.Parse(s)))},
                { typeof(ulong),    (s, Encoding)   => BitConverter.GetBytes((ulong.Parse(s)))},
                { typeof(float),    (s, Encoding)   => BitConverter.GetBytes(float.Parse(s))},
                { typeof (String),  (s, Encoding)   => Encoding.GetBytes(s)}
            };

        public XmlExporter() {
            
        }

        public void Export(String docpath, String interpreterPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(docpath);

            XmlDocument interpreter = new XmlDocument();
            interpreter.Load(interpreterPath);

            Export(doc, interpreter, docpath);
        }

        public void Export(XmlDocument doc, XmlDocument interpreter, String docpath) {
            //default type
            var defaultType = Type.GetType("System." + interpreter.SelectSingleNode("/Converts/Default").Attributes["Type"].Value);
            var converts = interpreter.SelectNodes("/Converts/Converts/Convert");

            foreach (XmlNode x in converts) {
                var type = Type.GetType("System." + x.Attributes["Type"].Value);
                String Path = x.Attributes["Path"].Value;

                var Nodes = doc.SelectNodes(Path);
                foreach (XmlNode match in Nodes)
                {
                    //make unicode as the default encoding
                    Encoding encoding = new UnicodeEncoding();

                    //if another encoding is specified, take that
                    if (x.Attributes["Encoding"] != null)
                    {
                        encoding = Encoding.GetEncoding(x.Attributes["Encoding"].Value);
                    }

                    //look for 

                    //make pass the object down together with the type
                    String Text = match.InnerText;
                    byte[] converted = ConversionRules[type](Text, encoding);
                    String hex = ByteArrayToString(converted);
                    match.InnerText = hex;
                }
            }

            //get a combined path of all
            String xPath = "";
            bool isFirst = true;
            foreach (XmlNode convert in converts)
            {
                if (!isFirst)
                {
                    xPath += " | ";
                }
                xPath += convert.Attributes["Path"].Value;
                isFirst = false;
            }
            //select all text not in combined path#
            var Base = doc.SelectNodes("//*[text()]");
            var toFilter = doc.SelectNodes(xPath);

            var defaults = HexHelper.ExceptNodelists(Base, toFilter);

            //convert that to default type
            foreach (XmlNode y in defaults)
            {
                Encoding encoding = new UnicodeEncoding();
                try
                {
                    //make pass the object down together with the type
                    String Text = y.InnerText;
                    byte[] converted = ConversionRules[defaultType](Text, encoding);
                    String hex = ByteArrayToString(converted);
                    y.InnerText = hex;
                }
                catch (Exception e)
                {

                }
            }

            doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(docpath, "_e"), "xml"));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }
}
