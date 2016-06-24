using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDxfReader.process;

namespace ConsoleDxfReader.parsers
{
    /// <summary>
    /// This class parses a list of segments which have a single delimiter code and
    /// a variable delimiter value. For each of these it creates a names object which includes
    /// a property for each entry in the segment.
    /// </summary>
    class SegmentList : DxfParser
    {
        private List<DxfParser> parsers = new List<DxfParser>();
        private DxfParser activeParser = null;

        public SegmentList(dynamic config, ParserFactory parserFactory)
        {
            dynamic parserList = config["parsers"];
            foreach (string parserName in parserList)
            {
                DxfParser parser = parserFactory.GetParser(parserName);
                parsers.Add(parser);
            }
        }

        public override bool IsDelimiter(DxfEntry entry)
        {
            bool isDelimiter = InternalIsDelimiter(entry);
            if(isDelimiter)
            {
                DataObject = new DxfListObject("xxx");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InternalIsDelimiter(DxfEntry entry)
        {
            foreach (DxfParser parser in parsers)
            {
                if (parser.IsDelimiter(entry))
                {
                    //add this newly started object to the object list
                    ((DxfListObject)DataObject).AddEntry(parser.DataObject);
                    activeParser = parser;
                    return true;
                }
            }

            //if we get here the parser was not found
            return false;
        }

        public override void AddEntry(DxfEntry entry)
        {
            //check if this is a new entry
            if (IsDelimiter(entry))
            {
                //this sets up the new object in the call. just return now.
                return;
            }

            //this is a value entry from the existing parser/object
            if(activeParser != null)
            {
                activeParser.AddEntry(entry);
            }
            
        }

    }
}

