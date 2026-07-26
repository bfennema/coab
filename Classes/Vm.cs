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

        internal static ushort get_player_values(ref bool found, ushort addr)
        {
            ushort return_val;

            found = true;

            //arg_4 -= 0x7c00;

            if (addr == 0x14)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Str.Current;
            }
            if (addr == 0x15)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Int.Current;
            }
            else if (addr == 0x16)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Wis.Current;
            }
            else if (addr == 0x17)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Dex.Current;
            }
            else if (addr == 0x18)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Con.Current;
            }
            else if (addr == 0x19)
            {
                return_val = (byte)gbl.SelectedPlayer.stats.Cha.Current;
            }
            else if (addr >= 0x20 && addr <= 0x70)
            {
                // memorized spells
                return_val = 0;
                found = false;
            }
            else if (addr == 0x71)
            {
                // thac0 ?
                return_val = 0;
                found = false;
            }
            else if (addr == 0x72)
            {
                return_val = (ushort)gbl.SelectedPlayer.race;
            }
            else if (addr == 0x73)
            {
                return_val = (ushort)gbl.SelectedPlayer._class;
            }
            else if (addr >= 0x9a && addr <= 0x9e)
            {
                return_val = gbl.SelectedPlayer.saveVerse[addr - 0x9A];
            }
            else if (addr == 0xa0)
            {
                return_val = gbl.SelectedPlayer.HitDice;
            }
            else if (addr >= 0xA5 && addr <= 0xAC)
            {
                return_val = gbl.SelectedPlayer.thief_skills[addr - 0xA5];
            }
            else if (addr == 0xB8)
            {
                return_val = gbl.SelectedPlayer.control_morale;
            }
            else if (addr == 0xBB)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Copper);
            }
            else if (addr == 0xBD)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Electrum);
            }
            else if (addr == 0xBF)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Silver);
            }
            else if (addr == 0xC1)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Gold);
            }
            else if (addr == 0xC3)
            {
                return_val = (ushort)gbl.SelectedPlayer.Money.GetCoins(Money.Platinum);
            }
            else if (addr == 0xC4)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.Cleric);
            }
            else if (addr == 0xC5)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.Druid);
            }
            else if (addr == 0xC6)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.Fighter);
            }
            else if (addr == 0xC7)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.Paladin);
            }
            else if (addr == 0xC8)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.Ranger);
            }
            else if (addr == 0xC9)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.MagicUser);
            }
            else if (addr == 0xCA)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.Thief);
            }
            else if (addr == 0xCB)
            {
                return_val = (ushort)gbl.SelectedPlayer.SkillLevel(SkillType.Monk);
            }
            else if (addr == 0xD6)
            {
                return_val = gbl.SelectedPlayer.sex;
            }
            else if (addr == 0xD8)
            {
                return_val = gbl.SelectedPlayer.alignment;
            }
            else if (addr == 0xE4)
            {
                return_val = (ushort)(gbl.SelectedPlayer.field_192 & 1);
            }
            else if (addr == 0xF7)
            {
                return_val = (ushort)gbl.SelectedPlayer.field_13C;
            }
            else if (addr == 0xF9)
            {
                return_val = gbl.SelectedPlayer.field_13E;
            }
            else if (addr == 0x100)
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
            else if (addr == 0x10C)
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
            else if (addr == 0x10D)
            {
                return_val = 0; /* Simeon */
                throw new System.NotImplementedException("Not sure what should happening this case");
                //jmp	func_end
            }
            else if (addr == 0x11B)
            {
                return_val = gbl.SelectedPlayer.movement;
            }
            else if (addr == 0x2B1)
            {
                return_val = find_gbl_player_index(gbl.SelectedPlayer);
            }
            else if (addr == 0x2B4)
            {
                return_val = find_gbl_player_index(gbl.SelectedPlayer);
            }
            else if (addr == 0x2CF)
            {
                switch (gbl.SelectedPlayer.stats.Cha.Current)
                {
                    case 3:
                        return_val = 0;
                        break;

                    case 4:
                        return_val = 5;
                        break;

                    case 5:
                        return_val = 10;
                        break;

                    case 6:
                        return_val = 15;
                        break;

                    case 7:
                        return_val = 20;
                        break;

                    case 8: case 9: case 10: case 11: case 12:
                        return_val = 25;
                        break;

                    case 13:
                        return_val = 30;
                        break;

                    case 14:
                        return_val = 35;
                        break;

                    case 15:
                        return_val = 40;
                        break;

                    case 16:
                        return_val = 50;
                        break;

                    case 17:
                        return_val = 65;
                        break;

                    case 18: case 19: case 20: case 21:
                    case 22: case 23: case 24: case 25:
                        return_val = 60;
                        break;

                    default:
                        return_val = 0;
                        break;
                }
            }
            else if (addr == 0x312)
            {
                return_val = gbl.game_area;
            }
            else if (addr == 0x33E)
            {
                return_val = gbl.area2_ptr.party_size;
            }
            else
            {
                return_val = 0; /* value not read if arg_0 is false */
                found = false;
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
                    val = gbl.stru_1B2CA[(loc - gbl.vm_mem2_offset) << 1]; // dword_1119E POR: dword_1119A
                    break;

                case 3:
                    val = gbl.ecl_ptr[loc - gbl.initial_ecl_offset]; // Read from tables in ecl POR: dword_1119E
                    break;

                case 4:
                    if (loc < 0xC04B)
                    {
                        switch (loc)
                        {
                            case 0x00B1:
                                val = (ushort)gbl.word_1D918; // POR: word_1372E
                                break;

                            case 0x00FB:
                                val = (ushort)gbl.word_1D914; // POR: word_1372A
                                break;

                            case 0x00FC:
                                val = (ushort)gbl.word_1D916; // POR: word_1372C
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

        internal static void CopyStringFromMemory(ushort location, ref CmdOperation.Operation op) // sub_31421, POR: sub_210BB
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

        internal static async Task<bool> set_player_values(ushort set_value, ushort switch_var)
        {
            //switch_var -= 0x7c00;

            if (switch_var == 0)
            {
                if (set_value == 0)
                {
                    gbl.redrawPartySummary2 = true;
                }
            }
            else if (switch_var >= 0x20 && switch_var <= 0x70)
            {
                int var_1 = switch_var - 0x1f;
                Logger.DebugWrite("Set Spell for: {0} slot: {1} to: {2}", gbl.SelectedPlayer, var_1, (byte)set_value);
                gbl.SelectedPlayer.spellList.AddLearnt(set_value & 0x0ff);
                //gbl.SelectedPlayer.spell_list[var_1] = (byte)(set_value);
            }
            else if (switch_var == 0xb8)
            {
                if (set_value > 178)
                {
                    set_value -= 50;
                }

                gbl.SelectedPlayer.control_morale = (byte)(set_value);
            }
            else if (switch_var == 0xbb)
            {
                gbl.SelectedPlayer.Money.SetCoins(Money.Copper, set_value);
            }
            else if (switch_var == 0xbd)
            {
                gbl.SelectedPlayer.Money.SetCoins(Money.Electrum, set_value);
            }
            else if (switch_var == 0xbf)
            {
                gbl.SelectedPlayer.Money.SetCoins(Money.Silver, set_value);
            }
            else if (switch_var == 0xc1)
            {
                gbl.SelectedPlayer.Money.SetCoins(Money.Gold, set_value);
            }
            else if (switch_var == 0xc3)
            {
                gbl.SelectedPlayer.Money.SetCoins(Money.Platinum, set_value);
            }
            else if (switch_var == 0xf7)
            {
                gbl.SelectedPlayer.field_13C = (short)(set_value);
            }
            else if (switch_var == 0xf9)
            {
                gbl.SelectedPlayer.field_13E = (byte)(set_value);
            }
            else if (switch_var == 0x100)
            {
                if (set_value >= 0x80)
                {
                    gbl.SelectedPlayer.in_combat = false;
                    if (set_value == 0x87)
                    {
                        gbl.SelectedPlayer.health_status = Status.stoned;
                    }
                }

                if (set_value == 0)
                {
                    gbl.redrawPartySummary1 = true;
                }
            }
            else if (switch_var == 0x10c)
            {
                switch (set_value)
                {
                    case 0:
                        gbl.SelectedPlayer.combat_team = CombatTeam.Ours;
                        gbl.SelectedPlayer.quick_fight = QuickFight.False;
                        break;

                    case 0x80:
                        gbl.SelectedPlayer.combat_team = CombatTeam.Ours;
                        gbl.SelectedPlayer.quick_fight = QuickFight.True;
                        break;

                    case 0x81:
                        gbl.SelectedPlayer.combat_team = CombatTeam.Enemy;
                        gbl.SelectedPlayer.quick_fight = QuickFight.True;
                        break;
                }
            }
            else if (switch_var == 0x312)
            {
                //seg042.set_game_area((byte)(set_value));
                gbl.game_area_backup = gbl.game_area;
                gbl.game_area = (byte)set_value;
            }
            else if (switch_var == 0x322)
            {
                if (set_value > 0x80)
                {
                    set_value &= 0x7f;

                    await ThreeD.LoadWalldef(1, (short)(set_value & 0xFF));
                }
            }
            else if (switch_var == 0x324)
            {
                if (set_value > 0x80)
                {
                    set_value &= 0x7f;

                    await ThreeD.LoadWalldef(2, (short)(set_value & 0xFF));
                }
            }
            else if (switch_var == 0x326)
            {
                if (set_value > 0x80)
                {
                    set_value &= 0x7f;

                    await ThreeD.LoadWalldef(3, (short)(set_value & 0xFF));
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> SetMemoryValue(ushort value, ushort location) // cmd_table01
        {
            Classes.Debug.OnMemoryWrite(location, value);

            byte var_2;

            int memType = GetMemoryValueType(location);

            //System.Console.WriteLine("  vm_SetMemoryValue: value: {0:X} loc: {1:X} type: {2:X}",
            //    value, location, memType);

            if (memType == 0)
            {
                if ((location - gbl.vm_mem0_offset) == 0x0FD || (location - gbl.vm_mem0_offset) == 0x0FE)
                {
                    //System.Console.WriteLine("    gbl.
                    //= 1");
                    gbl.skyColorChanged = true;
                }
                else if ((location - gbl.vm_mem0_offset) == 0x0E6 && gbl.area_ptr.inDungeon != value)
                {
                    gbl.last_game_state = gbl.game_state;
                    if (value == 0)
                    {
                        gbl.game_state = GameState.WildernessMap;
                    }
                    else
                    {
                        gbl.game_state = GameState.DungeonMap;
                    }
                }

                gbl.area_ptr.field_6A00_Set((location - gbl.vm_mem0_offset) * 2, value);
            }
            else if (memType == 1)
            {
                gbl.area2_ptr.field_800_Set((location - gbl.vm_mem1_offset) * 2, value);
                await set_player_values(value, (ushort)(location - gbl.vm_mem1_offset));
            }
            else if (memType == 2)
            {
                gbl.stru_1B2CA[(location - gbl.vm_mem2_offset) << 1] = value;
            }
            else if (memType == 3)
            {
                gbl.ecl_ptr[location - gbl.initial_ecl_offset] = (byte)value;
            }
            else if (memType == 4)
            {
                if (location < 0xBF68)
                {
                    switch (location)
                    {
                        case 0xFB:
                            gbl.word_1D914 = (short)value;
                            break;

                        case 0xFC:
                            gbl.word_1D916 = (short)value;
                            break;

                        case 0xB1:
                            gbl.word_1D918 = (short)value;
                            break;

                        case 0x3DE:
                            gbl.word_1EE76 = value;
                            break;

                        case 0xB8:
                            gbl.word_1EE78 = value;
                            break;

                        case 0xB9:
                            gbl.word_1EE7A = value;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    location -= 0xBF68;

                    switch (location)
                    {
                        case 0xE3:
                            gbl.mapPosX = (sbyte)(value);
                            gbl.positionChanged = true;
                            break;

                        case 0xE4:
                            gbl.mapPosY = (sbyte)(value);
                            gbl.positionChanged = true;
                            break;

                        case 0xE5:
                            do
                            {
                                var_2 = 1;
                                switch (value)
                                {
                                    case 0:
                                        gbl.mapDirection = 0;
                                        break;

                                    case 1:
                                        gbl.mapDirection = 2;
                                        break;

                                    case 2:
                                        gbl.mapDirection = 4;
                                        break;

                                    case 3:
                                        gbl.mapDirection = 6;
                                        break;

                                    default:
                                        var_2 = 0;
                                        value -= 4;
                                        break;
                                }
                            } while (var_2 != 1);

                            gbl.positionChanged = true;
                            break;

                        case 0xF1:
                            // POR: byte_13728 = (byte)value;
                            gbl.byte_1D912 = (byte)value;
                            gbl.paletteChanged = true; // POR: byte_14CA5
                            break;

                        case 0xF7:
                            // POR: byte_13729 = (byte)value;
                            gbl.byte_1D913 = (byte)value;
                            gbl.paletteChanged = true; // POR: byte_14CA5
                            break;

                        default:
                            break;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
