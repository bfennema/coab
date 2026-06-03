using Avalonia.Controls.Shapes;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public static class Limits
    {
        public static Dictionary<Race, int[]> RaceAgeBrackets = new Dictionary<Race, int[]>() //unk_1A434
        {
            [Race.monster]        = [ 9999, 9999, 9999, 9999, 9999, 9999],
            [Race.mountain_dwarf] = [   34,   50,  150,  250,  350,  450],
            [Race.hill_dwarf]     = [   34,   50,  150,  250,  350,  450],
            [Race.dwarf]          = [   34,   50,  150,  250,  350,  450],
            [Race.silvanesti_elf] = [   99,  175,  550,  875, 1200, 1600],
            [Race.qualinesti_elf] = [   99,  175,  550,  875, 1200, 1600],
            [Race.elf]            = [   99,  175,  550,  875, 1200, 1600],
            [Race.gnome]          = [   49,   90,  300,  450,  600,  750],
            [Race.half_elf]       = [   23,   40,  100,  175,  250,  325],
            [Race.halfling]       = [   21,   33,   68,  101,  144,  199],
            [Race.kender]         = [   21,   33,   68,  101,  144,  199],
            [Race.half_orc]       = [   11,   15,   30,   45,   60,   80],
            [Race.human]          = [   13,   20,   40,   60,   90,  120],
        };

        public static int[] StrAgeEffect = { 0, 1, -1, -2, -1 };
        public static int[] Str00AgeEffect = { 0, 0, 0, 0, 0 };
        public static int[] IntAgeEffect = { 0, 0, 1, 0, 1 };
        public static int[] WisAgeEffect = { -1, 1, 1, 1, 1 };
        public static int[] DexAgeEffect = { 0, 0, 0, -2, -1 };
        public static int[] ConAgeEffect = { 1, 0, -1, -1, -1 };
        public static int[] ChaAgeEffect = { 0, 0, 0, 0, 0 };


        public static Dictionary<Race, int[,]> StrRaceSexMinMax = new Dictionary<Race, int[,]>()
        {
            [Race.monster]        = new[,] { { 0, 5 }, { 10,  0 } },
            [Race.mountain_dwarf] = new[,] { { 8, 8 }, { 18, 17 } },
            [Race.hill_dwarf]     = new[,] { { 9, 9 }, { 18, 17 } },
            [Race.dwarf]          = new[,] { { 8, 8 }, { 18, 17 } },
            [Race.silvanesti_elf] = new[,] { { 3, 3 }, { 18, 16 } },
            [Race.qualinesti_elf] = new[,] { { 7, 7 }, { 18, 16 } },
            [Race.elf]            = new[,] { { 3, 3 }, { 18, 16 } },
            [Race.gnome]          = new[,] { { 6, 6 }, { 18, 15 } },
            [Race.half_elf]       = new[,] { { 3, 3 }, { 18, 17 } },
            [Race.halfling]       = new[,] { { 6, 6 }, { 17, 14 } },
            [Race.kender]         = new[,] { { 6, 6 }, { 16, 16 } },
            [Race.half_orc]       = new[,] { { 6, 6 }, { 18, 18 } },
            [Race.human]          = new[,] { { 3, 3 }, { 18, 18 } },
        };

        public static Dictionary<Race, int[,]> Str00RaceSexMinMax = new Dictionary<Race, int[,]>()
        {
            [Race.monster]        = new[,] { { 0, 0}, {  5,  5} },
            [Race.mountain_dwarf] = new[,] { { 0, 0}, { 99,  0} },
            [Race.hill_dwarf]     = new[,] { { 0, 0}, { 99,  0} },
            [Race.dwarf]          = new[,] { { 0, 0}, { 99,  0} },
            [Race.silvanesti_elf] = new[,] { { 0, 0}, { 75,  0} },
            [Race.qualinesti_elf] = new[,] { { 0, 0}, { 75,  0} },
            [Race.elf]            = new[,] { { 0, 0}, { 75,  0} },
            [Race.gnome]          = new[,] { { 0, 0}, { 50,  0} },
            [Race.half_elf]       = new[,] { { 0, 0}, { 90,  0} },
            [Race.halfling]       = new[,] { { 0, 0}, {  0,  0} },
            [Race.kender]         = new[,] { { 0, 0}, {  0,  0} },
            [Race.half_orc]       = new[,] { { 0, 0}, { 99, 75} },
            [Race.human]          = new[,] { { 0, 0}, {100, 50} },
        };

        public static Dictionary<Race, int[,]> IntRaceSexMinMax = new Dictionary<Race, int[,]>()
        {
            [Race.monster]        = new [,] { {10, 10}, {15, 15} },
            [Race.mountain_dwarf] = new [,] { { 3,  3}, {18, 18} },
            [Race.hill_dwarf]     = new [,] { { 3,  3}, {18, 18} },
            [Race.dwarf]          = new [,] { { 3,  3}, {18, 18} },
            [Race.silvanesti_elf] = new [,] { {10, 10}, {18, 18} },
            [Race.qualinesti_elf] = new [,] { { 8,  8}, {18, 18} },
            [Race.elf]            = new [,] { { 8,  8}, {18, 18} },
            [Race.gnome]          = new [,] { { 7,  7}, {18, 18} },
            [Race.half_elf]       = new [,] { { 4,  4}, {18, 18} },
            [Race.halfling]       = new [,] { { 6,  6}, {18, 18} },
            [Race.kender]         = new [,] { { 6,  6}, {18, 18} },
            [Race.half_orc]       = new [,] { { 3,  3}, {17, 17} },
            [Race.human]          = new [,] { { 3,  3}, {18, 18} },
        };

        public static Dictionary<Race, int[,]> WisRaceSexMinMax = new Dictionary<Race, int[,]>()
        {
            [Race.monster]        = new [,] { {5, 5}, {10, 10} },
            [Race.mountain_dwarf] = new [,] { {3, 3}, {18, 18} },
            [Race.hill_dwarf]     = new [,] { {3, 3}, {18, 18} },
            [Race.dwarf]          = new [,] { {3, 3}, {18, 18} },
            [Race.silvanesti_elf] = new [,] { {6, 6}, {18, 18} },
            [Race.qualinesti_elf] = new [,] { {6, 6}, {18, 18} },
            [Race.elf]            = new [,] { {3, 3}, {18, 18} },
            [Race.gnome]          = new [,] { {3, 3}, {18, 18} },
            [Race.half_elf]       = new [,] { {3, 3}, {18, 18} },
            [Race.halfling]       = new [,] { {3, 3}, {17, 17} },
            [Race.kender]         = new [,] { {3, 3}, {16, 16} },
            [Race.half_orc]       = new [,] { {3, 3}, {14, 14} },
            [Race.human]          = new [,] { {3, 3}, {18, 18} },
        };

        public static Dictionary<Race, int[,]> DexRaceSexMinMax = new Dictionary<Race, int[,]>()
        {
            [Race.monster]        = new [,] { {10, 10}, {15, 15} },
            [Race.mountain_dwarf] = new [,] { { 3,  3}, {17, 17} },
            [Race.hill_dwarf]     = new [,] { { 3,  3}, {17, 17} },
            [Race.dwarf]          = new [,] { { 3,  3}, {17, 17} },
            [Race.silvanesti_elf] = new [,] { { 7,  7}, {19, 19} },
            [Race.qualinesti_elf] = new [,] { { 7,  7}, {19, 19} },
            [Race.elf]            = new [,] { { 7,  7}, {19, 19} },
            [Race.gnome]          = new [,] { { 3,  3}, {18, 18} },
            [Race.half_elf]       = new [,] { { 6,  6}, {18, 18} },
            [Race.halfling]       = new [,] { { 8,  8}, {19, 19} },
            [Race.kender]         = new [,] { { 8,  8}, {19, 19} },
            [Race.half_orc]       = new [,] { { 3,  3}, {17, 17} },
            [Race.human]          = new [,] { { 3,  3}, {18, 18} },
        };

        public static Dictionary<Race, int[,]> ConRaceSexMinMax = new Dictionary<Race, int[,]>
        {
            [Race.monster]        = new [,] { {20, 20}, {10, 10} },
            [Race.mountain_dwarf] = new [,] { {12, 12}, {19, 19} },
            [Race.hill_dwarf]     = new [,] { {14, 14}, {19, 19} },
            [Race.dwarf]          = new [,] { {12, 12}, {19, 19} },
            [Race.silvanesti_elf] = new [,] { { 6,  6}, {18, 18} },
            [Race.qualinesti_elf] = new [,] { { 7,  7}, {18, 18} },
            [Race.elf]            = new [,] { { 6,  6}, {18, 18} },
            [Race.gnome]          = new [,] { { 8,  8}, {18, 18} },
            [Race.half_elf]       = new [,] { { 6,  6}, {18, 18} },
            [Race.halfling]       = new [,] { {10, 10}, {18, 18} },
            [Race.kender]         = new [,] { {10, 10}, {18, 18} },
            [Race.half_orc]       = new [,] { {13, 13}, {19, 19} },
            [Race.human]          = new [,] { { 3,  3}, {18, 18} },
        };

        public static Dictionary<Race, int[,]> ChaRaceSexMinMax = new Dictionary<Race, int[,]>()
        {
            [Race.monster]        = new [,] { {12, 12}, {12, 12} },
            [Race.mountain_dwarf] = new [,] { { 3,  3}, {16, 16} },
            [Race.hill_dwarf]     = new [,] { { 3,  3}, {12, 12} },
            [Race.dwarf]          = new [,] { { 3,  3}, {16, 16} },
            [Race.silvanesti_elf] = new [,] { {12, 12}, {18, 18} },
            [Race.qualinesti_elf] = new [,] { { 8,  8}, {18, 18} },
            [Race.elf]            = new [,] { { 8,  8}, {18, 18} },
            [Race.gnome]          = new [,] { { 3,  3}, {18, 18} },
            [Race.half_elf]       = new [,] { { 3,  3}, {18, 18} },
            [Race.halfling]       = new [,] { { 3,  3}, {18, 18} },
            [Race.kender]         = new [,] { { 6,  6}, {18, 18} },
            [Race.half_orc]       = new [,] { { 3,  3}, {12, 12} },
            [Race.human]          = new [,] { { 3,  3}, {18, 18} },
        };

        public static Dictionary<ClassId, int> StrClassMin = new Dictionary<ClassId, int>()
        {
            [ClassId.cleric] = 6, [ClassId.druid] = 6, [ClassId.fighter] = 9, [ClassId.paladin] = 12,
            [ClassId.knight] = 10, [ClassId.ranger] = 13, [ClassId.thief] = 6, [ClassId.monk] = 15,
            [ClassId.mc_c_f] = 9, [ClassId.mc_c_f_m] = 9, [ClassId.mc_c_r] = 13, [ClassId.mc_c_mu] = 6,
            [ClassId.mc_c_t] = 6, [ClassId.mc_f_mu] = 9, [ClassId.mc_f_t] = 9, [ClassId.mc_f_mu_t] = 9,
            [ClassId.mc_mu_t] = 6,
        };
        public static Dictionary<ClassId, int> Str00ClassMin = new Dictionary<ClassId, int>() { };
        public static Dictionary<ClassId, int> IntClassMin = new Dictionary<ClassId, int>()
        {
            { ClassId.cleric, 6 }, { ClassId.druid, 6 }, { ClassId.paladin, 9 }, { ClassId.knight, 7 },
            { ClassId.ranger, 13 }, { ClassId.magic_user, 9 }, { ClassId.thief, 6 }, { ClassId.monk, 6 },
            { ClassId.mc_c_f, 6 }, { ClassId.mc_c_f_m, 9 }, { ClassId.mc_c_r, 13 }, { ClassId.mc_c_mu, 9 },
            { ClassId.mc_c_t, 6 }, { ClassId.mc_f_mu, 9 }, { ClassId.mc_f_t, 6 }, { ClassId.mc_f_mu_t, 9 },
            { ClassId.mc_mu_t, 9 }
        };
        public static Dictionary<ClassId, int> WisClassMin = new Dictionary<ClassId, int>()
        {
            { ClassId.cleric, 9 }, { ClassId.druid, 12 }, { ClassId.fighter, 6 }, { ClassId.paladin, 13 },
            { ClassId.knight, 10 }, { ClassId.ranger, 14 }, { ClassId.magic_user, 6 }, { ClassId.monk, 15 },
            { ClassId.mc_c_f, 9 }, { ClassId.mc_c_f_m, 9 }, { ClassId.mc_c_r, 14 }, { ClassId.mc_c_mu, 9 },
            { ClassId.mc_c_t, 9 }, { ClassId.mc_f_mu, 6 }, { ClassId.mc_f_t, 6 }, { ClassId.mc_f_mu_t, 6 },
            { ClassId.mc_mu_t, 6 }
        };
        public static Dictionary<ClassId, int> DexClassMin = new Dictionary<ClassId, int>()
        {
            { ClassId.druid, 6 }, { ClassId.fighter, 6 }, { ClassId.paladin, 6 }, { ClassId.knight, 8 },
            { ClassId.ranger, 6 }, { ClassId.magic_user, 6 }, { ClassId.thief, 9 },{ ClassId.monk, 15 },
            { ClassId.mc_c_f, 6 }, { ClassId.mc_c_f_m, 6 }, { ClassId.mc_c_r, 6 }, { ClassId.mc_c_mu, 6 },
            { ClassId.mc_c_t, 9 }, { ClassId.mc_f_mu, 6 }, { ClassId.mc_f_t, 9 }, { ClassId.mc_f_mu_t, 9 },
            { ClassId.mc_mu_t, 9 }
        };
        public static Dictionary<ClassId, int> ConClassMin = new Dictionary<ClassId, int>()
        {
            { ClassId.cleric, 6 }, { ClassId.druid, 6 }, { ClassId.fighter, 7 }, { ClassId.paladin, 9 },
            { ClassId.knight, 10 }, { ClassId.ranger, 14 }, { ClassId.magic_user, 6 }, { ClassId.thief, 6 },
            { ClassId.monk, 11 }, { ClassId.mc_c_f, 7 }, { ClassId.mc_c_f_m, 7 }, { ClassId.mc_c_r, 14 },
            { ClassId.mc_c_mu, 6 }, { ClassId.mc_c_t, 6 }, { ClassId.mc_f_mu, 7 }, { ClassId.mc_f_t, 7 },
            { ClassId.mc_f_mu_t, 7 }, { ClassId.mc_mu_t, 6 }
        };
        public static Dictionary<ClassId, int> ChaClassMin = new Dictionary<ClassId, int>()
        {
            { ClassId.cleric, 6 }, { ClassId.druid, 15 }, { ClassId.fighter, 6 }, { ClassId.paladin, 17 },
            { ClassId.knight, 6 }, { ClassId.ranger, 6 }, { ClassId.magic_user, 6 }, { ClassId.thief, 6 },
            { ClassId.monk, 6 }, { ClassId.mc_c_f, 6 }, { ClassId.mc_c_f_m, 6 }, { ClassId.mc_c_r, 6 },
            { ClassId.mc_c_mu, 6 }, { ClassId.mc_c_t, 6 }, { ClassId.mc_f_mu, 6 }, { ClassId.mc_f_t, 6 },
            { ClassId.mc_f_mu_t, 6 }, { ClassId.mc_mu_t, 6 }
        };

        public static readonly Dictionary<ClassId, byte[]> ClassAlignments = new Dictionary<ClassId, byte[]>() // unk_1A4EA
        {
            [ClassId.cleric]  = [0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.druid]   = [1, 3, 4, 5, 7],
            [ClassId.fighter] = [0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.paladin] = [0],
            [ClassId.knight] = [0],
            [ClassId.ranger]  = [0, 3, 6],
            [ClassId.magic_user] = [0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.thief] = [1, 2, 3, 4, 5, 7, 8],
            [ClassId.monk] = [ 0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.mc_c_f] = [0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.mc_c_f_m] = [0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.mc_c_r] = [0, 3, 6],
            [ClassId.mc_c_mu] = [0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.mc_c_t] = [1, 2, 3, 4, 5, 7, 8],
            [ClassId.mc_f_mu] = [0, 1, 2, 3, 4, 5, 6, 7, 8],
            [ClassId.mc_f_t] = [1, 2, 3, 4, 5, 7, 8],
            [ClassId.mc_f_mu_t] = [1, 2, 3, 4, 5, 7, 8],
            [ClassId.mc_mu_t] = [1, 2, 3, 4, 5, 7, 8]
        };

        public static bool RaceClassLimit(int class_lvl, Player player, SkillType skill)
        {
            bool race_limited = false;

            switch (player.race)
            {
                case Race.dwarf:
                    if (skill == SkillType.Fighter)
                    {
                        if (class_lvl == 9 ||
                            (class_lvl == 8 && player.stats.Str.full == 17) ||
                            (class_lvl == 7 && player.stats.Str.full < 17))
                        {
                            race_limited = true;
                        }
                    }
                    break;

                case Race.mountain_dwarf:
                    if (skill == SkillType.Paladin && class_lvl == 8)
                    {
                        race_limited = true;
                    }
                    if (skill == SkillType.Thief && class_lvl == 8)
                    {
                        race_limited = true;
                    }
                    if (skill == SkillType.Cleric && class_lvl == 10)
                    {
                        race_limited = true;
                    }

                    break;

                case Race.hill_dwarf:
                    if (skill == SkillType.Ranger && class_lvl == 8)
                    {
                        race_limited = true;
                    }
                    if (skill == SkillType.Thief && class_lvl == 10)
                    {
                        race_limited = true;
                    }
                    if (skill == SkillType.Cleric && class_lvl == 10)
                    {
                        race_limited = true;
                    }

                    break;

                case Race.elf:
                    if (skill == SkillType.Fighter)
                    {
                        if (class_lvl == 7 ||
                            (class_lvl == 6 && player.stats.Str.full == 17) ||
                            (class_lvl == 5 && player.stats.Str.full < 17))
                        {
                            race_limited = true;
                        }
                    }

                    if (skill == SkillType.MagicUser)
                    {
                        if (class_lvl == 11 ||
                            (class_lvl == 9 && player.stats.Int.full < 17) ||
                            (class_lvl == 10 && player.stats.Int.full == 17))
                        {
                            race_limited = true;
                        }
                    }
                    break;

                case Race.qualinesti_elf:
                    if (skill == SkillType.Fighter && class_lvl == 14)
                    {
                        race_limited = true;
                    }
                    break;

                case Race.silvanesti_elf:
                    if (skill == SkillType.Paladin && class_lvl == 12)
                    {
                        race_limited = true;
                    }
                    else if (skill == SkillType.Fighter && class_lvl == 10)
                    {
                        race_limited = true;
                    }
                    break;

                case Race.gnome:
                    if (skill == SkillType.Fighter)
                    {
                        if (class_lvl == 6 ||
                            (class_lvl == 5 && player.stats.Str.full < 18))
                        {
                            race_limited = true;
                        }
                    }
                    break;

                case Race.half_elf:
                    if (skill == SkillType.Cleric && class_lvl == 5)
                    {
                        race_limited = true;
                    }
                    else if (skill == SkillType.Knight && class_lvl == 10)
                    {
                        race_limited = true;
                    }
                    else
                    {
                        if (skill == SkillType.Fighter || skill == SkillType.Ranger)
                        {
                            if (class_lvl == 8 ||
                                (class_lvl == 7 && player.stats.Str.full == 17) ||
                                (class_lvl == 6 && player.stats.Str.full < 17))
                            {
                                race_limited = true;
                            }
                        }

                        if (skill == SkillType.MagicUser)
                        {
                            if (class_lvl == 8 ||
                                (class_lvl == 7 && player.stats.Int.full == 17) ||
                                (class_lvl == 6 && player.stats.Int.full < 17))
                            {
                                race_limited = true;
                            }
                        }
                    }
                    break;

                case Race.halfling:
                    if (skill == SkillType.Fighter)
                    {
                        if (class_lvl == 6 ||
                            (class_lvl == 5 && player.stats.Str.full == 17) ||
                            (class_lvl == 4 && player.stats.Str.full < 17))
                        {
                            race_limited = true;
                        }
                    }
                    break;

                case Race.kender:
                    if (skill == SkillType.Fighter || skill == SkillType.Ranger)
                    {
                        if (class_lvl == 7 ||
                            (class_lvl == 6 && player.stats.Str.full == 17) ||
                            (class_lvl == 5 && player.stats.Str.full < 17))
                        {
                            race_limited = true;
                        }
                    }
                    break;

            }

            if (Cheats.no_race_level_limits)
            {
                race_limited = false;
            }

            return race_limited;
        }

        public static bool RaceStatLevelRestricted(SkillType skill, Player player) // sub_69138
        {
            bool race_limited = false;

            int class_lvl = player.ClassLevel[(int)skill];

            if (class_lvl > 0)
            {
                race_limited = RaceClassLimit(class_lvl, player, skill);
            }

            if (Cheats.no_race_level_limits)
            {
                race_limited = false;
            }

            return race_limited;
        }
    }
}
