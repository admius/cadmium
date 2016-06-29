using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfLib.Data
{
    public class DxfEntry
    {
        public string code;
        public string value;

        public void DebugPrint(StreamWriter stream, int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                stream.Write("\t");
            }
            stream.WriteLine(code + ": " + value);
        }
    }
}
