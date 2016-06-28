using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfProperty
    {
        public string key;
        public string value;

        public DxfProperty(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public virtual void DebugPrint(StreamWriter stream, int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                stream.Write("\t");
            }
            stream.WriteLine(key + ": " + value);
        }
    }
}
