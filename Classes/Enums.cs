using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public enum Status
    {
        okey = 0x0,
        animated = 0x1,
        tempgone = 0x2,
        running = 0x3,
        unconscious = 0x4,
        dying = 0x5,
        dead = 0x6,
        stoned = 0x7,
        gone = 0x8
    }

    public enum Stat
    {
        STR, // 0
        INT, // 1
        WIS, // 2
        DEX, // 3
        CON, // 4
        CHA  // 5
    }

    public enum Race
    {
        monster = 0,
        dwarf = 1,
        mountain_dwarf = 2,
        hill_dwarf = 3,
        elf = 4,
        silvanesti_elf = 5,
        qualinesti_elf = 6,
        gnome = 7,
        half_elf = 8,
        halfling = 9,
        kender = 10,
        half_orc = 11,
        human = 12
    }

    public enum SkillType
    {
        Cleric = 0,
        Druid = 1,
        Fighter = 2,
        Knight = 3,
        Paladin = 4,
        Ranger = 5,
        MagicUser = 6,
        Thief = 7,
        Monk = 8,
    }

    public enum ClassId
    {
        cleric = SkillType.Cleric,
        druid = SkillType.Druid,
        fighter = SkillType.Fighter,
        knight = SkillType.Knight,
        paladin = SkillType.Paladin,
        ranger = SkillType.Ranger,
        magic_user = SkillType.MagicUser,
        thief = SkillType.Thief,
        monk = SkillType.Monk,
        mc_c_f = 9,
        mc_c_f_m = 10,
        mc_c_r = 11,
        mc_c_mu = 12,
        mc_c_t = 13,
        mc_f_mu = 14,
        mc_f_t = 15,
        mc_f_mu_t = 16,
        mc_mu_t = 17,
        unknown = 18,
    }

    public enum CombatTeam
    {
        Ours = 0,
        Enemy = 1
    }

    public enum ThiefSkills
    {
        PickPockets = 0,
        OpenLocks = 1,
        FindRemoveTraps = 2,
        MoveSilently = 3,
        HideInShadows = 4,
        HearNoise = 5,
        ClimbWalls = 6,
        ReadLanguages = 7,
    }

    [Flags]
    public enum Flags
    {
        EvilSummon          = 0x000001,
        Mammal              = 0x000002,
        DwarfPenalty        = 0x000004,
        RangerBonus         = 0x000008,
        Snake               = 0x000010,
        GnomePenalty        = 0x000020,
        Animal              = 0x000040,
        DwarfBonus          = 0x000080,
        Giant               = 0x000100,
        HeldCharmed         = 0x000200,
        Reptile             = 0x000400,
        ImmuneDeathMagic    = 0x000800,
        ImmunePoison        = 0x001000,
        ImmuneVorpal        = 0x002000,
        ImmuneConfusion     = 0x004000,
        Dragon              = 0x008000,
        Undead              = 0x010000,
        Avian               = 0x020000,
        Plant               = 0x040000,
        GnomeBonus          = 0x080000,
        Fire                = 0x100000,
        Cold                = 0x200000,
        Regenerate          = 0x400000,
    }
}
