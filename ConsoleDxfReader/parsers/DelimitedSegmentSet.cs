
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

        private dynamic bodyParserInfo;

        private ParseState parseState;

        private ParserFactory parserFactory;

        public DelimitedSegmentSet(dynamic config, ParserFactory parserFactory)
        {
            this.parserFactory = parserFactory;

            headerParser = new FixedSegment(config["header"]);
            footerParser = new FixedSegment(config["footer"]);

            bodyParserInfo = config["bodyParser"];

            parseState = ParseState.HEADER;
            bodyParser = null;
        }

        public override bool IsDelimiter(DxfEntry entry)
        {
            bool isDelim = headerParser.IsDelimiter(entry);
            if(isDelim)
            {
                //set parse state to header, clear the body parser
                parseState = ParseState.HEADER;
                bodyParser = null;

                //prepare to read header body
                DataObject = new DxfDelimitedObject();
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
                    if (footerParser.IsDelimiter(entry))
                    {
                        //start footer, no body
                        parseState = ParseState.FOOTER;
                        ((DxfDelimitedObject)DataObject).Footer = footerParser.DataObject;
                    }
                    else if ((bodyParser != null)&&(bodyParser.IsDelimiter(entry)))
                    {
                        //body starting
                        parseState = ParseState.BODY;
                        ((DxfDelimitedObject)DataObject).Body = bodyParser.DataObject;
                    }
                    else
                    {
                        //still in header
                        headerParser.AddEntry(entry);
                        if(bodyParser == null)
                        {
                            TryToLoadBodyParser();
                        }
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

        /// <summary>
        /// This gets the body parser, as read from the header
        /// </summary>
        private void TryToLoadBodyParser()
        {
            if(bodyParserInfo["headerField"] != null)
            {
                //this will only work once the field is loaded. We have to keep trying until it is. 
                string headerField = bodyParserInfo["headerField"];
                DxfSimpleObject headerObject = (DxfSimpleObject)(((DxfDelimitedObject)(this.DataObject)).Header);
                if (headerObject != null)
                {
                    DxfProperty property = headerObject.GetProperty(headerField);
                    string prefix = bodyParserInfo["parserNamePrefix"] != null ? bodyParserInfo["parserNamePrefix"] : "";
                    string parserName = prefix + property.value;

                    bodyParser = parserFactory.GetParser(parserName);
                }   
            }
            else
            {
                bodyParser = parserFactory.GetParser(bodyParserInfo);
            }

        }
    }

}
