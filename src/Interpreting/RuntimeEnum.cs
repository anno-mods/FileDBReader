using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    public record RuntimeEnum : IEnumerable
    {
        private Dictionary<String, String> Enum = new Dictionary<String, String>();
        public RuntimeEnum() 
        {

        }

        //define [] indexer for this class
        public String this[String i] => Enum[i];

        public void AddValue(String Key, String Value) 
        {
            try
            {
                Enum.SafeAdd(Key, Value);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("[ENUM]: Duplicate item in an enum: {0} <-> {1}. Please make sure that both are unique.", Key, Value);
            }
        }

        public String GetValue(String Key)
        {
            if (Enum.ContainsKey(Key))
            {
                return Enum[Key];
            }
            else
            {
                Console.WriteLine("[ENUM]: An Enum did not contain a Value for the Key: {0}", Key);
                throw new Exception();
            }
        }

        public String GetKey(String Value)
        {
            if (Enum.ContainsValue(Value))
                //ignore duplicate values, just take the first.
                return Enum.FirstOrDefault(x => x.Value == Value).Key;
            else
            {
                Console.WriteLine("[ENUM]: An Enum did not contain a Value for the Key: {0}", Value);
                throw new Exception(); 
            }
        }

        public bool IsEmpty()
        {
            return (Enum.Count() == 0);
        }

        public int Count()
        {
            return Enum.Count(); 
        }

        public IEnumerator GetEnumerator()
        {
            return Enum.GetEnumerator(); 
        }

        public bool ContainsKey(String s)
        {
            return Enum.ContainsKey(s); 
        }
    }
}
