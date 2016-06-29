
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxfLib.Data;

namespace DxfLib.Parser
{
    public class DelimitedSegmentSet : DxfParser
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
                DataObject = new DxfObject();
                DataObject.AddEntry(headerParser.DataObject);
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
                        DataObject.AddEntry(footerParser.DataObject);
                    }
                    else if ((bodyParser != null)&&(bodyParser.IsDelimiter(entry)))
                    {
                        //body starting
                        parseState = ParseState.BODY;
                        DataObject.AddEntry(bodyParser.DataObject);
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
                        DataObject.AddEntry(footerParser.DataObject);
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
            string parserName;

            //this will only work once the field is loaded. We have to keep trying until it is. 
            string headerField = bodyParserInfo["headerField"];
            if (headerField != null)
            {
                DxfObject headerObject = this.DataObject.DataList[0];
                if (headerObject == null)
                {
                    throw new Exception("header object not found!");
                }

                DxfProperty property = headerObject.GetValue(headerField);
                string prefix = bodyParserInfo["parserNamePrefix"];
                parserName = prefix + property.Value;
            }
            else
            {
                parserName = bodyParserInfo["parserName"];
            }

            //load the body parser
            bodyParser = parserFactory.GetParser(parserName);

            //apply this name as the object display name
            this.DataObject.Key = parserName;
        }
    }

}
