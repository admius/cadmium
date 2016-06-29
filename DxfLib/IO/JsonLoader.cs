using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;

namespace DxfLib.IO
{
    public class JsonLoader
    {
        /// <summary>
        /// This opens a meta file
        /// </summary>
        /// <param name="fileName"></param>
        public static dynamic Open(string fileName)
        {
            //load the file
            string data = File.ReadAllText(fileName);

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            dynamic jsonData = serializer.Deserialize(data, typeof(object));

            return jsonData;
        }
    }
}
