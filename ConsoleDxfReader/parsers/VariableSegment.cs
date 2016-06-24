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
    class VariableSegment : DxfParser
    {
        private string delimCode;

        public VariableSegment(dynamic config)
        {
            delimCode = config["delimCode"];
        }

        public override bool IsDelimiter(DxfEntry entry)
        {
            if(entry.code.Equals(delimCode))
            {
                //instantiate object with proper name
                DataObject = new DxfSimpleObject(entry.value);
                return true;
            }
            else
            {
                //not a delimiter
                return false;
            }
        }

        public override void AddEntry(DxfEntry entry)
        {
            //this is a value entry
            ((DxfSimpleObject)DataObject).AddProperty(new DxfProperty(entry.code, entry.value));
        }

    }
}

