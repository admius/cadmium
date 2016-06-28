using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDxfReader.process;

namespace ConsoleDxfReader.parsers
{
    class ParserFactory
    {
        private dynamic parseConfig;

        public void init(dynamic parseConfig)
        {
            this.parseConfig = parseConfig;
        }

        public DxfParser GetParser(string parserName)
        {
            dynamic parserConfig = parseConfig["parsers"][parserName];
            if (parserConfig == null)
            {
                throw new Exception("Parser not found: " + parserName);
            }
            return GetParser(parserConfig);
        }

        public DxfParser GetParser(dynamic config)
        {
            dynamic parserConfig;

            string parserName = config["parserName"];
            if (parserName != null)
            {
                return GetParser(parserName);
            }
            else
            {
                parserConfig = config;
            }

            DxfParser parser = InstantiateParser(parserConfig);
            return parser;

        }


        private DxfParser InstantiateParser(dynamic config)
        {
            string parserClass = config["parserClass"];

            switch(parserClass)
            {
                case "DelimitedSegmentSet":
                    return new DelimitedSegmentSet(config, this);

                case "SegmentList":
                    return new SegmentList(config, this);

                case "FixedSegment":
                    return new FixedSegment(config);

                case "VariableSegment":
                    return new VariableSegment(config);

                default:
                    throw new Exception("Unknown segment type in Instantiate Parser");

            }
        }
    }
}
