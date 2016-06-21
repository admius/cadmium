using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDxfReader.process;
using ConsoleDxfReader.Io;

namespace ConsoleDxfReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string metaFile = args[0];
            string dxfFile = args[1];

            MetaReader metaReader = new MetaReader();
            metaReader.open(metaFile);

            List<string> dxfLines = LineReader.ReadFile(dxfFile);
            EntryReader entryReader = new EntryReader(dxfLines);

            //read the document structure
            DataObject docObject = new StructureObject("0", "doc", metaReader.DocDesciptor, entryReader);

        }
    }
}
