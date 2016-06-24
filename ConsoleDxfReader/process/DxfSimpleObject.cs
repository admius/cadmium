using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class DxfSimpleObject : DxfObject
    {
        private List<DxfProperty> properties = new List<DxfProperty>();

        public DxfSimpleObject(string type) : base(type)
        {
        }

        public void AddProperty(DxfProperty property)
        {
            properties.Add(property);
        }

        public override void debugPrint(int indentCount)
        {
            base.debugPrint(indentCount);
            foreach (DxfProperty property in properties)
            {
                property.debugPrint(indentCount + 1);
            }
        }
    }
}
