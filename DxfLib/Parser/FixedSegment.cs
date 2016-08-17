using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxfLib.Data;

namespace DxfLib.Parser
{
    /// <summary>
    /// This class handles a single segment with a fixed delimiter code and value 
    /// and a list of simple properties.
    /// </summary>
    public class FixedSegment : DxfParser
    {
        private string delimCode;
        private string delimValue;

        private dynamic bodyConfig;

        public FixedSegment(dynamic config)
        {
            this.delimCode = config["delimCode"];
            this.delimValue = config["delimValue"];
            this.bodyConfig = config["body"];
        }

        public override bool IsDelimiter(DxfEntry entry)
        {
            bool isDelim = (this.delimCode.Equals(entry.Code)) && (this.delimValue.Equals(entry.Value));

            if (isDelim)
            {
                this.DataObject = new DxfObject();
                this.DataObject.Code = this.delimValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AddEntry(DxfEntry entry)
        {

            string valueName = bodyConfig[entry.Code];
            if (valueName != null)
            {
                //this is a value entry - read the property directly
                DataObject.AddEntry(entry);
            }
            else
            {
                //unknown value!!!
                ProcessUnknownEntry(entry);
            }
        }

        /// <summary>
        /// This handles an unknown entry
        /// </summary>
        /// <param name="entry"></param>
        protected void ProcessUnknownEntry(DxfEntry entry)
        {
            //print message
            //Console.WriteLine("Unknown entry: " + entry.Code + ": " + entry.Value);
            //add it as another entry
            DataObject.AddEntry(entry);
        }
    }
}

