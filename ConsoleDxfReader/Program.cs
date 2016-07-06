using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxfLib.Data;
using DxfLib.IO;
using DxfLib.Parser;
using DxfLib;

namespace ConsoleDxfReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFile = args[0];
            string dxfFile = args[1];
            string outputFile = args[2];

            DxfReader dxfReader = new DxfReader();
            dxfReader.Init(configFile);

            DxfObject docObject = dxfReader.Open(dxfFile);

            //write the output to a file
//            using (StreamWriter fileStream = new StreamWriter(outputFile))
//            {
//                docObject.DebugPrint(fileStream,0);
//            }

            //lookups
            DxfObject entitiesSection = docObject.GetEntry("Section:ENTITIES");
            DxfObject entities = entitiesSection.GetEntry("Entities Body");

            DxfObject blocksSection = docObject.GetEntry("Section:BLOCKS");
            DxfObject blocks = blocksSection.GetEntry("Blocks Body");
        }
    }
}
