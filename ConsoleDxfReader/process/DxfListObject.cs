using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfListObject : DxfObject
    {
        private List<DxfObject> entries = new List<DxfObject>();

        public DxfListObject() : base()
        {
            Type = "List";
        }

        public List<DxfObject> Entries
        {
            get { return entries; }
        }

        public void AddEntry(DxfObject entry)
        {
            entries.Add(entry);
        }

        public override void DebugPrint(StreamWriter stream, int indentCount)
        {
            base.DebugPrint(stream, indentCount);
            foreach(DxfObject data in entries)
            {
                data.DebugPrint(stream, indentCount + 1);
            }
        }
    }
}
