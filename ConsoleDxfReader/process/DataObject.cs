using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DataObject
    {
        private string code;
        private string value = null;

        public DataObject(string code,  string value)
        {
            this.code = code;
            this.value = value;
        }
        
    }
}
