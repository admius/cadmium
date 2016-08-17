using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfLib.Data
{
    public class EntryReader
    {
        private List<string> lines;
        private int currentLine = 0;

        public delegate T ParseDelegate<T>(EntryReader entryReader); 

        public EntryReader(List<string> lines)
        {
            this.lines = lines;
        }

        public DxfEntry ReadEntry()
        {
            if (currentLine <= lines.Count - 2)
            {
                DxfEntry entry = new DxfEntry();
                entry.Code = ReadString().Trim();
                entry.Value = ReadString().Trim();
                return entry;
            }
            else
            {
                return null;
            }
        }

        //we should not use the below methods pubicly


        /// <summary>
        /// This reads a string from the reader. Optionally, an expected value can be set. If the read
        /// value is not the expected value, an exception is thrown.
        /// </summary>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public string ReadString(string expectedValue = null)
        {
            string line = lines[currentLine++];
            if((expectedValue != null)&&(!line.Equals(expectedValue, StringComparison.InvariantCultureIgnoreCase)))
            {
                ThrowUnexpectedLine(expectedValue, line);
            }
            return line;
        }

        /// <summary>
        /// This reads a int from the reader. Optionally, an expected value can be set. If the read
        /// value is not the expected value, an exception is thrown.
        /// </summary>
        /// <param name="requiredValue"></param>
        /// <returns></returns>
        public int ReadInt(int? requiredValue = null)
        {
            string line = ReadString().Trim();
            int value = Int32.Parse(line);
            if ((requiredValue != null)&&(value != requiredValue))
            {
                ThrowUnexpectedLine(requiredValue, value);
            }
            return value;
        }

        /// <summary>
        /// This reads a type from the reader. A delegate that reads the type should be passed in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseDelegate"></param>
        /// <returns></returns>
        public T ReadType<T>(ParseDelegate<T> parseDelegate)
        {
            return parseDelegate(this);
        }

        /// <summary>
        /// This method should be called when a line should be "unread", for example when we want to read it again.
        /// </summary>
        public void PushBack()
        {
            this.currentLine -= 1;
        }



        private void ThrowUnexpectedLine(object expected, object read)
        {
            throw new Exception("Unexpected line value: expecting " + expected.ToString() + ", read " + read.ToString());
        }

    }
}
