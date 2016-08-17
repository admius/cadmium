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
        private string code;
        private string value;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public void DebugPrint(TextWriter stream, int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                stream.Write("\t");
            }
            stream.WriteLine(code + ": " + value);
        }
    }
}
