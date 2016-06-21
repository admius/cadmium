using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.process
{
    class StructureObject : DataObject
    {
        private List<DataObject> children = null;

        public StructureObject(string code, string value, dynamic descriptor, EntryReader dxfReader) : base(code,value)
        {
            ReadChildren(descriptor,dxfReader);
        }

        // PRIVATE MEMBERS

        private void ReadChildren(dynamic descriptor, EntryReader dxfReader)
        {
            //loop through reading the child values for this structure
            bool readingChildren = true;
            while (readingChildren) {

                string code = dxfReader.readString();
                dynamic valueDescriptor = MetaReader.getDescriptorForCode(descriptor, code);

                if (valueDescriptor == null)
                {
                    //code not found, go back to parent replacing the code on the reader
                    dxfReader.pushBack();
                    readingChildren = false;
                }
                else
                {
                    //get the type for this value
                    string value = dxfReader.readString();
                    DataObject childEntry = null;
                    if (MetaReader.getValueDescriptorType(valueDescriptor) == MetaReader.VALUE_DESCRIPTOR_TYPE.ENUM)
                    {
                        //enumeration of allowed child values
                        dynamic keyDescriptor = MetaReader.getDescriptorForValue(valueDescriptor, value);
                        if (keyDescriptor == null)
                        {
                            //code value pair not found, go back to parent replacing code and value on the reader
                            dxfReader.pushBack();
                            dxfReader.pushBack();
                            readingChildren = false;
                        }
                        else
                        {
                            if (MetaReader.getKeyDescriptorType(keyDescriptor) == MetaReader.KEY_DESCRIPTOR_TYPE.CODES)
                            {
                                //there is a list of allowed codes
                                childEntry = new StructureObject(code, value, keyDescriptor, dxfReader);
                                this.children.Add(childEntry);
                            }
                            else if (MetaReader.getKeyDescriptorType(keyDescriptor) == MetaReader.KEY_DESCRIPTOR_TYPE.END)
                            {
                                //this is an end entry, go back to parent
                                readingChildren = false;
                            }
                        }
                    }
                    else if (MetaReader.getValueDescriptorType(valueDescriptor) == MetaReader.VALUE_DESCRIPTOR_TYPE.VALUE)
                    {
                        //this is a simple value
                        childEntry = new DataObject(code, value);
                        this.children.Add(childEntry);
                        readingChildren = false;
                    }

                }

            }

        }

    }
}
