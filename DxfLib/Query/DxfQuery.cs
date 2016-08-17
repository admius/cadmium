using DxfLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This name space contains query functions for reading from a dxf object and its children
/// </summary>
namespace DxfLib.Query
{

    /// <summary>
    /// This is a delegate used as a filter in the search.
    /// </summary>
    /// <param name="testObject"></param>
    /// <returns></returns>
    public delegate bool Matches(dynamic testObject);

    /// <summary>
    /// This is a static class with functions to generate filter delgates. 
    /// </summary>
    public class Has
    {
        /// <summary>
        /// The returned filter matches an item with the given Code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Matches Code(string code)
        {
            return delegate (dynamic testObject)
            {
                return code.Equals(testObject.Code, StringComparison.CurrentCultureIgnoreCase);
            };
        }

        /// <summary>
        /// Th returned filter matches an item with the given value It should be used on DxfObjects with property children. 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Matches Property(string code, string value)
        {
            return delegate (dynamic testObject)
            {
                string testValue = testObject.GetStringValue(code);
                return value.Equals(testValue, StringComparison.CurrentCultureIgnoreCase);
            };
        }

        /// <summary>
        /// The returned filter matches when the object has a child matching the given filter.
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static Matches Object(Matches matches)
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

        /// <summary>
        /// The returned filter does an AND between two filters.
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static Matches Both(Matches filter1, Matches filter2)
        {
            return delegate (dynamic testObject)
            {
                return (filter1(testObject) && filter2(testObject));
            };
        }
    }

    /// <summary>
    /// This is a static class generates a return list of dxf objects/entries
    /// </summary>
    public class GetList
    {
        /// <summary>
        /// Tise returns the list of dxf children from a dxf object.
        /// </summary>
        /// <param name="dxfObject"></param>
        /// <returns></returns>
        public static List<dynamic> FromDxf(DxfObject dxfObject, Matches matches)
        {
            return GetList.FromList(dxfObject.DataList,matches);
        }

        /// <summary>
        /// This returns search delegate returns a list of objects from the given list matching the passed filter.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static List<dynamic> FromList(List<dynamic> list, Matches matches)
        {
            List<dynamic> results = new List<dynamic>();
            foreach (dynamic child in list)
            {
                if (matches(child))
                {
                    results.Add(child);
                }
            }
            return results;
        }
    }

    /// <summary>
    /// This is a static class to generate search delegates
    /// </summary>
    public class GetObject
    {
        /// <summary>
        /// This returns the nth object from the DxfObject which is the nth match to the filter.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="matches"></param>
        /// <param name="nth"></param>
        /// <returns></returns>
        public static dynamic FromDxf(DxfObject dxfObject, Matches matches, int nth = 0)
        {
            return GetObject.FromList(dxfObject.DataList, matches, nth);
        }

        /// <summary>
        /// This returns the nth object from the list which is the nth match to the filter.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="matches"></param>
        /// <param name="nth"></param>
        /// <returns></returns>
        public static dynamic FromList(List<dynamic> list, Matches matches, int nth = 0)
        {
            foreach (dynamic child in list)
            {
                if (matches(child))
                {
                    if (nth-- <= 0)
                    {
                        return child;
                    }
                }
            }
            return null;
        }
    }




}
