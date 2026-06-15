using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Vm
    {
        static public void Write(string fmt, params object[] args)
        {
            if (gbl.printCommands == true)
            {
                Logger.DebugWrite(fmt, args);
            }
        }

        static public void WriteLine(string fmt, params object[] args)
        {
            if (gbl.printCommands == true)
            {
                Logger.Debug(fmt, args);
            }
        }

        public static ushort LoadCmdSets(ref CmdOperation cmd_ops, ushort ecl_offset, int numberOfSets) // parse_command_sub
        {
            cmd_ops.Clear();

            for (int loop_var = 1; loop_var <= numberOfSets; loop_var++)
            {
                byte code = gbl.ecl_ptr[ecl_offset + 1 - gbl.initial_ecl_offset];
                byte low = gbl.ecl_ptr[ecl_offset + 2 - gbl.initial_ecl_offset];


                cmd_ops[loop_var].Code = code;
                cmd_ops[loop_var].Low = low;

                ecl_offset += 2;

                if (code == 1 || code == 2 || code == 3)
                {
                    ecl_offset++;
                    byte high = gbl.ecl_ptr[ecl_offset - gbl.initial_ecl_offset];

                    cmd_ops[loop_var].High = high;

                    //System.Console.WriteLine("   code: {0,2:X} low: {1,2:X} high: {2,2:X}",
                    //   code, low, high);
                }
                else if (code == 0x80) // Load compressed string
                {
                    short strLen = low;

                    if (strLen > 0)
                    {
                        ecl_offset = LoadCompressedEclString(cmd_ops[loop_var], strLen, ecl_offset);
                    }
                    else
                    {
                        cmd_ops[loop_var].String = string.Empty;
                    }

                    //System.Console.WriteLine("   code: {0,2:X} strIndex: {1} strLen: {2} str: '{3}'",
                    //    code, strIndex, strLen, gbl.unk_1D972[strIndex]);
                }
                else if (code == 0x81)
                {
                    ecl_offset++;
                    byte high = gbl.ecl_ptr[ecl_offset - gbl.initial_ecl_offset];

                    cmd_ops[loop_var].High = high;

                    ushort loc = cmd_ops[loop_var].Word;

                    CopyStringFromMemory(loc, ref cmd_ops[loop_var]);

                    //System.Console.WriteLine("   code: {0,2:X} strIndex: {1} loc: {2} str: '{3}'",
                    //    code, strIndex, loc, gbl.unk_1D972[strIndex]);
                }
                else
                {
                    //System.Console.WriteLine("   code: {0,2:X} low: 0x{1,2:X}",
                    //    code, low);
                }
            }

            ecl_offset++;

            return ecl_offset;
        }

        internal static char inflateChar(uint arg_0)
        {
            if (arg_0 <= 0x1f)
            {
                arg_0 += 0x40;
            }

            return (char)arg_0;
        }

        public static int GetMemoryValueType(ushort arg_0) // sub_30723
        {
            int var_1 = 4;

            if (arg_0 >= gbl.vm_mem0_offset && arg_0 <= gbl.vm_mem0_offset + gbl.vm_mem0_size - 1)
            {
                var_1 = 0;
            }
            if (arg_0 >= gbl.vm_mem1_offset && arg_0 <= gbl.vm_mem1_offset + gbl.vm_mem1_size - 1)
            {
                var_1 = 1;
            }
            if (arg_0 >= gbl.vm_mem2_offset && arg_0 <= gbl.vm_mem2_offset + gbl.vm_mem2_size - 1)
            {
                var_1 = 2;
            }
            if (arg_0 >= gbl.initial_ecl_offset && arg_0 <= gbl.initial_ecl_offset + 0x1DFF)
            {
                var_1 = 3;
            }

            return var_1;
        }

        internal static ushort find_gbl_player_index(Player player)
        {
            int index = gbl.TeamList.IndexOf(player);

            if (index == -1)
                index = gbl.TeamList.Count;

            return (ushort)index;
        }

        internal static ushort get_player_values(ref bool arg_0, ushort arg_4)
        {
            ushort return_val;

            arg_0 = true;

            //arg_4 -= 0x7c00;

            if (arg_4 == 0x15)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Int.full;
            }
            else if (arg_4 == 0x18)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Con.full;
            }
            else if (arg_4 == 0x72)
            {
                return_val = (ushort)gbl.SelectedPlayer.race;
            }
            else if (arg_4 == 0x73)
            {
                return_val = (ushort)gbl.SelectedPlayer._class;
            }
            else if (arg_4 == 0x9b)
            {
                return_val = gbl.SelectedPlayer.saveVerse[(int)SaveVerseType.Petrification];
            }
            else if (arg_4 == 0xa0)
            {
                return_val = gbl.SelectedPlayer.HitDice;
            }
            else if (arg_4 >= 0xA5 && arg_4 <= 0xAC)
            {
                int var_3 = arg_4 - 0xA5;

                return_val = gbl.SelectedPlayer.thief_skills[var_3];
            }
            else if (arg_4 == 0xb8)
            {
                return_val = gbl.SelectedPlayer.control_morale;
            }
            else if (arg_4 == 0xBB)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Copper);
            }
            else if (arg_4 == 0xBD)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Electrum);
            }
            else if (arg_4 == 0xBF)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Silver);
            }
            else if (arg_4 == 0xC1)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Gold);
            }
            else if (arg_4 == 0xC3)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Platinum);
            }
            else if (arg_4 == 0xC9)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.MagicUser);
            }
            else if (arg_4 == 0xD6)
            {
                return_val = gbl.SelectedPlayer.sex;
            }
            else if (arg_4 == 0xD8)
            {
                return_val = gbl.SelectedPlayer.alignment;
            }
            else if (arg_4 == 0xE4)
            {
                return_val = (ushort)(gbl.SelectedPlayer.field_192 & 1);
            }
            else if (arg_4 == 0xF7)
            {
                return_val = (ushort)gbl.SelectedPlayer.field_13C;
            }
            else if (arg_4 == 0xF9)
            {
                return_val = gbl.SelectedPlayer.field_13E;
            }
            else if (arg_4 == 0x100)
            {
                if (gbl.SelectedPlayer.in_combat == true)
                {
                    return_val = 1;
                }
                else
                {
                    return_val = 0x80;
                }

                if (gbl.player_not_found == true)
                {
                    return_val = 0;
                }

                gbl.player_not_found = false;
            }
            else if (arg_4 == 0x10C)
            {
                if (gbl.SelectedPlayer.combat_team == CombatTeam.Ours &&
                    gbl.SelectedPlayer.quick_fight == QuickFight.True)
                {
                    return_val = 0x80;
                }
                else if (gbl.SelectedPlayer.combat_team == CombatTeam.Enemy)
                {
                    return_val = 0x81;
                }
                else
                {
                    return_val = 0;
                }
            }
            else if (arg_4 == 0x10D)
            {
                return_val = 0; /* Simeon */
                throw new System.NotImplementedException("Not sure what should happening this case");
                //jmp	func_end
            }
            else if (arg_4 == 0x11B)
            {
                return_val = gbl.SelectedPlayer.movement;
            }
            else if (arg_4 == 0x2B1)
            {
                return_val = find_gbl_player_index(gbl.SelectedPlayer);
            }
            else if (arg_4 == 0x2B4)
            {
                return_val = find_gbl_player_index(gbl.SelectedPlayer);
            }
            else if (arg_4 == 0x2CF)
            {
                switch (gbl.SelectedPlayer.stats.Cha.full)
                {
                    case 3:
                        return_val = 0;
                        break;

                    case 4:
                        return_val = 5;
                        break;

                    case 5:
                        return_val = 0x0A;
                        break;

                    case 6:
                        return_val = 0x0F;
                        break;

                    case 7:
                        return_val = 0x14;
                        break;

                    case 8:
                    case 9:
                    case 0x0a:
                    case 0x0b:
                    case 0x0c:
                        return_val = 0x19;
                        break;

                    case 0x0d:
                        return_val = 0x1E;
                        break;

                    case 0x0e:
                        return_val = 0x23;
                        break;

                    case 0x0f:
                        return_val = 0x28;
                        break;

                    case 0x10:
                        return_val = 0x32;
                        break;

                    case 0x11:
                        return_val = 0x37;
                        break;

                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                        return_val = 0x3C;
                        break;

                    default:
                        return_val = 0;
                        break;
                }
            }
            else if (arg_4 == 0x312)
            {
                return_val = gbl.game_area;
            }
            else if (arg_4 == 0x33E)
            {
                return_val = gbl.area2_ptr.party_size;
            }
            else
            {
                return_val = 0; /* value not read if arg_0 is false */
                arg_0 = false;
            }

            return return_val;
        }

        public static ushort GetMemoryValue(ushort loc) // sub_30F16
        {
            ushort val = 0;

            int mem_type = GetMemoryValueType(loc);

            switch (mem_type)
            {
                case 0:
                    val = gbl.area_ptr.field_6A00_Get((loc - gbl.vm_mem0_offset) * 2);
                    break;

                case 1:
                    bool var_4 = false;
                    val = get_player_values(ref var_4, (ushort)(loc - gbl.vm_mem1_offset));

                    if (var_4 == false)
                    {
                        val = gbl.area2_ptr.field_800_Get((loc - gbl.vm_mem1_offset) * 2);
                    }
                    break;

                case 2:
                    val = gbl.stru_1B2CA[(loc - gbl.vm_mem2_offset) << 1];
                    break;

                case 3:
                    val = gbl.ecl_ptr[loc - gbl.initial_ecl_offset]; // Read from tables in ecl
                    break;

                case 4:
                    if (loc < 0xC04B)
                    {
                        switch (loc)
                        {
                            case 0x00B1:
                                val = (ushort)gbl.word_1D918;
                                break;

                            case 0x00FB:
                                val = (ushort)gbl.word_1D914;
                                break;

                            case 0x00FC:
                                val = (ushort)gbl.word_1D916;
                                break;

                            case 0x033D:
                                val = gbl.mapDirection;
                                break;

                            case 0x035F:
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        loc -= 0xC04B;

                        switch (loc)
                        {
                            case 0:
                                val = (ushort)gbl.mapPosX;
                                break;

                            case 0x01:
                                val = (ushort)gbl.mapPosY;
                                break;

                            case 0x02:
                                val = (ushort)(gbl.mapDirection / 2);
                                break;

                            case 0x03:
                                val = gbl.mapWallType;
                                break;

                            case 0x04:
                                val = gbl.mapWallRoof;
                                break;

                            case 0x0E:
                                break;
                        }
                    }
                    break;
            }

            return val;
        }

        internal static string DecompressString(byte[] data)
        {
            var sb = new System.Text.StringBuilder();
            int state = 1;
            uint lastByte = 0;

            foreach (uint thisByte in data)
            {
                uint curr = 0;
                switch (state)
                {
                    case 1:
                        curr = (thisByte >> 2) & 0x3F;
                        if (curr != 0) sb.Append(inflateChar(curr));
                        state = 2;
                        break;

                    case 2:
                        curr = ((lastByte << 4) | (thisByte >> 4)) & 0x3F;
                        if (curr != 0) sb.Append(inflateChar(curr));
                        state = 3;
                        break;

                    case 3:
                        curr = ((lastByte << 2) | (thisByte >> 6)) & 0x3F;
                        if (curr != 0) sb.Append(inflateChar(curr));

                        curr = thisByte & 0x3F;
                        if (curr != 0) sb.Append(inflateChar(curr));
                        state = 1;
                        break;
                }
                lastByte = thisByte;
            }

            return sb.ToString();
        }

        internal static ushort LoadCompressedEclString(CmdOperation.Operation op, int inputLength, ushort ecl_offset)
        {
            byte[] data = new byte[inputLength];

            for (int i = 0; i < inputLength; i++)
            {
                data[i] = gbl.ecl_ptr[ecl_offset + 1 + i - gbl.initial_ecl_offset];
            }

            ecl_offset += (ushort)inputLength;

            op.String = DecompressString(data);

            return ecl_offset;
        }

        internal static void CopyStringFromMemory(ushort location, ref CmdOperation.Operation op) // sub_31421
        {
            int offset = 0;
            var sb = new System.Text.StringBuilder();

            switch (Vm.GetMemoryValueType(location))
            {
                case 0:
                    while (gbl.area_ptr.field_6A00_Get((offset + location - gbl.vm_mem0_offset) * 2) != 0)
                    {
                        sb.Append((char)((byte)gbl.area_ptr.field_6A00_Get((offset + location - gbl.vm_mem0_offset) * 2)));
                        offset++;
                    }
                    break;

                case 1:
                    if (location == gbl.vm_mem1_offset)
                    {
                        sb.Append(gbl.SelectedPlayer.name);
                    }
                    else
                    {
                        while (gbl.area2_ptr.field_800_Get((offset + location - gbl.vm_mem1_offset) << 1) != 0)
                        {
                            sb.Append((char)((byte)gbl.area2_ptr.field_800_Get((offset + location - gbl.vm_mem1_offset) << 1)));
                            offset++;
                        }
                    }
                    break;

                case 2:
                    while (gbl.stru_1B2CA[(offset + location - gbl.vm_mem2_offset) << 1] != 0)
                    {
                        sb.Append((char)gbl.stru_1B2CA[(offset + location - gbl.vm_mem2_offset) << 1]);
                        offset++;
                    }
                    break;

                case 3:
                    while (gbl.ecl_ptr[offset + location - gbl.initial_ecl_offset] != 0)
                    {
                        sb.Append((char)gbl.ecl_ptr[offset + location - gbl.initial_ecl_offset]);
                        offset++;
                    }
                    break;
            }

            op.String = sb.ToString();
        }
    }
}
