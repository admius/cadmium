using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDxfReader.process;
using ConsoleDxfReader.Io;
using ConsoleDxfReader.parsers;

namespace ConsoleDxfReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFile = args[0];
            string dxfFile = args[1];

            //load config
            dynamic configData = JsonLoader.open(configFile);
            ParserFactory parserFactory = new ParserFactory();
            parserFactory.init(configData);

            //load dxf file
            List<string> dxfLines = LineReader.ReadFile(dxfFile);
            EntryReader entryReader = new EntryReader(dxfLines);

            //get the doc parser
            DxfParser docParser = parserFactory.GetParser("DOC");

            //read and parse the document
            DxfEntry entry;
            bool started = false;
            while ((entry = entryReader.readEntry()) != null)
            {
                if (!started)
                {
                    //this better be true
                    started = docParser.IsDelimiter(entry);
                    if(!started)
                    {
                        throw new Exception("First line of document is not a proepr delimiter!");
                    }
                }
                else
                {
                    //pass all entries to the document parser
                    docParser.AddEntry(entry);
                }
            }

            DxfObject docObject = docParser.DataObject;
            docObject.debugPrint(0);

        }
    }
}
