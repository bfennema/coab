using Classes;
using System.Collections.Generic;

namespace engine
{
    class ovr026
    {
        static byte[,] /*seg600:42BC*/ ClericSpellLevels = { // unk_1A5CC
			{1, 0, 0, 0, 0},
			{0, 1, 0, 0, 0},
			{1, 1, 0, 0, 0},
			{0, 1, 1, 0, 0},
			{0, 0, 1, 0, 0}, 
			{0, 0, 0, 1, 0},    
			{0, 0, 1, 1, 0}, 
			{1, 1, 0, 0, 1},  //seg600:42EE
			{0, 0, 0, 1, 1},  //seg600:42F3
			{1, 0, 1, 0, 0},  //seg600:42F8
			{1, 1, 1, 0, 0},  //seg600:42FD
		};

        static byte[,] /*seg600:43E5*/ PaladinSpellLevels = { // unk_1A6F5
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0}, 
			{0, 0, 0, 0, 0},  
			{0, 0, 0, 0, 0}, 
			{1, 0, 0, 0, 0}, 
			{1, 0, 0, 0, 0}, 
			{0, 1, 0, 0, 0}, 
			{0, 1, 0, 0, 0}
		};

        static byte[,] /*seg600:4448*/ unk_1A758 = { 
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0},
			{1, 0, 0, 0, 0},
			{0, 0, 0, 1, 0},
			{1, 0, 0, 0, 0},
			{0, 0, 0, 1, 0},
			{0, 1, 0, 0, 0} 
		};



        internal static void sub_6A00F(Player player) // sub_6A00F
        {
            for (int i = 0; i < 5; i++)
            {
                player.spellCastCount[0, i] = 0;
                player.spellCastCount[1, i] = 0;
                player.spellCastCount[2, i] = 0;
            }

            for (SkillType skill = SkillType.Cleric; skill <= SkillType.Monk; skill++)
            {
                int skillLevel = player.SkillLevel(skill);

                if (skillLevel > 0)
                {
                    switch (skill)
                    {
                        case SkillType.Cleric:
                            player.spellCastCount[0, 0] += 1;

                            for (int PlayerLvl = 0; PlayerLvl <= (skillLevel - 2); PlayerLvl++)
                            {
                                for (int sp_lvl = 0; sp_lvl < 5; sp_lvl++)
                                {
                                    player.spellCastCount[0, sp_lvl] += ClericSpellLevels[PlayerLvl, sp_lvl];
                                }
                            }

                            calc_cleric_spells(false, player);

                            foreach (Spells spell in System.Enum.GetValues(typeof(Spells)))
                            {
                                SpellEntry se = gbl.spellCastingTable[(int)spell];

                                int sp_class = (se.spellLevel - 1) / 5;
                                int sp_lvl = (se.spellLevel - 1) % 5;

                                if (se.spellClass == 0 &&
                                    player.spellCastCount[sp_class, sp_lvl] > 0 &&
                                    spell != Spells.animate_dead)
                                {
                                    player.LearnSpell(spell);
                                }
                            }
                            break;

                        case SkillType.Paladin:
                            if (skillLevel > 8)
                            {
                                for (int addLvl = 8; addLvl < skillLevel; addLvl++)
                                {
                                    for (int spellLvl = 0; spellLvl < 5; spellLvl++)
                                    {
                                        player.spellCastCount[0, spellLvl] += PaladinSpellLevels[addLvl, spellLvl];
                                    }
                                }

                                foreach (Spells spell in System.Enum.GetValues(typeof(Spells)))
                                {
                                    SpellEntry se = gbl.spellCastingTable[(int)spell];
                                    int sp_class = (se.spellLevel - 1) / 5;
                                    int sp_lvl = (se.spellLevel - 1) % 5;

                                    if (se.spellClass == 0 &&
                                        player.spellCastCount[sp_class, sp_lvl] > 0)
                                    {
                                        player.LearnSpell(spell);
                                    }
                                }
                            }
                            break;

                        case SkillType.Ranger:
                            if (skillLevel > 7)
                            {
                                for (int var_3 = 8; var_3 <= skillLevel; var_3++)
                                {
                                    for (int sp_lvl = 0; sp_lvl < 3; sp_lvl++)
                                    {
                                        player.spellCastCount[1, sp_lvl] += unk_1A758[var_3, sp_lvl];
                                    }

                                    for (int sp_lvl = 3; sp_lvl < 5; sp_lvl++)
                                    {
                                        player.spellCastCount[2, sp_lvl - 3] += unk_1A758[var_3, sp_lvl];
                                    }
                                }

                                foreach (Spells spell in System.Enum.GetValues(typeof(Spells)))
                                {
                                    if (gbl.spellCastingTable[(int)spell].spellClass == SpellClass.Druid)
                                    {
                                        player.LearnSpell(spell);
                                    }
                                }
                            }

                            break;

                        case SkillType.MagicUser:
                            player.spellCastCount[2, 0] += 1;

                            for (int lvl = 0; lvl <= (skillLevel - 2); lvl++)
                            {
                                /* unk_1A7C6 = seg600:44B6 */
                                player.spellCastCount[2, 0] += ovr020.MU_spell_lvl_learn[lvl, 0];
                                player.spellCastCount[2, 1] += ovr020.MU_spell_lvl_learn[lvl, 1];
                                player.spellCastCount[2, 2] += ovr020.MU_spell_lvl_learn[lvl, 2];
                                player.spellCastCount[2, 3] += ovr020.MU_spell_lvl_learn[lvl, 3];
                                player.spellCastCount[2, 4] += ovr020.MU_spell_lvl_learn[lvl, 4];
                            }
                            break;
                    }
                }
            }

            foreach (Item item in player.items)
            {
                if (item.affect_3 == Affects.protect_magic && item.readied)
                {
                    for (int sp_lvl = 0; sp_lvl < 3; sp_lvl++)
                    {
                        player.spellCastCount[2, sp_lvl] *= 2;
                    }
                }
            }
        }


        internal static void ReclacClassBonuses(Player player) // sub_6A3C6
        {
            player.thac0 = 0;

            for (int _class = 0; _class <= 7; _class++)
            {
                byte class_lvl = player.ClassLevel[_class];

                player.thac0 = System.Math.Max(ovr018.thac0_table[_class, class_lvl], player.thac0);
                player.HitDice = System.Math.Max(class_lvl, player.HitDice);
            }

            if (player.fighter_lvl >= 7 || 
                player.paladin_lvl >= 7 ||
                player.ranger_lvl >= 8)
            {
                player.attacksCount = 3;
            }

            sub_6A00F(player);
            reclac_saving_throws(player);

            if (player.thief_lvl > 0)
            {
                recalc_thief_skills(player);
            }

            player.classFlags = 0;

            for (int skill = 0; skill <= 7; skill++)
            {
                if (player.ClassLevel[skill] > 0 ||
                    (player.ClassLevelsOld[skill] > 0 && player.ClassLevelsOld[skill] < player.HitDice))
                {
                    player.classFlags += ovr018.unk_1A1B2[skill];
                }
            }

            player.activeItems.UndreadyAll(player.classFlags);


            if (DualClassExceedLastLevel(player) == true)
            {
                for (int class_index = 0; class_index <= 7; class_index++)
                {
                    byte skill_lvl = player.ClassLevelsOld[class_index];

                    if (class_index == 2 || class_index == 3)
                    {
                        if (skill_lvl > 6)
                        {
                            player.attacksCount = 3;
                        }
                    }
                    else if (class_index == 4)
                    {
                        if (skill_lvl > 7)
                        {
                            player.attacksCount = 3;
                        }
                    }

                    if (ovr018.thac0_table[class_index, skill_lvl] > player.thac0)
                    {
                        player.thac0 = ovr018.thac0_table[class_index, skill_lvl];
                    }
                }

                if (player.fighter_old_lvl > 6 ||
                    player.ranger_old_lvl > 7 ||
                    player.paladin_old_lvl > 6)
                {
                    player.attacksCount = 3;
                }

                if (player.thief_old_lvl > 0)
                {
                    recalc_thief_skills(player);
                }
            }
        }


        internal static void calc_cleric_spells(bool ResetSpellLevels, Player player) /* sub_6A686 */
        {
            int clericLvl = player.SkillLevel(SkillType.Cleric);

            if (clericLvl > 0)
            {
                if (ResetSpellLevels == true)
                {
                    for (int sp_lvl = 1; sp_lvl < 5; sp_lvl++)
                    {
                        player.spellCastCount[0, sp_lvl] = 0;
                    }

                    player.spellCastCount[0, 0] = 1;

                    for (int playerLvl = 0; playerLvl <= (clericLvl - 2); playerLvl++)
                    {
                        for (int spellLvl = 0; spellLvl < 5; spellLvl++)
                        {
                            player.spellCastCount[0, spellLvl] += ClericSpellLevels[playerLvl, spellLvl];
                        }
                    }
                }

                if (player.stats2.Wis.full > 12 && player.spellCastCount[0, 0] > 0)
                {
                    player.spellCastCount[0, 0] += 1;
                }

                if (player.stats2.Wis.full > 13 && player.spellCastCount[0, 0] > 0)
                {
                    player.spellCastCount[0, 0] += 1;
                }

                if (player.stats2.Wis.full > 14 && player.spellCastCount[0, 1] > 0)
                {
                    player.spellCastCount[0, 1] += 1;
                }

                if (player.stats2.Wis.full > 15 && player.spellCastCount[0, 1] > 0)
                {
                    player.spellCastCount[0, 1] += 1;
                }

                if (player.stats2.Wis.full > 16 && player.spellCastCount[0, 2] > 0)
                {
                    player.spellCastCount[0, 2] += 1;
                }

                if (player.stats2.Wis.full > 17 && player.spellCastCount[0, 3] > 0)
                {
                    player.spellCastCount[0, 3] += 1;
                }
            }
        }

        static byte[, ,] SaveThrowValues = { // [8,13,5] class, level, save_type      
			{{20, 20, 20, 20, 20}, {10, 13, 14, 16, 15}, {10, 13, 14, 16, 15}, {10, 13, 14, 16, 15}, {9, 12, 13, 15, 14}, {9, 12, 13, 15, 14}, {9, 12, 13, 15, 14}, {7, 10, 11, 13, 12}, {7, 10, 11, 13, 12}, {7, 10, 11, 13, 12}, {6, 9, 10, 12, 11}, {6, 9, 10, 12, 11}, {6, 9, 10, 12, 11}},
			{{20, 20, 20, 20, 20}, {10, 13, 14, 16, 15}, {10, 13, 14, 16, 15}, {10, 13, 14, 16, 15}, {9, 12, 13, 15, 14}, {9, 12, 13, 15, 14}, {9, 12, 13, 15, 14}, {7, 10, 11, 13, 12}, {7, 10, 11, 13, 12}, {7, 10, 11, 13, 12}, {6, 9, 10, 12, 11}, {6, 9, 10, 12, 11}, {6, 9, 10, 12, 11}},
			{{20, 20, 20, 20, 20}, {14, 15, 16, 17, 17}, {14, 15, 16, 17, 17}, {13, 14, 15, 16, 16}, {13, 14, 15, 16, 16}, {11, 12, 13, 13, 14}, {11, 12, 13,13, 14}, {10, 11, 12, 12, 13}, {10, 11, 12, 12, 13}, {8, 9, 10, 9, 11}, {8, 9, 10, 9, 11}, {7, 8, 9, 8, 10}, {7, 8, 9, 8, 10}},                      
			{{20, 20, 20, 20, 20}, {12, 13, 14, 15, 15}, {12, 13, 14, 15, 15}, {11, 12, 13, 14, 14}, {11, 12, 13, 14, 14}, {9, 9, 11, 11, 12}, {9, 9, 11, 11, 12}, {8, 9, 10, 10, 11}, {9, 9, 10, 10, 11}, {6, 7, 8, 7, 9}, {6, 7, 8, 7, 9}, {5, 6, 7, 6, 8}, {5, 6, 7, 6, 8}},
			{{20, 20, 20, 20, 20}, {14, 15, 16, 17, 17}, {14, 15, 16, 17, 17}, {13, 14, 15, 16, 16}, {13, 14, 15, 16, 16}, {11, 12, 13, 13, 14}, {11, 12, 13, 13, 14}, {10, 11, 12, 12, 13}, {10, 11, 12, 12, 13}, {8, 9, 10, 9, 11}, {8, 9, 10, 9, 11}, {7, 8, 9, 8, 10}, {7, 8, 9, 8, 10}},
			{{20, 20, 20, 20, 20}, {14, 13, 11, 15, 12}, {14, 13, 11, 15, 12}, {14, 13, 11, 15, 12}, {14, 13, 11, 15, 12}, {14, 13, 11, 15, 12}, {13, 11, 9, 13, 10}, {13, 11, 9, 13, 10}, {13, 11, 9, 13, 10}, {13, 11, 9, 13, 10}, {13, 11, 9, 13, 10}, {11, 9, 7, 11, 8}, {11, 9, 7, 11, 8}}, 
			{{20, 20, 20, 20, 20}, {13, 12, 14, 16, 15}, {13, 12, 14, 16, 15}, {13, 12, 14, 16, 15}, {13, 12, 14, 16, 15}, {12, 11, 12, 15, 13}, {12, 11, 12, 15, 13}, {12, 11, 12, 15, 13}, {12, 11, 12, 15, 13}, {11, 10, 10, 14, 11}, {11, 10, 10, 14, 11}, {11, 10, 10, 14, 11}, {11, 10, 10, 14, 11}},
			{{20, 20, 20, 20, 20}, {13, 12, 14, 16, 15}, {13, 12, 14, 16, 15}, {13, 12, 14, 16, 15}, {13, 12, 14, 16, 15}, {12, 11, 12, 15, 13}, {12, 11, 12, 15, 13}, {12, 11, 12, 15, 13}, {12, 11, 12, 15, 13}, {11, 10, 10, 14, 11}, {13, 11, 9, 13, 10}, {11, 9, 7, 11, 8}, {11, 9, 7, 11, 8}}};


        internal static void reclac_saving_throws(Player player) // sub_6A7FB
        {
            Item item = player.items.Find(i => i.affect_3 == Affects.item_affect_6 && i.readied);
            bool applyBonus = item != null;

            for (int save = 0; save < 5; save++)
            {
                int _class;

                player.saveVerse[save] = 20;
                for (_class = 0; _class <= 7; _class++)
                {
                    if (player.ClassLevel[_class] > 0)
                    {
                        byte dl = SaveThrowValues[_class, player.ClassLevel[_class], save];

                        if (player.saveVerse[save] > dl)
                        {
                            player.saveVerse[save] = dl;
                        }
                    }
                }

                _class = 7; /* Orignal code had a post use test and would exit on 7,
                        * but this for loops uses are pre-test increment so fix var_2 */

                if (player.ClassLevel[_class] > player.ClassLevelsOld[_class])
                {
                    byte dl = SaveThrowValues[_class, player.ClassLevelsOld[_class], save];

                    if (player.saveVerse[save] > dl)
                    {
                        player.saveVerse[save] = dl;
                    }
                }

                if (save == 0)
                {
                    SaveVersePoisonBonus(player, applyBonus);
                }
            }
        }

        private static void SaveVersePoisonBonus(Player player, bool applyBonus)
        {
			int poison = (int)SaveVerseType.Poison;

            if (player.race == Race.dwarf ||
                player.race == Race.halfling ||
                applyBonus == true)
            {
                if (player.stats2.Con.full >= 4 && player.stats2.Con.full <= 6)
                {
                    player.saveVerse[poison] += 1;
                }
                else if (player.stats2.Con.full >= 7 && player.stats2.Con.full <= 10)
                {
                    player.saveVerse[poison] += 2;
                }
                else if (player.stats2.Con.full >= 11 && player.stats2.Con.full <= 13)
                {
                    player.saveVerse[poison] += 3;
                }
                else if (player.stats2.Con.full >= 14 && player.stats2.Con.full <= 17)
                {
                    player.saveVerse[poison] += 4;
                }
                else if (player.stats2.Con.full == 18)
                {
                    player.saveVerse[poison] += 5;
                }
            }

            if (player.stats2.Con.full == 19 || player.stats2.Con.full == 20)
            {
                player.saveVerse[poison] += 1;
            }
            else if (player.stats2.Con.full == 21 || player.stats2.Con.full == 22)
            {
                player.saveVerse[poison] += 2;
            }
            else if (player.stats2.Con.full == 23 || player.stats2.Con.full == 24)
            {
                player.saveVerse[poison] += 3;
            }
            else if (player.stats2.Con.full == 25)
            {
                player.saveVerse[poison] += 4;
            }
        }


        /* PickPockets, OpenLocks, FindRemoveTraps, MoveSilently, HideInShadows, HearNoise, ClimbWalls, ReadLanguage */
        static sbyte[,] /*seg600:3F20 unk_1A230 */ race_adj = {
            { 0,  0,  0,  0,  0,  0,   0,   0 }, // monster
            { 0, 10, 15,  0,  0,  0, -10,  -5 }, // dwarf
            { 5, -5,  0,  5, 10,  5,   0,   0 }, // elf
            { 0,  5, 10,  5,  5, 10, -15,   0 }, // gnome
            { 10, 0,  0,  0,  5,  0,   0,   0 }, // half-elf
            { 5,  5,  5, 10, 15,  5, -15,  -5 }, // halfling
            { 5,  5,  5,  0,  0,  5,   5, -10 }, // half-orc
            { 0,  0,  0,  0,  0,  0,   0,   0 }, // human
        };

        /* PickPockets, OpenLocks, FindRemoveTraps, MoveSilently, HideInShadows */
        static sbyte[,] /*seg600:3F33 unk_1A243 */ dex_adj = {
            { -15, -10,  -10, -20, -10 }, // 0
            { -15, -10,  -10, -20, -10 }, // 1
            { -15, -10,  -10, -20, -10 }, // 2
            { -15, -10,  -10, -20, -10 }, // 3
            { -15, -10,  -10, -20, -10 }, // 4
            { -15, -10,  -10, -20, -10 }, // 5
            { -15, -10,  -10, -20, -10 }, // 6
            { -15, -10,  -10, -20, -10 }, // 7
            { -15, -10,  -10, -20, -10 }, // 8
            { -15, -10,  -10, -20, -10 }, // 9
            { -10,  -5,  -10, -15,  -5 }, // 10
            {  -5,   0,   -5, -10,   0 }, // 11
            {   0,   0,    0,  -5,   0 }, // 12
            {   0,   0,    0,   0,   0 }, // 13
            {   0,   0,    0,   0,   0 }, // 14
            {   0,   0,    0,   0,   0 }, // 15
            {   0,   5,    0,   0,   0 }, // 16
            {   5,  10,    0,   5,   5 }, // 17
            {  10,  15,    5,  10,  10 }, // 18
            {  15,  20,   10,  12,  12 }, // 19
            {  20,  25,   15,  15,  15 }, // 20
            {  25,  30,   20,  18,  18 }, // 21
            {  30,  35,   25,  20,  20 }, // 22
            {  35,  40,   30,  23,  23 }, // 23
            {  40,  45,   35,  25,  25 }, // 24
            {  45,  50,   40,  30,  30 }, // 25
        };

        /* PickPockets, OpenLocks, FindRemoveTraps, MoveSilently, HideInShadows, HearNoise, ClimbWalls, ReadLanguage */
        static byte[,] /*seg600:3EC0 unk_1A1D0 */ base_chance = {
            {   0,  0,  0,  0,  0, 0,  0,  0 },
            {  30, 25, 20, 15, 10, 10, 85, 0 },    // 1
            {  35, 29, 25, 21, 15, 10, 86, 0 },    // 2
            {  40, 33, 30, 27, 20, 15, 87, 0 },    // 3
            {  45, 37, 35, 33, 25, 15, 88, 20 },   // 4
            {  50, 42, 40, 40, 31, 20, 90, 25 },   // 5
            {  55, 47, 45, 47, 37, 20, 92, 30 },   // 6
            {  60, 52, 50, 55, 43, 25, 94, 35 },   // 7
            {  65, 57, 55, 62, 49, 25, 96, 40 },   // 8
            {  70, 62, 60, 70, 56, 30, 98, 45 },   // 9
            {  80, 67, 65, 78, 63, 30, 99, 50 },   // 10
            {  90, 72, 70, 86, 70, 35, 99, 55 },   // 11
            { 100, 77, 75, 94, 77, 35, 99, 60 },   // 12
        };



        internal static void recalc_thief_skills(Player player) // sub_6AAEA
        {
            var item_found = player.items.Find(item => item.readied && (item.CheckMaskedAffect(3, 2) || item.CheckMaskedAffect(3, 11)));
            var gloves_of_thievery = item_found != null && item_found.CheckMaskedAffect(3, 11); // gloves of thievery
            var gauntlets_of_dexterity = item_found != null && item_found.CheckMaskedAffect(3, 2); // gauntlets of dexterity

            int thiefLvl = player.SkillLevel(SkillType.Thief);

            if (thiefLvl < 4 && gauntlets_of_dexterity == true)
            {
                thiefLvl = 4;
                gauntlets_of_dexterity = false;
            }

            int orig_thief_lvl = thiefLvl;

            for (ThiefSkills skill = ThiefSkills.PickPockets; skill <= ThiefSkills.ReadLanguages; skill++)
            {
                int calc_skill = race_adj[(int)player.race, (int)skill];
                thiefLvl = orig_thief_lvl;

                if (gloves_of_thievery == true)
                {
                    switch (skill)
                    {
                        case ThiefSkills.PickPockets:
                            if (thiefLvl < 5)
                            {
                                thiefLvl = 5;
                            }
                            else
                            {
                                calc_skill += 5;
                            }
                            break;

                        case ThiefSkills.OpenLocks:
                            if (thiefLvl < 7)
                            {
                                thiefLvl = 7;
                            }
                            else
                            {
                                calc_skill += 5;
                            }
                            break;
                    }
                }

                if (gauntlets_of_dexterity == true)
                {
                    calc_skill += 10;
                }

                calc_skill += base_chance[thiefLvl, (int)skill];

                if (skill <= ThiefSkills.HideInShadows)
                {
                    calc_skill += dex_adj[(int)player.stats2.Dex.full, (int)skill];
                }

                if ( calc_skill > 0)
                {
                    player.thief_skills[(int)skill] = (byte)calc_skill;
                }
                else
                {
                    player.thief_skills[(int)skill] = 0;
                }
            }
        }


        internal static bool SecondClassAllowed(ClassId _class, Player player) /* sub_6AD3E */
        {
            var firstClass = HumanCurrentClass_Unknown(player);
            bool var_2 = _class != firstClass;
            
            int var_3 = 0;
            while (var_3 <= 5 &&
                (gbl.class_stats_min[(int)firstClass][var_3] < 9 || player.stats2[var_3].cur > 14))
            {
                var_3++;
            }

            var_2 = (var_2 == true && var_3 > 5);
            var_3 = 0;

            while (var_3 <= 5 &&
                (gbl.class_stats_min[(int)_class][var_3] < 9 || player.stats2[var_3].cur > 16))
            {
                var_3++;
            }

            var_2 = (var_2 == true && var_3 > 5);

            byte var_4 = 1;

            while (gbl.class_alignments[(int)_class, 0] >= var_4 &&
                gbl.class_alignments[(int)_class, var_4] != player.alignment)
            {
                var_4++;
            }

            if (var_2 == false ||
                gbl.class_alignments[(int)_class, 0] < var_4)
            {
                var_2 = false;
            }
            else
            {
                var_2 = true;
            }

            return var_2;
        }


        internal static void DuelClass(Player player)
        {
            List<MenuItem> list = new List<MenuItem>();

            list.Add(new MenuItem("Pick New Class", true));

            foreach (var _class in gbl.RaceClasses[(int)player.race])
            {
                if (SecondClassAllowed(_class, player) == true)
                {
                    list.Add(new MenuItem(ovr020.classString[(int)_class]));
                }
            }

            if (list.Count == 1)
            {
                seg041.DisplayStatusText(15, 4, player.name + " doesn't qualify.");
                list.Clear();
                return;
            }

            MenuItem list_ptr;
            int index = 1;
            bool show_exit = true;
            bool var_F = true;

            char input_key;

            do
            {
                input_key = ovr027.sl_select_item(out list_ptr, ref index, ref var_F, show_exit, list,
                    0x16, 0x26, 2, 1, gbl.defaultMenuColors, "Select", string.Empty);

                if (input_key == 0)
                {
                    return;
                }
            } while (input_key != 'S');

            player.exp = 0;
            player.attacksCount = 2;
            int newClass = 0;

            while (newClass <= 7 && ovr020.classString[newClass] != list_ptr.Text)
            {
                newClass++;
            }

            list.Clear();

            player.ClassLevelsOld[(int)HumanCurrentClass_Unknown(player)] = HumanCurrentClassLevel_Zero(player);

            player.multiclassLevel = player.HitDice;
            player.HitDice = 1;

            player.ClassLevel[(int)HumanCurrentClass_Unknown(player)] = 0;
            player.ClassLevel[newClass] = 1;

            for (int i = 0; i < 5; i++)
            {
                player.spellCastCount[0, i] = 0;
                player.spellCastCount[1, i] = 0;
                player.spellCastCount[2, i] = 0;
            }

            if (newClass == 0)
            {
                player.spellCastCount[0, 0] = 1;
            }
            else if (newClass == 5)
            {
                player.spellCastCount[2, 0] = 1;
                player.LearnSpell(Spells.detect_magic_MU);
                player.LearnSpell(Spells.read_magic);
                player.LearnSpell(Spells.sleep);
            }

            player._class = (ClassId)newClass;

            seg041.DisplayStatusText(0, 10, player.name + " is now a 1st level " + ovr020.classString[newClass] + ".");

            player.spellList.Clear();

            ReclacClassBonuses(player);
            calc_cleric_spells(true, player);
            reclac_saving_throws(player);
            recalc_thief_skills(player);

            foreach (var item in player.items)
            {
                if ((gbl.ItemDataTable[item.type].classFlags & player.classFlags) == 0 &&
                    item.cursed == false)
                {
                    item.readied = false;
                }
            }
        }


        static ClassId HumanCurrentClass_Unknown(Player player) // getFirstSkill
        {
            if (player.race != Race.human)
            {
                return ClassId.unknown;
            }

            for (ClassId index = ClassId.cleric; index <= ClassId.monk; index++)
            {
                if (player.ClassLevel[(int)index] > 0)
                {
                    return index;
                }
            }

            return ClassId.unknown;
        }

        internal static byte HumanCurrentClassLevel_Zero(Player player) /* hasAnySkills */
        {
            if (player.race != Race.human)
            {
                return 0;
            }

            int loop_var = 0;

            while (loop_var < 7 &&
                player.ClassLevel[loop_var] == 0)
            {
                loop_var++;
            }

            return player.ClassLevel[loop_var];
        }


        static bool DualClassExceedLastLevel(Player player) // sub_6B3D1
        {
            return HumanCurrentClassLevel_Zero(player) > player.multiclassLevel;
        }
    }
}
