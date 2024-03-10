
using FileDBSerializer;
using System.Text;

var doc = new BBDocument();

var tag = doc.CreateTag("MyNodeName");
var attrib1 = doc.CreateAttrib("MyAttribName", 89f);
var attrib2 = doc.CreateAttrib("MyAttribWithContent", 53);
var stringAttrib = doc.CreateAttrib("MyStringAttribName", "Modders gonna take over the world", Encoding.UTF8);
var emptyTag = doc.CreateTag("Empty");

//create Node Structure
tag.AddChildren(attrib1, attrib2, stringAttrib);
doc.AddRoot(tag);
doc.AddRoot(emptyTag);

using var outStream = File.Create("lol.bb");
doc.WriteToStream(outStream, FileDBSerializing.BBDocumentVersion.V3);