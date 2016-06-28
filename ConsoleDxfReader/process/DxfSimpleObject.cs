using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfSimpleObject : DxfObject
    {
        private List<DxfProperty> properties = new List<DxfProperty>();

        public DxfSimpleObject() : base()
        {
        }

        public void AddProperty(DxfProperty property)
        {
            properties.Add(property);
        }

        public DxfProperty GetProperty(string key)
        {
            foreach(DxfProperty prop in properties)
            {
                if(prop.key.Equals(key))
                {
                    return prop;
                }
            }
            return null;
        }

        public override void DebugPrint(StreamWriter stream, int indentCount)
        {
            base.DebugPrint(stream, indentCount);
            foreach (DxfProperty property in properties)
            {
                property.DebugPrint(stream, indentCount + 1);
            }
        }
    }
}
