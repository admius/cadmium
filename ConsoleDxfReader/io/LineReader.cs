using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDxfReader.Io
{
    class LineReader
    {
        public static List<string> ReadFile(string fileName)
        {
            //output collection
            List<String> fileLines = new List<string>();

            // Read the file and display it line by line.
            string line;
            StreamReader file = null;
            try
            {
                file = new StreamReader(fileName);

                while ((line = file.ReadLine()) != null)
                {
                    fileLines.Add(line.Trim());
                }
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }

            return fileLines;

        }
    }
}
