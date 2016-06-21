using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DataObject
    {
        private string code;
        private string value = null;

        public DataObject(string code,  string value)
        {
            this.code = code;
            this.value = value;
        }

        public string Code
        {
            get { return this.code; }
        }

        public string Value
        {
            get { return this.value; }
        }

        public virtual void debugPrint(int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine(code + ": " + value);
        }
    }
}
