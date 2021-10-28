using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    public class RuntimeEnum
    {
        private Dictionary<String, String> Enum = new Dictionary<String, String>();
        public RuntimeEnum() 
        {

        }

        public void AddValue(String Key, String Value) 
        {
            try
            {
                Enum.Add(Key, Value);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Duplicate item in an enum: {0} <-> {1}. Please make sure that both are unique.", Key, Value);
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
                Console.WriteLine("An Enum did not contain a Value for the Key: {0}", Key);
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
                Console.WriteLine("An Enum did not contain a Value for the Key: {0}", Value);
                throw new Exception(); 
            }
        }

        public bool IsEmpty() 
        {
            return (Enum.Count() == 0);
        }
    }
}
