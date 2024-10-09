namespace Classes.Champ
{
    /// <summary>
    /// Summary description for Player.
    /// </summary>
    public class Player
    {
        public enum Race
        {
            silvanesti_elf = 0,
            qualinesti_elf = 1,
            half_elf = 2,
            mountain_dwarf = 3,
            hill_dwarf = 4,
            kender = 5,
            human = 6,
            monster = 7,
        }
        public enum ClassId
        {
            cleric = 0,
            druid = 1,
            fighter = 2,
            paladin = 3,
            ranger = 4,
            magic_user = 5,
            thief = 6,
            knight = 7,
            mc_c_f = 8,
            mc_c_f_m = 9,
            mc_c_r = 10,
            mc_c_mu = 11,
            mc_c_t = 12,
            mc_f_mu = 13,
            mc_f_t = 14,
            mc_f_mu_t = 15,
            mc_mu_t = 16,
            unknown = 17,
        }
        [System.Flags]
        enum ChampFlags1
        {
            EvilSummon = 0x01,
            Undead = 0x02,
            DwarfPenalty = 0x04,
            RangerBonus = 0x08,
            Snake = 0x10,
            Reptile = 0x20,
            Animal = 0x40,
            DwarfBonus = 0x80,
        }
        [System.Flags]
        enum ChampFlags2
        {
            Dragon = 0x01,
        }

        [DataOffset(0x00, DataType.PString, 15)]
        public string name; // 0x00 - 0x0E;

        [DataOffset(0x10, DataType.ByteArray, 14)]
        public byte[] stats = new byte[14]; // 0x10 - 0x1D;

        [DataOffset(0x1E, DataType.ByteArray, 58)]
        public byte[] memorizedSpells = new byte[58]; // 0x1E - 0x57;

        [DataOffset(0x58, DataType.Byte)]
        public byte spell_to_learn_count; // 0x58;
        [DataOffset(0x59, DataType.SByte)]
        public sbyte thac0; // 0x59;

        [DataOffset(0x5A, DataType.Byte)]
        public byte race; // 0x5A;

        [DataOffset(0x5B, DataType.Byte)]
        public byte _class; // 0x5B;
        [DataOffset(0x5C, DataType.Byte)]
        public byte knight; // 0x5C
        [DataOffset(0x5D, DataType.Byte)]
        public byte god; // 0x5D
        [DataOffset(0x5E, DataType.Byte)]
        public byte robe; // 0x5E
        [DataOffset(0x60, DataType.SWord)]
        public short age; // 0x60;

        [DataOffset(0x62, DataType.Byte)]
        public byte hit_point_max; // 0x62;

        [DataOffset(0x63, DataType.ByteArray, 70)]
        public byte[] spellBook = new byte[70]; // 0x63 - 0xCD

        [DataOffset(0xCE, DataType.Byte)]
        public byte attackLevel; // 0xCE;
        [DataOffset(0xCF, DataType.Byte)]
        public byte icon_dimensions; // 0xCF;
        [DataOffset(0xD0, DataType.ByteArray, 5)]
        public byte[] saveVerse = new byte[5]; // 0xD0 - 0xD4;

        [DataOffset(0xD5, DataType.Byte)]
        public byte base_movement; // 0xD5;
        [DataOffset(0xD6, DataType.Byte)]
        public byte HitDice; // 0xD6;
        [DataOffset(0xD7, DataType.Byte)]
        public byte multiclassLevel; // 0xD7;
        [DataOffset(0xD8, DataType.Byte)]
        public byte lost_lvls; // 0xD8;
        [DataOffset(0xD9, DataType.Byte)]
        public byte lost_hp; // 0xD9;
        [DataOffset(0xDA, DataType.Byte)]
        public byte level_undead; // 0xDA;
        [DataOffset(0xDB, DataType.ByteArray, 8)]
        public byte[] thief_skills = new byte[8]; // 0xDB - 0xE2; [] was 1 offset @ 0xdb, pick_pockets, open_locks, find_remove_traps, move_silently, hide_in_shadows, hear_noise, climb_walls, read_languages
        [DataOffset(0xE3, DataType.ByteArray, 4)]
        public byte[] affects = new byte[4]; // 0xE3 - 0xE6;

        [DataOffset(0xE7, DataType.Byte)]
        public byte control_morale; // 0xE7;
        [DataOffset(0xE8, DataType.Byte)]
        public byte npcTreasureShareCount; // 0xE8;
        [DataOffset(0xE9, DataType.Byte)]
        public byte field_E9; // 0xE9;
        [DataOffset(0xEA, DataType.Byte)]
        public byte field_EA; // 0xEA;
        [DataOffset(0xEB, DataType.Byte)]
        public byte field_EB; // 0xEB;
        [DataOffset(0xEC, DataType.Byte)]
        public byte field_EC; // 0xEC;

        [DataOffset(0xED, DataType.ShortArray, 6)]
        public ushort[] money = new ushort[6]; // 0xED - 0xF8

        [DataOffset(0xF9, DataType.ByteArray, 8)]
        public byte[] ClassLevel = new byte[8]; // 0xF9 - 0x100

        [DataOffset(0x111, DataType.ByteArray, 8)]
        public byte[] ClassLevelsOld = new byte[8]; // 0x101 - 0x108

        [DataOffset(0x109, DataType.Byte)]
        public byte sex; // 0x109;
        [DataOffset(0x10A, DataType.Byte)]
        public byte alignment; // 0x10A;
        /// <summary>
        /// half-attacks count
        /// </summary>
        [DataOffset(0x10B, DataType.Byte)]
        public byte attacksCount; // 0x10B;
        [DataOffset(0x10C, DataType.Byte)]
        public byte baseHalfMoves; // 0x10C;
        [DataOffset(0x10D, DataType.Byte)]
        public byte attack1_DiceCountBase; // 0x10D;
        [DataOffset(0x10E, DataType.Byte)]
        public byte attack2_DiceCountBase; // 0x10E;
        [DataOffset(0x10F, DataType.Byte)]
        public byte attack1_DiceSizeBase; // 0x10F;
        [DataOffset(0x110, DataType.Byte)]
        public byte attack2_DiceSizeBase; // 0x110;
        [DataOffset(0x111, DataType.Byte)]
        public byte attack1_DamageBonusBase; // 0x111;
        [DataOffset(0x112, DataType.Byte)]
        public byte attack2_DamageBonusBase; // 0x112;
        [DataOffset(0x113, DataType.Byte)]
        public byte base_ac; // 0x113;
        [DataOffset(0x114, DataType.Byte)]
        public byte useStrBonus; // 0x114;
        [DataOffset(0x115, DataType.Byte)]
        public byte mod_id; // 0x115;
        [DataOffset(0x116, DataType.Int)]
        public int exp; // 0x116
        [DataOffset(0x11A, DataType.Byte)]
        public byte classFlags; // 0x11A;
        [DataOffset(0x11B, DataType.Byte)]
        public byte hit_point_rolled; // 0x11B;

        [DataOffset(0x11C, DataType.ByteArray, 20)]
        public byte[] spellCastCount = new byte[20]; // 0x11C - 0x12F

        [DataOffset(0x130, DataType.SWord)]
        public short field_130; // 0x130
        [DataOffset(0x132, DataType.Byte)]
        public byte field_132; // 0x132;
        [DataOffset(0x133, DataType.Byte)]
        public byte head_portrait; // 0x133;
        [DataOffset(0x134, DataType.Byte)]
        public byte body_portrait; // 0x134;
        [DataOffset(0x135, DataType.Byte)]
        public byte head_icon; // 0x135;
        [DataOffset(0x136, DataType.Byte)]
        public byte weapon_icon; // 0x136;
        [DataOffset(0x137, DataType.Byte)]
        public byte icon_id; // 0x137;
        [DataOffset(0x138, DataType.Byte)]
        public byte icon_size; // 0x138; field_144  1 small 2 normal
        [DataOffset(0x139, DataType.ByteArray, 6)]
        public byte[] icon_colours = new byte[6]; // 0x139 = field_144[1] // byte[6]
        [DataOffset(0x13F, DataType.IByte)]
        ChampFlags1 flags_1; // 0x13F;
        [DataOffset(0x140, DataType.IByte)]
        ChampFlags2 flags_2; // 0x140;
        [DataOffset(0x141, DataType.Byte)]
        public byte field_141;

        //[DataOffset(0x14C, DataType.Byte)]
        //public byte number_of_items;

        //[DataOffset(0x14D, DataType.WordArray, 14)]
        //public int[] items = new int[14];

        [DataOffset(0x17B, DataType.Byte)]
        public byte weaponsHandsUsed; // 0x17B;
        [DataOffset(0x17C, DataType.SByte)]
        public sbyte field_17C; // 0x17C;
        [DataOffset(0x17D, DataType.SWord)]
        public short weight; // 0x17D;

        //[DataOffset(0x17F, DataType.Int)]
        //public uint nextCharacter; // 0x17F;
        //[DataOffset(0x183, DataType.Int)]
        //public uint actions; // 0x183;
        [DataOffset(0x184, DataType.Byte)]
        public byte paladinCuresLeft; // 0x184;
        [DataOffset(0x185, DataType.Byte)]
        public byte field_185; // 0x185;
        [DataOffset(0x186, DataType.Byte)]
        public byte field_186; // 0x186;
        [DataOffset(0x187, DataType.Byte)]
        public byte field_187; // 0x187;
        [DataOffset(0x188, DataType.Byte)]
        public byte health_status; // 0x188;
        [DataOffset(0x189, DataType.Bool)]
        public bool in_combat; // 0x189;
        [DataOffset(0x18A, DataType.Byte)]
        public byte combat_team; // 0x18A; 0 - our team, 1 - enemy
        [DataOffset(0x18B, DataType.Byte)]
        public byte quick_fight; // 0x18B;
        [DataOffset(0x18C, DataType.Byte)]
        public byte hitBonus; // 0x18C;
        [DataOffset(0x18D, DataType.Byte)]
        public byte ac; // 0x18D

        [DataOffset(0x18E, DataType.Byte)]
        public byte ac_behind; // 0x18E;

        [DataOffset(0x18F, DataType.Byte)]
        public byte attack1_AttacksLeft; // 0x18F;
        [DataOffset(0x190, DataType.Byte)]
        public byte attack2_AttacksLeft; // 0x190;

        [DataOffset(0x191, DataType.Byte)]
        public byte attack1_DiceCount; // 0x191
        [DataOffset(0x192, DataType.Byte)]
        public byte attack2_DiceCount; // 0x192

        [DataOffset(0x193, DataType.Byte)]
        public byte attack1_DiceSize; // 0x193;
        [DataOffset(0x194, DataType.Byte)]
        public byte attack2_DiceSize; // 0x194;

        [DataOffset(0x195, DataType.SByte)]
        public sbyte attack1_DamageBonus; // 0x195;
        [DataOffset(0x196, DataType.Byte)]
        public byte attack2_DamageBonus; // 0x196;

        [DataOffset(0x197, DataType.Byte)]
        public byte hit_point_current; // 0x197;

        [DataOffsetAttribute(0x198, DataType.Byte)]
        public byte movement; // 0x198;

        public const int StructSize = 0x199;


        public Player(byte[] data, int offset)
        {
            DataIO.ReadObject(this, data, offset);
        }

        public Player(Classes.Player player)
        {
            name = player.name;

            player.stats.Save(stats);

            Spell.Save(player.spellList, memorizedSpells, memorizedSpells.Length);
            spell_to_learn_count = player.spell_to_learn_count;
            thac0 = player.thac0;

            switch (player.race)
            {
                case Classes.Race.monster: race = (byte)Race.monster; break;
                case Classes.Race.human: race = (byte)Race.human; break;
                case Classes.Race.silvanesti_elf: race = (byte)Race.silvanesti_elf; break;
                case Classes.Race.qualinesti_elf: race = (byte)Race.qualinesti_elf; break;
                case Classes.Race.half_elf: race = (byte)Race.half_elf; break;
                case Classes.Race.mountain_dwarf: race = (byte)Race.mountain_dwarf; break;
                case Classes.Race.hill_dwarf: race = (byte)Race.hill_dwarf; break;
                case Classes.Race.kender: race = (byte)Race.kender; break;
            }

            switch (player._class)
            {
                case Classes.ClassId.cleric: _class = (byte)ClassId.cleric; break;
                case Classes.ClassId.druid: _class = (byte)ClassId.druid; break;
                case Classes.ClassId.fighter: _class = (byte)ClassId.fighter; break;
                case Classes.ClassId.paladin: _class = (byte)ClassId.paladin; break;
                case Classes.ClassId.knight: _class = (byte)ClassId.knight; break;
                case Classes.ClassId.ranger: _class = (byte)ClassId.ranger; break;
                case Classes.ClassId.magic_user: _class = (byte)ClassId.magic_user; break;
                case Classes.ClassId.thief: _class = (byte)ClassId.thief; break;
                case Classes.ClassId.mc_c_f: _class = (byte)ClassId.mc_c_f; break;
                case Classes.ClassId.mc_c_f_m: _class = (byte)ClassId.mc_c_f_m; break;
                case Classes.ClassId.mc_c_r: _class = (byte)ClassId.mc_c_r; break;
                case Classes.ClassId.mc_c_mu: _class = (byte)ClassId.mc_c_mu; break;
                case Classes.ClassId.mc_c_t: _class = (byte)ClassId.mc_c_t; break;
                case Classes.ClassId.mc_f_mu: _class = (byte)ClassId.mc_f_mu; break;
                case Classes.ClassId.mc_f_t: _class = (byte)ClassId.mc_f_t; break;
                case Classes.ClassId.mc_f_mu_t: _class = (byte)ClassId.mc_f_mu_t; break;
                case Classes.ClassId.mc_mu_t: _class = (byte)ClassId.mc_mu_t; break;
                case Classes.ClassId.unknown: _class = (byte)ClassId.unknown; break;
            }

            age = player.age;

            hit_point_max = player.hit_point_max;

            Spell.Save(player.spellBook, spellBook, spellBook.Length);

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

            control_morale = player.control_morale;
            npcTreasureShareCount = player.npcTreasureShareCount;
            field_E9 = player.field_F9;
            field_EA = player.field_FA;

            money[0] = (ushort)player.Money.GetCoins(Money.Copper);
            money[1] = (ushort)player.Money.GetCoins(Money.Bronze);
            money[3] = (ushort)player.Money.GetCoins(Money.Platinum);
            money[2] = (ushort)player.Money.GetCoins(Money.Steel);
            money[4] = (ushort)player.Money.GetCoins(Money.Gems);
            money[5] = (ushort)player.Money.GetCoins(Money.Jewelry);

            ClassLevel[(int)ClassId.cleric] = player.cleric_lvl;
            ClassLevel[(int)ClassId.druid] = player.druid_lvl;
            ClassLevel[(int)ClassId.fighter] = player.fighter_lvl;
            ClassLevel[(int)ClassId.paladin] = player.paladin_lvl;
            ClassLevel[(int)ClassId.ranger] = player.ranger_lvl;
            ClassLevel[(int)ClassId.magic_user] = player.magic_user_lvl;
            ClassLevel[(int)ClassId.thief] = player.thief_lvl;
            ClassLevel[(int)ClassId.knight] = player.knight_lvl;
            System.Array.Copy(player.ClassLevelsOld, ClassLevelsOld, 8);

            sex = player.sex;
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
            useStrBonus = player.useStrBonus;
            mod_id = player.mod_id;
            exp = player.exp;
            classFlags = player.classFlags;
            hit_point_rolled = player.hit_point_rolled;

            for (int spell_class = 0; spell_class < 3; spell_class++)
            {
                for (int spell_level = 0; spell_level < 5; spell_level++)
                {
                    spellCastCount[spell_class * 5 + spell_level] = player.spellCastCount[spell_class, spell_level];
                }
            }

            field_130 = player.field_13C;
            field_132 = player.field_13E;
            head_portrait = player.head_portrait;
            body_portrait = player.body_portrait;
            head_icon = player.head_icon;
            weapon_icon = player.weapon_icon;
            icon_id = player.icon_id;
            icon_size = player.icon_size;
            flags_1 = 0x00;
            flags_2 = 0x00;
            System.Array.Copy(player.icon_colours, icon_colours, 6);
            if (player.flags.HasFlag(Flags.EvilSummon))
            {
                flags_1 |= ChampFlags1.EvilSummon;
            }
            if (player.flags.HasFlag(Flags.Undead))
            {
                flags_1 |= ChampFlags1.Undead;
            }
            if (player.flags.HasFlag(Flags.DwarfPenalty))
            {
                flags_1 |= ChampFlags1.DwarfPenalty;
            }
            if (player.flags.HasFlag(Flags.RangerBonus))
            {
                flags_1 |= ChampFlags1.RangerBonus;
            }
            if (player.flags.HasFlag(Flags.Snake))
            {
                flags_1 |= ChampFlags1.Snake;
            }
            if (player.flags.HasFlag(Flags.Reptile))
            {
                flags_1 |= ChampFlags1.Reptile;
            }
            if (player.flags.HasFlag(Flags.Animal))
            {
                flags_1 |= ChampFlags1.Animal;
            }
            if (player.flags.HasFlag(Flags.DwarfBonus))
            {
                flags_1 |= ChampFlags1.DwarfBonus;
            }
            if (player.flags.HasFlag(Flags.Dragon))
            {
                flags_2 |= ChampFlags2.Dragon;
            }

            weaponsHandsUsed = player.weaponsHandsUsed;
            field_17C = player.field_186;
            weight = player.weight;

            paladinCuresLeft = player.paladinCuresLeft;
            field_185 = player.field_192;
            field_186 = player.field_193;
            field_187 = player.field_194;
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

        public Classes.Player Load()
        {
            Classes.Player player = new();

            player.name = name;

            player.stats.Load(stats);

            Spell.Load(player.spellList, memorizedSpells, memorizedSpells.Length);
            player.spell_to_learn_count = spell_to_learn_count;
            player.thac0 = thac0;

            switch ((Race)race)
            {
                case Race.monster: player.race = Classes.Race.monster; break;
                case Race.human: player.race = Classes.Race.human; break;
                case Race.silvanesti_elf: player.race = Classes.Race.silvanesti_elf; break;
                case Race.qualinesti_elf: player.race = Classes.Race.qualinesti_elf; break;
                case Race.half_elf: player.race = Classes.Race.half_elf; break;
                case Race.mountain_dwarf: player.race = Classes.Race.mountain_dwarf; break;
                case Race.hill_dwarf: player.race = Classes.Race.hill_dwarf; break;
                case Race.kender: player.race = Classes.Race.kender; break;
            }

            switch ((ClassId)_class)
            {
                case ClassId.cleric: player._class = Classes.ClassId.cleric; break;
                case ClassId.druid: player._class = Classes.ClassId.druid; break;
                case ClassId.fighter: player._class = Classes.ClassId.fighter; break;
                case ClassId.paladin: player._class = Classes.ClassId.paladin; break;
                case ClassId.knight: player._class = Classes.ClassId.knight; break;
                case ClassId.ranger: player._class = Classes.ClassId.ranger; break;
                case ClassId.magic_user: player._class = Classes.ClassId.magic_user; break;
                case ClassId.thief: player._class = Classes.ClassId.thief; break;
                case ClassId.mc_c_f: player._class = Classes.ClassId.mc_c_f; break;
                case ClassId.mc_c_f_m: player._class = Classes.ClassId.mc_c_f_m; break;
                case ClassId.mc_c_r: player._class = Classes.ClassId.mc_c_r; break;
                case ClassId.mc_c_mu: player._class = Classes.ClassId.mc_c_mu; break;
                case ClassId.mc_c_t: player._class = Classes.ClassId.mc_c_t; break;
                case ClassId.mc_f_mu: player._class = Classes.ClassId.mc_f_mu; break;
                case ClassId.mc_f_t: player._class = Classes.ClassId.mc_f_t; break;
                case ClassId.mc_f_mu_t: player._class = Classes.ClassId.mc_f_mu_t; break;
                case ClassId.mc_mu_t: player._class = Classes.ClassId.mc_mu_t; break;
                case ClassId.unknown: player._class = Classes.ClassId.unknown; break;
            }

            player.age = age;

            player.hit_point_max = hit_point_max;

            Spell.Load(player.spellBook, spellBook, spellBook.Length);

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

            player.field_F6 = 0;
            player.control_morale = control_morale;
            player.npcTreasureShareCount = npcTreasureShareCount;
            player.field_F9 = field_E9;
            player.field_FA = field_EA;

            player.Money.SetCoins(Money.Copper, money[0]);
            player.Money.SetCoins(Money.Bronze, money[1]);
            player.Money.SetCoins(Money.Platinum, money[2]);
            player.Money.SetCoins(Money.Steel, money[3]);
            player.Money.SetCoins(Money.Gems, money[4]);
            player.Money.SetCoins(Money.Jewelry, money[5]);

            player.cleric_lvl = ClassLevel[(int)ClassId.cleric];
            player.druid_lvl = ClassLevel[(int)ClassId.druid];
            player.fighter_lvl = ClassLevel[(int)ClassId.fighter];
            player.paladin_lvl = ClassLevel[(int)ClassId.paladin];
            player.ranger_lvl = ClassLevel[(int)ClassId.ranger];
            player.magic_user_lvl = ClassLevel[(int)ClassId.magic_user];
            player.thief_lvl = ClassLevel[(int)ClassId.thief];
            player.monk_lvl = 0;
            player.knight_lvl = ClassLevel[(int)ClassId.knight];

            player.cleric_old_lvl = 0;
            player.druid_old_lvl = 0;
            player.fighter_old_lvl = 0;
            player.paladin_old_lvl = 0;
            player.ranger_old_lvl = 0;
            player.magic_user_old_lvl = 0;
            player.thief_old_lvl = 0;
            player.monk_lvl = 0;
            player.knight_old_lvl = 0;

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
            player.useStrBonus = useStrBonus;
            player.mod_id = mod_id;
            player.exp = exp;
            player.classFlags = classFlags;
            player.hit_point_rolled = hit_point_rolled;

            for (int spell_class = 0; spell_class < 3; spell_class++)
            {
                for (int spell_level = 0; spell_level < 5; spell_level++)
                {
                    player.spellCastCount[spell_class, spell_level] = spellCastCount[spell_class * 5 + spell_level];
                }
            }

            player.field_13C = field_130;
            player.field_13E = field_132;
            player.head_portrait = head_portrait;
            player.body_portrait = body_portrait;
            player.head_icon = head_icon;
            player.weapon_icon = weapon_icon;
            player.icon_id = icon_id;
            player.icon_size = icon_size;
            System.Array.Copy(icon_colours, player.icon_colours, 6);
            player.flags = 0;
            if ((flags_1 & ChampFlags1.EvilSummon) == ChampFlags1.EvilSummon)
            {
                player.flags |= Flags.EvilSummon;
            }
            if ((flags_1 & ChampFlags1.Undead) == ChampFlags1.Undead)
            {
                player.flags |= Flags.Undead;
            }
            if ((flags_1 & ChampFlags1.DwarfPenalty) == ChampFlags1.DwarfPenalty)
            {
                player.flags |= Flags.DwarfPenalty;
            }
            if ((flags_1 & ChampFlags1.RangerBonus) == ChampFlags1.RangerBonus)
            {
                player.flags |= Flags.RangerBonus;
            }
            if ((flags_1 & ChampFlags1.Snake) == ChampFlags1.Snake)
            {
                player.flags |= Flags.Snake;
            }
            if ((flags_1 & ChampFlags1.Reptile) == ChampFlags1.Reptile)
            {
                player.flags |= Flags.Reptile;
            }
            if ((flags_1 & ChampFlags1.Animal) == ChampFlags1.Animal)
            {
                player.flags |= Flags.Animal;
            }
            if ((flags_1 & ChampFlags1.DwarfBonus) == ChampFlags1.DwarfBonus)
            {
                player.flags |= Flags.DwarfBonus;
            }
            if ((flags_2 & ChampFlags2.Dragon) == ChampFlags2.Dragon)
            {
                player.flags |= Flags.Dragon;
            }

            player.weaponsHandsUsed = weaponsHandsUsed;
            player.field_186 = field_17C;
            player.weight = weight;

            player.paladinCuresLeft = paladinCuresLeft;
            player.field_192 = field_185;
            player.field_193 = field_186;
            player.field_194 = field_187;
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

        public static async System.Threading.Tasks.Task<Classes.Player> LoadPlayer(System.IO.Stream player_stream, string path, string file)
        {
            byte[] data = new byte[StructSize];
            gbl.file.BlockRead(StructSize, data, player_stream);
            gbl.file.Close(player_stream);

            var player = new Player(data, 0).Load();

            var filename = string.Format("{0}.SWG", file);

            if (await gbl.file.Find(path, filename) == true)
            {
                var item_stream = await gbl.file.Open(gbl.SavePath, filename);

                LoadItems(player, item_stream);
            }

            filename = string.Format("{0}.FX", file);

            if (await gbl.file.Find(path, filename) == true)
            {
                var affect_stream = await gbl.file.Open(gbl.SavePath, filename);

                LoadAffects(player, affect_stream);
            }

            return player;
        }
        public static Classes.Player LoadPlayer(System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream)
        {
            byte[] data = new byte[StructSize];
            gbl.file.BlockRead(StructSize, data, player_stream);
            gbl.file.Close(player_stream);

            var player = new Player(data, 0).Load();

            if (item_stream != null)
            {
                LoadItems(player, item_stream);
            }

            if (affect_stream != null)
            {
                LoadAffects(player, affect_stream);
            }

            return player;
        }
        public static Classes.Player LoadPlayer(byte[] player_data, byte[] item_data, ushort item_len, byte[] affect_data, ushort affect_len)
        {
            var player = new Player(player_data, 0).Load();

            if (item_len != 0)
            {
                ushort offset = 0;

                do
                {
                    player.items.Add(new Item(item_data, offset).Load());

                    offset += Item.StructSize;
                } while (offset < item_len);
            }

            if (affect_len != 0)
            {
                ushort offset = 0;

                do
                {
                    new Affect(affect_data, offset).Load(player);

                    offset += Affect.StructSize;
                } while (offset < affect_len);
            }

            return player;
        }
        public static void SavePlayer(Classes.Player player, System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream)
        {
            gbl.file.Rewrite(player_stream);

            gbl.file.BlockWrite(Player.StructSize, new Player(player).Save(), player_stream);
            gbl.file.Close(player_stream);

            if (item_stream != null)
            {
                gbl.file.Rewrite(item_stream);

                player.items.ForEach(item => gbl.file.BlockWrite(Item.StructSize, new Item(item).Save(), item_stream));

                gbl.file.Close(item_stream);
            }

            if (affect_stream != null)
            {
                gbl.file.Rewrite(affect_stream);

                foreach (Classes.Affect affect in player.affects)
                {
                    gbl.file.BlockWrite(Affect.StructSize, new Affect(affect, player).Save(), affect_stream);
                }

                gbl.file.Close(affect_stream);
            }
        }
        public static void LoadItems(Classes.Player player, System.IO.Stream file)
        {
            byte[] data = new byte[Item.StructSize];

            while (true)
            {
                if (gbl.file.BlockRead(Item.StructSize, data, file) == Item.StructSize)
                {
                    player.items.Add(new Item(data, 0).Load());
                }
                else
                {
                    break;
                }
            }
            gbl.file.Close(file);
        }
        public static void LoadAffects(Classes.Player player, System.IO.Stream file)
        {
            byte[] data = new byte[Affect.StructSize];

            while (true)
            {
                if (gbl.file.BlockRead(Affect.StructSize, data, file) == Affect.StructSize)
                {
                    new Affect(data, 0).Load(player);
                }
                else
                {
                    break;
                }
            }
            gbl.file.Close(file);
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }
    }
}
