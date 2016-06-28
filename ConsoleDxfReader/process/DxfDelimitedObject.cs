using System;
using System.Collections.Generic;
using System.IO;
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

        public DxfDelimitedObject() : base()
        {
        }

        public DxfObject Header
        {
            set {
                header = value;
                Type = header.Type + " Group";
            }
            get { return header; }
        }

        public DxfObject Footer
        {
            set { footer = value; }
            get { return footer; }
        }

        public DxfObject Body
        {
            set { body = value; }
            get { return body; }
        }

        public override void DebugPrint(StreamWriter stream, int indentCount)
        {
            base.DebugPrint(stream, indentCount);
            header.DebugPrint(stream, indentCount + 1);
            stream.WriteLine();
            if (body != null)
            {
                body.DebugPrint(stream, indentCount + 1);
                stream.WriteLine();
            }
            footer.DebugPrint(stream, indentCount + 1);
        }
    }
}
