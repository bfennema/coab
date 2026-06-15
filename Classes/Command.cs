using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Command
    {
        public static Dictionary<int, CmdItem> Table = new Dictionary<int, CmdItem>();
    }

    public class CmdItem
    {
        public delegate Task<bool> CmdDelegate(ushort ecl_offset, string name);

        int size;
        string name;
        CmdDelegate cmd;

        public CmdItem(int Size, string Name, CmdDelegate Cmd)
        {
            size = Size;
            name = Name;
            cmd = Cmd;
        }

        public async Task<bool> Run()
        {
            return await cmd(gbl.ecl_offset, name);
        }

        public string Name()
        {
            return name;
        }

        public int Size => size;

        public void Skip()
        {
            if (gbl.printCommands == true)
            {
                //Logger.Debug("SKIPPING: {0}", name);
            }

            if (size == 0)
            {
                gbl.ecl_offset += 1;
            }
            else
            {
                gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, size);
            }
        }
    }
}
