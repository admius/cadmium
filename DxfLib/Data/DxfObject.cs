using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfLib.Data
{
    public class DxfObject
    {
        private string key = "<unspecified>";

        //data list entries should have two methods:
        //key - the name for the object
        //DebugPrint(streamwriter,indentCount) - prints the object
        private List<dynamic> dataList = new List<dynamic>();

        public DxfObject()
        {
        }

        public string Key
        {
            get { return key; }
            set { key = value;  }
        }

        public List<dynamic> DataList
        {
            get { return dataList; }
        }

        /// <summary>
        /// This returns the data associated with a given entry.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetEntry(string key, int nth = 0)
        {
            foreach(dynamic entry in dataList)
            {
                if(entry.Key.Equals(key))
                {
                    if(nth-- <= 0) return entry;
                }
            }
            return null;
        }

        //DxfProperty Accessors
        /// <summary>
        /// This gets a string value for a entry that is a DxfProperty
        /// </summary>
        /// <param name="dxfObject"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetStringValue(string code, int nth = 0)
        {
            DxfProperty prop = GetEntry(code,nth);
            if (prop != null)
            {
                return prop.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This gets a double value for a entry that is a DxfProperty
        /// </summary>
        /// <param name="dxfObject"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public int? GetIntValue(string code, int nth = 0)
        {
            DxfProperty prop = GetEntry(code,nth);
            if (prop != null)
            {
                return Int32.Parse(prop.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This gets a double value for a entry that is a DxfProperty
        /// </summary>
        /// <param name="dxfObject"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public double? GetDoubleValue(string code, int nth = 0)
        {
            DxfProperty prop = GetEntry(code,nth);
            if (prop != null)
            {
                return Double.Parse(prop.Value);
            }
            else
            {
                return null;
            }
        }

        public void AddEntry(dynamic entry)
        {
            dataList.Add(entry);
        }


        public virtual void DebugPrint(TextWriter stream, int indentCount)
        {
            for (int i = 0; i < indentCount; i++)
            {
                stream.Write("\t");
            }
            stream.WriteLine(key);
            foreach(dynamic entry in dataList)
            {
                entry.DebugPrint(stream, indentCount + 1);
            }
        }
    }
}
