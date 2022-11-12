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
            root.RefList = new() { new ChildElement() { ID = 69420 }, new ChildElement() { ID = 1234 } };
            root.SimpleString = "Modders gonna take over the world!";
            root.StringList = new() { "There are no plans for a console version of Anno 1800, or to support controllers in the PC version of the game", "We only made this complete Console UI for fun" };
            root.RefArray = new ChildElement[] { new ChildElement() { ID = 11 }, new ChildElement() { ID = 12 } };
            return root;
        }


        internal static RenamedRootObject GetTestAssetRenamed()
        {
            RenamedRootObject root = new RenamedRootObject();
            root.UnknownCount = 5;
            ChildElement Child = new ChildElement() { ID = 1337 };
            root.DumbChild = Child;
            root.IntelligentManager = new SomethingManager() { Child = Child, SomethingCount = 69420, SomethingValue = 3.1415f };
            root.IntArray = new int[5] { 1337, 42, 69, 420, 31 };
            root.ChildArray = new ChildElement[] { new ChildElement() { ID = 11 }, new ChildElement() { ID = 12 } };
            root.SimpleString = "Modders gonna take over the world!";
            root.StringNotAList = new() { "There are no plans for a console version of Anno 1800, or to support controllers in the PC version of the game", "We only made this complete Console UI for fun" };
            root.RefNotAList = new() { new ChildElement() { ID = 69420 }, new ChildElement() { ID = 1234 } };
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

            result.ReferenceList = new List<DeserChild>()
            {
                new DeserChild() {ID = 69},
                new DeserChild() {ID = 420},
            };
            result.PrimitiveList = new List<short>() { (short)-1, (short)0, (short)1, short.MaxValue };

            result.IndexTuple = new Tuple<int, DeserChild>(1, new DeserChild() { ID = 111 });

            result.NestedTuple = new Tuple<int, Tuple<short[], string, DeserChild>>(
                2, 
                new Tuple<short[], string, DeserChild>(
                    new short[] { short.MinValue, short.MaxValue }, "Hello World!", new DeserChild() { ID = 222 }
                    )
                );

            result.ListOfTuples = new List<Tuple<int, DeserChild, DeserChild>>()
            {
                new Tuple<int, DeserChild, DeserChild>(1, new DeserChild(){ ID = 10 }, new DeserChild(){ID = 100}),
                new Tuple<int, DeserChild, DeserChild>(2, new DeserChild(){ ID = 20 }, new DeserChild(){ID = 200}),
                new Tuple<int, DeserChild, DeserChild>(3, new DeserChild(){ ID = 30 }, new DeserChild(){ID = 300}),
            };

            result.ArrayOfTuples = new Tuple<int, DeserChild, DeserChild>[]
            {
                new Tuple<int, DeserChild, DeserChild>(4, new DeserChild(){ ID = 40 }, new DeserChild(){ID = 400}),
                new Tuple<int, DeserChild, DeserChild>(5, new DeserChild(){ ID = 50 }, new DeserChild(){ID = 500}),
                new Tuple<int, DeserChild, DeserChild>(6, new DeserChild(){ ID = 60 }, new DeserChild(){ID = 600}),
            };

            result.FlatLongList = new List<long>() { long.MinValue, -1L, 0L, 1L, long.MaxValue };

            result.FlatTupleList = new List<Tuple<int, DeserChild, DeserChild>>()
            {
                new Tuple<int, DeserChild, DeserChild>(7, new DeserChild(){ ID = 70 }, new DeserChild(){ID = 700}),
                new Tuple<int, DeserChild, DeserChild>(8, new DeserChild(){ ID = 80 }, new DeserChild(){ID = 800}),
                new Tuple<int, DeserChild, DeserChild>(9, new DeserChild(){ ID = 90 }, new DeserChild(){ID = 900}),
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
            obj.StringArray = new string[2]{ "lol", "12345" };
            obj.TupleArray = new (byte, byte)[2]
            {
                (5,5),
                (07,16)
            };   
            
            obj.IntList = new()
            {
                1,
                4,
                5
            };
            obj.StringList = new()
            {
                "We are taking all the time we can with our game updates",
                "we prefer a good game to money",
                "All the balancing decisions are carefully calculated and not based on dicerolls and eastereggs"
            };
            obj.ReferenceList = new()
            {
                new() { DefaultStr = "String1"},
                new() { EncAwareStr = "EncAwareString"},
                new()
            };
            obj.TupleList = new()
            {
                (5, new AnyChild() { DefaultStr = "str123", EncAwareStr = "ff" }),
                (2, new AnyChild() { DefaultStr = "str456", EncAwareStr = "dd" })
            };
            obj.AnotherTupleList = new()
            {
                ("another test string", 1337),
                ("yet another season?", 5)
            };
            obj.Tuple = (5, (2, (3, 0)));
            obj.NestedList = new()
            {
                obj.ReferenceList,
                obj.ReferenceList
            };
            obj.FlatList = new()
            {
                new() { DefaultStr = "Never gonna give you up" },
                new() { EncAwareStr = "Never gonna let you down" },
                new() { DefaultStr = "Never gonna run around",
                            EncAwareStr = "and desert you!"}
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
