using Classes;
using Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Transactions;

namespace engine
{
    class ovr003
    {
        internal static Task<bool> CMD_Exit(ushort ecl_offset, string name)
        {
            //VmLog.WriteLine("CMD_Exit: restore_player_ptr {0}", gbl.restore_player_ptr);
            //VmLog.WriteLine("");
            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}", ecl_offset, gbl.command, name);

            if (gbl.restore_player_ptr == true)
            {
                gbl.SelectedPlayer = gbl.LastSelectedPlayer;
                gbl.restore_player_ptr = false;
            }

            gbl.encounter_flags[0] = false;
            gbl.encounter_flags[1] = false;

            gbl.spriteChanged = false;
            gbl.stopVM = true;

            gbl.ecl_offset++;

            if (gbl.vmCallStack.Count > 0)
            {
                //System.Console.Write("  vmCallStack:");
                //foreach (ushort us in gbl.vmCallStack)
                //{
                //    System.Console.Write(" {0,4:X", us);
                //}
                //System.Console.WriteLine();

                gbl.vmCallStack.Clear();
            }

            gbl.textYCol = 0x11;
            gbl.textXCol = 1;

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_Goto(ushort ecl_offset, string name)
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            ushort newOffset = gbl.cmd_ops[1].Word;

            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1));
            //VmLog.WriteLine("CMD_Goto: was: 0x{0:X} now: 0x{1:X}", gbl.ecl_offset, newOffset);

            gbl.ecl_offset = newOffset;

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_Gosub(ushort ecl_offset, string name)
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            ushort newOffset = gbl.cmd_ops[1].Word;

            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1));
            //VmLog.WriteLine("CMD_Gosub: was: 0x{0:X} now: 0x{1:X}", gbl.ecl_offset, newOffset);

            gbl.vmCallStack.Push(gbl.ecl_offset);
            gbl.ecl_offset = newOffset;

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_Compare(ushort ecl_offset, string name) // sub_2611D
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);

            if (gbl.cmd_ops[1].Code >= 0x80 ||
                gbl.cmd_ops[2].Code >= 0x80)
            {
                Vm.WriteLine("CMD_Compare: Strings '{0}' '{1}'", gbl.cmd_ops[2].String, gbl.cmd_ops[1].String);

                ovr008.compare_strings(gbl.cmd_ops[2].String, gbl.cmd_ops[1].String);
            }
            else
            {
                ushort value_a = ovr008.vm_GetCmdValue(1);
                ushort value_b = ovr008.vm_GetCmdValue(2);

                Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}  {4}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1), ovr008.vm_PrintCmd(2));
                //VmLog.WriteLine("CMD_Compare: Values: {0} {1}", value_b, value_a);
                ovr008.compare_variables(value_b, value_a);
            }

            return Task.FromResult(true);
        }


        internal static async Task<bool> CMD_AddSubDivMulti(ushort ecl_offset, string name) // sub_2619A
        {
            ushort value;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);

            ushort val_a = ovr008.vm_GetCmdValue(1);
            ushort val_b = ovr008.vm_GetCmdValue(2);

            ushort location = gbl.cmd_ops[3].Word;

            switch (gbl.command)
            {
                case 4:
                    value = (ushort)(val_a + val_b);
                    break;

                case 5:
                    value = (ushort)(val_b - val_a);
                    break;

                case 6:
                    value = (ushort)(val_a / val_b);
                    gbl.area2_ptr.field_67E = (short)(val_a % val_b);
                    break;

                case 7:
                    value = (ushort)(val_a * val_b);
                    break;

                default:
                    value = 0;
                    throw (new System.Exception("can't get here."));
            }
            string[] sym = { "", "", "", "", "A + B", "B - A", "A / B", "A * B" };
            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}  {4}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1), ovr008.vm_PrintCmd(2));
            //VmLog.WriteLine("CMD_AdSubDivMulti: {0} A: {1} B: {2} Loc: {3} Res: {4}",
            //    sym[gbl.command], val_a, val_b, new MemLoc(location), value);

            await Vm.SetMemoryValue(value, location);

            return true;
        }


        internal static async Task<bool> CMD_Random(ushort ecl_offset, string name) // sub_2623D
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);

            byte rand_max = (byte)ovr008.vm_GetCmdValue(1);

            if (rand_max < 0xff)
            {
                rand_max++;
            }

            ushort loc = gbl.cmd_ops[2].Word;

            byte val = seg051.Random(rand_max);

            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1));
            //VmLog.WriteLine("CMD_Random: Max: {0} Loc: {1} Val: {2}", rand_max, new MemLoc(loc), val);

            await Vm.SetMemoryValue(val, loc);

            return true;
        }


        internal static async Task<bool> CMD_Save(ushort ecl_offset, string name)
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);

            ushort loc = gbl.cmd_ops[2].Word;

            if (gbl.cmd_ops[1].Code < 0x80)
            {
                ushort val = ovr008.vm_GetCmdValue(1);

                //VmLog.WriteLine("CMD_Save: Value {0} Loc: {1}", val, new MemLoc(loc));
                Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}  {4}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1), ovr008.vm_PrintCmd(2));
                await Vm.SetMemoryValue(val, loc);
            }
            else
            {
                Vm.WriteLine("CMD_Save: String '{0}' Loc: {1}", gbl.cmd_ops[1].String, new MemLoc(loc));
                ovr008.vm_WriteStringToMemory(gbl.cmd_ops[1].String, loc);
            }

            return true;
        }


        internal static Task<bool> CMD_LoadCharacter(ushort ecl_offset, string name) /* sub_262E9 */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);

            int player_index = (byte)ovr008.vm_GetCmdValue(1);
            Vm.WriteLine("CMD_LoadCharacter: 0x{0:X}", player_index);

            gbl.restore_player_ptr = true;

            bool high_bit_set = (player_index & 0x80) != 0;
            player_index = player_index & 0x7f;

            Player player = player_index > 0 && player_index < gbl.TeamList.Count ? gbl.TeamList[player_index] : null;

            if (player != null)
            {
                gbl.SelectedPlayer = player;
                gbl.player_not_found = false;
            }
            else
            {
                gbl.player_not_found = true;
            }

            if (high_bit_set == true &&
                gbl.redrawPartySummary1 == true &&
                gbl.redrawPartySummary2 == true)
            {
                if (gbl.LastSelectedPlayer == player)
                {
                    gbl.restore_player_ptr = false;
                }
                gbl.SelectedPlayer = ovr018.FreeCurrentPlayer(gbl.SelectedPlayer, true, false);

                ovr025.PartySummary(gbl.SelectedPlayer);
                gbl.redrawPartySummary1 = false;
                gbl.redrawPartySummary2 = false;
            }

            return Task.FromResult(true);
        }


        internal static async Task<bool> CMD_SetupMonster(ushort ecl_offset, string name) /* sub_263C9 */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);

            byte sprite_id = (byte)ovr008.vm_GetCmdValue(1);
            byte max_distance = (byte)ovr008.vm_GetCmdValue(2);
            byte pic_id = (byte)ovr008.vm_GetCmdValue(3);

            Vm.WriteLine("CMD_SetupMonster: sprite id: {0} area2_ptr.field_580: {1} pic id: {2}", sprite_id, max_distance, pic_id);

            gbl.sprite_block_id = sprite_id;
            gbl.area2_ptr.max_encounter_distance = max_distance;
            gbl.pic_block_id = pic_id;

            gbl.area2_ptr.encounter_distance = ovr008.sub_304B4(gbl.mapDirection, gbl.mapPosY, gbl.mapPosX);

            if (gbl.area2_ptr.max_encounter_distance < gbl.area2_ptr.encounter_distance)
            {
                gbl.area2_ptr.encounter_distance = gbl.area2_ptr.max_encounter_distance;
            }
            await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);

            return true;
        }

        internal static async Task<bool> CMD_LoadMonster(ushort ecl_offset, string name) /* sub_26465 */
        {
            Player current_player_bkup = gbl.SelectedPlayer;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);

            if (gbl.numLoadedMonsters < 63)
            {
                int mod_id = ovr008.vm_GetCmdValue(1) & 0xFF;

                Player mobMasterCopy = await ovr017.load_mob(mod_id);

                Player newMob = mobMasterCopy.ShallowClone();

                int num_copies = ovr008.vm_GetCmdValue(2) & 0xFF;

                if (num_copies <= 0)
                {
                    num_copies = 1;
                }

                int blockId = ovr008.vm_GetCmdValue(3) & 0xFF;
                await ovr034.chead_cbody_comspr_icon(gbl.monster_icon_id, blockId, "CPIC");

                newMob.icon_id = gbl.monster_icon_id;

                gbl.TeamList.Add(newMob);

                gbl.numLoadedMonsters++;
                int copy_count = 1;

                while (copy_count < num_copies &&
                       gbl.numLoadedMonsters < 63)
                {
                    newMob = mobMasterCopy.ShallowClone();

                    newMob.icon_id = gbl.monster_icon_id;

                    newMob.affects = new List<Affect>();
                    newMob.items = new List<Item>();

                    foreach (Item item in mobMasterCopy.items)
                    {
                        newMob.items.Add(item.ShallowClone());
                    }

                    foreach (Affect affect in mobMasterCopy.affects)
                    {
                        newMob.affects.Add(affect.ShallowClone());
                    }

                    copy_count++;
                    gbl.numLoadedMonsters++;
                    gbl.TeamList.Add(newMob);
                }

                gbl.monster_icon_id++;
                gbl.monstersLoaded = true;
                gbl.SelectedPlayer = current_player_bkup;

                return true;
            }
            else
            {
                return false;
            }
        }


        internal static async Task<bool> CMD_Approach(ushort ecl_offset, string name) // sub_26835
        {
            if (gbl.area2_ptr.encounter_distance > 0)
            {
                gbl.area2_ptr.encounter_distance--;

                await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);
            }
            gbl.ecl_offset++;

            return true;
        }


        internal static async Task<bool> CMD_Picture(ushort ecl_offset, string name) /* sub_26873 */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            byte blockId = (byte)ovr008.vm_GetCmdValue(1);

            if (blockId != 0xff)
            {
                gbl.encounter_flags[1] = true;
                gbl.spriteChanged = true;

                if (gbl.area2_ptr.HeadBlockId == 0xff)
                {
                    gbl.byte_1EE8D = true;

                    if (blockId >= gbl.game.BigpicImage)
                    {
                        await ovr030.load_bigpic(blockId);
                        ovr030.draw_bigpic();
                        gbl.can_draw_bigpic = false;
                    }
                    else
                    {
                        await ovr030.load_pic_final(gbl.byte_1D556, 0, blockId, "PIC", gbl.game_area);
                        ovr030.DrawMaybeOverlayed(gbl.byte_1D556.frames[0].picture, true, 3, 3);
                    }
                }
                else
                {
                    await ovr008.set_and_draw_head_body(gbl.game_area, blockId, (byte)gbl.area2_ptr.HeadBlockId, 3, 3);
                }
            }
            else
            {
                if (gbl.spriteChanged == true || gbl.displayPlayerSprite)
                {
                    gbl.can_draw_bigpic = true;
                    await ovr029.RedrawView();
                    gbl.spriteChanged = false;
                    gbl.displayPlayerSprite = false;
                    gbl.byte_1EE8D = true;
                }
                gbl.encounter_flags[0] = false;
                gbl.encounter_flags[1] = false;
            }

            return true;
        }


        internal static async Task<bool> CMD_InputNumber(ushort ecl_offset, string name) /* sub_2695E */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);

            ushort loc = gbl.cmd_ops[2].Word;

            ushort var_4 = seg041.getUserInputShort(0, 0x0a, string.Empty);

            await Vm.SetMemoryValue(var_4, loc);

            return true;
        }


        internal static Task<bool> CMD_InputString(ushort ecl_offset, string name) /* sub_269A4 */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);

            ushort loc = gbl.cmd_ops[2].Word;

            string str = seg041.getUserInputString(0x28, 0, 10, string.Empty);

            if (str.Length == 0)
            {
                str = " ";
            }

            ovr008.vm_WriteStringToMemory(str, loc);

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_Print(ushort ecl_offset, string name)
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);

            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1));
            //VmLog.WriteLine("CMD_Print: '{0}'",
            //    gbl.cmd_ops[1].Code < 0x80 ? ovr008.vm_GetCmdValue(1).ToString() : gbl.unk_1D972[1]);

            gbl.bottomTextHasBeenCleared = false;
            gbl.DelayBetweenCharacters = true;

            if (gbl.cmd_ops[1].Code < 0x80)
            {
                gbl.cmd_ops[1].String = ovr008.vm_GetCmdValue(1).ToString();
            }

            if (gbl.command == 0x11)
            {
                seg041.press_any_key(gbl.cmd_ops[1].String, false, 10, TextRegion.NormalBottom);
            }
            else
            {
                gbl.textYCol = 0x11;
                gbl.textXCol = 1;

                seg041.press_any_key(gbl.cmd_ops[1].String, true, 10, TextRegion.NormalBottom);
            }

            gbl.DelayBetweenCharacters = false;

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_Return(ushort ecl_offset, string name)
        {
            gbl.ecl_offset++;
            if (gbl.vmCallStack.Count > 0)
            {
                ushort newOffset = gbl.vmCallStack.Peek();
                //VmLog.WriteLine("CMD_Return: was: {0:X} now: {1:X}", gbl.ecl_offset, newOffset);
                Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}", ecl_offset, gbl.command, name);
                gbl.vmCallStack.Pop();
                gbl.ecl_offset = newOffset;
            }
            else
            {
                Vm.Write("CMD_Return: call stack empty ");
                CMD_Exit(ecl_offset, name);
            }

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_CompareAnd(ushort ecl_offset, string name) /* sub_26B0C */
        {
            for (int i = 0; i < 6; i++)
            {
                gbl.compare_flags[i] = false;
            }

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 4);

            ushort var_8 = ovr008.vm_GetCmdValue(1);
            ushort var_6 = ovr008.vm_GetCmdValue(2);
            ushort var_4 = ovr008.vm_GetCmdValue(3);
            ushort var_2 = ovr008.vm_GetCmdValue(4);

            if (var_8 == var_6 &&
                var_4 == var_2)
            {
                gbl.compare_flags[0] = true;
            }
            else
            {
                gbl.compare_flags[1] = true;
            }

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_If(ushort ecl_offset, string name)
        {
            gbl.ecl_offset++;

            int index = gbl.command - 0x16;
            string[] types = { "==", "!=", "<", ">", "<=", ">=" };

            //VmLog.WriteLine("CMD_if: {0} {1}", types[index], gbl.compare_flags[index]);
            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2}", ecl_offset, gbl.command, name);

            if (gbl.compare_flags[index] == false)
            {
                SkipNextCommand();
            }

            return Task.FromResult(true);
        }


        internal static async Task<bool> CMD_NewECL(ushort ecl_offset, string name)
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);

            byte block_id = (byte)ovr008.vm_GetCmdValue(1);

            Vm.WriteLine("CMD_NewECL: block_id {0}", block_id);

            gbl.area_ptr.LastEclBlockId = gbl.EclBlockId;
            gbl.EclBlockId = block_id;

            await ovr008.load_ecl_dax(block_id);
            ovr008.vm_init_ecl();
            gbl.stopVM = true;
            gbl.vmFlag01 = true;

            gbl.encounter_flags[0] = false;
            gbl.encounter_flags[1] = false;

            return true;
        }


        internal static async Task<bool> CMD_LoadFiles(ushort ecl_offset, string name) /* sub_26C41 */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);

            gbl.byte_1AB0B = true;

            byte var_3 = (byte)ovr008.vm_GetCmdValue(1);
            byte var_2 = (byte)ovr008.vm_GetCmdValue(2);
            byte var_1 = (byte)ovr008.vm_GetCmdValue(3);

            Vm.WriteLine("CMD_LoadFile: {0} A: {1} B: {2} C: {3}",
                gbl.command == 0x21 ? "Files" : "Pieces", var_1, var_2, var_3);


            if (gbl.command == 0x21)
            {
                gbl.filesLoaded = true;

                if ((var_3 & 0x7f) != 0x7f &&
                    gbl.area_ptr.inDungeon != 0)
                {
                    gbl.area_ptr.current_3DMap_block_id = var_3;
                    await ovr031.Load3DMap(var_3);
                    gbl.area2_ptr.field_592 = 0;
                }

                if ((var_1 & 0x7f) != 0x7f &&
                    gbl.area_ptr.inDungeon == 0 &&
                    gbl.lastDaxBlockId != 0x50 &&
                    gbl.game.WildernessImage != 0xFF)
                {
                    await ovr030.load_bigpic(gbl.game.WildernessImage);
                }
            }
            else
            {
                gbl.byte_1AB0C = true;

                if (var_3 == 0x7F)
                {
                    await ovr031.LoadWalldef(1, 0);
                }
                else
                {
                    if (gbl.area_ptr.field_1CE != 0 &&
                        gbl.area_ptr.field_1D0 != 0)
                    {
                        if ((var_3 & 0x7f) != 0x7f)
                        {
                            await ovr031.LoadWalldef(1, var_3);
                        }

                        if ((var_1 & 0x7f) != 0x7f)
                        {
                            await ovr031.LoadWalldef(3, var_1);
                        }
                    }
                    else
                    {
                        if ((var_3 & 0x7f) != 0x7f)
                        {
                            await ovr031.LoadWalldef(1, var_3);
                        }
                        else
                        {
                            gbl.setBlocks[0].Reset();
                        }

                        if ((var_2 & 0x7f) != 0x7f)
                        {
                            await ovr031.LoadWalldef(2, var_2);
                        }
                        else
                        {
                            gbl.setBlocks[1].Reset();
                        }

                        if ((var_1 & 0x7f) != 0x7f)
                        {
                            await ovr031.LoadWalldef(3, var_1);
                        }
                        else
                        {
                            gbl.setBlocks[2].Reset();
                        }
                    }
                }
            }


            if (gbl.byte_1AB0C == true && gbl.filesLoaded == true)
            {
                if (gbl.game_state != GameState.WildernessMap &&
                    gbl.byte_1EE98 == true)
                {
                    gbl.game.DrawFrame_Dungeon();
                    ovr025.PartySummary(gbl.SelectedPlayer);
                    ovr025.display_map_position_time();
                }
                gbl.byte_1EE98 = false;
            }

            return true;
        }


        internal static async Task<bool> CMD_AndOr(ushort ecl_offset, string name) /* sub_26DD0 */
        {
            byte resultant;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);
            ushort val_a = ovr008.vm_GetCmdValue(1);
            ushort val_b = ovr008.vm_GetCmdValue(2);

            ushort loc = gbl.cmd_ops[3].Word;
            string sym;
            if (gbl.command == 0x2F)
            {
                sym = "And";
                resultant = (byte)(val_a & val_b);
            }
            else
            {
                sym = "Or";
                resultant = (byte)(val_a | val_b);
            }

            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}  {4}  {5}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1), ovr008.vm_PrintCmd(2), ovr008.vm_PrintCmd(3));
            //VmLog.WriteLine("CMD_AndOr: {0} A: {1} B: {2} Loc: {3} Val: {4}", sym, val_a, val_b, new MemLoc(loc), resultant);

            ovr008.compare_variables(resultant, 0);
            await Vm.SetMemoryValue(resultant, loc);

            return true;
        }


        internal static async Task<bool> CMD_GetTable(ushort ecl_offset, string name) /* sub_26E3F */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);

            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}  {4}  {5}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1), ovr008.vm_PrintCmd(2), ovr008.vm_PrintCmd(3));

            ushort var_2 = gbl.cmd_ops[1].Word;
            byte var_9 = (byte)ovr008.vm_GetCmdValue(2);

            ushort result_loc = gbl.cmd_ops[3].Word;

            ushort var_6 = (ushort)(var_9 + var_2);

            ushort var_8 = Vm.GetMemoryValue(var_6);
            await Vm.SetMemoryValue(var_8, result_loc);

            return true;
        }


        internal static async Task<bool> CMD_SaveTable(ushort ecl_offset, string name) /* sub_26E9D */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);

            ushort var_6 = ovr008.vm_GetCmdValue(1);

            ushort result_loc = gbl.cmd_ops[2].Word;
            result_loc += ovr008.vm_GetCmdValue(3);

            await Vm.SetMemoryValue(var_6, result_loc);

            return true;
        }


        internal static async Task<bool> CMD_VertMenu(ushort ecl_offset, string name) /* sub_26EE9 */
        {
            gbl.bottomTextHasBeenCleared = false;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);
            ushort mem_loc = gbl.cmd_ops[1].Word;

            string delay_text = gbl.cmd_ops[2].String;

            byte menuCount = (byte)ovr008.vm_GetCmdValue(3);
            gbl.ecl_offset--;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, menuCount);

            List<MenuItem> menuList = new List<MenuItem>();

            gbl.textXCol = 1;
            gbl.textYCol = 0x11;

            seg041.press_any_key(delay_text, true, 10, 22, 38, 17, 1);

            for (int i = 1; i <= menuCount; i++)
            {
                menuList.Add(new MenuItem(gbl.cmd_ops[i].String));
            }

            int index = ovr008.VertMenuSelect(0, true, false, menuList, 0x16, 0x26, gbl.textYCol + 1, 1);

            await Vm.SetMemoryValue((ushort)index, mem_loc);

            menuList.Clear();
            seg037.draw8x8_clear_area(TextRegion.NormalBottom);

            return true;
        }


        internal static async Task<bool> CMD_HorizontalMenu(ushort ecl_offset, string name)
        {
            bool useOverlay;
            bool var_3B;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);

            ushort loc = gbl.cmd_ops[1].Word;
            byte string_count = (byte)ovr008.vm_GetCmdValue(2);

            gbl.ecl_offset--;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, string_count);

            MenuColorSet colors;
            if (string_count == 1)
            {
                var_3B = true;
                colors = new MenuColorSet(15, 15, 13);

                if (gbl.cmd_ops[1].String == "PRESS BUTTON OR RETURN TO CONTINUE.")
                {
                    gbl.cmd_ops[1].String = "PRESS <ENTER>/<RETURN> TO CONTINUE";
                }
            }
            else
            {
                colors = new MenuColorSet(1, 15, 15);
                var_3B = false;
                colors = gbl.defaultMenuColors;
            }

            if (gbl.spriteChanged == false ||
                gbl.byte_1EE8D == false)
            {
                useOverlay = false;
            }
            else
            {
                useOverlay = true;
            }

            string text = string.Empty;
            for (int i = 1; i < string_count; i++)
            {
                text += "~" + gbl.cmd_ops[i].String + " ";
            }

            text += "~" + gbl.cmd_ops[string_count].String;

            byte menu_selected = (byte)ovr008.sub_317AA(useOverlay, var_3B, colors, text, "");

            await Vm.SetMemoryValue(menu_selected, loc);

            ovr027.ClearPromptAreaNoUpdate();

            return true;
        }

        /// <summary>
        /// Clears the pooled items and pool money.
        /// </summary>
        internal static Task<bool> CMD_ClearMonsters(ushort ecl_offset, string name) /* sub_27240 */
        {
            gbl.ecl_offset++;
            gbl.numLoadedMonsters = 0;
            gbl.monstersLoaded = false;
            gbl.monster_icon_id = 8;

            Vm.WriteLine("CMD_ClearMonsters:");

            gbl.pooled_money.ClearAll();
            gbl.items_pointer.Clear();

            return Task.FromResult(true);
        }


        internal static async Task<bool> CMD_PartyStrength(ushort ecl_offset, string name) /* sub_272A9 */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            byte power_value = 0;

            foreach (Player player in gbl.TeamList)
            {
                int hit_points = player.hit_point_current;
                int armor_class = player.ac;
                int hit_bonus = player.hitBonus;

                int magic_power = player.SkillLevel(SkillType.MagicUser);
                int cleric_power = player.SkillLevel(SkillType.Cleric);

                if (armor_class > 60)
                {
                    armor_class -= 60;
                }
                else
                {
                    armor_class = 0;
                }

                if (hit_bonus > 39)
                {
                    hit_bonus -= 39;
                }
                else
                {
                    hit_bonus = 0;
                }

                power_value += (byte)(((cleric_power * 4) + hit_points + (armor_class * 5) + (hit_bonus * 5) + (magic_power * 8)) / 10);
            }

            ushort loc = gbl.cmd_ops[1].Word;
            await Vm.SetMemoryValue(power_value, loc);

            return true;
        }


        internal static async Task<bool> setMemoryFour(bool val_d, byte val_c, byte val_b, byte val_a,
        ushort loc_a, ushort loc_b, ushort loc_c, ushort loc_d) /* sub_273F6 */
        {
            await Vm.SetMemoryValue(val_a, loc_a);
            await Vm.SetMemoryValue(val_b, loc_b);
            await Vm.SetMemoryValue(val_c, loc_c);
            await Vm.SetMemoryValue(val_d ? (ushort)1 : (ushort)0, loc_d);

            return true;
        }


        internal static async Task<bool> CMD_CheckParty(ushort ecl_offset, string name) /* sub_27454 */
        {
            int var_4;
            ushort var_2;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 6);

            if (gbl.cmd_ops[1].Code == 1)
            {
                var_2 = gbl.cmd_ops[1].Word;
            }
            else
            {
                var_2 = ovr008.vm_GetCmdValue(1);
            }

            Classes.Affects affect_id = (Classes.Affects)ovr008.vm_GetCmdValue(2);

            var loc_a = gbl.cmd_ops[3].Word;
            var loc_b = gbl.cmd_ops[4].Word;
            var loc_c = gbl.cmd_ops[5].Word;
            var loc_d = gbl.cmd_ops[6].Word;

            var_4 = 0;
            byte val_a = 0x0FF;
            byte val_b = 0;
            byte val_c;

            var_2 -= 0x7fff;

            if (var_2 == 8001)
            {
                bool affect_found = gbl.TeamList.Exists(player => player.HasAffect(affect_id));

                await setMemoryFour(affect_found, 0, 0, 0, loc_a, loc_b, loc_c, loc_d);
            }
            else if (var_2 >= 0x00A5 && var_2 <= 0x00AC)
            {
                int index = var_2 - 0xA5;
                int count = 0;
                foreach (Player player in gbl.TeamList)
                {
                    count++;

                    if (player.thief_skills[index] < val_a)
                    {
                        val_a = player.thief_skills[index];
                    }

                    if (player.thief_skills[index] > val_b)
                    {
                        val_b = player.thief_skills[index];
                    }

                    var_4 += player.thief_skills[index];
                }

                val_c = (byte)(var_4 / count);

                await setMemoryFour(false, val_c, val_b, val_a, loc_a, loc_b, loc_c, loc_d);
            }
            else if (var_2 == 0x9f)
            {
                int count = 0;
                foreach (Player player in gbl.TeamList)
                {
                    count++;

                    if (player.movement < val_a)
                    {
                        val_a = player.movement;
                    }

                    if (player.movement > val_b)
                    {
                        val_b = player.movement;
                    }

                    var_4 += player.movement;
                }

                val_c = (byte)(var_4 / count);

                await setMemoryFour(false, val_c, val_b, val_a, loc_a, loc_b, loc_c, loc_d);
            }

            return true;
        }


        internal static async Task<bool> CMD_PartySurprise(ushort ecl_offset, string name) /* sub_2767E */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);

            byte val_a = 0;
            byte val_b = 0;

            foreach (Player player in gbl.TeamList)
            {
                if (player._class == ClassId.ranger ||
                    player._class == ClassId.mc_c_r)
                {
                    val_a = 1;
                }
            }

            ushort loc_a = gbl.cmd_ops[1].Word;
            ushort loc_b = gbl.cmd_ops[2].Word;

            await Vm.SetMemoryValue(val_a, loc_a);
            await Vm.SetMemoryValue(val_b, loc_b);

            return true;
        }


        internal static async Task<bool> CMD_Surprise(ushort ecl_offset, string name) /* sub_2771E */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 4);
            byte val_a = 0;

            byte var_8 = (byte)ovr008.vm_GetCmdValue(1);
            byte var_7 = (byte)ovr008.vm_GetCmdValue(2);
            byte var_6 = (byte)ovr008.vm_GetCmdValue(3);
            byte var_5 = (byte)ovr008.vm_GetCmdValue(4);

            byte var_9 = (byte)((var_5 + 2) - var_8);
            byte var_A = (byte)((var_7 + 2) - var_6);

            byte var_1 = ovr024.roll_dice(6, 1);
            byte var_2 = ovr024.roll_dice(6, 1);

            if (var_1 <= var_9)
            {
                if (var_2 <= var_A)
                {
                    val_a = 3;
                }
                else
                {
                    val_a = 1;
                }
            }

            if (var_2 <= var_A)
            {
                val_a = 2;
            }

            await Vm.SetMemoryValue(val_a, 0x2cb);

            return true;
        }


        internal static async Task<bool> CMD_Combat(ushort ecl_offset, string name) // sub_277E4
        {
            gbl.ecl_offset++;

            if (gbl.monstersLoaded == false &&
                gbl.combat_type == CombatType.normal)
            {
                if (gbl.area2_ptr.EnterShop == 1)
                {
                    gbl.area2_ptr.EnterShop = 0;

                    await ovr007.CityShop();
                }
                else if (gbl.area2_ptr.EnterTemple == 1)
                {
                    gbl.area2_ptr.EnterTemple = 0;

                    await ovr005.temple_shop();
                }
                else
                {
                    await ovr006.AfterCombatExpAndTreasure();
                }
            }
            else
            {
                ushort var_2 = ovr008.sub_304B4(gbl.mapDirection, gbl.mapPosY, gbl.mapPosX);

                if (var_2 < gbl.area2_ptr.encounter_distance)
                {
                    gbl.area2_ptr.encounter_distance = var_2;
                }

                await ovr009.MainCombatLoop();

                await ovr006.AfterCombatExpAndTreasure();

                if (gbl.area_ptr.inDungeon == 0 &&
                    gbl.game.WildernessImage != 0xFF)
                {
                    await ovr030.load_bigpic(gbl.game.WildernessImage);
                }
            }

            if (gbl.area_ptr.inDungeon != 0)
            {
                gbl.game_state = GameState.DungeonMap;
            }
            else
            {
                gbl.game_state = GameState.WildernessMap;
            }

            gbl.area2_ptr.search_flags &= 1;

            gbl.encounter_flags[0] = false;
            gbl.encounter_flags[1] = false;
            gbl.spriteChanged = false;
            await ovr025.LoadPic();

            return true;
        }


        internal static Task<bool> CMD_OnGotoGoSub(ushort ecl_offset, string name) /* sub_27AE5 */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);
            byte var_1 = (byte)ovr008.vm_GetCmdValue(1);
            byte var_2 = (byte)ovr008.vm_GetCmdValue(2);
            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}  {4}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1), ovr008.vm_PrintCmd(2));
            gbl.ecl_offset--;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, var_2);

            for (int i=1; i<=var_2; i++)
            {
                Vm.WriteLine("${0,4:X4}                    {1}             // {2}", ecl_offset + 3 + (i * 3), ovr008.vm_PrintCmd(i), i);
            }

            if (var_1 < var_2)
            {
                ushort newloc = gbl.cmd_ops[var_1 + 1].Word;
                //VmLog.WriteLine("CMD_OnGotoGoSub: {4} A: {0} B: {1} Was: 0x{2:X} Now: 0x{3:X}",
                //    var_1, var_2, gbl.ecl_offset, newloc,
                //    gbl.command == 0x25 ? "Goto" : "Gosub");

                if (gbl.command == 0x25)
                {
                    // Goto
                    gbl.ecl_offset = newloc;
                }
                else
                {
                    // Gosub
                    gbl.vmCallStack.Push(gbl.ecl_offset);
                    gbl.ecl_offset = newloc;
                }
            }
            else
            {
                //VmLog.WriteLine("CMD_OnGotoGoSub: {0} A: {1} B: {2}",
                //    gbl.command == 0x25 ? "Goto" : "Gosub", var_1, var_2);
            }

            return Task.FromResult(true);
        }



        internal static async Task<bool> CMD_Treasure(ushort ecl_offset, string name) /* load_item */
        {
            byte[] data;
            ushort dataSize;
            Item.Type item_type = 0;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 8);

            for (int coin = 0; coin < 7; coin++)
            {
                gbl.pooled_money.SetCoins(coin, ovr008.vm_GetCmdValue(coin + 1));
            }

            byte block_id = (byte)ovr008.vm_GetCmdValue(8);

            if (block_id < 0x80)
            {
                (data, dataSize) = await seg042.load_decode_dax(block_id, "ITEM", gbl.game_area);

                if (dataSize == 0)
                {
                    Logger.LogAndExit("Unable to find item file: {0}{1}", "ITEM", gbl.game_area);
                }

                for (int offset = 0; offset < dataSize; offset += Item.StructSize)
                {
                    gbl.items_pointer.Add(new Classes.Curse.Item(data, offset).Load());
                }

                data = null;
            }
            else if (block_id != 0xff)
            {
                for (int count = 0; count < (block_id - 0x80); count++)
                {
                    int roll1 = ovr024.roll_dice(100, 1);

                    if (roll1 >= 1 && roll1 <= 60)
                    {
                        int roll2 = ovr024.roll_dice(100, 1);

                        if ((roll2 >= (int)Item.Type.BattleAxe && roll2 <= (int)Item.Type.Sling) ||
                            (roll2 >= (int)Item.Type.LeatherArmor && roll2 <= (int)Item.Type.Shield))
                        {
                            if (roll2 == (int)Item.Type.FineBow)
                            {
                                item_type = Item.Type.Shield;
                            }
                            else
                            {
                                item_type = (Item.Type)roll2;
                            }
                        }
                        else if (roll2 >= 60 && roll2 <= 90)
                        {
                            int roll3 = ovr024.roll_dice(10, 1);

                            if (roll3 >= 1 && roll3 <= 4)
                            {
                                item_type = Item.Type.LongSword;
                            }
                            else if (roll3 >= 5 && roll3 <= 7)
                            {
                                item_type = Item.Type.BroadSword;
                            }
                            else if (roll3 == 8)
                            {
                                item_type = Item.Type.BastardSword;
                            }
                            else if (roll3 == 9)
                            {
                                item_type = Item.Type.ShortSword;
                            }
                            else if (roll3 == 10)
                            {
                                item_type = Item.Type.TwoHandedSword;
                            }
                        }
                        else if (roll2 >= 91 && roll2 <= 94)
                        {
                            item_type = Item.Type.Arrow;
                        }
                        else if (roll2 >= 95 && roll2 <= 97)
                        {
                            item_type = Item.Type.RingOfProt;
                        }
                        else if (roll2 >= 98 && roll2 <= 100)
                        {
                            item_type = Item.Type.Bracers;
                        }
                        else // roll == 48 || roll == 49
                        {
                            item_type = Item.Type.Shield;
                        }
                    }
                    else if (roll1 >= 61 && roll1 <= 85)
                    {
                        item_type = Item.Type.MUScroll;
                    }
                    else if (roll1 >= 86 && roll1 <= 92)
                    {
                        item_type = Item.Type.ClrcScroll;
                    }
                    else if (roll1 >= 93 && roll1 <= 98) // CoAB: 91 - 98
                    {
                        if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                        {
                            int roll2 = ovr024.roll_dice(16, 1);

                            if (roll2 >= 1 && roll2 <= 7)
                            {
                                item_type = Item.Type.Potion;
                            }
                            else if (roll2 >= 8 && roll2 <= 9)
                            {
                                item_type = Item.Type.GemsJewelry;
                            }
                            else if (roll2 == 10)
                            {
                                item_type = Item.Type.PotionOfGiantStr;
                            }
                            else if (roll2 >= 11 && roll2 <= 12)
                            {
                                item_type = Item.Type.WandA;
                            }
                            else if (roll2 >= 13 && roll2 <= 14)
                            {
                                item_type = Item.Type.WandB;
                            }
                            else if (roll2 == 15)
                            {
                                item_type = Item.Type.CloakOfDisplacement;
                            }
                            else if (roll2 == 16)
                            {
                                item_type = Item.Type.Cloak;
                            }
                        }
                        else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                        {
                            int roll2 = ovr024.roll_dice(15, 1);

                            if (roll2 >= 1 && roll2 <= 9)
                            {
                                item_type = Item.Type.Potion;
                            }
                            else if (roll2 == 10)
                            {
                                item_type = Item.Type.PotionOfGiantStr;
                            }
                            else if (roll2 >= 11 && roll2 <= 15)
                            {
                                item_type = Item.Type.WandB;
                            }
                        }
                    }
                    else if (roll1 == 99 || roll1 == 100)
                    {
                        if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                        {
                            item_type = Item.Type.Ring;
                        }
                        else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                        {
                            item_type = Item.Type.Shield;
                        }
                    }

                    gbl.items_pointer.Add(ovr022.create_item(item_type));
                }

                gbl.items_pointer.ForEach(item => ovr025.ItemDisplayNameBuild(false, false, 0, 0, item));
            }

            return true;
        }


        internal static Task<bool> CMD_Rob(ushort ecl_offset, string name) /* sub_27F76*/
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);
            byte allParty = (byte)ovr008.vm_GetCmdValue(1);
            byte var_2 = (byte)ovr008.vm_GetCmdValue(2);

            double percentage = (100 - var_2) / 100.0;
            int robChance = (byte)ovr008.vm_GetCmdValue(3);

            if (allParty == 0)
            {
                ovr008.RobMoney(gbl.SelectedPlayer, percentage);
                ovr008.RobItems(gbl.SelectedPlayer, robChance);
            }
            else
            {
                foreach (Player player in gbl.TeamList)
                {
                    ovr008.RobMoney(player, percentage);
                    ovr008.RobItems(player, robChance);
                }
            }

            return Task.FromResult(true);
        }


        internal static async Task<bool> CMD_EncounterMenu(ushort ecl_offset, string name)
        {
            ushort var_43D;
            int var_43B;
            byte var_43A;
            string displayText;
            bool useOverlay;
            bool clearTextArea;
            byte init_max;
            byte init_min;
            byte var_40A;
            byte var_408;
            byte var_407;
            string text = string.Empty; /* Simeon */
            List<string> strings = [];
            List<byte> actions = [];
            int menu_selected;

            gbl.byte_1EE95 = true;
            gbl.bottomTextHasBeenCleared = false;
            gbl.DelayBetweenCharacters = true;

            ovr008.calc_group_movement(out init_min, out var_40A);

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 14);

            gbl.sprite_block_id = (byte)ovr008.vm_GetCmdValue(1);
            gbl.area2_ptr.max_encounter_distance = ovr008.vm_GetCmdValue(2);
            gbl.pic_block_id = (byte)ovr008.vm_GetCmdValue(3);

            var_43D = gbl.cmd_ops[4].Word;

            for (int i = 5; i <= 9; i++)
            {
                actions.Add((byte)ovr008.vm_GetCmdValue(i));
            }

            for (int i = 10; i <= 12; i++)
            {
                strings.Add(gbl.cmd_ops[i].String);
            }

            var_407 = (byte)ovr008.vm_GetCmdValue(13);
            var_408 = (byte)ovr008.vm_GetCmdValue(14);

            gbl.area2_ptr.encounter_distance = ovr008.sub_304B4(gbl.mapDirection, gbl.mapPosY, gbl.mapPosX);

            if (gbl.area2_ptr.max_encounter_distance < gbl.area2_ptr.encounter_distance)
            {
                gbl.area2_ptr.encounter_distance = gbl.area2_ptr.max_encounter_distance;
            }

            await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);

            do
            {
                if (gbl.spriteChanged == false ||
                    gbl.byte_1EE8D == false ||
                    gbl.area_ptr.inDungeon == 0 ||
                    gbl.lastDaxBlockId == 0x50)
                {
                    useOverlay = false;
                }
                else
                {
                    useOverlay = true;
                }

                clearTextArea = (gbl.area_ptr.inDungeon != 0);

                init_max = 0;
                gbl.textXCol = 1;
                gbl.textYCol = 0x11;

                switch (gbl.area2_ptr.encounter_distance)
                {
                    case 0:
                        var_43B = 0;

                        do
                        {
                            text = strings[var_43B];
                            var_43B++;
                        } while (text.Length == 0 && var_43B < 3);
                        break;

                    case 1:
                        var_43B = 1;

                        do
                        {
                            text = strings[var_43B];
                            var_43B++;

                            if (var_43B > 2)
                            {
                                var_43B = 0;
                            }
                        } while (text.Length == 0 && var_43B != 1);
                        break;

                    case 2:
                        var_43B = 2;

                        do
                        {
                            text = strings[var_43B];

                            var_43B++;
                            if (var_43B > 2)
                            {
                                var_43B = 0;
                            }

                        } while (text.Length == 0 && var_43B != 2);
                        break;
                }

                if (text.Length == 0)
                {
                    clearTextArea = false;
                }

                seg041.press_any_key(text, clearTextArea, 10, TextRegion.NormalBottom);

                if (gbl.area2_ptr.encounter_distance == 0 ||
                    gbl.area_ptr.inDungeon == 0)
                {
                    displayText = "~COMBAT ~WAIT ~FLEE ~PARLAY";
                }
                else
                {
                    displayText = "~COMBAT ~WAIT ~FLEE ~ADVANCE";
                }

                menu_selected = ovr008.sub_317AA(useOverlay, false, gbl.defaultMenuColors, displayText, "");

                if (gbl.area2_ptr.encounter_distance == 0 ||
                    gbl.area_ptr.inDungeon == 0)
                {
                    if (menu_selected == 3)
                    {
                        menu_selected = 4;
                    }
                }

                var_43A = actions[menu_selected];

                switch (var_43A)
                {
                    case 0:
                        if (menu_selected != 2)
                        {
                            await Vm.SetMemoryValue(1, var_43D);
                        }
                        else
                        {
                            if (init_min >= var_407)
                            {
                                await Vm.SetMemoryValue(2, var_43D);
                            }
                            else
                            {
                                await Vm.SetMemoryValue(1, var_43D);
                            }
                        }
                        break;

                    case 1:
                        if (menu_selected == 0)
                        {
                            await Vm.SetMemoryValue(1, var_43D);
                        }
                        else if (menu_selected == 1)
                        {
                            init_max = 1;
                            seg041.press_any_key("Both sides wait.", true, 10, TextRegion.NormalBottom);
                        }
                        else if (menu_selected == 2)
                        {
                            await Vm.SetMemoryValue(2, var_43D);
                        }
                        else if (menu_selected == 3)
                        {
                            if (gbl.area2_ptr.encounter_distance != 0)
                            {
                                gbl.area2_ptr.encounter_distance--;

                                await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);
                            }
                            else
                            {
                                seg041.press_any_key("Both sides wait.", true, 10, TextRegion.NormalBottom);
                            }

                            init_max = 1;
                        }
                        else if (menu_selected == 4)
                        {
                            if (gbl.area2_ptr.encounter_distance > 0)
                            {
                                gbl.area2_ptr.encounter_distance--;
                                await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);
                                init_max = 1;
                            }
                            else
                            {
                                await Vm.SetMemoryValue(3, var_43D);
                            }
                        }
                        break;

                    case 2:
                        if (menu_selected == 0)
                        {
                            if (var_408 > var_40A)
                            {
                                await Vm.SetMemoryValue(0, var_43D);

                                gbl.textXCol = 1;
                                gbl.textYCol = 0x11;
                                seg041.press_any_key("The monsters flee.", true, 10, TextRegion.NormalBottom);
                            }
                            else
                            {
                                await Vm.SetMemoryValue(1, var_43D);
                            }
                        }
                        else if (menu_selected >= 1 && menu_selected <= 4)
                        {
                            await Vm.SetMemoryValue(0, var_43D);

                            gbl.textXCol = 1;
                            gbl.textYCol = 0x11;
                            seg041.press_any_key("The monsters flee.", true, 10, TextRegion.NormalBottom);
                        }
                        break;

                    case 3:
                        if (menu_selected == 0)
                        {
                            await Vm.SetMemoryValue(1, var_43D);
                        }
                        else if (menu_selected == 1 || menu_selected == 3)
                        {
                            if (gbl.area2_ptr.encounter_distance != 0)
                            {
                                gbl.area2_ptr.encounter_distance--;

                                await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);
                            }
                            else
                            {
                                seg041.press_any_key("Both sides wait.", true, 10, TextRegion.NormalBottom);
                            }

                            init_max = 1;
                        }
                        else if (menu_selected == 2)
                        {
                            await Vm.SetMemoryValue(2, var_43D);
                        }
                        else if (menu_selected == 4)
                        {
                            if (gbl.area2_ptr.encounter_distance <= 0)
                            {
                                await Vm.SetMemoryValue(3, var_43D);
                            }
                            else
                            {
                                gbl.area2_ptr.encounter_distance--;

                                await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);
                                init_max = 1;
                            }
                        }
                        break;

                    case 4:
                        if (menu_selected == 0)
                        {
                            await Vm.SetMemoryValue(1, var_43D);
                        }
                        else if (menu_selected == 1 || menu_selected == 3 || menu_selected == 4)
                        {

                            if (gbl.area2_ptr.encounter_distance <= 0)
                            {
                                await Vm.SetMemoryValue(3, var_43D);
                            }
                            else
                            {
                                gbl.area2_ptr.encounter_distance -= 1;

                                await ovr008.sub_30580(gbl.encounter_flags, gbl.area2_ptr.encounter_distance, gbl.pic_block_id, gbl.sprite_block_id);
                                init_max = 1;
                            }
                        }
                        else if (menu_selected == 2)
                        {
                            await Vm.SetMemoryValue(2, var_43D);
                        }

                        break;
                }
            } while (init_max != 0);

            ovr027.ClearPromptArea();
            gbl.DelayBetweenCharacters = false;
            gbl.byte_1EE95 = false;

            return true;
        }


        internal static async Task<bool> CMD_Parlay(ushort ecl_offset, string name) /* talk_style */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 6);

            byte[] values = new byte[5];
            for (int i = 0; i < 5; i++)
            {
                values[i] = (byte)ovr008.vm_GetCmdValue(i + 1);
            }

            int menu_selected = ovr008.sub_317AA(false, false, gbl.defaultMenuColors, "~HAUGHTY ~SLY ~NICE ~MEEK ~ABUSIVE", " ");

            ushort location = gbl.cmd_ops[6].Word;

            byte value = values[menu_selected];

            await Vm.SetMemoryValue(value, location);

            return true;
        }


        internal static Task<bool> CMD_FindItem(ushort ecl_offset, string name) // sub_28856
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);

            Item.Type item_type = (Item.Type)ovr008.vm_GetCmdValue(1);

            for (int i = 0; i < 6; i++)
            {
                gbl.compare_flags[i] = false;
            }

            gbl.compare_flags[1] = true;

            foreach (Player player in gbl.TeamList)
            {
                foreach (Item item in player.items)
                {
                    if (item_type == item.type)
                    {
                        gbl.compare_flags[0] = true;
                        gbl.compare_flags[1] = false;
                        return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
        }


        internal static Task<bool> CMD_Delay(ushort ecl_offset, string name)
        {
            gbl.ecl_offset++;
            seg041.GameDelay();

            return Task.FromResult(true);
        }


        internal static async Task<bool> CMD_Damage(ushort ecl_offset, string name) /* sub_28958 */
        {
            Player currentPlayerBackup = gbl.SelectedPlayer;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 5);
            byte var_1 = (byte)ovr008.vm_GetCmdValue(1);
            int dice_count = ovr008.vm_GetCmdValue(2);
            int dice_size = ovr008.vm_GetCmdValue(3);
            int dam_plus = ovr008.vm_GetCmdValue(4);
            byte var_6 = (byte)ovr008.vm_GetCmdValue(5);

            int damage = ovr024.roll_dice(dice_size, dice_count) + dam_plus;

            byte rnd_player_id = 0;
            if ((var_1 & 0x40) == 0)
            {
                rnd_player_id = ovr024.roll_dice(gbl.area2_ptr.party_size, 1);
            }

            if ((var_1 & 0x80) != 0)
            {
                int saveBonus = var_1 & 0x1f;
                int bonusType = var_6 & 7;

                if ((var_1 & 0x40) != 0)
                {
                    foreach (Player player03 in gbl.TeamList)
                    {
                        if ((var_1 & 0x20) != 0)
                        {
                            ovr008.sub_32200(player03, damage);
                        }
                        else if (await ovr024.RollSavingThrow(saveBonus, (SaveVerseType)bonusType, player03) == false)
                        {
                            ovr008.sub_32200(player03, damage);
                        }
                        else if ((var_1 & 0x10) != 0)
                        {
                            ovr008.sub_32200(player03, damage);
                        }
                    }
                }
                else
                {
                    if ((var_6 & 0x80) != 0)
                    {
                        if (bonusType == 0 ||
                            await ovr024.RollSavingThrow(saveBonus, (SaveVerseType)(bonusType - 1), gbl.SelectedPlayer) == false)
                        {
                            ovr008.sub_32200(gbl.SelectedPlayer, damage);
                        }
                        else if ((var_1 & 0x10) != 0)
                        {
                            ovr008.sub_32200(gbl.SelectedPlayer, damage);
                        }
                    }
                    else
                    {
                        Player target = gbl.TeamList[rnd_player_id - 1];

                        if (await ovr024.RollSavingThrow(saveBonus, (SaveVerseType)bonusType, target) == false)
                        {
                            ovr008.sub_32200(target, damage);
                        }
                        else if ((var_1 & 0x10) != 0)
                        {
                            ovr008.sub_32200(target, damage);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < var_1; i++)
                {
                    rnd_player_id = ovr024.roll_dice(gbl.area2_ptr.party_size, 1);
                    Player player03 = gbl.TeamList[rnd_player_id - 1];

                    if (await ovr024.CanHitTarget(var_6, player03) == true)
                    {
                        ovr008.sub_32200(player03, damage);
                    }

                    damage = ovr024.roll_dice(dice_size, dice_count) + dam_plus;
                }
            }

            gbl.party_killed = true;

            foreach (Player player in gbl.TeamList)
            {
                if (player.in_combat == true)
                {
                    gbl.party_killed = false;
                }
            }

            if (gbl.party_killed == true)
            {
                gbl.game.DrawFrame_Outer();
                gbl.textXCol = 2;
                gbl.textYCol = 2;

                seg041.press_any_key("The entire party is killed!", true, 10, 0x16, 0x26, 1, 1);
                seg049.SysDelay(3000);
            }

            gbl.SelectedPlayer = currentPlayerBackup;
            seg041.DisplayAndPause("press <enter>/<return> to continue", 15);

            return true;
        }


        internal static async Task<bool> CMD_SpriteOff(ushort ecl_offset, string name) /* sub_28CB6 */
        {
            gbl.ecl_offset++;
            if (gbl.displayPlayerSprite)
            {
                gbl.can_draw_bigpic = true;
                await ovr029.RedrawView();
                gbl.displayPlayerSprite = false;
                gbl.spriteChanged = false;

                return true;
            }
            else
            {
                return false;
            }
        }


        internal static async Task<bool> CMD_EclClock(ushort ecl_offset, string name) /* sub_28CDA */
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, gbl.game.EclClockArguments);
            int timeStep = ovr008.vm_GetCmdValue(1) & 0xff;
            int timeSlot = 1;

            if (gbl.game.EclClockArguments == 2)
            {
                timeSlot = ovr008.vm_GetCmdValue(2) & 0xff;
            }

            await ovr021.step_game_time(timeSlot, timeStep);

            return true;
        }


        internal static Task<bool> CMD_PrintReturn(ushort ecl_offset, string name) // sub_28D0F
        {
            gbl.ecl_offset++;

            Vm.WriteLine("CMD_PrintReturn:");

            gbl.textXCol = 1;
            gbl.textYCol++;

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_ClearBox(ushort ecl_offset, string name) // sub_28D38 
        {
            gbl.ecl_offset++;

            Vm.WriteLine("CMD_ClearBox:");

            gbl.game.DrawFrame_Dungeon();
            if (!gbl.monstersLoaded)
            {
                ovr025.PartySummary(gbl.SelectedPlayer);
            }
            ovr025.display_map_position_time();
            ovr030.DrawMaybeOverlayed(gbl.byte_1D556.frames[0].picture, true, 3, 3);
            ovr025.display_map_position_time();
            gbl.byte_1EE98 = false;

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_Who(ushort ecl_offset, string name) // sub_28D7F
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            string prompt = gbl.cmd_ops[1].String;

            Vm.WriteLine("CMD_Who: Prompt: '{0}'", prompt);

            seg037.draw8x8_clear_area(TextRegion.NormalBottom);
            ovr025.selectAPlayer(ref gbl.SelectedPlayer, false, prompt);

            return Task.FromResult(true);
        }


        internal static async Task<bool> CMD_AddNPC(ushort ecl_offset, string name) // sub_28DCA
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 2);
            int npc_id = (byte)ovr008.vm_GetCmdValue(1);

            byte morale = (byte)ovr008.vm_GetCmdValue(2);

            await ovr017.load_npc(npc_id, morale);

            ovr025.reclac_player_values(gbl.SelectedPlayer);
            ovr025.PartySummary(gbl.SelectedPlayer);

            return true;
        }


        internal static async Task<bool> CMD_Spell(ushort ecl_offset, string name)
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 3);

            byte spell_id = (byte)ovr008.vm_GetCmdValue(1);
            ushort loc_a = gbl.cmd_ops[2].Word;
            ushort loc_b = gbl.cmd_ops[3].Word;

            byte spell_index = 1;
            byte player_index = 0;

            bool spell_found = false;

            foreach (Player player in gbl.TeamList)
            {
                spell_index = 1;

                foreach (int id in player.spellList.IdList())
                {
                    if (id == spell_id)
                    {
                        spell_found = true;
                        break;
                    }

                    spell_index += 1;
                }

                if (spell_found) break;

                player_index++;
            }

            if (spell_found == false)
            {
                player_index--;
                spell_index = 0x0FF;
            }

            Vm.WriteLine("CMD_Spell: spell_id: {0} loc a: {1} val a: {2} loc b: {3} val b: {4}",
                spell_id, new MemLoc(loc_a), spell_index, new MemLoc(loc_b), player_index);

            await Vm.SetMemoryValue(spell_index, loc_a);
            await Vm.SetMemoryValue(player_index, loc_b);

            return true;
        }


        internal static async Task<bool> CMD_Call(ushort ecl_offset, string name)
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1); // POR: sub_2FD

            ushort addr = gbl.cmd_ops[1].Word;

            Vm.WriteLine("${0,4:X4}   {1,2:X2}   {2,-10}  {3}", ecl_offset, gbl.command, name, ovr008.vm_PrintCmd(1));
            //VmLog.WriteLine("CMD_Call: {0:X}", addr);

            if (gbl.game.CallRedraw == addr)
            {
                gbl.mapWallRoof = ovr031.get_wall_x2(gbl.mapPosY, gbl.mapPosX); // POR: byte_13271 = sub_3D84(byte_1326D, byte_1326E)

                if (gbl.byte_1AB0B == true) // POR: byte_10BF1
                {
                    if (gbl.spriteChanged == true ||   // POR: byte_14CA0
                        gbl.displayPlayerSprite ||     // POR: byte_14CA3
                        gbl.paletteChanged == true ||      // POR: byte_14CA5
                        gbl.positionChanged == true || // POR: byte_14CA6
                        gbl.skyColorChanged == true)        // POR: byte_14CA8
                    {
                        gbl.can_draw_bigpic = true;           // POR: nope?
                        await ovr029.RedrawView();            // POR: sub_3CC5
                        ovr025.display_map_position_time();   // POR: sub_34AB
                        gbl.skyColorChanged = false;
                        gbl.paletteChanged = false;
                        gbl.positionChanged = false;
                        gbl.spriteChanged = false;
                        gbl.displayPlayerSprite = false;

                        gbl.mapWallType = ovr031.getMap_wall_type(gbl.mapDirection, gbl.mapPosY, gbl.mapPosX); // POR: nope?
                    }
                }
            }
            else if (gbl.game.CallDuelPlayer == addr)
            {
                await ovr008.SetupDuel(true);
            }
            else if (gbl.game.CallDuelMonster == addr)
            {
                await ovr008.SetupDuel(false);
            }
            else if (gbl.game.CallSound == addr)
            {
                if (gbl.word_1EE76 == 8)
                {
                    seg044.PlaySound(Sound.sound_a);
                }
                else if (gbl.word_1EE76 == 10)
                {
                    seg044.PlaySound(Sound.sound_b);
                }
                else
                {
                    seg044.PlaySound(Sound.sound_a);
                }
            }
            else if (gbl.game.CallMove == addr)
            {
                ovr008.MovePositionForward();
            }
            else if (addr == 0xC01B)
            {
                //ovr025.display_map_position_time();
            }
            else if (gbl.game.CallWall == addr)
            {
                if ((gbl.game.Name == Logging.Game.PoolOfRadiance && gbl.wilderness_area == 1) ||
                    (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds && gbl.area_ptr.inDungeon == 0))
                {
                    gbl.mapWallType = ovr031.getMap_wall_type(gbl.mapDirection, gbl.mapPosY, gbl.mapPosX);
                }
                else
                {

                }
            }
            else if (gbl.game.CallDemo == addr)
            {
                ovr030.DrawMaybeOverlayed(gbl.byte_1D556.CurrentPicture(), true, 3, 3);

                gbl.byte_1D556.NextFrame();

                seg041.GameDelay();
            }
            else
            {

            }

            return true;
        }


        internal static async Task<bool> TryEncamp()
        {
            await RunEclVm(gbl.PreCampCheckAddr);

            if (await ovr016.MakeCamp() == true)
            {
                await ovr025.LoadPic();
                await RunEclVm(gbl.CampInterruptedAddr);
            }

            gbl.can_draw_bigpic = true;
            await ovr029.RedrawView();
            gbl.gameSaved = false;

            return true;
        }


        internal static async Task<bool> CMD_Program(ushort ecl_offset, string name) //YourHaveWon
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            byte cmd = (byte)ovr008.vm_GetCmdValue(1);

            if (gbl.restore_player_ptr == true)
            {
                gbl.SelectedPlayer = gbl.LastSelectedPlayer;
                gbl.restore_player_ptr = false;
            }


            if (cmd == 0)
            {
                await ovr018.startGameMenu();
                if (gbl.lastDaxBlockId != 0x50 &&
                    gbl.area_ptr.inDungeon == 0)
                {
                    await ovr025.LoadPic();
                }
            }
            else if (cmd == 8)
            {
                await ovr019.end_game_text();
                gbl.gameWon = true;
                gbl.area2_ptr.training_class_mask = 0xff;

                foreach (Player player in gbl.TeamList)
                {
                    Player play_ptr = player;
                    play_ptr.hit_point_current = play_ptr.hit_point_max;
                    play_ptr.health_status = Status.okey;
                    play_ptr.in_combat = true;
                }

                if (gbl.game.GameWonGameOver)
                {
                    gbl.area_ptr.gameOver = 0xff;

                    await ovr018.startGameMenu();
                    char saveYes = ovr027.yes_no(gbl.defaultMenuColors, "You've won. Save before quitting? ");

                    if (saveYes == 'Y')
                    {
                        await ovr017.SaveGame();
                    }

                    seg043.print_and_exit();
                }
            }
            else if (cmd == 9)
            {
                ushort ecl_bkup = gbl.ecl_offset;
                await TryEncamp();
                gbl.ecl_offset = ecl_bkup;
                await CMD_Exit(ecl_offset, name);
            }
            else if (cmd == 3)
            {
                gbl.party_killed = true;
                await CMD_Exit(ecl_offset, name);
            }

            return true;
        }


        static string translation = "A B C D E F G H I J K L M N O    P Q R S T U V W X Y Z 0 1 2 3 4 5 6 7 8 9";

        internal static async Task<bool> CMD_Protection(ushort ecl_offset, string name) // sub_2923F
        {
            Vm.WriteLine("CMD_Protection:");

            gbl.encounter_flags[0] = false;
            gbl.encounter_flags[1] = false;
            gbl.spriteChanged = false;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);

            if (gbl.game.Name == Logging.Game.PoolOfRadiance)
            {
                var sb = new System.Text.StringBuilder();
                ushort addr = gbl.cmd_ops[1].Word;
                ushort character = Vm.GetMemoryValue(addr);

                if (gbl.byte_1AB0C)
                {
                    gbl.game.DrawFrame_Wilderness();
                    gbl.byte_1EE98 = true;
                    gbl.byte_1AB0C = false;
                }

                for (int i = 1; character != 0x00; i++)
                {
                    if (character < 0x40 || character >= 0x40 + translation.Length)
                    {
                        sb.Append(character);
                    }
                    else if (translation[character - 0x40] != ' ')
                    {
                        sb.Append(translation[character - 0x40]);
                    }
                    character = Vm.GetMemoryValue((ushort)(addr + i));
                }
                gbl.textYCol += 1;
                gbl.textXCol += 1;
                seg041.press_any_key(sb.ToString(), false, 15, 16, 16, 5, 2);
            }
            else // if (gbl.game == Game.CurseOfTheAzureBonds)
            {
                if (Cheats.skip_copy_protection == false)
                {
                    ovr004.copy_protection();
                    await ovr025.LoadPic();
                }
            }
            return true;
        }


        internal static Task<bool> CMD_Dump(ushort ecl_offset, string name) // sub_29271
        {
            gbl.ecl_offset++;

            Vm.WriteLine("CMD_Dump: Player: {0}", gbl.SelectedPlayer);

            gbl.SelectedPlayer = ovr018.FreeCurrentPlayer(gbl.SelectedPlayer, true, false);

            gbl.LastSelectedPlayer = gbl.SelectedPlayer;

            ovr025.PartySummary(gbl.SelectedPlayer);

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_FindSpecial(ushort ecl_offset, string name) // sub_292A5
        {
            for (int i = 0; i < 6; i++)
            {
                gbl.compare_flags[i] = false;
            }

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            Classes.Affects affect_type = (Classes.Affects)ovr008.vm_GetCmdValue(1);

            if (gbl.SelectedPlayer.HasAffect(affect_type) == true)
            {
                gbl.compare_flags[0] = true;
            }
            else
            {
                gbl.compare_flags[1] = true;
            }

            return Task.FromResult(true);
        }


        internal static Task<bool> CMD_DestroyItems(ushort ecl_offset, string name) // sub_292F9
        {
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            Item.Type item_type = (Item.Type)ovr008.vm_GetCmdValue(1);

            Vm.WriteLine("CMD_DestroyItems: type: {0}", item_type);

            foreach (Player player in gbl.TeamList)
            {
                player.items.RemoveAll(item => item.type == item_type);

                ovr025.reclac_player_values(player);
            }

            return Task.FromResult(true);
        }



        //internal static Dictionary<int, CmdItem> CommandTable = new Dictionary<int, CmdItem>();

        public static void SetupCommandTable()
        {
            Command.Table.Add(0x00, new CmdItem(0, "EXIT", CMD_Exit));
            Command.Table.Add(0x01, new CmdItem(1, "GOTO", CMD_Goto));
            Command.Table.Add(0x02, new CmdItem(1, "GOSUB", CMD_Gosub));
            Command.Table.Add(0x03, new CmdItem(2, "COMPARE", CMD_Compare));
            Command.Table.Add(0x04, new CmdItem(3, "ADD", CMD_AddSubDivMulti));
            Command.Table.Add(0x05, new CmdItem(3, "SUBTRACT", CMD_AddSubDivMulti));
            Command.Table.Add(0x06, new CmdItem(3, "DIVIDE", CMD_AddSubDivMulti));
            Command.Table.Add(0x07, new CmdItem(3, "MULTIPLY", CMD_AddSubDivMulti));
            Command.Table.Add(0x08, new CmdItem(2, "RANDOM", CMD_Random));
            Command.Table.Add(0x09, new CmdItem(2, "SAVE", CMD_Save));
            Command.Table.Add(0x0A, new CmdItem(1, "LOAD CHARACTER", CMD_LoadCharacter));
            Command.Table.Add(0x0B, new CmdItem(3, "LOAD MONSTER", CMD_LoadMonster));
            Command.Table.Add(0x0C, new CmdItem(3, "SETUP MONSTER", CMD_SetupMonster));
            Command.Table.Add(0x0D, new CmdItem(0, "APPROACH", CMD_Approach));
            Command.Table.Add(0x0E, new CmdItem(1, "PICTURE", CMD_Picture));
            Command.Table.Add(0x0F, new CmdItem(2, "INPUT NUMBER", CMD_InputNumber));
            Command.Table.Add(0x10, new CmdItem(2, "INPUT STRING", CMD_InputString));
            Command.Table.Add(0x11, new CmdItem(1, "PRINT", CMD_Print));
            Command.Table.Add(0x12, new CmdItem(1, "PRINTCLEAR", CMD_Print));
            Command.Table.Add(0x13, new CmdItem(0, "RETURN", CMD_Return));
            Command.Table.Add(0x14, new CmdItem(4, "COMPARE AND", CMD_CompareAnd));
            Command.Table.Add(0x15, new CmdItem(0, "VERTICAL MENU", CMD_VertMenu));
            Command.Table.Add(0x16, new CmdItem(0, "IF =", CMD_If));
            Command.Table.Add(0x17, new CmdItem(0, "IF <>", CMD_If));
            Command.Table.Add(0x18, new CmdItem(0, "IF <", CMD_If));
            Command.Table.Add(0x19, new CmdItem(0, "IF >", CMD_If));
            Command.Table.Add(0x1A, new CmdItem(0, "IF <=", CMD_If));
            Command.Table.Add(0x1B, new CmdItem(0, "IF >=", CMD_If));
            Command.Table.Add(0x1C, new CmdItem(0, "CLEARMONSTERS", CMD_ClearMonsters));
            Command.Table.Add(0x1D, new CmdItem(1, "PARTYSTRENGTH", CMD_PartyStrength));
            Command.Table.Add(0x1E, new CmdItem(6, "CHECKPARTY", CMD_CheckParty));
            Command.Table.Add(0x1F, new CmdItem(2, "notsure 0x1f", null));
            Command.Table.Add(0x20, new CmdItem(1, "NEWECL", CMD_NewECL));
            Command.Table.Add(0x21, new CmdItem(3, "LOAD FILES", CMD_LoadFiles));
            Command.Table.Add(0x22, new CmdItem(2, "PARTY SURPRISE", CMD_PartySurprise));
            Command.Table.Add(0x23, new CmdItem(4, "SURPRISE", CMD_Surprise));
            Command.Table.Add(0x24, new CmdItem(0, "COMBAT", CMD_Combat));
            Command.Table.Add(0x25, new CmdItem(0, "ON GOTO", CMD_OnGotoGoSub));
            Command.Table.Add(0x26, new CmdItem(0, "ON GOSUB", CMD_OnGotoGoSub));
            Command.Table.Add(0x27, new CmdItem(8, "TREASURE", CMD_Treasure));
            Command.Table.Add(0x28, new CmdItem(3, "ROB", CMD_Rob));
            Command.Table.Add(0x29, new CmdItem(14, "ENCOUNTER MENU", CMD_EncounterMenu));
            Command.Table.Add(0x2A, new CmdItem(3, "GETTABLE", CMD_GetTable));
            Command.Table.Add(0x2B, new CmdItem(0, "HORIZONTAL MENU", CMD_HorizontalMenu));
            Command.Table.Add(0x2C, new CmdItem(6, "PARLAY", CMD_Parlay));
            Command.Table.Add(0x2D, new CmdItem(1, "CALL", CMD_Call));
            Command.Table.Add(0x2E, new CmdItem(5, "DAMAGE", CMD_Damage));
            Command.Table.Add(0x2F, new CmdItem(3, "AND", CMD_AndOr));
            Command.Table.Add(0x30, new CmdItem(3, "OR", CMD_AndOr));
            Command.Table.Add(0x31, new CmdItem(0, "SPRITE OFF", CMD_SpriteOff));
            Command.Table.Add(0x32, new CmdItem(1, "FIND ITEM", CMD_FindItem));
            Command.Table.Add(0x33, new CmdItem(0, "PRINT RETURN", CMD_PrintReturn));
            Command.Table.Add(0x34, new CmdItem(1, "ECL CLOCK", CMD_EclClock));
            Command.Table.Add(0x35, new CmdItem(3, "SAVE TABLE", CMD_SaveTable));
            Command.Table.Add(0x36, new CmdItem(2, "ADD NPC", CMD_AddNPC));
            Command.Table.Add(0x37, new CmdItem(3, "LOAD PIECES", CMD_LoadFiles));
            Command.Table.Add(0x38, new CmdItem(1, "PROGRAM", CMD_Program));
            Command.Table.Add(0x39, new CmdItem(1, "WHO", CMD_Who));
            Command.Table.Add(0x3A, new CmdItem(0, "DELAY", CMD_Delay));
            Command.Table.Add(0x3B, new CmdItem(3, "SPELL", CMD_Spell));
            Command.Table.Add(0x3C, new CmdItem(1, "PROTECTION", CMD_Protection));
            Command.Table.Add(0x3D, new CmdItem(0, "CLEAR BOX", CMD_ClearBox));
            Command.Table.Add(0x3E, new CmdItem(0, "DUMP", CMD_Dump));
            Command.Table.Add(0x3F, new CmdItem(1, "FIND SPECIAL", CMD_FindSpecial));
            Command.Table.Add(0x40, new CmdItem(1, "DESTROY ITEMS", CMD_DestroyItems));
        }

        static void SkipNextCommand()
        {
            gbl.command = gbl.ecl_ptr[gbl.ecl_offset - gbl.initial_ecl_offset];

            CmdItem cmd;
            if (Command.Table.TryGetValue(gbl.command, out cmd))
            {
                cmd.Skip();
            }
            else
            {
                Logger.Log("Skipping Unknown command id {0}", gbl.command);
                gbl.ecl_offset += 1;
            }
        }


        internal static async Task<bool> RunEclVm(ushort offset) // sub_29607
        {
            gbl.ecl_offset = offset;
            gbl.stopVM = false;

            //System.Console.Out.WriteLine("RunEclVm {0,4:X} start", offset);

            while (gbl.Token.IsCancellationRequested == false &&
                   gbl.stopVM == false &&
                   gbl.party_killed == false)
            {
                Classes.Debug.OnStep(gbl.ecl_offset);

                gbl.command = gbl.ecl_ptr[gbl.ecl_offset - gbl.initial_ecl_offset];

                // VmLog.Write("0x{0:X} ", gbl.ecl_offset);

                CmdItem cmd;
                if (Command.Table.TryGetValue(gbl.command, out cmd))
                {
                    if (gbl.printCommands)
                    {
                        //Logger.Debug("${0:X}   {1:X}   {2}", gbl.ecl_offset, gbl.command, cmd.Name());
                    }
                    await cmd.Run();
                }
                else
                {
                    Logger.Log("Unknown command id {0}", gbl.command);
                }
            }

            gbl.stopVM = false;

            return true;
        }


        internal static async Task<bool> sub_29677() // POR: sub_1CD04
        {
            do
            {
                ovr030.DaxArrayFreeDaxBlocks(gbl.byte_1D556);
                gbl.byte_1D5AB = string.Empty;
                gbl.byte_1D5B5 = 0x0FF;
                gbl.vmFlag01 = false;
                gbl.mapWallRoof = ovr031.get_wall_x2(gbl.mapPosY, gbl.mapPosX);

                gbl.area2_ptr.tried_to_exit_map = false;

                gbl.LastSelectedPlayer = gbl.SelectedPlayer;

                gbl.last_wilderness_area = gbl.wilderness_area;
                gbl.wilderness_area = 1;

                await RunEclVm(gbl.ecl_initial_entryPoint);

                gbl.game_state = GameState.DungeonMap;

                if (gbl.area_ptr.inDungeon == 0)
                {
                    if (gbl.EclBlockId == 0x19)
                    {
                        gbl.wilderness_area = 2;
                        gbl.game_state = GameState.WildernessMap;
                    }
                    else if (gbl.EclBlockId == 0x1A)
                    {
                        gbl.wilderness_area = 3;
                        gbl.game_state = GameState.WildernessMap;

                        if (gbl.area_ptr.field_366 == 0x00FE && gbl.area_ptr.field_344 == 0)
                        {
                            //sub_84A(1); TODO fixme
                        }
                    }
                    else if (gbl.EclBlockId == 0x1B)
                    {
                        gbl.wilderness_area = 4;
                        gbl.game_state = GameState.WildernessMap;
                    }
                }

                if (gbl.last_wilderness_area > 1 && gbl.wilderness_area == 1)
                {
                    // Map 3, 3, 0x0D, 0x0D, 0, 0x0A, ???);
                }

                if (gbl.vmFlag01 == false)
                {
                    gbl.area_ptr.LastEclBlockId = gbl.EclBlockId;
                }

                if (gbl.vmFlag01 == false)
                {
                    if (((gbl.last_game_state != GameState.DungeonMap || gbl.game_state == GameState.DungeonMap) && gbl.byte_1AB0B == true) ||
                        (gbl.last_game_state == GameState.DungeonMap && gbl.game_state == GameState.DungeonMap))
                    {
                        await ovr029.RedrawView();
                    }
                    gbl.vmFlag01 = false;

                    await RunEclVm(gbl.vm_run_addr_1);

                    if (gbl.vmFlag01 == false)
                    {
                        await RunEclVm(gbl.SearchLocationAddr);

                        if (gbl.vmFlag01 == false)
                        {
                            gbl.SelectedPlayer = gbl.LastSelectedPlayer;
                            ovr025.PartySummary(gbl.SelectedPlayer);
                        }
                    }

                }
            } while (gbl.vmFlag01 == true);

            gbl.last_game_state = gbl.game_state;

            return true;
        }


        internal static async Task<bool> sub_29758()
        {
            gbl.LastSelectedPlayer = gbl.SelectedPlayer;

            gbl.can_draw_bigpic = true;
            gbl.byte_1AB0C = false;
            gbl.filesLoaded = false;
            gbl.restore_player_ptr = false;
            gbl.byte_1EE98 = true;
            gbl.vmFlag01 = false;
            if (gbl.game_state == GameState.Camping)
            {
                gbl.game_state = GameState.DungeonMap;
            }

            if (gbl.area_ptr.LastEclBlockId == gbl.game.InvalidEclBlockId)
            {
                gbl.byte_1EE98 = false;

                if (gbl.inDemo == true)
                {
                    gbl.EclBlockId = gbl.game.DemoEclBlockId;
                }
                else
                {
                    gbl.EclBlockId = gbl.game.InitialEclBlockId;

                    ovr025.PartySummary(gbl.SelectedPlayer);
                }
            }
            else
            {
                gbl.EclBlockId = (byte)(gbl.area_ptr.LastEclBlockId);
            }

            if (gbl.reload_ecl_and_pictures == true ||
                gbl.area_ptr.LastEclBlockId == gbl.game.InvalidEclBlockId)
            {
                await ovr008.load_ecl_dax(gbl.EclBlockId);
            }
            else
            {
                gbl.byte_1AB0B = true;
            }

            ovr008.vm_init_ecl();

            await RunEclVm(gbl.ecl_initial_entryPoint);

            if (gbl.inDemo == true)
            {
                while (gbl.TeamList.Count > 0)
                {
                    ovr018.FreeCurrentPlayer(gbl.TeamList[0], true, true);
                }
                gbl.SelectedPlayer = null;
            }
            else
            {
                if (gbl.vmFlag01 == false)
                {
                    gbl.area_ptr.LastEclBlockId = gbl.EclBlockId;
                }
                else
                {
                    await sub_29677();
                }

                if (gbl.reload_ecl_and_pictures == true)
                {
                    if (gbl.byte_1EE98 == true)
                    {
                        await ovr025.LoadPic();
                    }

                    gbl.can_draw_bigpic = true;
                    await ovr029.RedrawView();
                }

                gbl.reload_ecl_and_pictures = false;

                do
                {
                    char var_1 = await ovr015.main_3d_world_menu();

                    gbl.LastSelectedPlayer = gbl.SelectedPlayer;

                    if (gbl.vmFlag01 == false)
                    {
                        gbl.area_ptr.LastEclBlockId = gbl.EclBlockId;
                    }

                    while ((gbl.area2_ptr.search_flags > 1 || char.ToUpper(var_1) == 'E') &&
                        gbl.party_killed == false)
                    {
                        if (char.ToUpper(var_1) == 'E')
                        {
                            await TryEncamp();
                        }
                        else
                        {
                            gbl.search_flag_bkup = gbl.area2_ptr.search_flags & 1;
                            if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds || gbl.game_state != GameState.WildernessMap)
                            {
                                gbl.area2_ptr.search_flags = 1;
                            }
                            gbl.can_draw_bigpic = true;
                            await ovr029.RedrawView();

                            await RunEclVm(gbl.SearchLocationAddr);

                            if (gbl.vmFlag01 == true)
                            {
                                await sub_29677();
                            }

                            gbl.area2_ptr.search_flags = (ushort)gbl.search_flag_bkup;
                        }

                        if (gbl.party_killed == false)
                        {
                            var_1 = await ovr015.main_3d_world_menu();
                            gbl.LastSelectedPlayer = gbl.SelectedPlayer;
                        }
                    }


                    if (gbl.party_killed == false)
                    {
                        await RunEclVm(gbl.vm_run_addr_1);
                    }

                    if (gbl.vmFlag01 == true)
                    {
                        await sub_29677();
                    }
                    else
                    {
                        if (gbl.party_killed == false)
                        {
                            if (gbl.game_state == GameState.WildernessMap)
                            {
                                if (gbl.area2_ptr.field_592 < 0xff)
                                {
                                    if (!ovr031.TerrainImpassable())
                                    {
                                        gbl.area_ptr.field_186 = (byte)gbl.word_1D914;
                                        gbl.area_ptr.field_188 = (byte)gbl.word_1D916;

                                        await ovr021.step_game_time(3, 12);
                                        //ovr025.display_map_position_time();
                                    }
                                }
                                else
                                {
                                    gbl.area2_ptr.field_592 = 0;
                                }
                            }
                            else
                            {
                                gbl.area_ptr.lastXPos = (short)gbl.mapPosX;
                                gbl.area_ptr.lastYPos = (short)gbl.mapPosY;

                                await ovr015.locked_door();

                                if (gbl.area_ptr.lastXPos != gbl.mapPosX ||
                                    gbl.area_ptr.lastYPos != gbl.mapPosY)
                                {
                                    seg044.PlaySound(Sound.sound_a);
                                }
                            }
                            await ovr029.RedrawView();

                            gbl.spriteChanged = false;
                            gbl.byte_1EE8D = true;
                            await RunEclVm(gbl.SearchLocationAddr);
                            if (gbl.vmFlag01 == true)
                            {
                                await sub_29677();

                            }
                        }
                    }
                } while (gbl.Token.IsCancellationRequested == false && gbl.party_killed == false);

                gbl.party_killed = false;
            }

            return true;
        }
    }
}
