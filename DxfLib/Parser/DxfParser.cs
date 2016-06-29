using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxfLib.Data;

namespace DxfLib.Parser
{
    /// <summary>
    /// This is a base class for parsing a list of DxfObjects.
    /// </summary>
    public abstract class DxfParser
    {
        private DxfObject dataObject;

        public abstract bool IsDelimiter(DxfEntry entry); 

        public abstract void AddEntry(DxfEntry entry);

        public DxfObject DataObject
        {
            get { return dataObject; }
            set { dataObject = value; }
        }
    }
}
