using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AnnoMods.BBDom.XML
{
    public static class BBXMLExtensions
    {
        public static XmlDocument ToXmlDocument(this BBDocument doc)
        { 
            BBToXmlConverter converter = new BBToXmlConverter();
            return converter.ToXmlDocument(doc);
        }

        public static BBDocument ToBBDocument(this XmlDocument doc)
        {
            XmlToBBConverter converter = new XmlToBBConverter(); 
            return converter.ToBBDocument(doc);
        }
    }
}
