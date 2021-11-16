using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader_Tests
{
    static class TestDataSources
    {
        internal static FileDBDocument BuildDocument<T>() where T : FileDBDocument, new()
        {
            var filedb = new T();

            //lets create a few tags. 

            Tag root1 = filedb.AddTag("TestRootOne");

            var Attr = filedb.AddAttrib("FloatAttrib_Child");
            Attr.Content = BitConverter.GetBytes(33.2f);
            root1.AddChild(Attr);

            var child1 = filedb.AddTag("None");
            root1.AddChild(child1);

            var root2 = filedb.AddAttrib("None");
            root2.Content = new UnicodeEncoding().GetBytes("Modders are gonna take over the world");

            var root3 = filedb.AddAttrib("StringAttrib_Root");
            root3.Content = new UTF32Encoding().GetBytes("Anno 1800 will release on Stadia, Xbox and PS5. Get ready to open your wallets for Season 4.");

            filedb.Roots.Add(root1);
            filedb.Roots.Add(root2);
            filedb.Roots.Add(root3);
            return filedb;
        }
    }
}
