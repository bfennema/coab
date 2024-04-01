namespace Classes
{
    public class PoolRadPlayer
    {
        [DataOffset(0x00, DataType.PString, 15)]
        public string name; // 0x0 - 0x0F
        [DataOffset(0x10, DataType.Byte)]
        public byte stat_str; // 0x10
        [DataOffset(0x11, DataType.Byte)]
        public byte stat_int; // 0x11
        [DataOffset(0x12, DataType.Byte)]
        public byte stat_wis; // 0x12
        [DataOffset(0x13, DataType.Byte)]
        public byte stat_dex; // 0x13
        [DataOffset(0x14, DataType.Byte)]
        public byte stat_con; // 0x14
        [DataOffset(0x15, DataType.Byte)]
        public byte stat_cha; // 0x15
        [DataOffset(0x16, DataType.Byte)]
        public byte stat_str00; // 0x16
        [DataOffset(0x17, DataType.ByteArray, 21)]
        public byte[] field_17 = new byte[21]; // 0x17 Array 0x15, 0x17 - 0x2B
        [DataOffset(0x2D, DataType.SByte)]
        public sbyte thac0; // 0x2D
        [DataOffset(0x2E, DataType.Byte)]
        public byte race; // 0x2e
        [DataOffset(0x2F, DataType.Byte)]
        public byte _class; // 0x2F
        [DataOffset(0x30, DataType.SWord)]
        public short age; // 0x30
        [DataOffset(0x32, DataType.Byte)]
        public byte hp_max; // 0x32
        [DataOffset(0x33, DataType.ByteArray, 56)]
        public byte[] field_33 = new byte[56]; // 0x33 Array 0x38, 0x33 - 0x6A
        [DataOffset(0x6B, DataType.Byte)]
        public byte field_6B; // 0x6B
        [DataOffset(0x6C, DataType.Byte)]
        public byte icon_dimensions; // 0x6C
        [DataOffset(0x6D, DataType.ByteArray, 5)]
        public byte[] saveVerse = new byte[5]; // 0x6D Array 5, 0x6D - 0x71
        [DataOffset(0x72, DataType.Byte)]
        public byte field_72; // 0x72
        [DataOffset(0x73, DataType.Byte)]
        public byte field_73; // 0x73
        [DataOffset(0x74, DataType.Byte)]
        public byte field_74; // 0x74
        [DataOffset(0x75, DataType.Byte)]
        public byte field_75; // 0x75
        [DataOffset(0x76, DataType.Byte)]
        public byte field_76; // 0x76
        [DataOffset(0x77, DataType.ByteArray, 8)]
        public byte[] field_77 = new byte[8]; // 0x77 Array 8, 0x77 - 0x7E
        [DataOffset(0x83, DataType.Byte)]
        public byte field_83; // 0x83
        [DataOffset(0x84, DataType.Byte)]
        public byte field_84; // 0x84
        [DataOffset(0x85, DataType.Byte)]
        public byte field_85; // 0x85
        [DataOffset(0x86, DataType.Byte)]
        public byte field_86; // 0x86
        [DataOffset(0x87, DataType.Byte)]
        public byte field_87; // 0x87

        [DataOffset(0x88, DataType.Word)]
        public ushort field_88; // 0x88 - 0x89
        [DataOffset(0x8A, DataType.Word)]
        public ushort field_8A; // 0x8A - 0x8B
        [DataOffset(0x8C, DataType.Word)]
        public ushort field_8C; // 0x8C - 0x8D
        [DataOffset(0x8E, DataType.Word)]
        public ushort field_8E; // 0x8E - 0x8F
        [DataOffset(0x90, DataType.Word)]
        public ushort field_90; // 0x90 - 0x91
        [DataOffset(0x92, DataType.Word)]
        public ushort field_92; // 0x92 - 0x93
        [DataOffset(0x94, DataType.Word)]
        public ushort field_94; // 0x94 - 0x95

        [DataOffset(0x96, DataType.ByteArray, 8)]
        public byte[] field_96 = new byte[8]; // 0x96 Array 8 0x96 - 0x9D
        [DataOffset(0x9E, DataType.Byte)]
        public byte sex; // 0x9E
        [DataOffset(0x9F, DataType.Byte)]
        public byte field_9F; // 0x9F
        [DataOffset(0xA0, DataType.Byte)]
        public byte field_A0; // 0xA0

        [DataOffset(0xA1, DataType.Byte)]
        public byte field_A1; // 0xA1
        [DataOffset(0xA2, DataType.Byte)]
        public byte field_A2; // 0xA2
        [DataOffset(0xA3, DataType.Byte)]
        public byte field_A3; // 0xA3
        [DataOffset(0xA4, DataType.Byte)]
        public byte field_A4; // 0xA4
        [DataOffset(0xA5, DataType.Byte)]
        public byte field_A5; // 0xA5
        [DataOffset(0xA6, DataType.Byte)]
        public byte field_A6; // 0xA6
        [DataOffset(0xA7, DataType.Byte)]
        public byte field_A7; // 0xA7
        [DataOffset(0xA8, DataType.Byte)]
        public byte field_A8; // 0xA8

        [DataOffset(0xA9, DataType.Byte)]
        public byte field_A9; // 0xA9
        [DataOffset(0xAA, DataType.Byte)]
        public byte field_AA; // 0xAA
        [DataOffset(0xAB, DataType.Byte)]
        public byte field_AB; // 0xAB

        [DataOffset(0xAC, DataType.Int)]
        public int field_AC; //0xAC
        [DataOffset(0xB0, DataType.Byte)]
        public byte field_B0; // 0xB0
        [DataOffset(0xB1, DataType.Byte)]
        public byte field_B1; // 0xB1
        [DataOffset(0xB2, DataType.ByteArray, 3)]
        public byte[] field_B2 = new byte[3]; // 0xB2 - 3
        [DataOffset(0xB5, DataType.ByteArray, 3)]
        public byte[] field_B5 = new byte[3]; // 0xB5 - 3

        [DataOffset(0xB8, DataType.SWord)]
        public short field_B8; // 0xB8

        [DataOffset(0xBA, DataType.Byte)]
        public byte field_BA; // 0xBA
        [DataOffset(0xBB, DataType.Byte)]
        public byte field_BB; // 0xBB
        [DataOffset(0xBC, DataType.Byte)]
        public byte field_BC; // 0xBC
        [DataOffset(0xBD, DataType.Byte)]
        public byte field_BD; // 0xBD
        [DataOffset(0xBE, DataType.Byte)]
        public byte field_BE; // 0xBE
        [DataOffset(0xC0, DataType.Byte)]
        public byte field_C0; // 0xC0

        [DataOffset(0xC1, DataType.ByteArray, 6)]
        public byte[] field_C1 = new byte[6]; // 0xC1
        [DataOffset(0xC7, DataType.Byte)]
        public byte field_C7;// 0xC7

        [DataOffset(0x100, DataType.Byte)]
        public byte field_100; // 0x100
        [DataOffset(0x101, DataType.Byte)]
        public byte field_101; // 0x101
        [DataOffset(0x102, DataType.SWord)]
        public short field_102; // 0x102

        [DataOffset(0x10C, DataType.Byte)]
        public byte field_10C; // 0x10C
        [DataOffset(0x10D, DataType.Byte)]
        public byte field_10D; // 0x10D
        [DataOffset(0x10E, DataType.Byte)]
        public byte field_10E; // 0x10E
        [DataOffset(0x10F, DataType.Byte)]
        public byte field_10F; // 0x10F
        [DataOffset(0x110, DataType.SByte)]
        public sbyte field_110; // 0x110

        [DataOffset(0x111, DataType.Byte)]
        public byte field_111; // 0x111
        [DataOffset(0x112, DataType.Byte)]
        public byte field_112; // 0x112
        [DataOffset(0x113, DataType.Byte)]
        public byte field_113; // 0x113
        [DataOffset(0x114, DataType.Byte)]
        public byte field_114; // 0x114
        [DataOffset(0x115, DataType.Byte)]
        public byte field_115; // 0x115
        [DataOffset(0x116, DataType.Byte)]
        public byte field_116; // 0x116
        [DataOffset(0x117, DataType.Byte)]
        public byte field_117; // 0x117
        [DataOffset(0x118, DataType.Byte)]
        public byte field_118; // 0x118
        [DataOffset(0x119, DataType.Byte)]
        public byte field_119; // 0x119
        [DataOffset(0x11A, DataType.Byte)]
        public byte field_11A; // 0x11a
        [DataOffset(0x11B, DataType.Byte)]
        public byte field_11B; // 0x11b
        [DataOffset(0x11C, DataType.SByte)]
        public sbyte field_11C; // 0x11c

        public const int StructSize = 0x011D;


        public PoolRadPlayer(byte[] data)
        {
            DataIO.ReadObject(this, data, 0);
        }

        public PoolRadPlayer(Player player)
        {
            race = (byte)player.race;
            sex = player.sex;

            name = player.name;

            stat_str = (byte)player.stats2.Str.full;
            stat_int = (byte)player.stats2.Int.full;
            stat_wis = (byte)player.stats2.Wis.full;
            stat_dex = (byte)player.stats2.Dex.full;
            stat_con = (byte)player.stats2.Con.full;
            stat_cha = (byte)player.stats2.Cha.full;
            stat_str00 = (byte)player.stats2.Str00.full;

            player.spellList.Save(field_17, 0, field_17.Length);

            thac0 = player.thac0;
            _class = (byte)player._class;
            age = player.age;
            hp_max = player.hit_point_max;

            player.spellBook.Save(field_33, 56);

            field_6B = player.attackLevel;
            icon_dimensions = player.icon_dimensions;

            System.Array.Copy(player.saveVerse, saveVerse, 5);

            field_72 = player.base_movement;
            field_73 = player.HitDice;
            field_74 = player.lost_lvls;
            field_75 = player.lost_hp;
            field_76 = player.level_undead;

            System.Array.Copy(player.thief_skills, field_77, 8);

            field_83 = player.field_F6;
            field_84 = player.control_morale;
            field_85 = player.npcTreasureShareCount;
            field_86 = player.field_F9;
            field_87 = player.field_FA;

            field_88 = (ushort)player.Money.GetCoins(Money.Copper);
            field_8A = (ushort)player.Money.GetCoins(Money.Silver);
            field_8C = (ushort)player.Money.GetCoins(Money.Electrum);
            field_8E = (ushort)player.Money.GetCoins(Money.Gold);
            field_90 = (ushort)player.Money.GetCoins(Money.Platinum);
            field_92 = (ushort)player.Money.GetCoins(Money.Gems);
            field_94 = (ushort)player.Money.GetCoins(Money.Jewelry);

            System.Array.Copy(player.ClassLevel, field_96, 8);

            field_9F = (byte)player.monsterType;
            if (player.monsterType == MonsterType.animal)
            {
                field_9F = (byte)MonsterType.animal;
            }
            field_A0 = player.alignment;

            field_A1 = player.attacksCount;
            field_A2 = player.baseHalfMoves;
            field_A3 = player.attack1_DiceCountBase;
            field_A4 = player.attack2_DiceCountBase;
            field_A5 = player.attack1_DiceSizeBase;
            field_A6 = player.attack2_DiceSizeBase;
            field_A7 = player.attack1_DamageBonusBase;
            field_A8 = player.attack2_DamageBonusBase;

            field_A9 = player.base_ac;
            field_AA = player.field_125;
            field_AB = player.mod_id;

            field_AC = player.exp;
            field_B0 = player.classFlags;
            field_B1 = player.hit_point_rolled;

            for (int var_2 = 1; var_2 <= 3; var_2++)
            {
                field_B2[var_2 - 1] = player.spellCastCount[0, var_2 - 1];
                field_B5[var_2 - 1] = player.spellCastCount[2, var_2 - 1];
            }

            field_B8 = player.field_13C;

            field_BA = player.field_13E;
            field_BB = player.field_13F;

            field_BC = player.field_140;
            field_BD = player.head_icon;
            field_BE = player.weapon_icon;
            field_C0 = player.icon_size;

            System.Array.Copy(player.icon_colours, field_C1, 6);

            field_100 = player.weaponsHandsUsed;
            field_101 = (byte)player.field_186;
            field_102 = player.weight;

            field_10C = (byte)player.health_status;
            field_10D = (byte)(player.in_combat ? 1 : 0);
            field_10E = (byte)player.combat_team;
            field_10F = (byte)player.quick_fight;
            field_110 = (sbyte)player.hitBonus;

            field_111 = player.ac;
            field_112 = player.ac_behind;

            field_113 = player.attack1_AttacksLeft;
            field_114 = player.attack2_AttacksLeft;

            field_115 = player.attack1_DiceCount;
            field_116 = player.attack2_DiceCount;

            field_117 = player.attack1_DiceSize;
            field_118 = player.attack2_DiceSize;

            field_119 = (byte)player.attack1_DamageBonus;
            field_11A = player.attack2_DamageBonus;
            field_11B = player.hit_point_current;
            field_11C = (sbyte)player.movement;
        }

        public Player Load()
        {
            Player player = new Player();

            player.race = (Race)race;
            player.sex = sex;

            player.name = name;

            player.stats2.Str.Load(stat_str);
            player.stats2.Int.Load(stat_int);
            player.stats2.Wis.Load(stat_wis);
            player.stats2.Dex.Load(stat_dex);
            player.stats2.Con.Load(stat_con);
            player.stats2.Cha.Load(stat_cha);
            player.stats2.Str00.Load(stat_str00);

            player.spellList.Load(field_17, 0, 21);

            player.thac0 = thac0;
            player._class = (ClassId)_class;
            player.age = age;
            player.hit_point_max = hp_max;

            player.spellBook.Load(field_33, 56);

            player.attackLevel = field_6B;
            player.icon_dimensions = icon_dimensions;

            System.Array.Copy(saveVerse, player.saveVerse, 5);

            player.base_movement = field_72;
            player.HitDice = field_73;
            player.multiclassLevel = 0;
            player.lost_lvls = field_74;
            player.lost_hp = field_75;
            player.level_undead = field_76;

            System.Array.Copy(field_77, player.thief_skills, 8);

            player.field_F6 = field_83;
            player.control_morale = field_84;
            player.npcTreasureShareCount = field_85;
            player.field_F9 = field_86;
            player.field_FA = field_87;

            player.Money.SetCoins(Money.Copper, field_88);
            player.Money.SetCoins(Money.Silver, field_8A);
            player.Money.SetCoins(Money.Electrum, field_8C);
            player.Money.SetCoins(Money.Gold, field_8E);
            player.Money.SetCoins(Money.Platinum, field_90);
            player.Money.SetCoins(Money.Gems, field_92);
            player.Money.SetCoins(Money.Jewelry, field_94);

            System.Array.Copy(field_96, player.ClassLevel, 8);

            player.monsterType = (MonsterType)field_9F;
            if (player.monsterType == MonsterType.animal_old)
            {
                player.monsterType = MonsterType.animal;
            }
            player.alignment = field_A0;

            player.attacksCount = field_A1;
            player.baseHalfMoves = field_A2;
            player.attack1_DiceCountBase = field_A3;
            player.attack2_DiceCountBase = field_A4;
            player.attack1_DiceSizeBase = field_A5;
            player.attack2_DiceSizeBase = field_A6;
            player.attack1_DamageBonusBase = field_A7;
            player.attack2_DamageBonusBase = field_A8;

            player.base_ac = field_A9;
            player.field_125 = field_AA;
            player.mod_id = field_AB;

            player.exp = field_AC;
            player.classFlags = field_B0;
            player.hit_point_rolled = field_B1;

            for (int var_2 = 1; var_2 <= 3; var_2++)
            {
                player.spellCastCount[0, var_2 - 1] = field_B2[var_2 - 1];
                player.spellCastCount[2, var_2 - 1] = field_B5[var_2 - 1];
            }

            player.field_13C = field_B8;

            player.field_13E = field_BA;
            player.field_13F = field_BB;

            player.field_140 = field_BC;
            player.head_icon = field_BD;
            player.weapon_icon = field_BE;
            player.icon_size = field_C0;

            System.Array.Copy(field_C1, player.icon_colours, 6);


            //player.field_14c = bp_var_1C0.field_C7; // Item count

            //mov	di, [bp+arg_0] // copy item pointers...
            //les	di, ss:[di-0x1C0]
            //add	di, 0x0CC
            //push	es
            //push	di
            //les	di, int ptr [bp+player.offset]
            //add	di, 0x151
            //push	es
            //push	di
            //mov	ax, 0x34
            //push	ax
            //call	Move(Any &,Any &,Word)

            player.weaponsHandsUsed = field_100;
            player.field_186 = (sbyte)field_101;
            player.weight = field_102;

            player.health_status = (Status)field_10C;
            player.in_combat = field_10D != 0;
            player.combat_team = (CombatTeam)field_10E;
            player.quick_fight = (QuickFight)field_10F;
            player.hitBonus = field_110;

            player.ac = field_111;
            player.ac_behind = field_112;

            player.attack1_AttacksLeft = field_113;
            player.attack2_AttacksLeft = field_114;

            player.attack1_DiceCount = field_115;
            player.attack2_DiceCount = field_116;

            player.attack1_DiceSize = field_117;
            player.attack2_DiceSize = field_118;

            player.attack1_DamageBonus = (sbyte)field_119;
            player.attack2_DamageBonus = field_11A;
            player.hit_point_current = field_11B;
            player.movement = (byte)field_11C;

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
