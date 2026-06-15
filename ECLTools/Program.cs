using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECLTools
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Classes.gbl.file = new File();
            Classes.gbl.DataPath = "Data";
            if (args.Length == 2)
            {
                if (int.TryParse(args[1], out var block_id) == true)
                {
                    var bytes = await Classes.DaxFiles.DaxCache.LoadDax(args[0], block_id);
                    if (bytes != null)
                    {
                        string output = Eclh.EclhDecompilerProgram.Run(bytes, 0x9900);
                        System.IO.File.WriteAllText("output.txt", output);
                    }
                }
            }
        }
    }
}
