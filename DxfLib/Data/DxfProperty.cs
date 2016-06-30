using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfLib.Data
{
    public class DxfProperty
    {
        public string Key;
        public string Value;

        public DxfProperty(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public virtual void DebugPrint(TextWriter stream, int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                stream.Write("\t");
            }
            stream.WriteLine(Key + ": " + Value);
        }
    }
}
