using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDxfReader.process;

namespace ConsoleDxfReader.parsers
{
    class DelimitedSegmentSet : DxfParser
    {
        private enum ParseState
        {
            HEADER,
            BODY,
            FOOTER
        }

        private FixedSegment headerParser;
        private DxfParser bodyParser;
        private FixedSegment footerParser;

        private ParseState parseState;

        private ParserFactory parserFactory;

        public DelimitedSegmentSet(dynamic config, ParserFactory parserFactory)
        {
            this.parserFactory = parserFactory;

            headerParser = new FixedSegment(config["header"]);
            footerParser = new FixedSegment(config["footer"]);

            parseState = ParseState.HEADER;
        }

        public override bool IsDelimiter(DxfEntry entry)
        {
            bool isDelim = headerParser.IsDelimiter(entry);
            if(isDelim)
            {
                DataObject = new DxfDelimitedObject("delimited segment - save name!");
                ((DxfDelimitedObject)DataObject).Header = headerParser.DataObject;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AddEntry(DxfEntry entry)
        {
            switch(parseState)
            {
                case ParseState.HEADER:
                    //check if we should go to the next state
                    if(bodyParser.IsDelimiter(entry))
                    {    
                        //body starting
                        parseState = ParseState.BODY;
                        ((DxfDelimitedObject)DataObject).Body = bodyParser.DataObject;
                    }
                    else if(footerParser.IsDelimiter(entry))
                    {
                        //no body
                        parseState = ParseState.FOOTER;
                        ((DxfDelimitedObject)DataObject).Footer = footerParser.DataObject;
                    }
                    else
                    {
                        //still in header
                        headerParser.AddEntry(entry);
//NEED TO GET THE BODY PARSER!!! - might not be ready yet though
//read the body parser - might be "headerField" for the name. Thing of other options
                    }
                    break;

                case ParseState.BODY:
                    //check if we should go to the next state
                    if(footerParser.IsDelimiter(entry))
                    {
                        parseState = ParseState.FOOTER;
                        ((DxfDelimitedObject)DataObject).Footer = footerParser.DataObject;
                    }
                    else
                    {
                        bodyParser.AddEntry(entry);
                    }
                    break;

                case ParseState.FOOTER:
                    //check if we should go to a new object
                    footerParser.AddEntry(entry);
                    break;
            }
        }

        
    }
}
