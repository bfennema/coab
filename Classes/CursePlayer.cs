using System;

namespace Classes
{
    /// <summary>
    /// Summary description for Player.
    /// </summary>
    public class CursePlayer
    {
        enum MonsterType
        {
            humanoid = 1,
            giant = 2,
            dragon = 3,
            animated_dead = 4,
            genie = 7,
            fire = 8,
            cold = 9,
            troll = 10,
            reptile = 11,
            avian = 12,
            squid = 13,
            snake = 14,
            giant_bug = 16,
            magic_beast = 17,
            plant = 18,
            animal = 19,
        }

        [Flags]
        enum CurseFlags
        {
            EvilSummon = 0x01,
            DwarfPenalty = 0x02,
            GnomePenalty = 0x04,
            RangerBonus = 0x08,
        }

        [DataOffset(0x00, DataType.PString, 15)]
        string name; // 0x00 - 0x0E;

        [DataOffset(0x10, DataType.CustSaveLoad, 14)]
        public PlayerStats stats; // 0x10 - 0x1D;

        [DataOffset(0x1E, DataType.ByteArray, 84)]
        public byte[] memorizedSpells = new byte[84]; // 0x1E - 0x71;

        [DataOffset(0x72, DataType.Byte)]
        public byte spell_to_learn_count; // 0x72;
        [DataOffset(0x73, DataType.SByte)]
        public sbyte thac0; // 0x73;

        [DataOffset(0x74, DataType.Byte)]
        public byte race; // 0x74;

        [DataOffset(0x75, DataType.Byte)]
        public byte _class; // 0x75;
        [DataOffset(0x76, DataType.SWord)]
        public short age; // 0x76;

        [DataOffset(0x78, DataType.Byte)]
        public byte hit_point_max; // 0x78;

        [DataOffset(0x79, DataType.CustSaveLoad, 100)]
        public SpellBook spellBook; // 0x79 - 0xDC

        public byte attackLevel; // 0xDD;
        public byte icon_dimensions; // 0xDE;
        public byte[] saveVerse = new byte[5]; // 0xDF - 0xE3;

        public byte base_movement; // 0xE4;
        public byte HitDice; // 0xE5;
        public byte multiclassLevel; // 0xE6;
        public byte lost_lvls; // 0xE7;
        public byte lost_hp; // 0xE8;
        public byte level_undead; // 0xE9;
        public byte[] thief_skills = new byte[8]; // 0xEA - 0xF1; [] was 1 offset @ 0xe9, pick_pockets, open_locks, find_remove_traps, move_silently, hide_in_shadows, hear_noise, climb_walls, read_languages
        public byte[] affects = new byte[5]; // 0xF2 - 0xF5;

        [DataOffset(0xF6, DataType.Byte)]
        public byte field_F6; // 0xF6;
        [DataOffset(0xF7, DataType.Byte)]
        public byte control_morale; // 0xF7;
        [DataOffset(0xF8, DataType.Byte)]
        public byte npcTreasureShareCount; // 0xF8;
        [DataOffset(0xF9, DataType.Byte)]
        public byte field_F9; // 0xF9;
        [DataOffset(0xFA, DataType.Byte)]
        public byte field_FA; // 0xFA;

        [DataOffset(0xFB, DataType.ShortArray, 7)]
        public ushort[] money = new ushort[7]; // 0xFB - 0x108

        [DataOffset(0x109, DataType.ByteArray, 8)]
        public byte[] ClassLevel = new byte[8]; // 0x109 - 0x110

        [DataOffset(0x111, DataType.ByteArray, 8)]
        public byte[] ClassLevelsOld = new byte[8]; // 0x111 - 0x118

        [DataOffset(0x119, DataType.Byte)]
        public byte sex; // 0x119;
        [DataOffset(0x11A, DataType.IByte)]
        MonsterType monsterType; // 0x11A;
        [DataOffset(0x11B, DataType.Byte)]
        public byte alignment; // 0x11B;
        /// <summary>
        /// half-attacks count
        /// </summary>
        [DataOffset(0x11c, DataType.Byte)]
        public byte attacksCount; // 0x11C;
        [DataOffset(0x11d, DataType.Byte)]
        public byte baseHalfMoves; // 0x11D;
        [DataOffset(0x11e, DataType.Byte)]
        public byte attack1_DiceCountBase; // 0x11E;
        [DataOffset(0x11f, DataType.Byte)]
        public byte attack2_DiceCountBase; // 0x11F;
        [DataOffset(0x120, DataType.Byte)]
        public byte attack1_DiceSizeBase; // 0x120;
        [DataOffset(0x121, DataType.Byte)]
        public byte attack2_DiceSizeBase; // 0x121;
        [DataOffset(0x122, DataType.Byte)]
        public byte attack1_DamageBonusBase; // 0x122;
        [DataOffset(0x123, DataType.Byte)]
        public byte attack2_DamageBonusBase; // 0x123;
        [DataOffset(0x124, DataType.Byte)]
        public byte base_ac; // 0x124;
        [DataOffset(0x125, DataType.Byte)]
        public byte field_125; // 0x125;
        [DataOffset(0x126, DataType.Byte)]
        public byte mod_id; // 0x126;
        [DataOffset(0x127, DataType.Int)]
        public int exp; // 0x127
        [DataOffset(0x12B, DataType.Byte)]
        public byte classFlags; // 0x12B;
        [DataOffset(0x12C, DataType.Byte)]
        public byte hit_point_rolled; // 0x12C;

        [DataOffset(0x12D, DataType.ByteArray, 15)]
        public byte[] spellCastCount = new byte[15]; // 0x12D - 0x13B

        [DataOffset(0x13C, DataType.SWord)]
        public short field_13C; // 0x13C
        [DataOffset(0x13E, DataType.Byte)]
        public byte field_13E; // 0x13E;
        [DataOffset(0x13F, DataType.Byte)]
        public byte field_13F; // 0x13F;
        [DataOffset(0x140, DataType.Byte)]
        public byte field_140; // 0x140;
        [DataOffset(0x141, DataType.Byte)]
        public byte head_icon; // 0x141;
        [DataOffset(0x142, DataType.Byte)]
        public byte weapon_icon; // 0x142;
        [DataOffset(0x143, DataType.Byte)]
        public byte icon_id; // 0x143;
        [DataOffset(0x144, DataType.Byte)]
        public byte icon_size; // 0x144; field_144  1 small 2 normal
        [DataOffset(0x145, DataType.ByteArray, 6)]
        public byte[] icon_colours = new byte[6]; // 0x145 = field_144[1] // byte[6]
        [DataOffset(0x14B, DataType.IByte)]
        CurseFlags flags_1; // 0x14B;

        //[DataOffset(0x14C, DataType.Byte)]
        //public byte number_of_items;

        //[DataOffset(0x14D, DataType.WordArray, 14)]
        //public int[] items = new int[14];

        [DataOffset(0x185, DataType.Byte)]
        public byte weaponsHandsUsed; // 0x185;
        [DataOffset(0x186, DataType.SByte)]
        public sbyte field_186; // 0x186;
        [DataOffset(0x187, DataType.SWord)]
        public short weight; // 0x187;

        //[DataOffset(0x189, DataType.Int)]
        //public uint nextCharacter; // 0x189;
        //[DataOffset(0x18D, DataType.Int)]
        //public uint actions; // 0x18D;
        [DataOffset(0x191, DataType.Byte)]
        public byte paladinCuresLeft; // 0x191;
        [DataOffset(0x192, DataType.Byte)]
        public byte field_192; // 0x192;
        [DataOffset(0x193, DataType.Byte)]
        public byte field_193; // 0x193;
        [DataOffset(0x194, DataType.Byte)]
        public byte field_194; // 0x194;
        [DataOffset(0x195, DataType.Byte)]
        public byte health_status; // 0x195;
        [DataOffset(0x196, DataType.Bool)]
        public bool in_combat; // 0x196;
        [DataOffset(0x197, DataType.Byte)]
        public byte combat_team; // 0x197; 0 - our team, 1 - enemy
        [DataOffset(0x198, DataType.Byte)]
        public byte quick_fight; // 0x198;
        [DataOffset(0x199, DataType.Byte)]
        public byte hitBonus; // 0x199;
        [DataOffset(0x19A, DataType.Byte)]
        public byte ac; // 0x19A

        [DataOffset(0x19B, DataType.Byte)]
        public byte ac_behind; // 0x19B;

        [DataOffset(0x19C, DataType.Byte)]
        public byte attack1_AttacksLeft; // 0x19C;
        [DataOffset(0x19D, DataType.Byte)]
        public byte attack2_AttacksLeft; // 0x19D;

        [DataOffset(0x19E, DataType.Byte)]
        public byte attack1_DiceCount; // 0x19E
        [DataOffset(0x19F, DataType.Byte)]
        public byte attack2_DiceCount; // 0x19F

        [DataOffset(0x1A0, DataType.Byte)]
        public byte attack1_DiceSize; // 0x1A0;
        [DataOffset(0x1A1, DataType.Byte)]
        public byte attack2_DiceSize; // 0x1A1;

        [DataOffset(0x1a2, DataType.SByte)]
        public sbyte attack1_DamageBonus; // 0x1A2;
        [DataOffset(0x1a3, DataType.Byte)]
        public byte attack2_DamageBonus; // 0x1A3;

        [DataOffset(0x1A4, DataType.Byte)]
        public byte hit_point_current; // 0x1A4;

        [DataOffsetAttribute(0x1A5, DataType.Byte)]
        public byte movement; // 0x1A5;

        public const int StructSize = 0x1A6;


        public CursePlayer(byte[] data, int offset)
        {
            DataIO.ReadObject(this, data, offset);
        }

        public CursePlayer(Player player)
        {
            name = player.name;

            stats = player.stats2;

            player.spellList.Save(memorizedSpells, 0, memorizedSpells.Length);
            spell_to_learn_count = player.spell_to_learn_count;
            thac0 = player.thac0;

            race = (byte)player.race;

            _class = (byte)player._class;
            age = player.age;

            hit_point_max = player.hit_point_max;

            spellBook = player.spellBook;

            attackLevel = player.attackLevel;
            icon_dimensions = player.icon_dimensions;
            System.Array.Copy(player.saveVerse, saveVerse, 5);

            base_movement = player.base_movement;
            HitDice = player.HitDice;
            multiclassLevel = player.multiclassLevel;
            lost_lvls = player.lost_lvls;
            lost_hp = player.lost_hp;
            level_undead = player.level_undead;
            System.Array.Copy(player.thief_skills, thief_skills, 8);

            field_F6 = player.field_F6;
            control_morale = player.control_morale;
            npcTreasureShareCount = player.npcTreasureShareCount;
            field_F9 = player.field_F9;
            field_FA = player.field_FA;

            money[0] = (ushort)player.Money.GetCoins(Money.Copper);
            money[1] = (ushort)player.Money.GetCoins(Money.Silver);
            money[2] = (ushort)player.Money.GetCoins(Money.Electrum);
            money[3] = (ushort)player.Money.GetCoins(Money.Gold);
            money[4] = (ushort)player.Money.GetCoins(Money.Platinum);
            money[5] = (ushort)player.Money.GetCoins(Money.Gems);
            money[6] = (ushort)player.Money.GetCoins(Money.Jewelry); ;

            System.Array.Copy(player.ClassLevel, ClassLevel, 8);
            System.Array.Copy(player.ClassLevelsOld, ClassLevelsOld, 8);

            sex = player.sex;
            monsterType = 0;
            alignment = player.alignment;

            attacksCount = player.attacksCount;
            baseHalfMoves = player.baseHalfMoves;
            attack1_DiceCountBase = player.attack1_DiceCountBase;
            attack2_DiceCountBase = player.attack2_DiceCountBase;
            attack1_DiceSizeBase = player.attack1_DiceSizeBase;
            attack2_DiceSizeBase = player.attack2_DiceSizeBase;
            attack1_DamageBonusBase = player.attack1_DamageBonusBase;
            attack2_DamageBonusBase = player.attack2_DamageBonusBase;
            base_ac = player.base_ac;
            field_125 = player.field_125;
            mod_id = player.mod_id;
            exp = player.exp;
            classFlags = player.classFlags;
            hit_point_rolled = player.hit_point_rolled;

            for (int spell_class = 0; spell_class < 3; spell_class++)
            {
                for (int spell_level = 0; spell_level < 5; spell_level++)
                {
                    spellCastCount[spell_class * 5 + spell_level] = player.spellCastCount[spell_class][spell_level];
                }
            }

            field_13C = player.field_13C;
            field_13E = player.field_13E;
            field_13F = player.field_13F;
            field_140 = player.field_140;
            head_icon = player.head_icon;
            weapon_icon = player.weapon_icon;
            icon_id = player.icon_id;
            icon_size = player.icon_size;
            flags_1 = 0x00;
            System.Array.Copy(player.icon_colours, icon_colours, 6);
            if (player.flags.HasFlag(Flags.EvilSummon))
            {
                flags_1 |= CurseFlags.EvilSummon;
            }
            if (player.flags.HasFlag(Flags.DwarfPenalty))
            {
                flags_1 |= CurseFlags.DwarfPenalty;
            }
            if (player.flags.HasFlag(Flags.GnomePenalty))
            {
                flags_1 |= CurseFlags.GnomePenalty;
            }
            if (player.flags.HasFlag(Flags.RangerBonus))
            {
                flags_1 |= CurseFlags.RangerBonus;
            }

            weaponsHandsUsed = player.weaponsHandsUsed;
            field_186 = player.field_186;
            weight = player.weight;

            paladinCuresLeft = player.paladinCuresLeft;
            field_192 = player.field_192;
            field_193 = player.field_193;
            field_194 = player.field_194;
            health_status = (byte)player.health_status;
            in_combat = player.in_combat;
            combat_team = (byte)player.combat_team;
            quick_fight = (byte)player.quick_fight;
            hitBonus = (byte)player.hitBonus;
            ac = player.ac;

            ac_behind = player.ac_behind;

            attack1_AttacksLeft = player.attack1_AttacksLeft;
            attack2_AttacksLeft = player.attack2_AttacksLeft;

            attack1_DiceCount = player.attack1_DiceCount;
            attack2_DiceCount = player.attack2_DiceCount;

            attack1_DiceSize = player.attack1_DiceSize;
            attack2_DiceSize = player.attack2_DiceSize;

            attack1_DamageBonus = player.attack1_DamageBonus;
            attack2_DamageBonus = player.attack2_DamageBonus;

            hit_point_current = player.hit_point_current;

            movement = player.movement;
        }

        public Player Load()
        {
            Player player = new Player();

            player.name = name;

            player.stats2 = stats;

            player.spellList.Load(memorizedSpells, 0, memorizedSpells.Length);
            player.spell_to_learn_count = spell_to_learn_count;
            player.thac0 = thac0;

            player.race = (Race)race;

            player._class = (ClassId)_class;
            player.age = age;

            player.hit_point_max = hit_point_max;

            player.spellBook = spellBook;

            player.attackLevel = attackLevel;
            player.icon_dimensions = icon_dimensions;
            System.Array.Copy(saveVerse, player.saveVerse, 5);

            player.base_movement = base_movement;
            player.HitDice = HitDice;
            player.multiclassLevel = multiclassLevel;
            player.lost_lvls = lost_lvls;
            player.lost_hp = lost_hp;
            player.level_undead = level_undead;
            System.Array.Copy(thief_skills, player.thief_skills, 8);

            player.field_F6 = field_F6;
            player.control_morale = control_morale;
            player.npcTreasureShareCount = npcTreasureShareCount;
            player.field_F9 = field_F9;
            player.field_FA = field_FA;

            player.Money.SetCoins(Money.Copper, money[0]);
            player.Money.SetCoins(Money.Silver, money[1]);
            player.Money.SetCoins(Money.Electrum, money[2]);
            player.Money.SetCoins(Money.Gold, money[3]);
            player.Money.SetCoins(Money.Platinum, money[4]);
            player.Money.SetCoins(Money.Gems, money[5]);
            player.Money.SetCoins(Money.Jewelry, money[6]);

            System.Array.Copy(ClassLevel, player.ClassLevel, 8);
            System.Array.Copy(ClassLevelsOld, player.ClassLevelsOld, 8);

            player.sex = sex;
            player.alignment = alignment;

            player.attacksCount = attacksCount;
            player.baseHalfMoves = baseHalfMoves;
            player.attack1_DiceCountBase = attack1_DiceCountBase;
            player.attack2_DiceCountBase = attack2_DiceCountBase;
            player.attack1_DiceSizeBase = attack1_DiceSizeBase;
            player.attack2_DiceSizeBase = attack2_DiceSizeBase;
            player.attack1_DamageBonusBase = attack1_DamageBonusBase;
            player.attack2_DamageBonusBase = attack2_DamageBonusBase;
            player.base_ac = base_ac;
            player.field_125 = field_125;
            player.mod_id = mod_id;
            player.exp = exp;
            player.classFlags = classFlags;
            player.hit_point_rolled = hit_point_rolled;

            for (int spell_class = 0; spell_class < 3; spell_class++)
            {
                for (int spell_level = 0; spell_level < 5; spell_level++)
                {
                    player.spellCastCount[spell_class][spell_level] = spellCastCount[spell_class * 5 + spell_level];
                }
            }

            player.field_13C = field_13C;
            player.field_13E = field_13E;
            player.field_13F = field_13F;
            player.field_140 = field_140;
            player.head_icon = head_icon;
            player.weapon_icon = weapon_icon;
            player.icon_id = icon_id;
            player.icon_size = icon_size;
            System.Array.Copy(icon_colours, player.icon_colours, 6);
            player.flags = 0;
            if ((flags_1 & CurseFlags.EvilSummon) == CurseFlags.EvilSummon)
            {
                player.flags |= Flags.EvilSummon;
            }
            if ((flags_1 & CurseFlags.DwarfPenalty) == CurseFlags.DwarfPenalty)
            {
                player.flags |= Flags.DwarfPenalty;
            }
            if ((flags_1 & CurseFlags.GnomePenalty) == CurseFlags.GnomePenalty)
            {
                player.flags |= Flags.GnomePenalty;
            }
            if ((flags_1 & CurseFlags.RangerBonus) == CurseFlags.RangerBonus)
            {
                player.flags |= Flags.RangerBonus;
            }

            if (monsterType == 0)
            {
                if (name.Contains("HOBGOBLIN"))
                {
                    player.flags |= Flags.DwarfBonus;
                }
                if (icon_dimensions == 1)
                {
                    player.flags |= Flags.HeldCharmed;
                }
            }
            else if (monsterType == MonsterType.humanoid)
            {
                if (name.Contains("ORC"))
                {
                    player.flags |= Flags.DwarfBonus;
                }
                else if (name.Contains("GOBLIN"))
                {
                    player.flags |= Flags.DwarfBonus | Flags.GnomeBonus;
                }
                else if (name.Contains("KOBOLD"))
                {
                    player.flags |= Flags.GnomeBonus | Flags.Reptile;
                }
                if (name.Contains("GNOLL") || icon_dimensions == 1)
                {
                    player.flags |= Flags.HeldCharmed;
                }
            }
            else if (monsterType == MonsterType.giant)
            {
                player.flags |= Flags.Giant;
            }
            else if (monsterType == MonsterType.dragon)
            {
                player.flags |= Flags.Dragon;
            }
            else if (monsterType == MonsterType.animated_dead)
            {
                player.flags |= Flags.Undead;
            }
            else if (monsterType == MonsterType.fire)
            {
                player.flags |= Flags.Fire;
            }
            else if (monsterType == MonsterType.cold)
            {
                player.flags |= Flags.Cold;
            }
            else if (monsterType == MonsterType.troll)
            {
                player.flags |= Flags.DwarfPenalty | Flags.GnomePenalty | Flags.RangerBonus | Flags.Regenerate;
            }
            else if (monsterType == MonsterType.reptile)
            {
                player.flags |= Flags.Reptile;
                if (name.Contains("MAN"))
                {
                    player.flags |= Flags.HeldCharmed;
                }
                else
                {
                    player.flags |= Flags.Animal;
                }
            }
            else if (monsterType == MonsterType.avian)
            {
                player.flags |= Flags.Avian | Flags.Animal;
            }
            else if (monsterType == MonsterType.snake)
            {
                player.flags |= Flags.Snake | Flags.Animal;
            }
            else if (monsterType == MonsterType.animal)
            {
                player.flags |= Flags.Animal | Flags.Mammal;
            }

            player.weaponsHandsUsed = weaponsHandsUsed;
            player.field_186 = field_186;
            player.weight = weight;

            player.paladinCuresLeft = paladinCuresLeft;
            player.field_192 = field_192;
            player.field_193 = field_193;
            player.field_194 = field_194;
            player.health_status = (Status)health_status;
            player.in_combat = in_combat;
            player.combat_team = (CombatTeam)combat_team;
            player.quick_fight = (QuickFight)quick_fight;
            player.hitBonus = hitBonus;
            player.ac = ac;

            player.ac_behind = ac_behind;

            player.attack1_AttacksLeft = attack1_AttacksLeft;
            player.attack2_AttacksLeft = attack2_AttacksLeft;

            player.attack1_DiceCount = attack1_DiceCount;
            player.attack2_DiceCount = attack2_DiceCount;

            player.attack1_DiceSize = attack1_DiceSize;
            player.attack2_DiceSize = attack2_DiceSize;

            player.attack1_DamageBonus = attack1_DamageBonus;
            player.attack2_DamageBonus = attack2_DamageBonus;

            player.hit_point_current = hit_point_current;

            player.movement = movement;

            return player;
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }
    }
}
