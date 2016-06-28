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
        private String typeName;

        public SegmentList(dynamic config, ParserFactory parserFactory)
        {
            dynamic parserList = config["parsers"];
            foreach (dynamic childConfig in parserList)
            {
                DxfParser parser = parserFactory.GetParser(childConfig);
                parsers.Add(parser);
            }

            if (config["typeName"] != null)
            {
                typeName = config["typeName"];
            }
            else
            {
                typeName = "List";
            }
        }

        /// <summary>
        /// This is the extenal delimiter check. If this is a delimiter a new object is initialized
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public override bool IsDelimiter(DxfEntry entry)
        {
            bool isDelim = DelimiterCheck(entry);

            if (isDelim)
            {
                //create list data object
                DataObject = new DxfListObject();
                DataObject.Type = this.typeName;
                //add latest entry to list
                ((DxfListObject)DataObject).AddEntry(activeParser.DataObject);
                return true;
            }
            else
            {
                return false;
            }
        }
        

        /// <summary>
        /// This is the internal delimiter check, to see if we are starting a new list entry. If so,
        /// a new data object is initialized and added to the overall list.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private bool InternalIsDelimiter(DxfEntry entry)
        {
            bool isDelim = DelimiterCheck(entry);
            if (isDelim)
            {
                //add latest entry to list
                ((DxfListObject)DataObject).AddEntry(activeParser.DataObject);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method checks if the entry is a delimitere for a segment in the list.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private bool DelimiterCheck(DxfEntry entry) { 
            foreach (DxfParser parser in parsers)
            {
                if (parser.IsDelimiter(entry))
                {
                    //set as the active parser
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
            if (InternalIsDelimiter(entry))
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

