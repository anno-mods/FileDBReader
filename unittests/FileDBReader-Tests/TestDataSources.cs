using FileDBSerializing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDBSerializing.Tests.TestData;
using FileDBReader_Tests.TestSerializationData;

namespace FileDBReader_Tests
{
    static class TestDataSources
    {
        internal static RootObject GetTestAsset()
        {
            RootObject root = new RootObject();
            root.RootCount = 5;
            ChildElement Child = new ChildElement() { ID = 1337 };
            root.DumbChild = Child;
            root.DumbManager = new SomethingManager() { Child = Child, SomethingCount = 69420, SomethingValue = 3.1415f };
            root.PrimitiveArray = new int[5] { 1337, 42, 69, 420, 31 };
            root.RefArray = new ChildElement[2] { new ChildElement() { ID = 69420 }, new ChildElement() { ID = 1234 } };
            root.SimpleString = "Modders gonna take over the world!";
            root.StringArray = new string[] { "There are no plans for a console version of Anno 1800, or to support controllers in the PC version of the game", "We only made this complete Console UI for fun" };
            return root;
        }

        internal static NewObject GetSmallTestAsset()
        {
            NewObject obj = new NewObject();
            obj.Child = new AnyChild() { 
                ID = 5,
                Name = "No Plans for a fifth season whatsoever",
                Description = "We continue to listen to your feedback"};
            obj.Count = 1337;
            obj.Value = 1.1415f;
            obj.IsNew = false;

            return obj;
        }

        internal static IFileDBDocument BuildDocument<T>() where T : IFileDBDocument, new()
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
