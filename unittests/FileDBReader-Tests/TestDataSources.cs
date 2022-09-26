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

        internal static DeserObject GetDeserializerTestAsset()
        {
            DeserObject result = new();
            result.Reference = new DeserChild();
            result.Reference.ID = 1337;
            result.IntValue = 14;
            result.IntArray = new int[] { 1, 2, 3 };
            result.ReferenceArray = new DeserChild[]
                {
                    new DeserChild() { ID = 42},
                    new DeserChild() { ID = 34 }
                };
            result.EncodingAwareString = "teststring 1234";
            result.DefaultString = "Oh no you don't";
            return result;
        }

        internal static NewObject GetSmallTestAsset()
        {
            NewObject obj = new NewObject();
            obj.RefObject = new AnyChild() { 
                DefaultStr = "No Plans for a fifth season whatsoever",
                EncAwareStr = "We continue to listen to your feedback"};
            obj.PrimitiveObject = 1337;

            obj.PrimitiveArray = new int[] { 3, 1, 4, 1, 5 };
            obj.RefArray = new ArrayElement[2]
            {
                new ArrayElement() { ElementContent = 3 },
                new ArrayElement() { ElementContent = 42 }
            };
            obj.FlatArray = new ArrayElement[3]
            {
                new ArrayElement() { ElementContent = 18 },
                new ArrayElement() { ElementContent = 15},
                new ArrayElement() { ElementContent = 120 }
            };
            obj.StringArray = new string[2]{ "lol", "12345" };
            obj.TupleArray = new (string, int)[] {
                ("another test string", 1337),
                ("yet another season?", 5)
            };

            obj.counts = new()
            {
                size = 4,
                None = new CountsElementEntry[]
                {
                    new() { None = (5,5) },
                    new() { None = (7,22)}
                }
            };
            obj.IntList = new()
            {
                1,
                4,
                5
            };
            obj.ReferenceList = new()
            {
                new() { DefaultStr = "String1"},
                new() { EncAwareStr = "EncAwareString"},
                new()
            };
            obj.TupleList = new()
            {
                ("String1", 2, 3),
                ("String", 5, 1),
                ("Modders gonna take over the world", 5, 5),
                (" ", 0, 0)
            };
            obj.Tuple = (5, (2, (3, 0)));
            obj.ComplexTupleArray = new (byte, AnyChild)[]
            {
                (5, new AnyChild() { DefaultStr = "str123", EncAwareStr = "ff" }),
                (2, new AnyChild() { DefaultStr = "str456", EncAwareStr = "dd"})
            };

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
