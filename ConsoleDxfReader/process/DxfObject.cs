using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfObject
    {
        private string type;
        public DxfObject(string type)
        {
            this.type = type;
        }

        public string Type
        {
            get { return type; }
        }


        public virtual void debugPrint(int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine(type);
        }
    }
}
