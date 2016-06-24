using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfListObject : DxfObject
    {
        private List<DxfObject> entries;

        public DxfListObject(string type) : base(type)
        {
        }

        public List<DxfObject> Entries
        {
            get { return entries; }
        }

        public void AddEntry(DxfObject entry)
        {
            entries.Add(entry);
        }

        public void DebugPrint(int indentCount)
        {
            foreach(DxfObject data in entries)
            {
                data.debugPrint(indentCount);
            }
        }
    }
}
