using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfDelimitedObject : DxfObject
    {
        private DxfObject header;
        private DxfObject body;
        private DxfObject footer;

        public DxfDelimitedObject(string type) : base(type)
        {
        }

        public DxfObject Header
        {
            set { header = Header; }
            get { return header; }
        }

        public DxfObject Footer
        {
            set { header = Header; }
            get { return header; }
        }

        public DxfObject Body
        {
            set { body = Body; }
            get { return body; }
        }

        public void DebugPrint(int indentCount)
        {
            header.debugPrint(indentCount);
            Console.WriteLine("----------");
            body.debugPrint(indentCount + 1);
            Console.WriteLine("----------");
            footer.debugPrint(indentCount);
        }
    }
}
