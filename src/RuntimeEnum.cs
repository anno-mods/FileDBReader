using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    class RuntimeEnum
    {
        private Dictionary<String, String> Enum = new Dictionary<String, String>();
        public RuntimeEnum() 
        {

        }

        public void AddValue(String Key, String Value) 
        {
            Enum.Add(Key, Value);
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
    }
}
