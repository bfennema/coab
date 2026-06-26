using Avalonia.Data;
using Avalonia.Platform;
using Classes;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace engine
{
    class ovr022
    {
        internal static int get_max_load(Player player)
        {
            return 1500 + ovr025.max_encumberance(player);
        }

        internal static bool willOverload(int item_weight, Player player)
        {
            int dummyInt;
            return willOverload(out dummyInt, item_weight, player);
        }


        internal static bool willOverload(out int weight, int item_weight, Player player)
        {
            bool ret_val;

            if ((player.weight + item_weight) > get_max_load(player))
            {
                weight = get_max_load(player) - player.weight;
                ret_val = true;
            }
            else
            {
                weight = 0;
                ret_val = false;
            }

            return ret_val;
        }

        internal static void addPlayerGold(int item_weight)
        {
            int capasity;

            if (willOverload(out capasity, item_weight, gbl.SelectedPlayer) == true)
            {
                ovr025.string_print01("Overloaded. Money will be put in Pool.");
                gbl.SelectedPlayer.Money.AddCoins(Money.Platinum, capasity);
                gbl.SelectedPlayer.AddWeight(capasity);

                gbl.pooled_money.AddCoins(Money.Platinum, item_weight - capasity);
            }
            else
            {
                gbl.SelectedPlayer.Money.AddCoins(Money.Platinum, item_weight);
                gbl.SelectedPlayer.AddWeight(item_weight);
            }
        }


        internal static short AskNumberValue(byte fgColor, string prompt, int maxValue) // sub_592AD
        {
            ovr027.ClearPromptAreaNoUpdate();
            seg041.displayString(prompt, 0, fgColor, 0x18, 0);

            int prompt_width = prompt.Length;
            int xCol = prompt_width;

            char inputKey;
            string maxValueStr = maxValue.ToString();
            string currentValueStr = string.Empty;

            do
            {
                inputKey = (char)Input.GetInputKey();

                if (inputKey >= 0x30 &&
                    inputKey <= 0x39)
                {
                    currentValueStr += inputKey.ToString();

                    int tmpValue = int.Parse(currentValueStr);

                    if (maxValue >= tmpValue)
                    {
                        xCol++;
                    }
                    else
                    {
                        currentValueStr = maxValueStr;

                        xCol = maxValueStr.Length + prompt_width;
                    }

                    seg041.displayString(currentValueStr, 0, 15, 0x18, prompt_width);
                }
                else if (inputKey == 8 && currentValueStr.Length > 0)
                {
                    int i = currentValueStr.Length - 1;
                    currentValueStr = seg051.Copy(i, 0, currentValueStr);

					seg041.displaySpaceChar(0x18, xCol-1);
                    xCol--;
                }
            } while (inputKey != 0x0D && inputKey != 0x1B);

            ovr027.ClearPromptAreaNoUpdate();

            int var_44;
            if (inputKey == 0x1B || 
                (inputKey == 0x0D && currentValueStr.Length == 0))
            {
                var_44 = 0;
            }
            else
            {
                var_44 = int.Parse(currentValueStr);
            }

            return (short)var_44;
        }


        internal static void trade_money(int money_slot, short num_coins, Player dest, Player source) /* add_object */
        {
            if ((dest.weight + num_coins) <= get_max_load(dest))
            {
                source.Money.AddCoins(money_slot, -num_coins);
                source.RemoveWeight(num_coins);

                dest.Money.AddCoins(money_slot, num_coins);
                dest.AddWeight(num_coins);
            }
            else
            {
                ovr025.string_print01("Overloaded");
            }
        }


        internal static void poolMoney()
        {
            foreach (Player player in gbl.TeamList)
            {
                if (player.control_morale == Control.PC_Base ||
                    player.control_morale == Control.PC_Berserk)
                {
                    gbl.pooled_money += player.Money;
                    for (int coin = 0; coin < 7; coin++)
                    {
                        player.RemoveWeight(player.Money.GetCoins(coin));
                    }
                    player.Money.ClearAll();
                }
            }
        }


        internal static int GetPartyCount() /* sub_595FF */
        {
            int count = 0;

            foreach (Player player in gbl.TeamList)
            {
                if (player.control_morale == Control.PC_Base ||
                    player.control_morale == Control.PC_Berserk)
                {
                    count++;
                }
            }

            return count;
        }


        internal static void share_pooled()
        {
            int[] money_remander = new int[9];
            int[] money_each = new int[9];

            int partySize = GetPartyCount();

            for (int coin = 0; coin <= 6; coin++)
            {
                if (gbl.pooled_money.GetCoins(coin) > 0)
                {
                    money_each[coin] = gbl.pooled_money.GetCoins(coin) / partySize;
                    money_remander[coin] = gbl.pooled_money.GetCoins(coin) % partySize;
                }
                else
                {
                    money_each[coin] = 0;
                    money_remander[coin] = 0;
                }
            }

            foreach (Player player in gbl.TeamList)
            {
                if (player.control_morale < Control.NPC_Base)
                {
                    for (int coin = 6; coin >= 0; coin--)
                    {
                        int overflow;
                        if (willOverload(out overflow, money_each[coin], player) == false)
                        {
                            player.Money.AddCoins(coin, money_each[coin]);
                            player.AddWeight(money_each[coin]);

                            if (money_remander[coin] > 0 &&
                                willOverload(1, player) == false)
                            {
                                player.Money.AddCoins(coin, 1);
                                player.AddWeight(1);
                                money_remander[coin] -= 1;
                            }
                        }
                        else
                        {
                            player.Money.AddCoins(coin, overflow);

                            money_remander[coin] += money_each[coin] - overflow;

                            player.AddWeight(overflow);
                        }
                    }
                }
            }

            for (int coin = 8; coin >= 0; coin--)
            {
                if (money_remander[coin] > 0)
                {
                    foreach (Player player in gbl.TeamList)
                    {
                        int capacity = get_max_load(player) - player.weight;

                        if (capacity > 0)
                        {
                            if (money_remander[coin] > capacity)
                            {
                                player.Money.AddCoins(coin, capacity);
                                player.AddWeight(capacity);
                                money_remander[coin] -= capacity;
                            }
                            else
                            {
                                player.Money.AddCoins(coin, money_remander[coin]);
                                player.AddWeight(money_remander[coin]);
                                money_remander[coin] = 0;
                            }
                        }
                    }
                }
            }

            for (int coin = Money.Copper; coin <= Money.Jewelry; coin++)
            {
                gbl.pooled_money.SetCoins(coin, money_remander[coin]);
            }
        }


        internal static void DropCoins(int money_slot, int num_coins, Player player) /* sub_59A19 */
        {
            player.Money.AddCoins(money_slot, -num_coins);
            player.RemoveWeight(num_coins);

            if (gbl.game_state == GameState.AfterCombat ||
                gbl.game_state == GameState.Shop)
            {
                gbl.pooled_money.AddCoins(money_slot, num_coins);
            }
        }


        internal static void PickupCoins(int money_slot, int num_coins, Player player) /* sub_59AA0 */
        {
            if (willOverload(num_coins, player) == true)
            {
                ovr025.string_print01("Overloaded");
            }
            else
            {
                if (num_coins > gbl.pooled_money.GetCoins(money_slot))
                {
                    num_coins = gbl.pooled_money.GetCoins(money_slot);
                }

                gbl.pooled_money.AddCoins(money_slot, -num_coins);

                player.Money.AddCoins(money_slot, num_coins);
                player.AddWeight(num_coins);
            }
        }


        internal static int GetMoneyIndexFromString(out string displayText, string input) // sub_59BAB
        {
            int offset = 0;
            int index = 7; // this is outofbounds.
            displayText = string.Empty;

            while (input[offset] == ' ')
            {
                offset++;
            }

            char ch = input[offset];
            if (ch == 'G')
            {
                ch = input[offset + 1];
                if (char.ToUpper(ch) == 'E')
                {
                    index = 5;
                    displayText = "Gems ";
                }
                else
                {
                    displayText = "Gold ";
                    index = 3;
                }
            }
            else if (ch == 'P')
            {
                displayText = "Platinum ";
                index = 4;
            }
            else if (ch == 'E')
            {
                displayText = "Electrum ";
                index = 2;
            }
            else if (ch == 'S')
            {
                displayText = "Silver ";
                index = 1;
            }
            else if (ch == 'C')
            {
                displayText = "Copper ";
                index = 0;
            }
            else if (ch == 'J')
            {
                displayText = "Jewelry ";
                index = 6;
            }

            return index;
        }


		internal static void TakePoolMoney() // takeItems
		{
			bool noMoneyLeft;

			List<MenuItem> money = new List<MenuItem>();

			gbl.game.DrawFrame_Outer();

			do
			{
				bool var_118 = true;

				money.Clear();

				for (int coin = 6; coin >= 0; coin--)
				{
					if (gbl.pooled_money.GetCoins(coin) > 0)
					{
						money.Add(new MenuItem(string.Format("{0} {1}", Money.names[coin], gbl.pooled_money.GetCoins(coin))));
					}
				}

				int dummyIndex = 0;

				MenuItem var_C;
				char input_key = ovr027.sl_select_item(out var_C, ref dummyIndex, ref var_118, true, money,
					8, 15, 2, 2, gbl.defaultMenuColors, "Select", "Select type of coin ");

				if (var_C == null || input_key == 0)
				{
					noMoneyLeft = true;
				}
				else
				{
					noMoneyLeft = false;
					string text;

					int money_slot = GetMoneyIndexFromString(out text, var_C.Text);

					text = string.Format("How much {0} will you take? ", text);

					int num_coins = AskNumberValue(10, text, gbl.pooled_money.GetCoins(money_slot));

					PickupCoins(money_slot, num_coins, gbl.SelectedPlayer);
					money.Clear();

					noMoneyLeft = true;
					for (int coin = 0; coin < 7; coin++)
					{
						if (gbl.pooled_money.GetCoins(coin) > 0)
						{
							noMoneyLeft = false;
						}
					}
				}
			} while (noMoneyLeft == false);
		}


        internal static void treasureOnGround(out bool items, out bool money)
        {
            money = gbl.pooled_money.AnyMoney();
            items = gbl.items_pointer.Count > 0;
        }


        internal static sbyte randomBonus() // sub_59FCF
        {
            sbyte bonus = 0;

            int roll = ovr024.roll_dice(20, 1);

            if (roll >= 1 && roll <= 14)
            {
                bonus = 1;
            }
            else if (roll >= 15 && roll <= 19)
            {
                bonus = 2;
            }
            else if (roll == 20)
            {
                if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                {
                    bonus = 3;
                }
                else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                {
                    bonus = 2;
                }
            }

            return bonus;
        }

        /*
Potion of Extra [0]
0C7C:0970  00 00 BB 00 A7 00 40 00  01 00 20 03 03 00 03 00  ..+.º.@.. ..

Potion of Giant Strength [1]: EF, A7, 40, 01, 044C, 01 00 52
0C7C:0980  00 00 EF 00 A7 00 40 00  01 00 4C 04 01 00 52 00  ..n.º.@..L.R.

Potion of Healing [2]: B9, A7, 40, 01, 0190 01 00 55
0C7C:0990  00 00 B9 00 A7 00 40 00  01 00 90 01 01 00 55 00  ..¦.º.@..É.U.

 Potion of Speed [3]: AD, A7, 40, 01, 450, 01, 80
0C7C:09A0  00 00 AD 00 A7 00 40 00  01 00 C2 01 01 00 50 00  ..¡.º.@..-.P.

Scroll of Restoration? [4] F0, A7, 41, 1, 2100, 56, 56, 0
0C7C:09B0  00 00 F0 00 A7 00 41 00  01 00 34 08 38 00 38 00  ..=.º.A..48.8.

Ring of Feather Fall [5] EE A7 42, 1, 5000, 0, 27, 128
0C7C:09C0  00 00 EE 00 A7 00 42 00  01 00 88 13 00 00 1B 00  ..e.º.B..ê...

Wand of Lightning [6] 9D A7 45, 1, 6000, 20, 51, 0
0C7C:09D0  80 00 9D 00 A7 00 45 00  01 00 70 17 14 00 33 00  Ç.¥.º.E..p¶.3.

Wand of Magic Missile [7], 1, 11000, 30, 88, 0
0C7C:09E0  00 00 CE 00 A7 00 45 00  01 00 F8 2A 1E 00 58 00  ..+.º.E..°*.X.

Wand of Paralyzation [8], 1, 5000, 20, 84, 0
0C7C:09F0  00 00 E1 00 A7 00 45 00  01 00 88 13 14 00 54 00  ..ß.º.E..ê¶.T.

Gauntlets of Ogre Power [9], 10, 15000, 0, 38, 131
0C7C:0A00  00 00 E2 00 A7 00 64 00  0A 00 98 3A 00 00 26 00  ..G.º.d..ÿ:..&.

Keoghtum's Ointment [10], 1, 10000, 5, 81, 0
0C7C:0A10  83 00 00 00 6F 00 B6 00  01 00 10 27 05 00 51 00  â...o.¦..'.Q.

Necklace of Missiles [11], 1, 16000, 20, 87, 0
0C7C:0A20  00 00 E4 00 A7 00 77 00  01 00 80 3E 14 00 57 00  ..S.º.w..Ç>¶.W.

Cloak of Displacement [12], 25, 17500, 0, 89, 133
0C7C:0A30  00 00 AA 00 A7 00 58 00  19 00 5C 44 00 00 59 00  ..¬.º.X..\D..Y.

Javelin of Lightning [13], 20, 3000, 1, 83, 0
0C7C:0A40  85 00 9D 00 A7 00 15 00  14 00 B8 0B 01 00 53 00  à.¥.º.§.¶.+.S.

Wand of Fireballs [14], 1, 5000, 20, 47, 0
0C7C:0A50  00 00 F2 00 A7 00 45 00  01 00 88 13 14 00 2F 00  ..=.º.E..ê¶./.

Ring of Fire Resistance [15], 1, 5000, 0, 61, 0
0C7C:0A60  00 00 CD 00 A7 00 42 00  01 00 88 13 00 00 3D 00  ..-.º.B..ê..=.

Ring of Invisibility [16], 1, 7500, 0, 56, 139
0C7C:0A70  81 00 E3 00 A7 00 42 00  01 00 4C 1D 00 00 38 00  ü.p.º.B..L..8.

Bag of Holding [17], 150, 25000, 0, 0, 0
0C7C:0A80  8B 00 BA 00 A7 00 49 00  96 00 A8 61 00 00 00 00  ï.¦.º.I.û.¿a....

Cloak of Elvenkind [18], 25, 6000, 0, 72, 134
0C7C:0A90  00 00 E5 00 A7 00 58 00  19 00 70 17 00 00 48 00  ..s.º.X..p..H.
0C7C:0AA0  86 00

        */
        static short[,]
        preconfiguredItems_Pool =
        {
            {(short)Item.Names.Extra,          (short)Item.Names.of,    (short)Item.Names.Potion,           1,   600,  3,  3,   0}, // potion extra healing
            {(short)Item.Names.Giant_Strength, (short)Item.Names.of,    (short)Item.Names.Potion,           1,  1100,  1, 82,   0}, // potion of giant strength
            {(short)Item.Names.Healing,        (short)Item.Names.of,    (short)Item.Names.Potion,           1,   400,  1, 85,   0}, // potion of healing
            {(short)Item.Names.Speed,          (short)Item.Names.of,    (short)Item.Names.Potion,           1,   450,  1, 80,   0}, // potion of speed
            {(short)Item.Names.Restoring_LevelOPsCP, (short)Item.Names.of, (short)Item.Names.Scroll,        1,  2100, 56, 56,   0}, // scroll of restoring level's
            {(short)Item.Names.Feather_Falling, (short)Item.Names.of,   (short)Item.Names.Ring,             1,  5000,  0, 27, 128}, // Ring of feather falling
            {(short)Item.Names.Lightning, (short)Item.Names.of,         (short)Item.Names.Wand,             1,  6000, 20, 51,   0}, // Wand of lightning
            {(short)Item.Names.Magic_Missiles, (short)Item.Names.of,    (short)Item.Names.Wand,             1, 11000, 30, 88,   0}, // Wand of magic missile
            {(short)Item.Names.Paralyzation,   (short)Item.Names.of,    (short)Item.Names.Wand,             1,  5000, 20, 84,   0}, // Wand of paralyzation
            {(short)Item.Names.Ogre_Power,     (short)Item.Names.of,    (short)Item.Names.Gauntlets,       10, 15000,  0, 38, 131}, // Gauntlets of ogre power
            {(short)Item.Names.empty,          (short)Item.Names.Ointment, (short)Item.Names.KeoghtumAPOSs, 1, 10000,  5, 81,   0}, // Koeghtum's Ointment
            {(short)Item.Names.Missiles,       (short)Item.Names.of,    (short)Item.Names.Necklace,         1, 16000, 20, 87,   0}, // Necklace of Missiles
            {(short)Item.Names.Displacement,   (short)Item.Names.of,    (short)Item.Names.Cloak,           25, 17500,  0, 89, 133}, // Cloak of Displacement
            {(short)Item.Names.Javelin,        (short)Item.Names.of,    (short)Item.Names.WEAPONJavelin,   20,  3000,  1, 83,   0}, // Javelin of lightning
            {(short)Item.Names.Fireballs,      (short)Item.Names.of,    (short)Item.Names.Wand,             1,  5000, 20, 47,   0}, // Wand of Fireballs
            {(short)Item.Names.Fire_Resistance,(short)Item.Names.of,    (short)Item.Names.Ring,             1,  5000,  0, 61, 129}, // Ring of Fire Resistance
            {(short)Item.Names.Invisibility,   (short)Item.Names.of,    (short)Item.Names.Ring,             1,  7500,  0, 56, 139}, // Ring of Invisibility
            {(short)Item.Names.Holding,        (short)Item.Names.of,    (short)Item.Names.Bag,            150, 25000,  0,  0,   0}, // Bag of Holding
            {(short)Item.Names.Elvenkind,      (short)Item.Names.of,    (short)Item.Names.Cloak,           25,  6000,  0, 72, 134}, // Cloak of Elvenkind
        };
        static short[,] /*seg600:082E unk_16B3E */
        preconfiguredItems_Curse = {
            {(short)Item.Names.Healing,        (short)Item.Names.Extra, (short)Item.Names.Potion,           1,   800,  3, 99,   0}, // potion extra healing
            {(short)Item.Names.Giant_Strength, (short)Item.Names.of,    (short)Item.Names.Potion,           1,  1100,  1, 59,   0}, // potion of giant strength
            {(short)Item.Names.Healing,        (short)Item.Names.of,    (short)Item.Names.Potion,           1,   400,  1,  3,   0}, // potion of healing
            {(short)Item.Names.Speed,          (short)Item.Names.of,    (short)Item.Names.Potion,           1,   450,  1, 48,   0}, // potion of speed (unused)
            {(short)Item.Names.Magic_Missiles, (short)Item.Names.of,    (short)Item.Names.Wand,             1, 11000, 30, 15,   0}, // wand of magic missile
            {(short)Item.Names.Ogre_Power,     (short)Item.Names.of,    (short)Item.Names.Gauntlets,       10, 15000,  0, 38, 131}, // gauntlets of ogre power (unused)
            {(short)Item.Names.Javelin,        (short)Item.Names.of,    (short)Item.Names.WEAPONJavelin,   20,  3000,  1, 51,   0}, // javelin of lightning
        };

        internal static Item create_item(Item.Type item_type) /* sub_5A007 */
        {
            int preconfig = -1;

            Item item = new Item(0, 0, 0, false, 6, false, 0, 0, 0, 0, 0, item_type, false);

            var type = item.type;

            if ((type >= Item.Type.BattleAxe && type <= Item.Type.Shield) ||
                type == Item.Type.Arrow ||
                type == Item.Type.Bracers ||
                type == Item.Type.RingOfProt)
            {
                item.plus = randomBonus();

                if (item.type == Item.Type.Javelin)
                {
                    int roll = ovr024.roll_dice(5, 1);
                    if (roll == 5)
                    {
                        if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                        {
                            preconfig = 13; // Javelin of Lightning
                        }
                        else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                        {
                            preconfig = 6;
                        }
                    }
                    else
                    {
                        item.namenum[2] = (Item.Names)item.type;
                        item.namenum[1] = Item.GetNamesPlus(item.plus);
                    }
                }
                else if (item.type == Item.Type.Quarrel)
                {
                    item.namenum[2] = (Item.Names)item.type;
                    item.namenum[1] = Item.GetNamesPlus(item.plus);
                }
                else if (item.type == Item.Type.LeatherArmor ||
                         item.type == Item.Type.PaddedArmor)
                {
                    item.namenum[2] = (Item.Names)item.type;
                    item.namenum[1] = Item.Names.ARMORArmor;
                    item.namenum[0] = Item.GetNamesPlus(item.plus);
                    item.hidden_names_flag = 4;
                }
                else if (item.type == Item.Type.StuddedLeather)
                {
                    item.namenum[2] = (Item.Names)item.type;
                    item.namenum[1] = Item.Names.ARMORLeather;
                    item.namenum[0] = Item.GetNamesPlus(item.plus);
                    item.hidden_names_flag = 4;
                }
                else if (item.type >= Item.Type.RingMail &&
                         item.type <= Item.Type.PlateMail)
                {
                    item.namenum[2] = (Item.Names)item.type;
                    item.namenum[1] = Item.Names.ARMORMail;
                    item.namenum[0] = Item.GetNamesPlus(item.plus);
                    item.hidden_names_flag = 4;
                }
                else if (item.type == Item.Type.Arrow)
                {
                    item.namenum[2] = Item.Names.WEAPONArrow;
                    item.namenum[1] = Item.GetNamesPlus(item.plus);
                }
                else if (item.type == Item.Type.Bracers)
                {
                    item.namenum[2] = Item.Names.Bracers;
                    item.namenum[1] = Item.Names.of;
                    item.plus = (item.plus << 1) + 2;

                    if (item.plus == 4)
                    {
                        item.namenum[0] = Item.Names.AC_6;
                    }
                    else if (item.plus == 6)
                    {
                        item.namenum[0] = Item.Names.AC_4;
                    }
                    else if (item.plus == 8)
                    {
                        item.namenum[0] = Item.Names.AC_2;
                    }
                }
                else if (item.type == Item.Type.RingOfProt)
                {
                    item.namenum[2] = Item.Names.Ring;
                    item.namenum[1] = Item.Names.of_ProtDOT;
                    item.namenum[0] = Item.GetNamesPlus(item.plus);
                    item.plus_save = (byte)item.plus;
                }
                else
                {
                    item.namenum[2] = (Item.Names)item.type;
                    item.namenum[1] = Item.GetNamesPlus(item.plus);
                }

                item.plus_save = 0;
                item.count = 0;

                switch (item.type)
                {
                    case Item.Type.BattleAxe:
                    case Item.Type.MilitaryFork:
                    case Item.Type.Glaive:
                    case Item.Type.BroadSword:
                        item.weight = 75;
                        break;

                    case Item.Type.HandAxe:
                    case Item.Type.Hammer:
                    case Item.Type.Ranseur:
                    case Item.Type.Spear:
                    case Item.Type.Spetum:
                    case Item.Type.QuarterStaff:
                    case Item.Type.Trident:
                    case Item.Type.CompositeShortBow:
                    case Item.Type.ShortBow:
                    case Item.Type.LightCrossbow:
                    case Item.Type.Shield:
                        item.weight = 50;
                        break;

                    case Item.Type.Bardiche:
                    case Item.Type.MorningStar:
                    case Item.Type.Voulge:
                        item.weight = 125;
                        break;

                    case Item.Type.BecDeCorbin:
                    case Item.Type.GlaiveGuisarme:
                    case Item.Type.Mace:
                    case Item.Type.BastardSword:
                    case Item.Type.LongBow:
                    case Item.Type.FineBow:
                    case Item.Type.PaddedArmor:
                        item.weight = 100;
                        break;

                    case Item.Type.BillGuisarme:
                    case Item.Type.Flail:
                    case Item.Type.GuisarmeVoulge:
                    case Item.Type.LucernHammer:
                    case Item.Type.LeatherArmor:
                        item.weight = 150;
                        break;

                    case Item.Type.BoStick:
                        item.weight = 15;
                        break;

                    case Item.Type.Club:
                        item.weight = 30;
                        break;

                    case Item.Type.Dagger:
                    case Item.Type.Bracers:
                        item.weight = 10;
                        break;

                    case Item.Type.Dart:
                        item.weight = 25;
                        item.count = 5;
                        break;

                    case Item.Type.Fauchard:
                    case Item.Type.MilitaryPick:
                    case Item.Type.LongSword:
                        item.weight = 60;
                        break;

                    case Item.Type.FauchardFork:
                    case Item.Type.Guisarme:
                    case Item.Type.Partisan:
                    case Item.Type.AwlPike:
                    case Item.Type.CompositeLongBow:
                    // case ItemType.Sling: - this exists twice. other seems correct (weight 80 for a sling seems off)
                        item.weight = 80;
                        break;

                    case Item.Type.Halberd:
                        item.weight = 175;
                        break;

                    case Item.Type.Javelin:
                        item.weight = 20;
                        break;

                    case Item.Type.JoStick:
                    case Item.Type.Scimitar:
                        item.weight = 40;
                        break;

                    case Item.Type.ShortSword:
                        item.weight = 35;
                        break;

                    case Item.Type.TwoHandedSword:
                    case Item.Type.RingMail:
                        item.weight = 250;
                        break;

                    case Item.Type.StuddedLeather:
                        item.weight = 200;
                        break;

                    case Item.Type.ScaleMail:
                    case Item.Type.SplintMail:
                        item.weight = 400;
                        break;

                    case Item.Type.ChainMail:
                        item.weight = 300;
                        break;

                    case Item.Type.BandedMail:
                        item.weight = 350;
                        break;

                    case Item.Type.PlateMail:
                        item.weight = 450;
                        break;

                    case Item.Type.Sling:
                    case Item.Type.RingOfProt:
                        item.weight = 1;
                        break;

                    default:
                        item.weight = 40;
                        item.count = 10;
                        break;
                }

                if (item.type == Item.Type.Shield)
                {
                    item._value = (short)(item.plus * 2500);
                }
                else if (item.type == Item.Type.Arrow || item.type == Item.Type.Quarrel)
                {
                    item._value = (short)(item.plus * 150);
                }
                else if (item.type == Item.Type.RingMail || item.type == Item.Type.ScaleMail)
                {
                    item._value = (short)(item.plus * 3000);
                }
                else if (item.type == Item.Type.ChainMail || item.type == Item.Type.SplintMail)
                {
                    item._value = (short)(item.plus * 3500);
                }
                else if (item.type == Item.Type.BandedMail)
                {
                    item._value = (short)(item.plus * 4000);
                }
                else if (item.type == Item.Type.PlateMail)
                {
                    item._value = (short)(item.plus * 5000);
                }
                else if (item.type == Item.Type.Bracers)
                {
                    item._value = (short)(item.plus * 3000);
                }
                else
                {
                    item._value = (short)(item.plus * 2000);
                }
            }
            else if (type == Item.Type.MUScroll || type == Item.Type.ClrcScroll)
            {
                byte spellsCount = ovr024.roll_dice(3, 1);

                if (item.type == Item.Type.MUScroll)
                {
                    item.namenum[2] = Item.Names.MU_Scroll;
                }
                else
                {
                    item.namenum[2] = Item.Names.Clrc_Scroll;
                }

                item.namenum[1] = Item.GetNamesSpellCount(spellsCount);
                item.namenum[0] = 0;
                item.plus = 1;
                item.weight = 0x19;
                item.count = 0;
                item._value = 0;

                for (int affect = 1; affect <= spellsCount; affect++)
                {
                    int roll = 0;
                    if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                    {
                        ovr024.roll_dice(3, 1);
                    }
                    else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                    {
                        ovr024.roll_dice(5, 1);
                    }
                    Spells spell = 0;

                    if (item.type == Item.Type.MUScroll)
                    {
                        switch (roll)
                        {
                            case 1:
                                spell = Spells.burning_hands + ovr024.roll_dice(13, 1) - 1;
                                break;

                            case 2:
                                spell = Spells.detect_invisibility + ovr024.roll_dice(7, 1) - 1;
                                break;

                            case 3:
                                spell = Spells.blink + ovr024.roll_dice(11, 1) - 1;
                                break;

                            case 4:
                                spell = Spells.charm_monsters + ovr024.roll_dice(9, 1) - 1;
                                break;

                            case 5:
                                spell = Spells.cloud_kill + ovr024.roll_dice(4, 1) - 1;
                                break;
                        }
                    }
                    else
                    {
                        switch (roll)
                        {
                            case 1:
                                spell = Spells.bless + ovr024.roll_dice(8, 1) - 1;
                                break;

                            case 2:
                                spell = Spells.find_traps + ovr024.roll_dice(7, 1) - 1;
                                break;

                            case 3:
                                if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                                {
                                    spell = Spells.animate_dead + ovr024.roll_dice(9, 1) - 1;
                                }
                                else
                                {
                                    spell = Spells.cure_blindness + ovr024.roll_dice(8, 1) - 1;
                                }
                                break;

                            case 4:
                                spell = Spells.cause_serious_wounds_CL + ovr024.roll_dice(5, 1) - 1;
                                break;

                            case 5:
                                spell = Spells.cure_critical_wounds + ovr024.roll_dice(6, 1) - 1;
                                break;
                        }
                    }

                    item.setAffect(affect, (Classes.Affects)spell);
                    item._value += (short)(roll * 300);
                }
            }
            else if (type == Item.Type.Gauntlets)
            {
                if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                {
                    preconfig = 9; // Gauntlets of Ogre Power
                }
                else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                {
                    preconfig = 5; // Gauntlets of Ogre Power
                }
            }
            else if (type == Item.Type.Cloak)
            {
                preconfig = 18; // Cloak of Elvenkind
            }
            else if (type == Item.Type.Ring) // Ring unused to CoAB
            {
                int roll = ovr024.roll_dice(10, 1);

                if (roll == 1)
                {
                    preconfig = 5; // Ring of Feather Falling
                }
                else if (roll >= 2 && roll <= 6)
                {
                    preconfig = 15; // Ring of Fire Resistance
                }
                else if (roll >= 7 && roll <= 10)
                {
                    preconfig = 16; // Ring of Invisibility;
                }
            }
            else if (type == Item.Type.WandA) // WandA unused in CoAB
            {
                int roll = ovr024.roll_dice(3, 1);
                if (roll == 1)
                {
                    preconfig = 6; // Wand of Lightning
                }
                else if (roll == 2)
                {
                    preconfig = 8; // Wand of Paralyzation
                }
                else if (roll == 3)
                {
                    preconfig = 14; // Wand of Fireballs
                }
            }
            else if (type == Item.Type.WandB) // WandA unused
            {
                if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                {
                    preconfig = 7; // Wand of Magic Missiles
                }
                else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                {
                    preconfig = 4; // Wand of Magic Missiles
                }
            }
            else if (type == Item.Type.PotionOfGiantStr)
            {
                preconfig = 1; // Potion of Giant Strength
            }
            else if (type == Item.Type.CloakOfDisplacement)
            {
                preconfig = 12; // Cloak of Displacement
            }
            else if (type == Item.Type.Potion)
            {
                if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                {
                    int roll = ovr024.roll_dice(11, 1);

                    if (roll >= 1 && roll <= 5)
                    {
                        preconfig = 2; // Potion of Healing
                    }
                    else if (roll >= 6 && roll <= 8)
                    {
                        preconfig = 0; // Potion of Extra
                    }
                    else if (roll == 9)
                    {
                        preconfig = 3; // Potion of Speed;
                    }
                    else if (roll == 10)
                    {
                        preconfig = 10; // Keoghtum's Ointment
                    }
                    else // 11 
                    {
                        // missed
                    }
                }
                else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
                {
                    int roll = ovr024.roll_dice(8, 1);

                    if (roll >= 1 && roll <= 5)
                    {
                        preconfig = 2; // Potion of Healing
                    }
                    else if (roll >= 6 && roll <= 8)
                    {
                        preconfig = 0; // Potion of Extra
                    }
                }
            }
            else if (type == Item.Type.GemsJewelry)
            {
                int roll = ovr024.roll_dice(10, 1);

                if (roll >= 1 && roll <= 7)
                {
                    preconfig = 11; // Necklace of Missiles
                }
                else if (roll >= 8 && roll <= 10)
                {
                    preconfig = 17; // Bag of Holding
                }
            }

            short[,] preconfiguredItems = { { } };
            if (gbl.game.Name == Logging.Game.PoolOfRadiance)
            {
                preconfiguredItems = preconfiguredItems_Pool;
            }
            else if (gbl.game.Name == Logging.Game.CurseOfTheAzureBonds)
            {
                preconfiguredItems = preconfiguredItems_Curse;
            }

            if (preconfig > -1 && preconfiguredItems.Length > preconfig)
            {
                item.namenum[0] = (Item.Names)preconfiguredItems[preconfig, 0];
                item.namenum[1] = (Item.Names)preconfiguredItems[preconfig, 1];
                item.namenum[2] = (Item.Names)preconfiguredItems[preconfig, 2];

                item.plus = 1;
                item.plus_save = 1;

                item.weight = preconfiguredItems[preconfig, 3];
                item.count = 0;

                item._value = preconfiguredItems[preconfig, 4];

                for (int affect = 1; affect <= 3; affect++)
                {
                    item.setAffect(affect, (Classes.Affects)(byte)preconfiguredItems[preconfig, 4 + affect]);
                }
            }

            ItemLibrary.Add(item);
            return item;
        }


        /// <summary>
        /// Turns if pictures need re-loading
        /// </summary>
        internal static bool appraiseGemsJewels()
        {
            bool special_key;
            short value;
            string sell_text;

            if (gbl.SelectedPlayer.Money.Gems == 0 && gbl.SelectedPlayer.Money.Jewels == 0)
            {
                ovr025.string_print01("No Gems or Jewelry");
                return false;
            }

            bool stop_loop;

            do
            {
                if (gbl.SelectedPlayer.Money.Gems == 0 && gbl.SelectedPlayer.Money.Jewels == 0)
                {
                    stop_loop = true;
                }
                else
                {
                    stop_loop = false;

                    string gem_text = gbl.SelectedPlayer.Money.Gems.ToString();
                    string jewel_text = gbl.SelectedPlayer.Money.Jewels.ToString();

                    if (gbl.SelectedPlayer.Money.Gems == 0)
                    {
                        gem_text = string.Empty;
                    }
                    else if (gbl.SelectedPlayer.Money.Gems == 1)
                    {
                        gem_text += " Gem";
                    }
                    else
                    {
                        gem_text += " Gems";
                    }

                    if (gbl.SelectedPlayer.Money.Jewels == 0)
                    {
                        jewel_text = string.Empty;
                    }
                    else if (gbl.SelectedPlayer.Money.Jewels == 1)
                    {
                        jewel_text += " piece of Jewelry";
                    }
                    else
                    {
                        jewel_text += " pieces of Jewelry";
                    }

                    seg037.draw8x8_clear_area(0x16, 0x26, 1, 1);
                    ovr025.displayPlayerName(false, 1, 1, gbl.SelectedPlayer);

                    seg041.displayString("You have a fine collection of:", 0, 0xf, 7, 1);
                    seg041.displayString(gem_text, 0, 0x0f, 9, 1);
                    seg041.displayString(jewel_text, 0, 0x0f, 0x0a, 1);
                    string prompt = string.Empty;

                    if (gbl.SelectedPlayer.Money.Gems != 0)
                    {
                        prompt = "  Gems";
                    }

                    if (gbl.SelectedPlayer.Money.Jewels != 0)
                    {
                        prompt += " Jewelry";
                    }

                    prompt += " Exit";

                    char input_key = ovr027.displayInput(out special_key, false, 1, gbl.defaultMenuColors, prompt, "Appraise : ");

                    if (input_key == 'G')
                    {
                        if (gbl.SelectedPlayer.Money.Gems > 0)
                        {
                            gbl.SelectedPlayer.Money.AddCoins(Money.Gems, -1);

                            int roll = ovr024.roll_dice(100, 1);

                            if (roll >= 1 && roll <= 25)
                            {
                                value = 10;
                            }
                            else if (roll >= 26 && roll <= 50)
                            {
                                value = 50;
                            }
                            else if (roll >= 51 && roll <= 70)
                            {
                                value = 100;
                            }
                            else if (roll >= 71 && roll <= 90)
                            {
                                value = 500;
                            }
                            else if (roll >= 91 && roll <= 99)
                            {
                                value = 1000;
                            }
                            else if (roll == 100)
                            {
                                value = 5000;
                            }
                            else
                            {
                                value = 0;
                            }

                            string value_text = "The Gem is Valued at " + value.ToString() + " gp.";

                            seg041.displayString(value_text, 0, 15, 12, 1);

                            bool must_sell;

                            if (willOverload(1, gbl.SelectedPlayer) == true ||
                                gbl.SelectedPlayer.items.Count >= Player.MaxItems)
                            {
                                sell_text = "Sell";
                                must_sell = true;
                            }
                            else
                            {
                                sell_text = "Sell Keep";
                                must_sell = false;
                            }

                            input_key = ovr027.displayInput(out special_key, false, 1, gbl.defaultMenuColors, sell_text, "You can : ");

                            if (input_key == 'K' && must_sell == false)
                            {
                                Item gem_item = new Item(value, 0, 1, false, 0, false, 0, 0, Item.Names.Gem, 0, 0, Item.Type.GemsJewelry, true);

                                gbl.SelectedPlayer.items.Add(gem_item);
                            }
                            else
                            {
                                value /= 5;
                                addPlayerGold(value);
                            }
                        }
                    }
                    else if (input_key == 'J')
                    {
                        if (gbl.SelectedPlayer.Money.Jewels > 0)
                        {
                            gbl.SelectedPlayer.Money.AddCoins(Money.Jewelry, -1);

                            int roll = ovr024.roll_dice(100, 1);

                            if (roll >= 1 && roll <= 10)
                            {
                                value = (short)(seg051.Random(900) + 100);
                            }
                            else if (roll >= 11 && roll <= 20)
                            {
                                value = (short)(seg051.Random(1000) + 200);
                            }
                            else if (roll >= 21 && roll <= 40)
                            {
                                value = (short)(seg051.Random(1500) + 300);
                            }
                            else if (roll >= 41 && roll <= 50)
                            {
                                value = (short)(seg051.Random(2500) + 500);
                            }
                            else if (roll >= 51 && roll <= 70)
                            {
                                value = (short)(seg051.Random(5000) + 1000);
                            }
                            else if (roll >= 0x47 && roll <= 0x5A)
                            {
                                value = (short)(seg051.Random(6000) + 2000);
                            }
                            else if (roll >= 0x5B && roll <= 0x64)
                            {
                                value = (short)(seg051.Random(10000) + 2000);
                            }
                            else
                            {
                                value = 0;
                            }

                            string value_text = string.Format("The Jewel is Valued at {0} gp.", value);
                            seg041.displayString(value_text, 0, 15, 12, 1);

                            bool must_sell;
                            if (willOverload(1, gbl.SelectedPlayer) == true ||
                                gbl.SelectedPlayer.items.Count >= Player.MaxItems)
                            {
                                sell_text = "Sell";
                                must_sell = true;
                            }
                            else
                            {
                                sell_text = "Sell Keep";
                                must_sell = false;
                            }

                            input_key = ovr027.displayInput(out special_key, false, 1, gbl.defaultMenuColors, sell_text, "You can : ");

                            if (input_key == 'K' && must_sell == false)
                            {
                                Item jewel_item = new Item(value, 0, 1, false, 0, false, 0, 0, Item.Names.Jewelry, 0, 0, Item.Type.GemsJewelry, true);

                                gbl.SelectedPlayer.items.Add(jewel_item);
                            }
                            else
                            {
                                value /= 5;
                                addPlayerGold(value);
                            }
                        }
                    }
                    else if (input_key == 'E' || input_key == 0)
                    {
                        stop_loop = true;
                    }

                    ovr025.reclac_player_values(gbl.SelectedPlayer);
                }

            } while (stop_loop == false);

            return true;
        }
    }
}
