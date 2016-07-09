using DxfLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfLib.Query
{

    public delegate bool Matches(dynamic testObject);

    public delegate List<dynamic> GetList();

    public class QFilter
    {
        public static Matches HasKey(string key)
        {
            return delegate (dynamic testObject)
            {
                return key.Equals(testObject.Key, StringComparison.CurrentCultureIgnoreCase);
            };
        }

        public static Matches HasProperty(string key, string value)
        {
            return delegate (dynamic testObject)
            {
                string testValue = testObject.GetStringValue(key);
                return value.Equals(testValue, StringComparison.CurrentCultureIgnoreCase);
            };
        }

        public static Matches HasObject(Matches matches)
        {
            return delegate (dynamic testObject)
            {
                foreach (dynamic child in testObject.DataList)
                {
                    if (matches(child)) return true;
                }
                return false;
            };
        }

        public static Matches And(Matches filter1, Matches filter2)
        {
            return delegate (dynamic testObject)
            {
                return (filter1(testObject) && filter2(testObject));
            };
        }
    }

    public class QSource
    {
        public static GetList Dxf(DxfObject dxfObject)
        {
            return delegate ()
            {
                return dxfObject.DataList;
            };
        }

        public static GetList Search(GetList list, Matches matches)
        {
            return delegate ()
            {
                List<dynamic> results = new List<dynamic>();
                foreach (dynamic child in list())
                {
                    if (matches(child))
                    {
                        results.Add(child);
                    }
                }
                return results;
            };
        }


    }   

    
  

}
