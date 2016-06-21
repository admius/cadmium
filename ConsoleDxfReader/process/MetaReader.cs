using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using ConsoleDxfReader.Io;

namespace ConsoleDxfReader.process
{
    class MetaReader
    {
        public enum KEY_DESCRIPTOR_TYPE
        {
            CODES,
            END,
            VALUE,
            UNKNOWN
        }

        public enum VALUE_DESCRIPTOR_TYPE
        {
            ENUM,
            VALUE,
            UNKNOWN
        }

        private dynamic metaFile;

        /// <summary>
        /// This opens a meta file
        /// </summary>
        /// <param name="fileName"></param>
        public void open(string fileName)
        {
            //load the file
            string data = File.ReadAllText(fileName);

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            metaFile = serializer.Deserialize(data, typeof(object));
        }

        /// <summary>
        /// This gets the root key entry for the doc
        /// </summary>
        public dynamic DocDesciptor
        {
            get
            {
                return metaFile["doc"];
            }
        }

        /// <summary>
        /// This gets the type for a key entry - code, end or unknown
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static KEY_DESCRIPTOR_TYPE getKeyDescriptorType(dynamic descriptor)
        {
            if (descriptor["codes"] != null)
            {
                return KEY_DESCRIPTOR_TYPE.CODES;
            }
            else if (descriptor["isEnd"] != null)
            {
                return KEY_DESCRIPTOR_TYPE.END;
            }
            else if (descriptor["isEnd"] != null)
            {
                return KEY_DESCRIPTOR_TYPE.VALUE;
            }
            else
            {
                return KEY_DESCRIPTOR_TYPE.UNKNOWN;
            }
        }

        /// <summary>
        /// This gets the type for a value entry - enum or value
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static VALUE_DESCRIPTOR_TYPE getValueDescriptorType(dynamic descriptor)
        {
            if (descriptor["enum"] != null)
            {
                return VALUE_DESCRIPTOR_TYPE.ENUM;
            }
            else
            {
                return VALUE_DESCRIPTOR_TYPE.VALUE;
            }

        }

        /// <summary>
        /// This looks up a value entry for a given code
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static dynamic getDescriptorForCode(dynamic descriptor, string code)
        {
            return descriptor["codes"][code];
        }

        /// <summary>
        /// This looks up a key entry for the given enum value
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static dynamic getDescriptorForValue(dynamic descriptor, string value)
        {
            return descriptor["enum"][value];
        }
    }
}
