using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace FileDBReader
{
    //expected structure of interpreter
    /*<Converts>
          <Default Type ="int" />
          <Converts>
            <Convert Path ="//Text" Type ="String" />
          </Converts>
        </Converts>
     */

    /// <summary>
    /// converts hex strings in an xml file into their types using conversion rules set up in an external xml file.
    /// </summary>
    class XmlInterpreter
    {
        Dictionary<Type, Func<string, Encoding, String>> ConversionRules = new Dictionary<Type, Func<string, Encoding, String>>
            {
                { typeof(bool),   (s, Encoding) => HexHelper.ToBool(s).ToString()},
                { typeof(byte),   (s, Encoding) => byte.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(sbyte),  (s, Encoding) => sbyte.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(short),  (s, Encoding) => short.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(ushort), (s, Encoding) => ushort.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(int),    (s, Encoding) => int.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(uint),   (s, Encoding) => uint.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(long),   (s, Encoding) => long.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(ulong),  (s, Encoding) => ulong.Parse(HexHelper.flip(s), NumberStyles.AllowHexSpecifier).ToString() },
                { typeof(float),  (s, Encoding) => HexHelper.ToFloat(HexHelper.flip(s)).ToString() },
                { typeof(String), (s, Encoding) => HexHelper.FromHexString(s, Encoding) }
            };

        public XmlInterpreter() {
            
        }

        public void Interpret(String docPath, String InterpreterPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(docPath);

            XmlDocument interpreter = new XmlDocument();
            interpreter.Load(InterpreterPath);

            Interpret(doc, interpreter, docPath);
        }

        public void Interpret(XmlDocument doc, XmlDocument interpreter, String docpath) {
            //default type
            var defaultType = Type.GetType("System." + interpreter.SelectSingleNode("/Converts/Default").Attributes["Type"].Value);
            var converts = interpreter.SelectNodes("/Converts/Converts/Convert");

            foreach (XmlNode x in converts) {
                var type = Type.GetType("System." + x.Attributes["Type"].Value);

                String Path = x.Attributes["Path"].Value;

                var Nodes = doc.SelectNodes(Path);
                foreach (XmlNode match in Nodes) {

                    //make unicode as the default encoding
                    Encoding encoding = new UnicodeEncoding();

                    //if another encoding is specified, take that
                    if (x.Attributes["Encoding"] != null)
                    {
                        encoding = Encoding.GetEncoding(x.Attributes["Encoding"].Value);
                    }

                    //look for List arg, if list then split, else use normal stuff
                    if (x.Attributes["Structure"] != null)
                    {
                        switch (x.Attributes["Structure"].Value)
                        {
                            case "List":
                                String BinaryData = match.InnerText;

                                //get size of target datatype.
                                int bytesize = Marshal.SizeOf(type) * 2;
                                var List = HexHelper.Split(BinaryData, bytesize);

                                foreach (String s in List)
                                {
                                    //encoding gets ignored for int/float etc. in the conversion rules. 
                                    String converted = ConversionRules[type](BinaryData, encoding);
                                    match.InnerText = converted;
                                }
                                break;
                        }
                    }
                    else
                    {
                        String BinaryData = match.InnerText;

                        //encoding gets ignored for int/float etc. in the conversion rules. 
                        String converted = ConversionRules[type](BinaryData, encoding);
                        match.InnerText = converted;
                    }
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
                try {
                    String BinaryData = y.InnerText;
                    String converted = ConversionRules[defaultType](BinaryData, encoding);
                    y.InnerText = converted;
                }
                catch (Exception e) { 
                
                }
            }
            doc.Save(Path.ChangeExtension(HexHelper.AddSuffix(docpath, "_i"), "xml"));
        }
    }
}
