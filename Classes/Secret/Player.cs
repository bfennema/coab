namespace Classes.Secret
{
    /// <summary>
    /// Summary description for Player.
    /// </summary>
    public class Player
    {
        public enum MonsterType
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
        public enum Race
        {
            elf = 1,
            half_elf = 2,
            dwarf = 3,
            gnome = 4,
            halfling = 5,
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
            monk = 7,
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
        public enum SilverFlags1
        {
            EvilSummon = 0x01,
            Mammal = 0x02,
            DwarfPenalty = 0x04,
            RangerBonus = 0x08,
            Snake = 0x10,
            GnomePenalty = 0x20,
            Animal = 0x40,
            DwarfBonus = 0x80,
        }

        [System.Flags]
        public enum SilverFlags2
        {
            Giant = 0x01,
            HeldCharmed = 0x02,
            Reptile = 0x04,
            ImmuneDeathMagic = 0x08,
            ImmunePoison = 0x10,
            ImmuneVorpal = 0x20,
            ImmuneConfusion = 0x40,
            Dragon = 0x80,
        }

        [DataOffset(0x00, DataType.PString, 15)]
        public string name; // 0x00 - 0x0F;

        [DataOffset(0x10, DataType.ByteArray, 14)]
        public byte[] stats = new byte[14]; // 0x10 - 0x1D;

        [DataOffset(0x1E, DataType.ByteArray, 75)]
        public byte[] memorizedSpells = new byte[75]; // 0x1E - 0x68;

        [DataOffset(0x69, DataType.Byte)]
        public byte spell_to_learn_count; // 0x69;
        [DataOffset(0x6A, DataType.SByte)]
        public sbyte thac0; // 0x6A;

        [DataOffset(0x6B, DataType.Byte)]
        public byte race; // 0x6B;

        [DataOffset(0x6C, DataType.Byte)]
        public byte _class; // 0x6C;
        [DataOffset(0x6D, DataType.Byte)]
        public byte paladinCuresLeft; // 0x6D
        [DataOffset(0x6E, DataType.SWord)]
        public short age; // 0x6E;

        [DataOffset(0x70, DataType.Byte)]
        public byte hit_point_max; // 0x70;

        [DataOffset(0x71, DataType.ByteArray, 117)]
        public byte[] spellBook = new byte[117]; // 0x71 - 0xE5

        [DataOffset(0xE6, DataType.Byte)]
        public byte attackLevel; // 0xE6;
        [DataOffset(0xE7, DataType.Byte)]
        public byte icon_dimensions; // 0xE7;
        [DataOffset(0xE8, DataType.ByteArray, 5)]
        public byte[] saveVerse = new byte[5]; // 0xE8 - 0xEC;

        [DataOffset(0xED, DataType.Byte)]
        public byte base_movement; // 0xED;
        [DataOffset(0xEE, DataType.Byte)]
        public byte HitDice; // 0xEE;
        [DataOffset(0xEF, DataType.Byte)]
        public byte multiclassLevel; // 0xEF;
        [DataOffset(0xF0, DataType.Byte)]
        public byte lost_lvls; // 0xF0;
        [DataOffset(0xF1, DataType.Byte)]
        public byte lost_hp; // 0xF1;
        [DataOffset(0xF2, DataType.Byte)]
        public byte level_undead; // 0xF2;
        [DataOffset(0xF3, DataType.ByteArray, 8)]
        public byte[] thief_skills = new byte[8]; // 0xF3 - 0xFA; [] was 1 offset @ 0xe9, pick_pockets, open_locks, find_remove_traps, move_silently, hide_in_shadows, hear_noise, climb_walls, read_languages
        [DataOffset(0xFB, DataType.ByteArray, 5)]
        public byte[] affects = new byte[5]; // 0xFB - 0xFE;

        [DataOffset(0xFF, DataType.Byte)]
        public byte control_morale; // 0xFF;
        [DataOffset(0x100, DataType.Byte)]
        public byte npcTreasureShareCount; // 0x100;
        [DataOffset(0x101, DataType.Byte)]
        public byte field_101; // 0x101;
        [DataOffset(0x102, DataType.Byte)]
        public byte field_102; // 0x102;

        [DataOffset(0x103, DataType.ShortArray, 7)]
        public ushort[] money = new ushort[7]; // 0x103 - 0x110

        [DataOffset(0x111, DataType.ByteArray, 7)]
        public byte[] ClassLevel = new byte[7]; // 0x111 - 0x117

        [DataOffset(0x118, DataType.ByteArray, 7)]
        public byte[] ClassLevelsOld = new byte[7]; // 0x118 - 0x11E

        [DataOffset(0x11F, DataType.Byte)]
        public byte sex; // 0x11F;
        [DataOffset(0x120, DataType.Byte)]
        public byte alignment; // 0x120;
        /// <summary>
        /// half-attacks count
        /// </summary>
        [DataOffset(0x121, DataType.Byte)]
        public byte attacksCount; // 0x121;
        [DataOffset(0x122, DataType.Byte)]
        public byte baseHalfMoves; // 0x122;
        [DataOffset(0x123, DataType.Byte)]
        public byte attack1_DiceCountBase; // 0x123;
        [DataOffset(0x124, DataType.Byte)]
        public byte attack2_DiceCountBase; // 0x124;
        [DataOffset(0x125, DataType.Byte)]
        public byte attack1_DiceSizeBase; // 0x125;
        [DataOffset(0x126, DataType.Byte)]
        public byte attack2_DiceSizeBase; // 0x126;
        [DataOffset(0x127, DataType.Byte)]
        public byte attack1_DamageBonusBase; // 0x127;
        [DataOffset(0x128, DataType.Byte)]
        public byte attack2_DamageBonusBase; // 0x128;
        [DataOffset(0x129, DataType.Byte)]
        public byte base_ac; // 0x129;
        [DataOffset(0x12A, DataType.Byte)]
        public byte useStrBonus; // 0x12A;
        [DataOffset(0x12B, DataType.Byte)]
        public byte mod_id; // 0x12B;
        [DataOffset(0x12C, DataType.Int)]
        public int exp; // 0x12C
        [DataOffset(0x130, DataType.Byte)]
        public byte classFlags; // 0x130;
        [DataOffset(0x131, DataType.Byte)]
        public byte hit_point_rolled; // 0x131;

        [DataOffset(0x132, DataType.ByteArray, 28)]
        public byte[] spellCastCount = new byte[28]; // 0x132 - 0x14D

        [DataOffset(0x14E, DataType.SWord)]
        public short field_14E; // 0x14E
        [DataOffset(0x150, DataType.Byte)]
        public byte field_150; // 0x150;
        [DataOffset(0x151, DataType.Byte)]
        public byte head_portrait; // 0x151;
        [DataOffset(0x152, DataType.Byte)]
        public byte body_portrait; // 0x152;
        [DataOffset(0x153, DataType.Byte)]
        public byte head_icon; // 0x153;
        [DataOffset(0x154, DataType.Byte)]
        public byte weapon_icon; // 0x154;
        [DataOffset(0x155, DataType.Byte)]
        public byte icon_id; // 0x155;
        [DataOffset(0x156, DataType.Byte)]
        public byte icon_size; // 0x156; field_156  1 small 2 normal
        [DataOffset(0x157, DataType.ByteArray, 6)]
        public byte[] icon_colours = new byte[6]; // 0x157 = field_144[1] // byte[6]
        [DataOffset(0x15D, DataType.IByte)]
        public SilverFlags1 flags_1; // 0x15D;
        [DataOffset(0x15E, DataType.IByte)]
        public SilverFlags2 flags_2; // 0x15E;

        //[DataOffset(0x160, DataType.Byte)]
        //public byte number_of_items;

        //[DataOffset(0x161, DataType.WordArray, 14)]
        //public int[] items = new int[14];

        [DataOffset(0x199, DataType.Byte)]
        public byte weaponsHandsUsed; // 0x199;
        [DataOffset(0x19A, DataType.SByte)]
        public sbyte field_19A; // 0x19A;
        [DataOffset(0x19B, DataType.SWord)]
        public short weight; // 0x19B;

        //[DataOffset(0x19C, DataType.Int)]
        //public uint nextCharacter; // 0x19C;
        //[DataOffset(0x1A1, DataType.Int)]
        //public uint actions; // 0x1A1;
        [DataOffset(0x1A5, DataType.Byte)]
        public byte magic_resistance; // 0x1A5;
        [DataOffset(0x1A6, DataType.Byte)]
        public byte health_status; // 0x1A6;
        [DataOffset(0x1A7, DataType.Bool)]
        public bool in_combat; // 0x1A7;
        [DataOffset(0x1A8, DataType.Byte)]
        public byte combat_team; // 0x1A8; 0 - our team, 1 - enemy
        [DataOffset(0x1A9, DataType.Byte)]
        public byte quick_fight; // 0x1A9;
        [DataOffset(0x1AA, DataType.Byte)]
        public byte hitBonus; // 0x1AA;
        [DataOffset(0x1AB, DataType.Byte)]
        public byte ac; // 0x1AB

        [DataOffset(0x1AC, DataType.Byte)]
        public byte ac_behind; // 0x1AC;

        [DataOffset(0x1AD, DataType.Byte)]
        public byte attack1_AttacksLeft; // 0x1AD;
        [DataOffset(0x1AE, DataType.Byte)]
        public byte attack2_AttacksLeft; // 0x1AE;

        [DataOffset(0x1AF, DataType.Byte)]
        public byte attack1_DiceCount; // 0x1AF
        [DataOffset(0x1B0, DataType.Byte)]
        public byte attack2_DiceCount; // 0x1B0

        [DataOffset(0x1B1, DataType.Byte)]
        public byte attack1_DiceSize; // 0x1B1;
        [DataOffset(0x1B2, DataType.Byte)]
        public byte attack2_DiceSize; // 0x1B2;

        [DataOffset(0x1B3, DataType.SByte)]
        public sbyte attack1_DamageBonus; // 0x1B3;
        [DataOffset(0x1B4, DataType.Byte)]
        public byte attack2_DamageBonus; // 0x1B4;

        [DataOffset(0x1B5, DataType.Byte)]
        public byte hit_point_current; // 0x1B5;

        [DataOffsetAttribute(0x1B6, DataType.Byte)]
        public byte movement; // 0x1B7;

        public const int StructSize = 0x1B7;

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
                case Classes.Race.dwarf: race = (byte)Race.dwarf; break;
                case Classes.Race.elf: race = (byte)Race.elf; break;
                case Classes.Race.gnome: race = (byte)Race.gnome; break;
                case Classes.Race.half_elf: race = (byte)Race.half_elf; break;
                case Classes.Race.halfling: race = (byte)Race.halfling; break;
                case Classes.Race.human: race = (byte)Race.human; break;
            }

            switch (player._class)
            {
                case Classes.ClassId.cleric: _class = (byte)ClassId.cleric; break;
                case Classes.ClassId.druid: _class = (byte)ClassId.druid; break;
                case Classes.ClassId.fighter: _class = (byte)ClassId.fighter; break;
                case Classes.ClassId.paladin: _class = (byte)ClassId.paladin; break;
                case Classes.ClassId.ranger: _class = (byte)ClassId.ranger; break;
                case Classes.ClassId.magic_user: _class = (byte)ClassId.magic_user; break;
                case Classes.ClassId.thief: _class = (byte)ClassId.thief; break;
                case Classes.ClassId.monk: _class = (byte)ClassId.monk; break;
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
            paladinCuresLeft = player.paladinCuresLeft;
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

            //field_F6 = player.field_F6;
            control_morale = player.control_morale;
            npcTreasureShareCount = player.npcTreasureShareCount;
            field_101 = player.field_F9;
            field_102 = player.field_FA;

            money[0] = (ushort)player.Money.GetCoins(Money.Copper);
            money[1] = (ushort)player.Money.GetCoins(Money.Silver);
            money[2] = (ushort)player.Money.GetCoins(Money.Electrum);
            money[3] = (ushort)player.Money.GetCoins(Money.Gold);
            money[4] = (ushort)player.Money.GetCoins(Money.Platinum);
            money[5] = (ushort)player.Money.GetCoins(Money.Gems);
            money[6] = (ushort)player.Money.GetCoins(Money.Jewelry);

            ClassLevel[(int)ClassId.cleric] = player.cleric_lvl;
            ClassLevel[(int)ClassId.druid] = player.druid_lvl;
            ClassLevel[(int)ClassId.fighter] = player.fighter_lvl;
            ClassLevel[(int)ClassId.paladin] = player.paladin_lvl;
            ClassLevel[(int)ClassId.ranger] = player.ranger_lvl;
            ClassLevel[(int)ClassId.magic_user] = player.magic_user_lvl;
            ClassLevel[(int)ClassId.thief] = player.thief_lvl;

            ClassLevelsOld[(int)ClassId.cleric] = player.cleric_old_lvl;
            ClassLevelsOld[(int)ClassId.druid] = player.druid_old_lvl;
            ClassLevelsOld[(int)ClassId.fighter] = player.fighter_old_lvl;
            ClassLevelsOld[(int)ClassId.paladin] = player.paladin_old_lvl;
            ClassLevelsOld[(int)ClassId.ranger] = player.ranger_old_lvl;
            ClassLevelsOld[(int)ClassId.magic_user] = player.magic_user_old_lvl;
            ClassLevelsOld[(int)ClassId.thief] = player.thief_old_lvl;

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

            for (int spell_class = 0; spell_class < 4; spell_class++)
            {
                for (int spell_level = 0; spell_level < 7; spell_level++)
                {
                    if (spell_class == 3)
                    {
                        spellCastCount[spell_class * 7 + spell_level] = player.spellCastCount[spell_class - 1][spell_level];
                    }
                    else if (spell_class == 0 || spell_class == 1)
                    {
                        spellCastCount[spell_class * 7 + spell_level] = player.spellCastCount[spell_class][spell_level];
                    }
                }
            }

            field_14E = player.field_13C;
            field_150 = player.field_13E;
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
                flags_1 |= SilverFlags1.EvilSummon;
            }
            if (player.flags.HasFlag(Flags.Mammal))
            {
                flags_1 |= SilverFlags1.Mammal;
            }
            if (player.flags.HasFlag(Flags.DwarfPenalty))
            {
                flags_1 |= SilverFlags1.DwarfPenalty;
            }
            if (player.flags.HasFlag(Flags.RangerBonus))
            {
                flags_1 |= SilverFlags1.RangerBonus;
            }
            if (player.flags.HasFlag(Flags.Snake))
            {
                flags_1 |= SilverFlags1.Snake;
            }
            if (player.flags.HasFlag(Flags.GnomePenalty))
            {
                flags_1 |= SilverFlags1.GnomePenalty;
            }
            if (player.flags.HasFlag(Flags.Animal))
            {
                flags_1 |= SilverFlags1.Animal;
            }
            if (player.flags.HasFlag(Flags.DwarfBonus))
            {
                flags_1 |= SilverFlags1.DwarfBonus;
            }
            if (player.flags.HasFlag(Flags.Giant))
            {
                flags_2 |= SilverFlags2.Giant;
            }
            if (player.flags.HasFlag(Flags.HeldCharmed))
            {
                flags_2 |= SilverFlags2.HeldCharmed;
            }
            if (player.flags.HasFlag(Flags.Reptile))
            {
                flags_2 |= SilverFlags2.Reptile;
            }
            if (player.flags.HasFlag(Flags.ImmuneDeathMagic))
            {
                flags_2 |= SilverFlags2.ImmuneDeathMagic;
            }
            if (player.flags.HasFlag(Flags.ImmunePoison))
            {
                flags_2 |= SilverFlags2.ImmunePoison;
            }
            if (player.flags.HasFlag(Flags.ImmuneVorpal))
            {
                flags_2 |= SilverFlags2.ImmuneVorpal;
            }
            if (player.flags.HasFlag(Flags.ImmuneConfusion))
            {
                flags_2 |= SilverFlags2.ImmuneConfusion;
            }
            if (player.flags.HasFlag(Flags.Dragon))
            {
                flags_2 |= SilverFlags2.Dragon;
            }

            weaponsHandsUsed = player.weaponsHandsUsed;
            field_19A = player.field_186;
            weight = player.weight;

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
                case Race.dwarf: player.race = Classes.Race.dwarf; break;
                case Race.elf: player.race = Classes.Race.elf; break;
                case Race.gnome: player.race = Classes.Race.gnome; break;
                case Race.half_elf: player.race = Classes.Race.half_elf; break;
                case Race.halfling: player.race = Classes.Race.halfling; break;
                case Race.human: player.race = Classes.Race.human; break;
            }

            switch ((ClassId)_class)
            {
                case ClassId.cleric: player._class = Classes.ClassId.cleric; break;
                case ClassId.druid: player._class = Classes.ClassId.druid; break;
                case ClassId.fighter: player._class = Classes.ClassId.fighter; break;
                case ClassId.paladin: player._class = Classes.ClassId.paladin; break;
                case ClassId.ranger: player._class = Classes.ClassId.ranger; break;
                case ClassId.magic_user: player._class = Classes.ClassId.magic_user; break;
                case ClassId.thief: player._class = Classes.ClassId.thief; break;
                case ClassId.monk: player._class = Classes.ClassId.monk; break;
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
            player.paladinCuresLeft = paladinCuresLeft;
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

            player.field_F6 = 0; // field_F6;
            player.control_morale = control_morale;
            player.npcTreasureShareCount = npcTreasureShareCount;
            player.field_F9 = field_101;
            player.field_FA = field_102;

            player.Money.SetCoins(Money.Copper, money[0]);
            player.Money.SetCoins(Money.Silver, money[1]);
            player.Money.SetCoins(Money.Electrum, money[2]);
            player.Money.SetCoins(Money.Gold, money[3]);
            player.Money.SetCoins(Money.Platinum, money[4]);
            player.Money.SetCoins(Money.Gems, money[5]);
            player.Money.SetCoins(Money.Jewelry, money[6]);

            player.cleric_lvl = ClassLevel[(int)ClassId.cleric];
            player.druid_lvl = ClassLevel[(int)ClassId.druid];
            player.fighter_lvl = ClassLevel[(int)ClassId.fighter];
            player.paladin_lvl = ClassLevel[(int)ClassId.paladin];
            player.ranger_lvl = ClassLevel[(int)ClassId.ranger];
            player.magic_user_lvl = ClassLevel[(int)ClassId.magic_user];
            player.thief_lvl = ClassLevel[(int)ClassId.thief];
            player.monk_lvl = 0;
            player.knight_lvl = 0;

            player.cleric_old_lvl = ClassLevelsOld[(int)ClassId.cleric];
            player.druid_old_lvl = ClassLevelsOld[(int)ClassId.druid];
            player.fighter_old_lvl = ClassLevelsOld[(int)ClassId.fighter];
            player.paladin_old_lvl = ClassLevelsOld[(int)ClassId.paladin];
            player.ranger_old_lvl = ClassLevelsOld[(int)ClassId.ranger];
            player.magic_user_old_lvl = ClassLevelsOld[(int)ClassId.magic_user];
            player.thief_old_lvl = ClassLevelsOld[(int)ClassId.thief];
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

            for (int spell_class = 0; spell_class < 4; spell_class++)
            {
                for (int spell_level = 0; spell_level < 7; spell_level++)
                {
                    if (spell_class == 3)
                    {
                        player.spellCastCount[spell_class-1][spell_level] = spellCastCount[spell_class * 7 + spell_level];
                    }
                    else if (spell_class == 0 || spell_class == 1)
                    {
                        player.spellCastCount[spell_class][spell_level] = spellCastCount[spell_class * 7 + spell_level];
                    }
                }
            }

            player.field_13C = field_14E;
            player.field_13E = field_150;
            player.head_portrait = head_portrait;
            player.body_portrait = body_portrait;
            player.head_icon = head_icon;
            player.weapon_icon = weapon_icon;
            player.icon_id = icon_id;
            player.icon_size = icon_size;
            System.Array.Copy(icon_colours, player.icon_colours, 6);
            player.flags = 0;
            if ((flags_1 & SilverFlags1.EvilSummon) == SilverFlags1.EvilSummon)
            {
                player.flags |= Flags.EvilSummon;
            }
            if ((flags_1 & SilverFlags1.Mammal) == SilverFlags1.Mammal)
            {
                player.flags |= Flags.Mammal;
            }
            if ((flags_1 & SilverFlags1.DwarfPenalty) == SilverFlags1.DwarfPenalty)
            {
                player.flags |= Flags.DwarfPenalty;
            }
            if ((flags_1 & SilverFlags1.RangerBonus) == SilverFlags1.RangerBonus)
            {
                player.flags |= Flags.RangerBonus;
            }
            if ((flags_1 & SilverFlags1.Snake) == SilverFlags1.Snake)
            {
                player.flags |= Flags.Snake;
            }
            if ((flags_1 & SilverFlags1.GnomePenalty) == SilverFlags1.GnomePenalty)
            {
                player.flags |= Flags.GnomePenalty;
            }
            if ((flags_1 & SilverFlags1.Animal) == SilverFlags1.Animal)
            {
                player.flags |= Flags.Animal;
            }
            if ((flags_1 & SilverFlags1.DwarfBonus) == SilverFlags1.DwarfBonus)
            {
                player.flags |= Flags.DwarfBonus;
            }
            if ((flags_2 & SilverFlags2.Giant) == SilverFlags2.Giant)
            {
                player.flags |= Flags.Giant;
            }
            if ((flags_2 & SilverFlags2.HeldCharmed) == SilverFlags2.HeldCharmed)
            {
                player.flags |= Flags.HeldCharmed;
            }
            if ((flags_2 & SilverFlags2.Reptile) == SilverFlags2.Reptile)
            {
                player.flags |= Flags.Reptile;
            }
            if ((flags_2 & SilverFlags2.ImmuneDeathMagic) == SilverFlags2.ImmuneDeathMagic)
            {
                player.flags |= Flags.ImmuneDeathMagic;
            }
            if ((flags_2 & SilverFlags2.ImmunePoison) == SilverFlags2.ImmunePoison)
            {
                player.flags |= Flags.ImmunePoison;
            }
            if ((flags_2 & SilverFlags2.ImmuneVorpal) == SilverFlags2.ImmuneVorpal)
            {
                player.flags |= Flags.ImmuneVorpal;
            }
            if ((flags_2 & SilverFlags2.ImmuneConfusion) == SilverFlags2.ImmuneConfusion)
            {
                player.flags |= Flags.ImmuneConfusion;
            }
            if ((flags_2 & SilverFlags2.Dragon) == SilverFlags2.Dragon)
            {
                player.flags |= Flags.Dragon;
            }

            player.weaponsHandsUsed = weaponsHandsUsed;
            player.field_186 = field_19A;
            player.weight = weight;

            player.paladinCuresLeft = 0; // paladinCuresLeft;
            player.field_192 = 0; // field_192;
            player.field_193 = 0; // field_193;
            player.field_194 = 0; // field_194;
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

            var filename = string.Format("{0}.STF", file);

            if (await gbl.file.Find(path, filename) == true)
            {
                var item_stream = await gbl.file.Open(gbl.SavePath, filename);

                LoadItems(player, item_stream);
            }

            filename = string.Format("{0}.SFX", file);

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
            byte[] data = new byte[Item.StructSizeSave];

            while (true)
            {
                if (gbl.file.BlockRead(Item.StructSizeSave, data, file) == Item.StructSizeSave)
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
