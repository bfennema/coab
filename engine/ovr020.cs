using Classes;

namespace engine
{
    internal enum SpellLoc
    {
        memory = 0,
        grimoire = 1,
        scroll = 2,
        scrolls = 3,
        choose = 4,
        memorize = 5,
        scribe = 6
    }

    class ovr020
    {
        internal static string[] sexString = { "Male", "Female" };
        internal static string[] raceString = { "Monster", "Dwarf", "Elf", "Gnome", 
                                         "Half-Elf", "Halfling", "Half-Orc", "Human" };

        internal static string[] alignmentString = { "Lawful Good", "Lawful Neutral", "Lawful Evil",
                                              "Neutral Good", "True Neutral", "Neutral Evil",
                                              "Chaotic Good", "Chaotic Neutral", "Chaotic Evil" };

        internal static string[] classString = { "Cleric", "Druid", "Fighter", "Paladin", "Ranger",
                                          "Magic-User", "Thief", "Monk", "Cleric/Fighter", 
                                          "Cleric/Fighter/Magic-User", "Cleric/Ranger",
                                          "Cleric/Magic-User","Cleric/Thief", "Fighter/Magic-User", 
                                          "Fighter/Thief", "Fighter/Magic-User/Thief",
                                          "Magic-User/Thief" };

        static string[] statShortString = { "STR ", "INT ", "WIS ", "DEX ", "CON ", "CHA " };

        internal static string[] statusString = { "Okay", "Animated", "tempgone", "Running",
                                         "Unconscious", "Dying", "Dead", "Stoned",
                                         "Gone" };

        static string[] moneyString = { "Copper", "Silver", "Electrum", "Gold", "Platinum",
                                        "Gems", "Jewelry" };


        internal static void playerDisplayFull()
        {
            string var_307;
            string var_106;
            byte var_6;
            byte var_5;
            int xCol;
            byte var_2;

            gbl.player_ptr02 = gbl.player_ptr;

            seg037.draw8x8_01();

            ovr025.displayPlayerName(false, 1, 1, gbl.player_ptr02);

            if (gbl.player_ptr02.field_F7 > 0x7F)
            {
                seg041.displayString("(NPC)", 0, 10, 1, gbl.player_ptr02.name.Length + 3);
            }

            xCol = 1;

            var_106 = sexString[gbl.player_ptr02.sex];

            seg041.displayString(sexString[gbl.player_ptr02.sex], 0, 15, 3, xCol);

            xCol += (byte)(var_106.Length + 1);
            var_106 = raceString[(int)gbl.player_ptr02.race];
            seg041.displayString(var_106, 0, 15, 3, xCol);

            xCol += (byte)(var_106.Length + 1);
            var_307 = "Age " + gbl.player_ptr02.age.ToString();

            seg041.displayString(var_307, 0, 15, 3, xCol);

            var_106 = alignmentString[gbl.player_ptr02.alignment];
            seg041.displayString(var_106, 0, 15, 4, 1);

            var_106 = classString[(int)gbl.player_ptr02._class];
            seg041.displayString(var_106, 0, 15, 5, 1);

            for (var_5 = 0; var_5 < 6; var_5++)
            {
                var_106 = statShortString[var_5];
                seg041.displayString(var_106, 0, 10, var_5 + 7, 1);
                display_stat(false, var_5);
            }

            displayMoney();
            seg041.displayString("Level", 0, 15, 15, 1);

            bool displaySlash = false;
            var_106 = string.Empty;

            for (var_6 = 0; var_6 <= 7; var_6++)
            {
                byte tmp = gbl.player_ptr02.Skill_B_lvl[var_6];

                if (gbl.player_ptr02.Skill_A_lvl[var_6] > 0 ||
                    (tmp < ovr026.hasAnySkills(gbl.player_ptr02) && tmp > 0))
                {
                    if (displaySlash )
                    {
                        var_106 += "/";
                    }

                    var_106 += (gbl.player_ptr02.Skill_A_lvl[var_6] + gbl.player_ptr02.Skill_B_lvl[var_6]).ToString();

                    displaySlash = true;
                }
            }

            seg041.displayString(var_106, 0, 15, 15, 7);

            var_307 = "Exp " + gbl.player_ptr02.exp.ToString();
            seg041.displayString(var_307, 0, 15, 15, 17);

            ovr020.display_player_stats01();
            var_2 = 0x14;

            if (gbl.player_ptr02.field_151 != null)
            {
                seg041.displayString("Weapon", 0, 15, var_2, 1);
                ovr025.ItemDisplayNameBuild(true, false, var_2, 8, gbl.player_ptr02.field_151, gbl.player_ptr02);
            }

            var_2++;
            if (gbl.player_ptr02.field_159 != null)
            {
                seg041.displayString("Armor", 0, 15, var_2, 2);
                ovr025.ItemDisplayNameBuild(true, false, var_2, 8, gbl.player_ptr02.field_159, gbl.player_ptr02);
            }

            var_2++;

            seg041.displayString("Status", 0, 15, var_2, 1);
            seg041.displayString(statusString[(int)gbl.player_ptr02.health_status], 0, 10, var_2, 8);
        }

        internal static void displayMoney()
        {
            seg037.draw8x8_clear_area(0x0e, 0x1a, 7, 0x0c);

            int yCol = 7;

            for (int coinType = 6; coinType >= 0; coinType--)
            {
                if (gbl.player_ptr.Money[coinType] > 0)
                {
                    string mString = moneyString[coinType];

                    seg041.displayString(mString, 0, 10, yCol, 20 - mString.Length);
                    seg041.displayString(gbl.player_ptr.Money[coinType].ToString(), 0, 10, yCol, 21);

                    yCol++;
                }
            }
        }


        internal static void display_player_stats01()
        {
            Affect var_34;
            string var_30;
            byte var_7;
            int xCol;
            int yCol;
            Player player;

            player = gbl.player_ptr;

            ovr025.sub_66C20(player);
            yCol = 0x11;

            seg041.displayString("AC    ", 0, 15, yCol, 1);
            ovr025.display_AC(yCol, 4, player);

            seg041.displayString("HP    ", 0, 15, yCol + 1, 1);
            ovr025.display_hp(false, yCol + 1, 4, player);

            xCol = 8;

            seg041.displayString("THAC0   ", 0, 15, yCol, xCol + 1);
            seg041.displayString((0x3c - player.hitBonus).ToString(), 0, 10, yCol, xCol + 7);


            var_30 = player.field_19E.ToString() + "d" + player.field_1A0.ToString();
            if (player.damageBonus > 0)
            {
                var_30 += "+" + player.damageBonus.ToString();
            }
            if (player.damageBonus < 0)
            {
                var_30 += player.damageBonus.ToString();
            }

            seg041.displayString("Damage  ", 0, 15, yCol + 1, xCol);
            seg041.displayString(var_30, 0, 10, yCol + 1, xCol + 7);
            
            xCol = 0x16;
            seg041.displayString("Encumbrance  ", 0, 15, yCol, xCol);
            seg041.displayString(player.weight.ToString(), 0, 10, yCol, xCol + 12);

            var_7 = player.initiative;

            if (ovr025.find_affect(out var_34, Affects.slow, player) == true)
            {
                var_7 *= 2;
            }

            if (ovr025.find_affect(out var_34, Affects.haste, player) == true)
            {
                var_7 /= 2;
            }

            seg041.displayString("Movement ", 0, 15, yCol + 1, xCol + 3);
            seg041.displayString(var_7.ToString(), 0, 10, yCol + 1, xCol + 0x0c);
        }


        internal static void display_stat(bool arg_0, byte stat_index)
        {
            int color = arg_0 ? 0x0D : 0x0A;
            int col_x = 5;
            seg037.draw8x8_clear_area(stat_index + 7, 0x0b, stat_index + 7, col_x);

            if (gbl.player_ptr.stats[stat_index].max < 10)
            {
                col_x++;
            }

            string s = gbl.player_ptr.stats[stat_index].max.ToString();
            seg041.displayString(s, 0, color, stat_index + 7, col_x);

            if (stat_index == 0 &&
                gbl.player_ptr.strength == 18 &&
                gbl.player_ptr.strength_18_100 > 0)
            {
                string text = gbl.player_ptr.strength_18_100.ToString();

                if (gbl.player_ptr.strength_18_100 < 10)
                {
                    text = "0" + text;
                }

                if (gbl.player_ptr.strength_18_100 == 100)
                {
                    text = "00";
                }

                seg041.displayString("(" + text + ")", 0, color, 7, 7);
            }
        }

        static Set asc_54B50 = new Set(0x0902, new byte[] { 0x02, 0x18 });
        static Set unk_54B03 = new Set(0x0009, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20 });

        internal static void viewPlayer(out bool arg_0)
        {
            if (gbl.game_state == 5)
            {
                ovr033.Color_0_8_normal();
            }

            char input_key = ' ';
            arg_0 = false;

            gbl.player_ptr01 = gbl.player_ptr;

            playerDisplayFull();

            while (unk_54B03.MemberOf(input_key) == false && arg_0 == false)
            {
                string text = string.Empty;
                bool hasSpells = false;
                bool hasMoney = false;

                for (int i = 0; i < gbl.max_spells; i++)
                {
                    if (gbl.player_ptr02.spell_list[i] > 0)
                    {
                        hasSpells = true;
                    }
                }

                for (int i = 0; i <= 6; i++)
                {
                    if (gbl.player_ptr02.Money[i] > 0)
                    {
                        hasMoney = true;
                    }
                }

                if (gbl.player_ptr02.itemsPtr != null)
                {
                    text += "Items ";
                }

                if (hasSpells == true)
                {
                    text += "Spells ";
                }

                if (gbl.player_ptr02.field_F7 < 0x80 ||
                    gbl.player_ptr02.in_combat == false ||
                    gbl.player_ptr02.health_status == Status.animated)
                {
                    if (hasMoney && gbl.game_state != 5)
                    {
                        text += "Trade ";
                    }
                }

                if (hasMoney)
                {
                    text += "Drop ";
                }

                if (CanCastHeal(gbl.player_ptr) == true)
                {
                    text += "Heal ";
                }

                if (CanCastCure(gbl.player_ptr) == true)
                {
                    text += "Cure ";
                }

                text += "Exit";

                bool dummyBool;
                input_key = ovr027.displayInput(out dummyBool, false, 0, 15, 10, 13, text, string.Empty);

                short var_32 = -1;

                switch (input_key)
                {
                    case 'I':
                        PlayerItemsMenu(ref arg_0);
                        break;

                    case 'S':
                        spell_menu2(out hasSpells, ref var_32, 0, SpellLoc.memory);
                        break;

                    case 'T':
                        tradeCoin();
                        break;

                    case 'D':
                        drop_coin();
                        displayMoney();
                        break;

                    case 'H':
                        sub_576D7(gbl.player_ptr);
                        break;

                    case 'C':
                        paladin_cure_disease(gbl.player_ptr);
                        break;
                }

                if (arg_0 == false &&
                    asc_54B50.MemberOf(input_key) == true)
                {
                    playerDisplayFull();
                }
            }

            if (gbl.game_state == 5)
            {
                ovr033.Color_0_8_inverse();
            }
            ovr025.load_pic();
        }


        internal static bool sub_54EC1(Item arg_0)
        {
            bool var_1;

            var_1 = false;
            if (arg_0.readied)
            {
                ovr025.string_print01("Must be unreadied");
            }
            else if (ovr023.item_is_scroll(arg_0) == false)
            {
                var_1 = true;
            }
            else if ((int)arg_0.affect_1 > 0x7F || (int)arg_0.affect_2 > 0x7F || (int)arg_0.affect_3 > 0x7F)
            {
                ovr025.displayPlayerName(false, 15, 1, gbl.player_ptr);

                gbl.textXCol = (byte)(gbl.player_ptr.name.Length + 2);
                gbl.textYCol = 0x15;

                seg041.press_any_key(" was going to scribe from that scroll", false, 0, 0x0E, 0x16, 0x26, 0x15, 1);
                if (ovr027.yes_no(15, 10, 13, "is it Okay to lose it? ") == 'Y')
                {
                    var_1 = true;
                }
            }
            else
            {
                var_1 = true;
            }

            seg037.draw8x8_clear_area(0x16, 0x26, 0x15, 1);

            return var_1;
        }

        internal static void ItemDisplayStats(Item arg_0) /*sub_550A6*/
        {
            seg037.draw8x8_01();

            seg041.displayString("itemptr:      ", 0, 10, 1, 1);
            seg041.displayString(arg_0.type.ToString(), 0, 10, 1, 0x14);
            
            seg041.displayString("namenum(1):   ", 0, 10, 2, 1);
            seg041.displayString(arg_0.field_2F.ToString(), 0, 10, 2, 0x14);
            
            seg041.displayString("namenum(2):   ", 0, 10, 3, 1);
            seg041.displayString(arg_0.field_30.ToString(), 0, 10, 3, 0x14);
            
            seg041.displayString("namenum(3):   ", 0, 10, 4, 1);
            seg041.displayString(arg_0.field_31.ToString(), 0, 10, 4, 0x14);
            
            seg041.displayString("plus:         ", 0, 10, 5, 1);
            seg041.displayString(arg_0.plus.ToString(), 0, 10, 5, 0x14);
            
            seg041.displayString("plussave:     ", 0, 10, 6, 1);
            seg041.displayString(arg_0.plus_save.ToString(), 0, 10, 6, 0x14);
            
            seg041.displayString("ready:        ", 0, 10, 7, 1);
            seg041.displayString(arg_0.readied.ToString(), 0, 10, 7, 0x14);
            
            seg041.displayString("identified:   ", 0, 10, 8, 1);
            seg041.displayString(arg_0.hidden_names_flag.ToString(), 0, 10, 8, 0x14);
            
            seg041.displayString("cursed:       ", 0, 10, 9, 1);
            seg041.displayString(arg_0.cursed.ToString(), 0, 10, 9, 0x14);
            
            seg041.displayString("value:        ", 0, 10, 10, 1);
            seg041.displayString(arg_0._value.ToString(), 0, 10, 10, 0x14);
            
            seg041.displayString("special(1):   ", 0, 10, 11, 1);
            seg041.displayString(arg_0.affect_1.ToString(), 0, 10, 11, 0x14);
            
            seg041.displayString("special(2):   ", 0, 10, 12, 1);
            seg041.displayString(arg_0.affect_2.ToString(), 0, 10, 12, 0x14);
            
            seg041.displayString("special(3):   ", 0, 10, 13, 1);
            seg041.displayString(arg_0.affect_3.ToString(), 0, 10, 13, 0x14);
            
            seg041.displayString("dice large:   ", 0, 10, 14, 1);
            seg041.displayString(gbl.unk_1C020[arg_0.type].diceCount.ToString(), 0, 10, 14, 0x14);
            
            seg041.displayString("sides large:  ", 0, 10, 15, 1);
            seg041.displayString(gbl.unk_1C020[arg_0.type].diceSize.ToString(), 0, 10, 15, 0x14);

            seg041.displayAndDebug("press a key", 0, 10);
        }

        static Set unk_554EE = new Set(0x0009, new byte[] { 0x01, 0, 0, 0, 0, 0, 0, 0, 0x20 });

        internal static void PlayerItemsMenu(ref bool arg_0) /*use_item*/
        {
            byte var_40;
            Player player;
            short var_37;
            Item curr_item;
            bool var_2D;
            byte var_2C;
            string text;
            char var_1;

            player = gbl.player_ptr;
            var_1 = ' ';

            curr_item = player.itemsPtr;

            var_37 = 0;
            var_2D = true;
            var_2C = 1;

            while (unk_554EE.MemberOf(var_1) == false &&
                arg_0 == false &&
                player.field_14C > 0)
            {
                var_40 = player.field_14C;

                if (player.itemsPtr != null)
                {
                    text = "Ready";

                    if (Cheats.view_item_stats)
                    {
                        text += " View";
                    }

                    if (player.in_combat == true &&
                        gbl.area_ptr.field_1CA == 0 &&
                        (gbl.game_state == 2 || gbl.game_state == 3 ||
                        gbl.game_state == 4 || gbl.game_state == 5 ||
                        (player.actions != null && player.actions.field_2 != 0)))
                    {
                        text += " Use";
                    }

                    if (player.field_F7 < 0x80 ||
                        player.in_combat == false ||
                        player.health_status == Status.animated)
                    {
                        if (gbl.game_state != 5)
                        {
                            text += " Trade";
                        }
                    }

                    text += " Drop";

                    if (player.field_14C < 16)
                    {
                        text += " Halve";
                    }

                    text += " Join";

                    if (player.field_F7 < 0x80 ||
                        player.in_combat == false ||
                        player.health_status == Status.animated)
                    {
                        if (gbl.game_state == 1)
                        {
                            text += " Sell";
                        }
                    }

                    if (gbl.game_state == 1)
                    {
                        text += " Id";
                    }

                    Item tmpItem = player.itemsPtr;

                    while (tmpItem != null)
                    {
                        ovr025.ItemDisplayNameBuild(false, true, 0, 0, tmpItem, player);

                        tmpItem = tmpItem.next;
                    }

                    if (var_2C != 0 || gbl.byte_1D2C8 == true)
                    {
                        seg037.draw8x8_07();

                        ovr025.displayPlayerName(true, 1, 1, player);

                        seg041.displayString("Items", 0, 10, 1, player.name.Length + 4);
                        seg041.displayString("Ready Item", 0, 15, 3, 1);

                        var_2D = true;
                        var_2C = 0;
                        gbl.byte_1D2C8 = false;
                    }

                    var_1 = ovr027.sl_select_item(out curr_item, ref var_37, ref var_2D, true,
                        player.itemsPtr, 0x16, 0x26, 5, 1, 15, 10, 13, text, string.Empty);

                    if (curr_item != null)
                    {
                        switch (var_1)
                        {
                            case 'V':
                                ItemDisplayStats(curr_item);
                                var_2D = true;
                                var_2C = 1;
                                break;

                            case 'R':
                                ready_Item(curr_item);
                                break;

                            case 'U':
                                if (curr_item.readied == false)
                                {
                                    ovr025.string_print01("Must be Readied");
                                    var_1 = ' ';
                                }
                                else if (ovr023.item_is_scroll(curr_item) == true ||
                                    (curr_item.affect_2 > 0 && (int)curr_item.affect_3 < 0x80))
                                {
                                    sub_56478(ref arg_0, curr_item);
                                    if (gbl.game_state != 5)
                                    {
                                        arg_0 = false;
                                    }

                                    if (arg_0 == false)
                                    {
                                        var_2C = 1;
                                    }

                                }
                                break;

                            case 'T':
                                if (sub_54EC1(curr_item) == true)
                                {
                                    trade_item(curr_item);
                                }
                                else
                                {
                                    var_1 = ' ';
                                }
                                var_2C = 1;
                                break;

                            case 'D':
                                if (sub_54EC1(curr_item) == true)
                                {
                                    ovr025.ItemDisplayNameBuild(false, false, 0, 0, curr_item, player);

                                    seg041.press_any_key("Your " + curr_item.name + "will be gone forever", true, 0, 14, 22, 0x26, 21, 1);

                                    if (ovr027.yes_no(15, 10, 13, "Drop It? ") == 'Y')
                                    {
                                        ovr025.lose_item(curr_item, gbl.player_ptr);
                                        var_2D = true;
                                    }

                                    seg037.draw8x8_clear_area(0x16, 0x26, 0x15, 1);
                                }
                                else
                                {
                                    var_1 = ' ';
                                }
                                break;

                            case 'H':
                                halve_items(curr_item);
                                break;

                            case 'J':
                                join_items(curr_item);
                                break;

                            case 'S':
                                if (sub_54EC1(curr_item) == true)
                                {
                                    sell_Item(curr_item);
                                }
                                else
                                {
                                    var_1 = ' ';
                                }
                                break;

                            case 'I':
                                IdentifyItem(ref var_2D, curr_item);
                                break;
                        }
                    }

                    ovr025.sub_66C20(player);
                }

                if (player.field_14C != var_40)
                {
                    var_2D = true;
                }
            }
        }

        /*seg600:44B6 unk_1A7C6*/
        public readonly static byte[,] unk_1A7C6 = { 
            {1, 0, 0, 0, 0},
            {0, 1, 0, 0, 0},
            {1, 1, 0, 0, 0},
            {1, 0, 1, 0, 0},
            {0, 0, 1, 0, 0},
            {0, 1, 0, 1, 0},
            {0, 0, 1, 1, 0},
            {0, 0, 0, 0, 1},
            {0, 1, 0, 0, 1},
            {0, 0, 1, 1, 1},
            {0, 0, 0, 1, 1}  };

        internal static void sub_55B04(byte arg_0, Item item)
        {
            Player player = gbl.player_ptr;

            int masked_affect = (int)item.affect_3 & 0x7F;

            switch (masked_affect)
            {
                case 0:
                    gbl.byte_1D8AC = 1;
                    ovr024.CallSpellJumpTable((arg_0 == 0)?(byte)1:(byte)0, item, player, item.affect_3);
                    break;

                case 1:
                    if (arg_0 != 0)
                    {
                        player.field_12D[2,0] *= 2;
                        player.field_12D[2,1] *= 2;
                        player.field_12D[2,2] *= 2;
                    }
                    else
                    {
                        int var_9 = player.magic_user_lvl + (player.field_116 * ovr026.sub_6B3D1(player));

                        player.field_12D[2,0] = 0;
                        player.field_12D[2,1] = 0;
                        player.field_12D[2,2] = 0;
                        player.field_12D[2,3] = 0;
                        player.field_12D[2,4] = 0;

                        player.field_12D[2,0] = 1;

                        for (int sp_lvl = 0; sp_lvl <= (var_9 - 2); sp_lvl++)
                        {
                            /* unk_1A7C6 = seg600:44B6 */
                            player.field_12D[2,0] += unk_1A7C6[sp_lvl, 0];
                            player.field_12D[2,1] += unk_1A7C6[sp_lvl, 1];
                            player.field_12D[2,2] += unk_1A7C6[sp_lvl, 2];
                            player.field_12D[2,3] += unk_1A7C6[sp_lvl, 3];
                            player.field_12D[2,4] += unk_1A7C6[sp_lvl, 4];
                        }

                        byte[] var_11 = new byte[5];
                        seg051.FillChar(0, 5, var_11);

                        for (int i = 0; i < gbl.max_spells; i++)
                        {
                            if (player.spell_list[i] != 0 &&
                                gbl.unk_19AEC[player.spell_list[i]].spellClass == 2)
                            {
                                int var_C = gbl.unk_19AEC[player.spell_list[i]].spellLevel;
                                var_11[var_C - 1] += 1;

                                if (var_11[var_C - 1] > player.field_12D[2, var_C - 1])
                                {
                                    player.spell_list[i] = 0;
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    ovr024.sub_648D9(3, player);
                    ovr026.sub_6AAEA(player);
                    break;

                case 4:
                    if (((int)item.affect_2 & 0x0f) != player.alignment)
                    {
                        item.readied = false;
                        int var_3 = (int)item.affect_2 << 4;

                        gbl.damage_flags = 8;
                        if (gbl.game_state == 5)
                        {
                            ovr025.sub_68DC0();
                        }

                        ovr024.damage_person(false, 0, var_3, player);
                        gbl.byte_1D2C8 = true;
                    }
                    break;

                case 5:
                    ovr024.sub_648D9(0, player);
                    break;

                case 6:
                    ovr024.sub_648D9(4, player);
                    ovr024.sub_648D9(5, player);
                    break;

                case 8:
                    switch ((int)item.affect_2)
                    {
                        case 0:
                            ovr024.sub_648D9(0, player);
                            break;

                        case 1:
                            ovr024.sub_648D9(1, player);
                            break;

                        case 2:
                            ovr024.sub_648D9(2, player);
                            break;

                        case 3:
                            ovr024.sub_648D9(3, player);
                            break;

                        case 4:
                            ovr024.sub_648D9(4, player);
                            break;

                        case 5:
                            ovr024.sub_648D9(5, player);
                            break;
                    }
                    break;

                case 9:
                    if (arg_0 == 0)
                    {
                        ovr024.remove_affect(null, Affects.spiritual_hammer, player);
                    }
                    break;

                case 10:
                    ovr024.sub_648D9(3, player);
                    break;

                case 11:
                    ovr026.sub_6AAEA(player);
                    break;

                case 12:
                    ovr024.sub_648D9(1, player);
                    break;

                case 13:
                    ovr024.sub_648D9(0, player);
                    ovr024.sub_648D9(1, player);
                    break;
            }
        }


        internal static void ready_Item(Item item)
        {
            Player player;
            byte var_3;
            byte var_2;
            bool var_1;

            var_1 = ((int)item.affect_3 > 0x7f);

            player = gbl.player_ptr;

            if (item.readied)
            {
                if (item.cursed == true)
                {
                    ovr025.string_print01("It's Cursed");
                }
                else
                {
                    item.readied = false;

                    if (var_1 == true)
                    {
                        sub_55B04(0, item);
                    }
                }
                return;
            }
            else
            {
                var_2 = 0;

                if ((player.field_185 + gbl.unk_1C020[item.type].field_1) > 2)
                {
                    var_2 = 3;
                }
   
                var_3 = gbl.unk_1C020[item.type].field_0;

                if (var_3 >= 0 && var_3 <= 8)
                {
                    if (player.itemArray[var_3] != null)
                    {
                        var_2 = 2;
                    }
                }
                else if (var_3 == 9)
                {
                    if (player.Item_ptr_02 != null)
                    {
                        var_2 = 2;
                    }
                }

                if (item.type == 0x49)
                {
                    if (player.Item_ptr_03 != null)
                    {
                        var_2 = 2;
                        var_3 = 0x0B;
                    }
                }

                if (item.type == 0x1C)
                {
                    if (player.Item_ptr_04 != null)
                    {
                        var_2 = 2;
                        var_3 = 0x0C;
                    }
                }

                if ((player.classFlags & gbl.unk_1C020[item.type].classFlags) == 0)
                {
                    var_2 = 1;
                }

                switch (var_2)
                {
                    case 0:
                        item.readied = true;
                        if (var_1 == true)
                        {
                            sub_55B04(1, item);
                        }
                        break;

                    case 1:
                        ovr025.string_print01("Wrong Class");
                        break;

                    case 2:
                        ovr025.ItemDisplayNameBuild(false, false, 0, 0, player.itemArray[var_3], player);
                        ovr025.string_print01("already using " + player.itemArray[var_3].name);
                        break;

                    case 3:
                        if (gbl.game_state != 5 ||
                            player.quick_fight == 0)
                        {
                            ovr025.string_print01("Your hands are full!");
                        }
                        break;
                }
            }
        }


        internal static void trade_item(Item item)
        {
            Player player_ptr = gbl.player_ptr01;
            ovr025.load_pic();

            ovr025.selectAPlayer(ref player_ptr, true, "Trade with Whom?");

            if (player_ptr != null)
            {
                gbl.player_ptr01 = player_ptr;
                if (canCarry(item, player_ptr) == true)
                {
                    ovr025.string_print01("Overloaded");
                }
                else
                {
                    ovr025.addItem(item, player_ptr);
                    ovr025.lose_item(item, gbl.player_ptr);
                    ovr025.sub_66C20(player_ptr);
                }
            }
        }


        internal static void halve_items(Item item)
        {
            Item item_ptr;

            int half_number = item.count / 2;

            if (half_number > 0)
            {
                int half_and_remander = item.count - half_number;

                item_ptr = item.ShallowClone();

                item_ptr.count = half_number;
                item_ptr.readied = false;

                item_ptr.next = item.next;

                item.count = half_and_remander;

                item.next = item_ptr;
            }
            else
            {
                ovr025.string_print01("Can't halve that");
            }
        }


        internal static void join_items(Item item) /*sub_56285*/
        {
            Item this_item = item;
            int items_count = this_item.count;

            Item item_ptr = gbl.player_ptr.itemsPtr;
            while (item_ptr != null)
            {
                Item next_item = item_ptr.next;

                if (item_ptr != this_item &&
                    item_ptr.count > 0 &&
                    item_ptr.field_2F == this_item.field_2F &&
                    item_ptr.field_30 == this_item.field_30 &&
                    item_ptr.field_31 == this_item.field_31 &&
                    item_ptr.type == this_item.type &&
                    item_ptr.plus == this_item.plus &&
                    item_ptr.plus_save == this_item.plus_save &&
                    item_ptr.cursed == this_item.cursed &&
                    item_ptr.weight == this_item.weight &&
                    item_ptr.affect_1 == this_item.affect_1 &&
                    (int)item_ptr.affect_1 < 2 &&
                    item_ptr.affect_2 == this_item.affect_2 &&
                    item_ptr.affect_3 == this_item.affect_3 )
                {
                    if (item_ptr.count + items_count <= 255)
                    {
                        items_count += item_ptr.count;
                        ovr025.lose_item(item_ptr, gbl.player_ptr);
                    }
                    else
                    {
                        this_item.count = 255;

                        item_ptr.count -= (byte)(255 - items_count);
                        items_count = item_ptr.count;
                        this_item = item_ptr;
                    }
                }

                item_ptr = next_item;
            }
            this_item.count = items_count;
        }


        internal static void sub_56478(ref bool arg_0, Item item)
        {
            bool var_4;
            short var_3;
            byte var_1;

            var_3 = -1;
            gbl.byte_1D88D = 0;
            var_1 = 0;

            if (ovr023.item_is_scroll(item) == true)
            {
                gbl.dword_1D5C6 = item;

                var_1 = spell_menu2(out var_4, ref var_3, 1, SpellLoc.scroll);
            }
            else if( item.affect_2 > 0 && (int)item.affect_3 < 0x80 )
            {
                gbl.byte_1D88D = 1;
                var_1 = (byte)((int)item.affect_2 & 0x7F);
            }

            if (var_1 == 0)
            {
                arg_0 = false;
            }
            else
            {
                if (gbl.game_state == 5 &&
                    gbl.player_ptr.quick_fight == 0)
                {
                    ovr025.sub_68DC0();
                }

                if (gbl.byte_1D88D != 0)
                {
                    ovr025.DisplayPlayerStatusString(false, 10, "uses an item", gbl.player_ptr);

                    if (gbl.game_state == 5)
                    {
                        seg041.displayString("Item:", 0, 10, 0x17, 0);

                        ovr025.ItemDisplayNameBuild(true, false, 0x17, 5, item, gbl.player_ptr);
                    }
                    else
                    {
                        ovr025.ItemDisplayNameBuild(true, false, 0x16, 1, item, gbl.player_ptr);
                    }

                    seg041.GameDelay();
                    ovr025.ClearPlayerTextArea();
                }

                gbl.byte_1D88D = 1;

                if (ovr023.item_is_scroll(item) == true)
                {
                    if (ovr026.getExtraFirstSkill(gbl.player_ptr) == (sbyte)Skills.magic_user ||
                        ovr026.getFirstSkill(gbl.player_ptr) == (sbyte)Skills.magic_user ||
                        ovr026.getExtraFirstSkill(gbl.player_ptr) == 0 ||
                        ovr026.getFirstSkill(gbl.player_ptr) == 0 ||
                        gbl.player_ptr.magic_user_lvl > 0 ||
                        gbl.player_ptr.cleric_lvl > 0)
                    {
                        ovr023.sub_5D2E1(ref arg_0, 0, gbl.player_ptr.quick_fight, var_1);
                    }
                    else
                    {
                        if (gbl.player_ptr.thief_lvl > 9 &&
                            ovr024.roll_dice(100, 1) <= 0x4b)
                        {
                            ovr023.sub_5D2E1(ref arg_0, 0, gbl.player_ptr.quick_fight, var_1);
                        }
                        else
                        {
                            ovr025.DisplayPlayerStatusString(true, gbl.textYCol, "oops!", gbl.player_ptr);
                        }
                    }
                }
                else
                {
                    ovr023.sub_5D2E1(ref arg_0, 0, gbl.player_ptr.quick_fight, var_1);
                }

                gbl.byte_1D88D = 0;

                if (gbl.game_state == 5 &&
                    gbl.unk_19AEC[var_1].field_B != 0)
                {

                    arg_0 = ovr025.clear_actions(gbl.player_ptr);
                }
            }

            if (arg_0 == true)
            {

                if (ovr023.item_is_scroll(item) == true)
                {
                    ovr023.remove_spell_from_scroll(var_1, item, gbl.player_ptr);
                }
                else if ( item.affect_1 > 0 )
                {
                    if (item.count > 1)
                    {
                        item.count -= 1;
                    }
                    else
                    {
                        item.affect_1 -= 1;
                        if (item.affect_1 == 0)
                        {
                            ovr025.lose_item(item, gbl.player_ptr);
                        }
                    }
                }
            }
        }


        internal static void sell_Item(Item item)
        {
            string var_208;
            short var_8;
            short var_6;
            short var_4;
            int item_value;

            item_value = 0;

            if (item._value > 0)
            {
                item_value = item._value / 2;
            }

            if (item.count > 1)
            {
                if (item.type != 73 ||
                    item.type != 28)
                {
                    item_value = (item.count * item_value) / 20;
                }
                else
                {
                    item_value *= item.count;
                }
            }

            ovr025.ItemDisplayNameBuild(false, false, 0, 0, item, gbl.player_ptr02);

            var_208 = "I'll give you " + item_value.ToString() + " gold pieces for your " + item.name;

            seg041.press_any_key(var_208, true, 0, 14, 0x16, 0x26, 0x15, 1);

            if (ovr027.yes_no(15, 10, 13, "Is It a Deal? ") == 'Y')
            {
                ovr025.string_print01("Sold!");

                ovr025.lose_item(item, gbl.player_ptr);

                var_4 = (short)(item_value / 5);
                var_6 = (short)(item_value % 5);

                if (ovr022.willOverload(out var_8, var_4 + var_6, gbl.player_ptr) == true)
                {
                    ovr025.string_print01("Overloaded. Money will be put in pool.");

                    if (var_8 > var_4)
                    {
                        gbl.player_ptr.platinum += var_4;
                    }
                    else
                    {
                        gbl.player_ptr.platinum += var_8;
                        gbl.pooled_money[money.platum] += var_4 - var_8;
                    }

                    gbl.player_ptr.gold += var_6;
                }
                else
                {
                    gbl.player_ptr.platinum += var_4;
                    gbl.player_ptr.gold += var_6;
                }
            }

            seg037.draw8x8_clear_area(0x16, 0x26, 0x15, 1);
        }


        internal static void IdentifyItem(ref bool arg_0, Item item)
        {
            byte var_9;
            int var_6;
            int var_2;

            var_9 = 0;
            ovr025.ItemDisplayNameBuild(false, false, 0, 0, item, gbl.player_ptr02);

            seg041.press_any_key("For 200 gold pieces I'll identify your " + item.name, true, 0, 0x0e, 0x16, 0x26, 0x15, 1);

            if (ovr027.yes_no(15, 10, 13, "Is It a Deal? ") == 'Y')
            {
                var_2 = getPlayerGold(gbl.player_ptr);

                if (var_2 >= 200)
                {
                    var_9 = 1;

                    ovr022.setPlayerMoney((short)(var_2 - 200));
                }
                else
                {
                    var_6 = ovr022.getPooledGold(gbl.pooled_money);

                    if (var_6 > 200)
                    {
                        var_9 = 1;

                        ovr022.setPooledGold(var_6 - 200);
                    }
                    else
                    {
                        ovr025.string_print01("Not Enough Money");
                    }
                }
            }

            if (var_9 != 0)
            {
                if (item.hidden_names_flag == 0)
                {
                    seg041.press_any_key("I can't tell anything new about your " + item.name, true, 0, 0x0e, 0x16, 0x26, 0x15, 1);
                }
                else
                {
                    item.hidden_names_flag = 0;
                    ovr025.ItemDisplayNameBuild(false, false, 0, 0, item, gbl.player_ptr02);

                    seg041.press_any_key("It looks like some sort of " + item.name, true, 0, 0x0e, 0x16, 0x26, 0x15, 1);

                    arg_0 = true;
                }
            }

            if (var_9 != 0)
            {
                seg041.GameDelay();
            }

            seg037.draw8x8_clear_area(0x16, 0x26, 0x15, 1);
        }


        internal static void tradeCoin()
        {
            bool var_133;
            bool var_132;
            bool var_131 = false; /* Simeon */
            short var_130;
            char var_12E;
            string var_12D;
            string var_2D;
            string var_28;
            short var_18;
            short var_16;
            StringList var_14;
            StringList var_10;
            StringList var_C;
            int var_8;
            byte var_7;
            byte var_6;
            byte var_5;
            Player var_4;

            do
            {
                var_4 = gbl.player_ptr01;
                ovr025.load_pic();

                ovr025.selectAPlayer(ref var_4, true, "Trade to?");

                if (var_4 == null)
                {
                    var_131 = true;
                }
                else
                {
                    playerDisplayFull();
                    do
                    {
                        displayMoney();
                        gbl.player_ptr01 = var_4;
                        var_C = null;
                        var_10 = null;

                        var_14 = null;
                        var_16 = 0;

                        for (var_5 = 0; var_5 <= 6; var_5++)
                        {
                            if (gbl.player_ptr.Money[var_5] != 0)
                            {
                                var_16++;

                                var_10 = var_C;
                                var_C = new StringList();
                                var_C.next = var_10;

                                seg051.Str(15, out var_28, 0, gbl.player_ptr.Money[var_5]);
                                var_8 = 8 - moneyString[var_5].Length;

                                var_2D = string.Empty;

                                for (var_7 = 0; var_7 < var_8; var_7++)
                                {
                                    var_2D += ' ';
                                }

                                var_C.s = var_2D + moneyString[var_5] + ' ' + var_28;
                                var_C.field_29 = 0;
                            }
                        }

                        var_14 = var_C;
                        var_18 = 0;
                        var_133 = true;

                        var_12E = ovr027.sl_select_item(out var_10, ref var_18, ref var_133, true,
                            var_C, 13, 0x19, 7, 12, 15, 10, 13, " Select", "Select type of coin ");

                        if (var_10 == null)
                        {
                            var_132 = true;
                        }
                        else
                        {
                            var_6 = ovr022.sub_59BAB(out var_12D, var_10.s);

                            var_12D = "How much " + var_12D + "will you trade? ";

                            var_130 = ovr022.sub_592AD(10, var_12D, gbl.player_ptr.Money[var_6]);

                            ovr022.add_object(var_6, var_130, var_4, gbl.player_ptr);
                            var_132 = true;
                            var_131 = true;

                            for (var_5 = 0; var_5 <= 6; var_5++)
                            {
                                if (gbl.player_ptr.Money[var_5] > 0)
                                {
                                    var_131 = false;
                                    var_132 = false;
                                }
                            }
                        }

                        if (var_14 != null)
                        {
                            ovr027.free_stringList(ref var_14);
                        }

                    } while (var_132 == false);
                }
            } while (var_131 == false);
        }


        internal static void drop_coin()
        {
            short var_12E;
            char var_12C;
            string var_12B;
            string var_2B;
            string var_26;
            bool var_16;
            bool var_15;
            StringList var_12;
            StringList var_10;
            StringList var_C;
            short var_8;
            short var_6;
            byte var_4;
            int var_3;
            byte var_2;
            byte var_1;

            do
            {
                displayMoney();
                var_C = null;
                var_10 = null;
                var_12 = null;
                var_6 = 0;

                for (var_1 = 0; var_1 <= 6; var_1++)
                {
                    if (gbl.player_ptr.Money[var_1] != 0)
                    {
                        var_6++;

                        var_10 = var_C;
                        var_C = new StringList();
                        var_C.next = var_10;

                        seg051.Str(15, out var_26, 0, gbl.player_ptr.Money[var_1]);
                        var_3 = 8 - moneyString[var_1].Length;
                        var_2B = string.Empty;

                        for (var_4 = 0; var_4 < var_3; var_4++)
                        {
                            var_2B += ' ';
                        }

                        var_C.s = var_2B + moneyString[var_1] + " " + var_26;
                        var_C.field_29 = 0;
                    }
                }

                var_12 = var_C;

                var_8 = 0;
                var_16 = true;

                var_12C = ovr027.sl_select_item(out var_10, ref var_8, ref var_16, true, var_C, 13, 0x19, 7,
                    12, 15, 10, 13, " Select", "Select type of coin ");

                if (var_10 == null)
                {
                    var_15 = true;
                }
                else
                {
                    var_2 = ovr022.sub_59BAB(out var_12B, var_10.s);

                    var_12B = "How much " + var_12B + "will you drop? ";

                    var_12E = ovr022.sub_592AD(10, var_12B, gbl.player_ptr.Money[var_2]);

                    ovr022.sub_59A19(var_2, var_12E, gbl.player_ptr);
                    var_15 = true;

                    for (var_1 = 0; var_1 <= 6; var_1++)
                    {
                        if (gbl.player_ptr.Money[var_1] > 0)
                        {
                            var_15 = false;
                        }
                    }
                }

                if (var_12 != null)
                {
                    ovr027.free_stringList(ref var_12);
                }
            } while (var_15 == false);
        }


        internal static bool canCarry(Item item, Player player)
        {
            short item_weight;
            bool tooHeavy;

            ovr025.sub_66C20(player);
            tooHeavy = false;

            if (player.field_14C > 15)
            {
                tooHeavy = true;
            }

            item_weight = item.weight;

            if (item.count > 0)
            {
                item_weight *= (short)item.count;
            }

            if ((player.weight + item_weight) > (ovr025.max_encumberance(player) + 1500))
            {
                tooHeavy = true;
            }

            return tooHeavy;
        }


        internal static void scroll_team_list(char input_key)
        {
            Player player = gbl.player_next_ptr;

            if (player != null)
            {
                if (input_key == 'G')
                {
                    if (player == gbl.player_ptr)
                    {
                        while (player.next_player != null)
                        {
                            player = player.next_player;
                        }
                    }
                    else
                    {
                        while (player != null &&
                            player.next_player != gbl.player_ptr)
                        {
                            player = player.next_player;
                        }
                    }
                }
                else if (input_key == 'O')
                {
                    if (gbl.player_ptr.next_player != null)
                    {
                        player = gbl.player_ptr.next_player;
                    }
                }
                else
                {
                    player = gbl.player_next_ptr;
                }
            }

            gbl.player_ptr = player;
        }


        internal static int getPlayerGold(Player player)
        {
            int var_6;
            int var_2;

            var_6 = 0;

            for (int i = 0; i < 5; i++)
            {
                var_6 += player.Money[i] * money.per_copper[i];
            }

            var_2 = (var_6 + 100) / money.per_copper[money.gold];

            return var_2;
        }

        internal static byte spell_menu2(out bool arg_0, ref short arg_4, byte arg_8, SpellLoc spl_location)
        {
            string text;
            byte result;

            switch (spl_location)
            {
                case SpellLoc.memory:

                    text = "in Memory";
                    break;

                case SpellLoc.grimoire:

                    text = "in Grimoire";
                    break;

                case SpellLoc.scroll:
                    text = "on Scroll";
                    break;

                case SpellLoc.scrolls:
                    text = "on Scrolls";
                    break;

                case SpellLoc.choose:
                    text = "to Choose";
                    break;

                case SpellLoc.memorize:
                    text = "to Memorize";
                    break;

                case SpellLoc.scribe:
                    text = "to Scribe";
                    break;

                default:
                    text = string.Empty;
                    break;
            }

            arg_0 = ovr023.sub_5CA74(spl_location);

            if (arg_0 == true )
            {
                if (arg_4 < 0 ||
                    arg_8 == 1)
                {
                    if (gbl.game_state != 5)
                    {
                        if (arg_8 == 2)
                        {
                            seg037.draw8x8_05();
                        }
                        else
                        {
                            seg037.draw8x8_07();
                        }
                    }
                    else
                    {
                        seg037.draw8x8_01();
                    }
                }

                ovr025.displayPlayerName(true, 1, 1, gbl.player_ptr);

                seg041.displayString("Spells " + text, 0, 10, 1, gbl.player_ptr.name.Length + 4);

                result = ovr023.spell_menu(ref arg_4, arg_8);
            }
            else
            {
                result = 0;
            }

            return result;
        }


        internal static bool CanCastHeal(Player player) /* sub_575F0 */
        {
            Affect dummyAffect;
            bool result;

            if ((player._class == ClassId.paladin || (player.field_114 > 0 && ovr026.sub_6B3D1(player) != 0)) &&
                 gbl.game_state != 5 &&
                    player.health_status == Status.okey &&
                    ovr025.find_affect(out dummyAffect, Affects.affect_8c, player) == false)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }


        internal static bool CanCastCure(Player player) /* sub_57655 */
        {
            bool result;

            if ((player._class == ClassId.paladin || (player.field_114 > 0 && ovr026.sub_6B3D1(player) != 0)) &&
                gbl.game_state != 5 &&
                player.health_status == Status.okey &&
                player.field_191 > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }


        internal static void sub_576D7(Player player)
        {
            ovr025.load_pic();
            Player player_ptr = gbl.player_next_ptr;

            ovr025.selectAPlayer(ref player_ptr, true, "Heal whom? ");

            if (player_ptr == null)
            {
                playerDisplayFull();
            }
            else
            {
                int dx = player.field_114 * ovr026.sub_6B3D1(player);
                int ax = (player.paladin_lvl + dx) * 2;

                if (ovr024.heal_player(0, (byte)ax, player_ptr) == true)
                {
                    ovr025.string_print01(player_ptr.name + " feels better");
                }
                else
                {
                    ovr025.string_print01(player_ptr.name + " is unaffected");
                }

                ovr024.add_affect(false, 0, 0x5a0, Affects.affect_8c, player);
                playerDisplayFull();
            }
        }

        static Affects[] unk_16B39 = { 
            Affects.dispel_evil /* not used one offset array */,
            Affects.helpless, 
            Affects.cause_disease_1, 
            Affects.affect_2b, 
            Affects.cause_disease_2, 
            Affects.hot_fire_shield, 
            Affects.affect_39 };

        internal static void paladin_cure_disease(Player player) /* sub_577EC */
        {
            Affect dummy_affect;

            ovr025.load_pic();
            Player target = gbl.player_next_ptr;

            ovr025.selectAPlayer(ref target, true, "Cure whom? ");

            if (target == null)
            {
                playerDisplayFull();
            }
            else
            {
                bool is_diseased = false;
                for (gbl.byte_1DA71 = 1; gbl.byte_1DA71 < 7; gbl.byte_1DA71++)
                {
                    if (ovr025.find_affect(out dummy_affect, unk_16B39[gbl.byte_1DA71], target) == true)
                    {
                        is_diseased = true;
                    }
                }

                char input = 'Y';

                if (is_diseased == false)
                {
                    ovr025.DisplayPlayerStatusString(false, 0, "is not diseased", target);

                    input = ovr027.yes_no(15, 10, 13, "cure anyway: ");

                    ovr025.ClearPlayerTextArea();
                }

                if (input == 'Y')
                {
                    gbl.byte_1D2C6 = true;
                    for (gbl.byte_1DA71 = 1; gbl.byte_1DA71 < 7; gbl.byte_1DA71++)
                    {
                        ovr024.remove_affect(null, unk_16B39[gbl.byte_1DA71], target);
                    }

                    gbl.byte_1D2C6 = false;

                    if (player.field_191 > 0)
                    {
                        player.field_191--;
                    }

                    if (ovr025.find_affect(out dummy_affect, Affects.affect_8D, player) == false)
                    {
                        ovr024.add_affect(true, 0, 0x2760, Affects.affect_8D, player);
                    }

                    ovr025.string_print01(target.name + " is cured");
                }

                playerDisplayFull();
            }
        }
    }
}
