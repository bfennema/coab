using Classes;
using Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace engine
{
    public class ovr008
    {
        internal static ushort vm_GetCmdValue(int arg_0) // sub_30168
        {
            return gbl.cmd_ops[arg_0].GetCmdValue();
            //TODO replace calls to vm_GetCmdValue function with gbl.cmd_opps[arg_0].GetCmdValue();
        }
        internal static string vm_PrintCmd(int arg_0)
        {
            return gbl.cmd_ops[arg_0].PrintCmd();
        }


        internal static void vm_init_ecl() // sub_301E8
        {
            gbl.spriteChanged = false;
            gbl.byte_1EE8E = false;
            gbl.redrawPartySummary1 = false;
            gbl.redrawPartySummary2 = false;
            gbl.byte_1D912 = 0x41; // POR: byte_13728
            gbl.byte_1D913 = 9; // POR: byte_13729
            gbl.paletteChanged = true; // POR: byte_14CA5

            gbl.encounter_flags[0] = false;
            gbl.encounter_flags[1] = false;
            gbl.monster_icon_id = 8; // POR: byte_13743
            gbl.ecl_offset = gbl.initial_ecl_offset; // POR: byte_111AD
            gbl.byte_1DA70 = false;

            gbl.vmCallStack.Clear();

            for (int i = 0; i < 6; i++)
            {
                gbl.compare_flags[i] = false;
            }

            gbl.area2_ptr.HeadBlockId = 0xFF;

            gbl.area2_ptr.rest_incounter_period = 0;
            gbl.area2_ptr.rest_incounter_percentage = 0;
            gbl.area_ptr.can_cast_spells = false;

            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            gbl.vm_run_addr_1 = gbl.cmd_ops[1].Word;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            gbl.SearchLocationAddr = gbl.cmd_ops[1].Word;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            gbl.PreCampCheckAddr = gbl.cmd_ops[1].Word;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            gbl.CampInterruptedAddr = gbl.cmd_ops[1].Word;
            gbl.ecl_offset = Vm.LoadCmdSets(ref gbl.cmd_ops, gbl.ecl_offset, 1);
            gbl.ecl_initial_entryPoint = gbl.cmd_ops[1].Word;

            Debug.NotifyEclChanged();

            gbl.area_ptr.inDungeon = 1;

            if (gbl.reload_ecl_and_pictures == false)
            {
                gbl.area_ptr.RestField200Values();
                gbl.area2_ptr.RestField6F2Values();
            }
            else
            {
                gbl.reload_ecl_and_pictures = false;
            }
        }


        internal static async Task<bool> load_ecl_dax(byte block_id)
        {
            byte[] block_mem;
            ushort block_size = 0;

            gbl.ecl_ptr.Clear();

            do
            {
                ovr027.ClearPromptArea();
                seg041.displayString("Loading...Please Wait", 0, 10, 0x18, 0);

                (block_mem, block_size) = await seg042.load_decode_dax(block_id, "ECL", gbl.game_area);
            } while (block_size < 2);

            gbl.ecl_ptr.SetData(block_mem, 2, block_size - 2);

            ovr027.ClearPromptArea();

            return true;
        }


        internal static byte sub_304B4(int map_dir, int map_y, int map_x)
        {
            byte var_1 = 0;

            if (gbl.area_ptr.inDungeon == 0)
            {
                var_1 = 2;
                gbl.area2_ptr.encounter_distance = 2;
            }
            else
            {
                bool var_2 = false;
                byte var_3 = 0;

                while (var_3 < 2 && var_2 == false)
                {
                    if (ovr031.getMap_wall_type(map_dir, map_y, map_x) == 0)
                    {
                        var_3++;
                        var_1 = var_3;

                        switch (map_dir)
                        {
                            case 0:
                                map_y--;
                                break;

                            case 2:
                                map_x++;
                                break;

                            case 4:
                                map_y++;
                                break;

                            case 6:
                                map_x--;
                                break;
                        }
                    }
                    else
                    {
                        var_2 = true;
                    }
                }
            }

            return var_1;
        }


        internal static async Task<bool> set_and_draw_head_body(byte area, byte body_id, byte head_id, byte rowY, byte colX) /* sub_30543 */
            {
                gbl.byte_1EE8D = false;

            gbl.head_block_id = head_id;
            gbl.body_block_id = body_id;

            await ovr030.head_body(area, body_id, head_id);
            ovr030.draw_head_and_body(true, rowY, colX);

            return true;
        }


        internal static async Task<bool> sub_30580(bool[] flags, int encounter_distance, byte pic_block_id, byte sprite_block_id)
        {
            if (flags[1] == false)
            {
                if (flags[0] == false)
                {
                    if (gbl.mapAreaDisplay == true)
                    {
                        gbl.mapAreaDisplay = false;
                        gbl.can_draw_bigpic = true;
                        await ovr029.RedrawView();
                    }

                    if (gbl.area_ptr.inDungeon != 0)
                    {
                        await ovr030.load_pic_final(gbl.byte_1D556, 1, sprite_block_id, "SPRIT", gbl.game_area);
                        flags[0] = true;
                        gbl.displayPlayerSprite = true;
                    }
                }
                else
                {
                    gbl.can_draw_bigpic = true;
                    await ovr029.RedrawView();
                }

                if (gbl.game_state == GameState.DungeonMap)
                {
                    ovr030.Show3DSprite(gbl.byte_1D556, encounter_distance + 1);
                }
            }

            if (flags[1] == false ||
                gbl.byte_1EE96 != gbl.area2_ptr.HeadBlockId)
            {
                if (encounter_distance == 0 &&
                    gbl.game_state == GameState.DungeonMap &&
                    gbl.byte_1EE95 == false)
                {
                    gbl.byte_1EE96 = (byte)gbl.area2_ptr.HeadBlockId;
                    gbl.spriteChanged = true;
                    if (gbl.area2_ptr.HeadBlockId == 0xff)
                    {
                        await ovr030.load_pic_final(gbl.byte_1D556, 0, pic_block_id, "PIC", gbl.game_area);
                        flags[1] = true;

                        ovr030.DrawMaybeOverlayed(gbl.byte_1D556.frames[0].picture, true, 3, 3);
                    }
                    else
                    {
                        await set_and_draw_head_body(gbl.game_area, pic_block_id, (byte)gbl.area2_ptr.HeadBlockId, 3, 3);
                        flags[1] = true;
                        gbl.byte_1EE8D = false;
                    }
                }
            }

            return true;
        }

        internal static uint deflateChar(char ch)
        {
            uint output = (uint)ch;

            if (output >= 0x40)
            {
                output -= 0x40;
            }
            return output;
        }

        internal static void vm_WriteStringToMemory(string text, ushort loc) // sub_3105D, POR: sub_20D13
        {
            byte var_104;

            int mem_type = Vm.GetMemoryValueType(loc);

            int text_len = text.Length;

            //System.Console.WriteLine("  vm_WriteStringToMemory: str: '{0}' loc: {1:X} type: {2:X}",
            //    arg_0, arg_4, var_101);


            if (mem_type == 0)
            {
                if (text_len > 0)
                {
                    var_104 = (byte)(text_len - 1);

                    for (int i = 0; i <= var_104; i++)
                    {
                        gbl.area_ptr.field_6A00_Set((loc + i - gbl.vm_mem0_offset) * 2, text[i]);
                    }
                }

                gbl.area_ptr.field_6A00_Set((text_len + loc - gbl.vm_mem0_offset) * 2, 0);
            }
            else if (mem_type == 1)
            {
                if (loc == gbl.vm_mem1_offset)
                {
                    gbl.SelectedPlayer.name = text;
                }
                else
                {
                    if (text_len > 0)
                    {
                        for (int i = 0; i <= text_len - 1; i++)
                        {
                            gbl.area2_ptr.field_800_Set((loc + i - gbl.vm_mem1_offset) * 2, text[i]);
                        }
                    }

                    gbl.area2_ptr.field_800_Set((text_len + loc - gbl.vm_mem1_offset) * 2, 0);
                }
            }
            else if (mem_type == 2)
            {
                if (text_len > 0)
                {
                    var_104 = (byte)(text_len - 1);
                    for (int i = 0; i <= var_104; i++)
                    {
                        gbl.stru_1B2CA[(i + loc - gbl.vm_mem2_offset) * 2] = text[i];
                    }
                }

                gbl.stru_1B2CA[(text_len + loc - gbl.vm_mem2_offset) * 2] = 0;
            }
            else if (mem_type == 3)
            {
                if (text_len > 0)
                {
                    for (int i = 0; i <= text_len - 1; i++)
                    {
                        gbl.ecl_ptr[i + loc - gbl.initial_ecl_offset] = (byte)text[i];
                    }
                }

                gbl.ecl_ptr[text_len + loc - gbl.initial_ecl_offset] = 0;
            }
        }

        internal static byte[] compressString(string input)
        {
            byte[] data = new byte[((input.Length * 3) / 4) + 1];
            int state = 1;
            int last = 0;
            int curr = 0;

            foreach (char ch in input)
            {
                uint bits = deflateChar(ch) & 0x3F;
                if (state == 1)
                {
                    data[curr] = (byte)(bits << 2);
                    last = curr++;
                    state = 2;
                }
                else if (state == 2)
                {
                    data[last] |= (byte)(bits >> 4);
                    data[curr] = (byte)(bits << 4);
                    last = curr++;
                    state = 3;
                }
                else if (state == 3)
                {
                    data[last] |= (byte)(bits >> 2);
                    data[curr] = (byte)(bits << 6);
                    last = curr++;
                    state = 4;
                }
                else //if (state == 4)
                {
                    data[last] |= (byte)(bits);
                    state = 1;
                }

            }

            return data;
        }

        static Set unk_31673 = new Set(48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90); 

        internal static string buildMenuStrings(ref string MenuString)
        {
            System.Text.StringBuilder sbA = new System.Text.StringBuilder();
            System.Text.StringBuilder sbB = new System.Text.StringBuilder();

            bool mFlag = false;

            for (int i = 0; i < MenuString.Length; i++)
            {
                char ch = MenuString[i];

                if (unk_31673.MemberOf(ch) == true)
                {
                    ch += ' ';
                }

                if (ch == '~')
                {
                    mFlag = true;
                    continue;
                }

                if (mFlag == true)
                {
                    mFlag = false;
                    ch = char.ToUpper(ch);
                    sbA.Append(ch);
                }

                sbB.Append(ch);
            }

            MenuString = sbB.ToString();
            return sbA.ToString();
        }

        static Set validkeys = new Set(48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90); // unk_3178A


        internal static int sub_317AA(bool useOverlay, bool acceptReturn, MenuColorSet colors, string displayString, string extraString)
        {
            char key_pressed;
            int ret_val;

            string menu_keys = buildMenuStrings(ref displayString);

            do
            {
                bool special_key_pressed;
                key_pressed = ovr027.displayInput(out special_key_pressed, useOverlay, 1, colors, displayString, extraString);

                if (special_key_pressed == true)
                {
                    ovr020.scroll_team_list(key_pressed);
                    ovr025.PartySummary(gbl.SelectedPlayer);
                    key_pressed = '\0';
                }
            } while (gbl.Token.IsCancellationRequested == false && validkeys.MemberOf(key_pressed) == false && (key_pressed != '\r' || acceptReturn == false));

            if (key_pressed == '\r')
            {
                ret_val = 0;
            }
            else
            {
                int var_154 = 0;

                while (var_154 < menu_keys.Length && menu_keys[var_154] != key_pressed)
                {
                    var_154++;
                }

                if (var_154 < menu_keys.Length)
                {
                    ret_val = var_154;
                }
                else
                {
                    ret_val = -1;
                }
            }

            return ret_val;
        }


        internal static int VertMenuSelect(int index, bool menuRedraw, bool showExit,
            List<MenuItem> list, sbyte endY, sbyte endX, int startY, sbyte startX)
        {
            MenuItem dummyMenuItem;

            ovr027.sl_select_item(out dummyMenuItem, ref index, ref menuRedraw, showExit, list, endY, endX,
                startY, startX, gbl.defaultMenuColors, string.Empty, string.Empty);

            return index;
        }


        internal static void compare_strings(string string_a, string string_b) /* sub_3193B */
        {
            gbl.compare_flags[0] = (string_b.CompareTo(string_a) == 0);
            gbl.compare_flags[1] = (string_b.CompareTo(string_a) != 0);
            gbl.compare_flags[2] = (string_b.CompareTo(string_a) < 0);
            gbl.compare_flags[3] = (string_b.CompareTo(string_a) > 0);
            gbl.compare_flags[4] = (string_b.CompareTo(string_a) <= 0);
            gbl.compare_flags[5] = (string_b.CompareTo(string_a) >= 0);
        }

        /// <summary>
        /// sets global based of arg_0 and arg_2 relation.
        /// sub_31A11
        /// </summary>
        internal static void compare_variables(ushort arg_0, ushort arg_2) /* sub_31A11 */
        {
            //System.Console.WriteLine("  Compare_variables: {0} {1}", arg_2, arg_0);

            gbl.compare_flags[0] = arg_2 == arg_0;
            gbl.compare_flags[1] = arg_2 != arg_0;
            gbl.compare_flags[2] = arg_2 < arg_0;
            gbl.compare_flags[3] = arg_2 > arg_0;
            gbl.compare_flags[4] = arg_2 <= arg_0;
            gbl.compare_flags[5] = arg_2 >= arg_0;
        }


        internal static void MovePositionForward() // sub_31B01
        {
            if (gbl.mapDirection == 0)
            {
                gbl.mapPosY = DecrimentWrap(gbl.mapPosY, 15);
            }
            else if (gbl.mapDirection == 2)
            {
                gbl.mapPosX = IncrementWrap(gbl.mapPosX, 15);
            }
            else if (gbl.mapDirection == 4)
            {
                gbl.mapPosY = IncrementWrap(gbl.mapPosY, 15);
            }
            else if (gbl.mapDirection == 6)
            {
                gbl.mapPosX = DecrimentWrap(gbl.mapPosX, 15);
            }

            gbl.mapWallRoof = ovr031.get_wall_x2(gbl.mapPosY, gbl.mapPosX);
            gbl.mapWallType = ovr031.getMap_wall_type(gbl.mapDirection, gbl.mapPosY, gbl.mapPosX);

            gbl.positionChanged = true;
        }

        static int DecrimentWrap(int value, int max)
        {
            if (value > 0)
            {
                return value - 1;
            }
            else
            {
                return max;
            }
        }

        static int IncrementWrap(int value, int max)
        {
            if (value < max)
            {
                return value + 1;
            }
            else
            {
                return 0;
            }
        }

        internal static async Task<bool> SetupDuel(bool isDuel)
        {
            gbl.combat_type = CombatType.duel;

            gbl.area2_ptr.isDuel = isDuel;
            Player dueler = gbl.SelectedPlayer;

            foreach (Player player in gbl.TeamList)
            {
                if (player.name != dueler.name)
                {
                    player.in_combat = false;
                }
            }

            if (isDuel)
            {
                await ovr034.chead_cbody_comspr_icon(gbl.monster_icon_id, 11, "CPIC");

                Player DuelMaster = dueler.ShallowClone();
                DuelMaster.in_combat = true;
                DuelMaster.name = "ROLF";
                DuelMaster.quick_fight = QuickFight.True;
                DuelMaster.combat_team = CombatTeam.Enemy;

                DuelMaster.control_morale = Control.NPC_Berserk;
                DuelMaster.icon_id = gbl.monster_icon_id;

                DuelMaster.affects = new List<Affect>();
                DuelMaster.items = new List<Item>();

                gbl.TeamList.Add(DuelMaster);

                foreach (Item item in dueler.items)
                {
                    DuelMaster.items.Add(item.ShallowClone());
                }
            }
            return true;
        }


        internal static void RobMoney(Player player, double scale) /* sub_31DEF */
        {
            player.Money.ScaleAll(scale);
        }


        internal static void RobItems(Player player, int robChance) /* sub_31F1C */
        {
            player.items.RemoveAll(item =>
            {
                if (item.weight > 255)
                {
                    robChance = (robChance > 90) ? robChance - 90 : 0;
                }
                else if (item.weight > 24)
                {
                    robChance = (robChance > 50) ? robChance - 50 : 0;
                }

                return (ovr024.roll_dice(100, 1) <= robChance);
            });
        }


        internal static void calc_group_movement(out byte mov_min, out byte mov_max) /* calc_group_inituative */
        {
            mov_max = byte.MinValue;
            mov_min = byte.MaxValue;

            foreach (Player player in gbl.TeamList)
            {
                byte movement = player.movement;

                if (player.HasAffect(Classes.Affects.haste) == true)
                {
                    movement *= 2;
                }
                else if (player.HasAffect(Classes.Affects.slow) == true)
                {
                    movement /= 2;
                }

                if (movement > mov_max)
                {
                    mov_max = movement;
                }

                if (movement < mov_min)
                {
                    mov_min = movement;
                }
            }
        }


        internal static void sub_32200(Player player, int damage) /* sub_32200 */
        {
            if (player.health_status != Status.dead)
            {
                string text;
                bool clear_text_area = false;

                if ((player.hit_point_current + 10) < damage)
                {
                    text = string.Format("  {0} dies. ", player.name);
                }
                else
                {
                    text = string.Format("  {0} is hit FOR {1} points of Damage.", player.name, damage);
                }

                if (gbl.textYCol > 0x16)
                {
                    gbl.textYCol = 0x11;
                    clear_text_area = true;
                    seg041.DisplayAndPause("press <enter>/<return> to continue", 15);
                }
                else
                {
                    clear_text_area = false;
                }

                gbl.textXCol = 0x26;

                seg041.press_any_key(text, clear_text_area, 15, 0x16, 0x26, 17, 1);

                ovr025.damage_player(damage, player);
                seg037.draw8x8_clear_area(0x0f, 0x26, 1, 0x11);

                ovr025.PartySummary(player);
            }
        }
    }
}
