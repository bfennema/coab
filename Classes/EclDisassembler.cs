using Classes;
using System;
using System.Collections.Generic;

namespace Classes
{
    public static class EclDisassembler
    {
        public static (List<string>, List<ushort>) Disassemble(ushort ecl_offset, out ushort nextOffset)
        {
            nextOffset = (ushort)(ecl_offset + 1);
            if (gbl.ecl_ptr == null) return (["No ECL loaded"],[]);

            int index = ecl_offset - gbl.initial_ecl_offset;
            if (index < 0 || index >= EclBlock.ecl_struct_size)
            {
                return ([$"Invalid Address: ${ecl_offset:X4}"],[]);
            }

            byte commandId = gbl.ecl_ptr[index];
            if (!Classes.Command.Table.TryGetValue(commandId, out var cmdItem))
            {
                return ([$"${ecl_offset:X4}: DB {commandId:X2} (Unknown)"],[]);
            }

            string name = cmdItem.Name();
            ushort numSets = (ushort)cmdItem.Size;
            if (commandId == 0x15)
            {
                numSets = 3;
            }
            else if (commandId == 0x25 || commandId == 0x26 || commandId == 0x2B)
            {
                numSets = 2;
            }

            var args = new List<string>();
            var cmd_ops = new CmdOperation(gbl.cmdOpsLimit);
            int extraArgs = 0;
            cmd_ops.Init(Vm.GetMemoryValue);
            nextOffset--;
            nextOffset = Vm.LoadCmdSets(ref cmd_ops, nextOffset, numSets);

            string result;
            var addresses = new List<ushort>();

            for (int loop_var = 1; loop_var <= numSets; loop_var++)
            {
                try
                {
                    result = cmd_ops[loop_var].PrintCmd();
                }
                catch (InvalidOperationException)
                {
                    result = "???";
                }
                args.Add(result);
                if (cmd_ops[loop_var].IsPointer)
                    addresses.Add(cmd_ops[loop_var].Word);
                if (loop_var == 3 && (commandId == 0x15))
                    extraArgs = cmd_ops[loop_var].GetCmdValue();
                else if (loop_var == 2 && (commandId == 0x25 || commandId == 0x26 || commandId == 0x2B))
                    extraArgs = cmd_ops[loop_var].GetCmdValue();
            }

            string argList = string.Join(" ", args);
            result   = $"${ecl_offset:X4}: {commandId:X2} {name,-20} {argList}".Trim();

            var lines = new List<string>();

            // Append live compare_flags value for IF commands (0x16–0x1B)
            if (commandId >= 0x16 && commandId <= 0x1B)
            {
                int  flagIndex = commandId - 0x16;
                bool flagValue = gbl.compare_flags[flagIndex];
                string flag = flagValue ? "[TRUE]" : "[false]";
                lines.Add($"{result}  {flag}");
            }
            else if (extraArgs > 0)
            {
                lines.Add(result);
                nextOffset--;
                nextOffset = Vm.LoadCmdSets(ref cmd_ops, nextOffset, extraArgs);
                for (int loop_var = 1; loop_var <= extraArgs; loop_var ++)
                {
                    result = cmd_ops[loop_var].PrintCmd();
                    lines.Add($"                               {result}                // {loop_var - 1}");
                    if (cmd_ops[loop_var].IsPointer)
                        addresses.Add(cmd_ops[loop_var].Word);
                }
            }
            else
            {
                lines.Add(result);
            }

            return (lines,addresses);
        }
    }
}
