using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxfLib.Data;
using DxfLib.IO;
using DxfLib.Parser;

namespace DxfLib
{
    public class DxfReader
    {
        private ParserFactory parserFactory;

        public void Init(string configFileName)
        {
            dynamic configData = JsonLoader.Open(configFileName);
            parserFactory = new ParserFactory();
            parserFactory.init(configData);

        }

        public DxfObject Open(string fileName)
        {
            //load dxf file
            List<string> dxfLines = LineReader.ReadFile(fileName);
            EntryReader entryReader = new EntryReader(dxfLines);

            //get the doc parser
            Parser.DxfParser docParser = parserFactory.GetParser("DOC");

            //read and parse the document
            DxfEntry entry;
            bool started = false;
            while ((entry = entryReader.ReadEntry()) != null)
            {
                //Console.WriteLine("Entry: " + entry.Code + ": " + entry.Value);
                if (!started)
                {
                    //this better be true
                    started = docParser.IsDelimiter(entry);
                    if (!started)
                    {
                        throw new Exception("First line of document is not a proper delimiter!");
                    }
                }
                else
                {
                    //pass all entries to the document parser
                    docParser.AddEntry(entry);
                }
            }

            return docParser.DataObject;
        }
    }
}
