using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfObject
    {
        private string type = "<unspecified>";
        public DxfObject()
        {
        }

        public string Type
        {
            get { return type; }
            set { type = value;  }
        }


        public virtual void DebugPrint(StreamWriter stream, int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                stream.Write("\t");
            }
            stream.WriteLine(type);
        }
    }
}
