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
        private string code = "<unspecified>";

        //data list entries should have two methods:
        //Code - the name for the object
        //DebugPrint(streamwriter,indentCount) - prints the object
        private List<dynamic> dataList = new List<dynamic>();

        public DxfObject()
        {
        }

        public string Code
        {
            get { return code; }
            set { code = value;  }
        }

        public List<dynamic> DataList
        {
            get { return dataList; }
        }

        /// <summary>
        /// This returns the data associated with a given entry.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public dynamic GetEntry(string code, int nth = 0)
        {
            foreach(dynamic entry in dataList)
            {
                if(entry.Code.Equals(code))
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
            DxfEntry entry = GetEntry(code, nth);
            if (entry != null)
            {
                return entry.Value;
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
            DxfEntry entry = GetEntry(code, nth);
            if (entry != null)
            {
                return Int32.Parse(entry.Value);
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
            DxfEntry entry = GetEntry(code, nth);
            if (entry != null)
            {
                return Double.Parse(entry.Value);
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
            stream.WriteLine(code);
            foreach(dynamic entry in dataList)
            {
                entry.DebugPrint(stream, indentCount + 1);
            }
        }
    }
}
