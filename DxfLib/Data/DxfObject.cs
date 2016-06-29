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
        public dynamic GetValue(string key)
        {
            foreach(dynamic entry in dataList)
            {
                if(entry.Key.Equals(key))
                {
                    return entry;
                }
            }
            return null;
        }

        public void AddEntry(dynamic entry)
        {
            dataList.Add(entry);
        }


        public virtual void DebugPrint(StreamWriter stream, int indentCount)
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
