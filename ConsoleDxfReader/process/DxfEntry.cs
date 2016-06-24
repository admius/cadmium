using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfEntry
    {
        public string code;
        public string value;

        public void debugPrint(int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine(code + ": " + value);
        }
    }
}
