namespace Classes.PoolRad
{
    public class Player
    {
        public enum MonsterType
        {
            humanoid = 1,
            giant = 2,
            dragon = 3,
            animated_dead = 4,
            genie = 7,
            troll = 10,
            reptile = 11,
            snake = 14,
            animal = 15,
        }

        [DataOffset(0x00, DataType.PString, 15)]
        public string name; // 0x0 - 0x0F
        [DataOffset(0x10, DataType.ByteArray, 7)]
        public byte[] stats = new byte[7]; // 0x10 - 0x16;
        [DataOffset(0x17, DataType.ByteArray, 21)]
        public byte[] memorizedSpells = new byte[21]; // 0x17 Array 0x15, 0x17 - 0x2B
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
        public byte[] spellBook = new byte[56]; // 0x33 Array 0x38, 0x33 - 0x6A
        [DataOffset(0x6B, DataType.Byte)]
        public byte attackLevel; // 0x6B
        [DataOffset(0x6C, DataType.Byte)]
        public byte icon_dimensions; // 0x6C
        [DataOffset(0x6D, DataType.ByteArray, 5)]
        public byte[] saveVerse = new byte[5]; // 0x6D Array 5, 0x6D - 0x71
        [DataOffset(0x72, DataType.Byte)]
        public byte base_movement; // 0x72
        [DataOffset(0x73, DataType.Byte)]
        public byte HitDice; // 0x73
        [DataOffset(0x74, DataType.Byte)]
        public byte lost_lvls; // 0x74
        [DataOffset(0x75, DataType.Byte)]
        public byte lost_hp; // 0x75
        [DataOffset(0x76, DataType.Byte)]
        public byte level_undead; // 0x76
        [DataOffset(0x77, DataType.ByteArray, 8)]
        public byte[] thief_skills = new byte[8]; // 0x77 Array 8, 0x77 - 0x7E
        [DataOffset(0x83, DataType.Byte)]
        public byte field_83; // 0x83
        [DataOffset(0x84, DataType.Byte)]
        public byte control_morale; // 0x84
        [DataOffset(0x85, DataType.Byte)]
        public byte npcTreasureShareCount; // 0x85
        [DataOffset(0x86, DataType.Byte)]
        public byte field_86; // 0x86
        [DataOffset(0x87, DataType.Byte)]
        public byte field_87; // 0x87

        [DataOffset(0x88, DataType.Word)]
        public ushort Copper; // 0x88 - 0x89
        [DataOffset(0x8A, DataType.Word)]
        public ushort Silver; // 0x8A - 0x8B
        [DataOffset(0x8C, DataType.Word)]
        public ushort Electrum; // 0x8C - 0x8D
        [DataOffset(0x8E, DataType.Word)]
        public ushort Gold; // 0x8E - 0x8F
        [DataOffset(0x90, DataType.Word)]
        public ushort Platinum; // 0x90 - 0x91
        [DataOffset(0x92, DataType.Word)]
        public ushort Gems; // 0x92 - 0x93
        [DataOffset(0x94, DataType.Word)]
        public ushort Jewelry; // 0x94 - 0x95

        [DataOffset(0x96, DataType.ByteArray, 8)]
        public byte[] ClassLevel = new byte[8]; // 0x96 Array 8 0x96 - 0x9D
        [DataOffset(0x9E, DataType.Byte)]
        public byte sex; // 0x9E
        [DataOffset(0x9F, DataType.IByte)]
        public MonsterType monsterType; // 0x9F;
        [DataOffset(0xA0, DataType.Byte)]
        public byte alignment; // 0xA0

        [DataOffset(0xA1, DataType.Byte)]
        public byte attacksCount; // 0xA1
        [DataOffset(0xA2, DataType.Byte)]
        public byte baseHalfMoves; // 0xA2
        [DataOffset(0xA3, DataType.Byte)]
        public byte attack1_DiceCountBase; // 0xA3
        [DataOffset(0xA4, DataType.Byte)]
        public byte attack2_DiceCountBase; // 0xA4
        [DataOffset(0xA5, DataType.Byte)]
        public byte attack1_DiceSizeBase; // 0xA5
        [DataOffset(0xA6, DataType.Byte)]
        public byte attack2_DiceSizeBase; // 0xA6
        [DataOffset(0xA7, DataType.Byte)]
        public byte attack1_DamageBonusBase; // 0xA7
        [DataOffset(0xA8, DataType.Byte)]
        public byte attack2_DamageBonusBase; // 0xA8

        [DataOffset(0xA9, DataType.Byte)]
        public byte base_ac; // 0xA9
        [DataOffset(0xAA, DataType.Byte)]
        public byte useStrBonus; // 0xAA
        [DataOffset(0xAB, DataType.Byte)]
        public byte mod_id; // 0xAB

        [DataOffset(0xAC, DataType.Int)]
        public int exp; //0xAC
        [DataOffset(0xB0, DataType.Byte)]
        public byte classFlags; // 0xB0
        [DataOffset(0xB1, DataType.Byte)]
        public byte hit_point_rolled; // 0xB1
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
        public byte head_icon; // 0xBD
        [DataOffset(0xBE, DataType.Byte)]
        public byte weapon_icon; // 0xBE
        [DataOffset(0xC0, DataType.Byte)]
        public byte icon_size; // 0xC0

        [DataOffset(0xC1, DataType.ByteArray, 6)]
        public byte[] icon_colours = new byte[6]; // 0xC1
        [DataOffset(0xC7, DataType.Byte)]
        public byte field_C7;// 0xC7

        [DataOffset(0x100, DataType.Byte)]
        public byte weaponsHandsUsed; // 0x100
        [DataOffset(0x101, DataType.Byte)]
        public byte field_101; // 0x101
        [DataOffset(0x102, DataType.SWord)]
        public short weight; // 0x102

        [DataOffset(0x10C, DataType.Byte)]
        public byte health_status; // 0x10C
        [DataOffset(0x10D, DataType.Byte)]
        public byte field_10D; // 0x10D
        [DataOffset(0x10E, DataType.Byte)]
        public byte combat_team; // 0x10E
        [DataOffset(0x10F, DataType.Byte)]
        public byte quick_fight; // 0x10F
        [DataOffset(0x110, DataType.SByte)]
        public sbyte hitBonus; // 0x110

        [DataOffset(0x111, DataType.Byte)]
        public byte ac; // 0x111
        [DataOffset(0x112, DataType.Byte)]
        public byte ac_behind; // 0x112
        [DataOffset(0x113, DataType.Byte)]
        public byte attack1_AttacksLeft; // 0x113
        [DataOffset(0x114, DataType.Byte)]
        public byte attack2_AttacksLeft; // 0x114
        [DataOffset(0x115, DataType.Byte)]
        public byte attack1_DiceCount; // 0x115
        [DataOffset(0x116, DataType.Byte)]
        public byte attack2_DiceCount; // 0x116
        [DataOffset(0x117, DataType.Byte)]
        public byte attack1_DiceSize; // 0x117
        [DataOffset(0x118, DataType.Byte)]
        public byte attack2_DiceSize; // 0x118
        [DataOffset(0x119, DataType.Byte)]
        public byte attack1_DamageBonus; // 0x119
        [DataOffset(0x11A, DataType.Byte)]
        public byte attack2_DamageBonus; // 0x11a
        [DataOffset(0x11B, DataType.Byte)]
        public byte hit_point_current; // 0x11b
        [DataOffset(0x11C, DataType.SByte)]
        public sbyte movement; // 0x11c

        public const int StructSize = 0x011D;


        public Player(byte[] data)
        {
            DataIO.ReadObject(this, data, 0);
        }

        public Player(Classes.Player player)
        {
            race = (byte)player.race;
            sex = player.sex;

            name = player.name;

            player.stats2.Save(stats);

            Spell.Save(player.spellList, memorizedSpells, memorizedSpells.Length);

            thac0 = player.thac0;
            _class = (byte)player._class;
            age = player.age;
            hp_max = player.hit_point_max;

            Spell.Save(player.spellBook, spellBook, spellBook.Length);

            attackLevel = player.attackLevel;
            icon_dimensions = player.icon_dimensions;

            System.Array.Copy(player.saveVerse, saveVerse, 5);

            base_movement = player.base_movement;
            HitDice = player.HitDice;
            lost_lvls = player.lost_lvls;
            lost_hp = player.lost_hp;
            level_undead = player.level_undead;

            System.Array.Copy(player.thief_skills, thief_skills, 8);

            field_83 = player.field_F6;
            control_morale = player.control_morale;
            npcTreasureShareCount = player.npcTreasureShareCount;
            field_86 = player.field_F9;
            field_87 = player.field_FA;

            Copper = (ushort)player.Money.GetCoins(Money.Copper);
            Silver = (ushort)player.Money.GetCoins(Money.Silver);
            Electrum = (ushort)player.Money.GetCoins(Money.Electrum);
            Gold = (ushort)player.Money.GetCoins(Money.Gold);
            Platinum = (ushort)player.Money.GetCoins(Money.Platinum);
            Gems = (ushort)player.Money.GetCoins(Money.Gems);
            Jewelry = (ushort)player.Money.GetCoins(Money.Jewelry);

            System.Array.Copy(player.ClassLevel, ClassLevel, 8);

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

            for (int var_2 = 1; var_2 <= 3; var_2++)
            {
                field_B2[var_2 - 1] = player.spellCastCount[0, var_2 - 1];
                field_B5[var_2 - 1] = player.spellCastCount[2, var_2 - 1];
            }

            field_B8 = player.field_13C;

            field_BA = player.field_13E;

            field_BB = player.field_13F;
            field_BC = player.field_140;

            head_icon = player.head_icon;
            weapon_icon = player.weapon_icon;
            icon_size = player.icon_size;

            System.Array.Copy(player.icon_colours, icon_colours, 6);

            monsterType = 0;

            weaponsHandsUsed = player.weaponsHandsUsed;
            field_101 = (byte)player.field_186;
            weight = player.weight;

            health_status = (byte)player.health_status;
            field_10D = (byte)(player.in_combat ? 1 : 0);
            combat_team = (byte)player.combat_team;
            quick_fight = (byte)player.quick_fight;
            hitBonus = (sbyte)player.hitBonus;

            ac = player.ac;
            ac_behind = player.ac_behind;

            attack1_AttacksLeft = player.attack1_AttacksLeft;
            attack2_AttacksLeft = player.attack2_AttacksLeft;

            attack1_DiceCount = player.attack1_DiceCount;
            attack2_DiceCount = player.attack2_DiceCount;

            attack1_DiceSize = player.attack1_DiceSize;
            attack2_DiceSize = player.attack2_DiceSize;

            attack1_DamageBonus = (byte)player.attack1_DamageBonus;
            attack2_DamageBonus = player.attack2_DamageBonus;
            hit_point_current = player.hit_point_current;
            movement = (sbyte)player.movement;
        }


        public Classes.Player Load()
        {
            Classes.Player player = new();

            player.race = (Race)race;
            player.sex = sex;

            player.name = name;

            player.stats2.Load(stats);

            Spell.Load(player.spellList, memorizedSpells, memorizedSpells.Length);

            player.thac0 = thac0;
            player._class = (ClassId)_class;
            player.age = age;
            player.hit_point_max = hp_max;

            Spell.Load(player.spellBook, spellBook, spellBook.Length);

            player.attackLevel = attackLevel;
            player.icon_dimensions = icon_dimensions;

            System.Array.Copy(saveVerse, player.saveVerse, 5);

            player.base_movement = base_movement;
            player.HitDice = HitDice;
            player.multiclassLevel = player.HitDice;
            player.lost_lvls = lost_lvls;
            player.lost_hp = lost_hp;
            player.level_undead = level_undead;

            System.Array.Copy(thief_skills, player.thief_skills, 8);

            player.field_F6 = field_83;
            player.control_morale = control_morale;
            player.npcTreasureShareCount = npcTreasureShareCount;
            player.field_F9 = field_86;
            player.field_FA = field_87;

            player.Money.SetCoins(Money.Copper, Copper);
            player.Money.SetCoins(Money.Silver, Silver);
            player.Money.SetCoins(Money.Electrum, Electrum);
            player.Money.SetCoins(Money.Gold, Gold);
            player.Money.SetCoins(Money.Platinum, Platinum);
            player.Money.SetCoins(Money.Gems, Gems);
            player.Money.SetCoins(Money.Jewelry, Jewelry);

            System.Array.Copy(ClassLevel, player.ClassLevel, 8);

            if (monsterType == 0)
            {
                if (name.Contains("HOBGOBLIN"))
                {
                    player.flags |= Flags.DwarfBonus | Flags.RangerBonus;
                }
                if (icon_dimensions == 1)
                {
                    player.flags |= Flags.HeldCharmed;
                }
            }
            else if (monsterType == MonsterType.humanoid)
            {
                player.flags |= Flags.RangerBonus;

                if (name.Contains("BUGBEAR"))
                {
                    player.flags |= Flags.GnomePenalty;
                }
                else if (name.Contains("ORC"))
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
                player.flags |= Flags.DwarfPenalty | Flags.GnomePenalty | Flags.RangerBonus | Flags.Giant;
            }
            else if (monsterType == MonsterType.dragon)
            {
                player.flags |= Flags.Dragon;
            }
            else if (monsterType == MonsterType.animated_dead)
            {
                player.flags |= Flags.Undead;
            }
            else if (monsterType == MonsterType.genie)
            {
                player.flags |= Flags.EvilSummon;
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
            else if (monsterType == MonsterType.snake)
            {
                player.flags |= Flags.Snake | Flags.Animal;
            }
            else if (monsterType == MonsterType.animal)
            {
                player.flags |= Flags.Animal | Flags.Mammal;
            }

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

            for (int var_2 = 1; var_2 <= 3; var_2++)
            {
                player.spellCastCount[0, var_2 - 1] = field_B2[var_2 - 1];
                player.spellCastCount[2, var_2 - 1] = field_B5[var_2 - 1];
            }

            player.field_13C = field_B8;

            player.field_13E = field_BA;
            player.field_13F = field_BB;

            player.field_140 = field_BC;
            player.head_icon = head_icon;
            player.weapon_icon = weapon_icon;
            player.icon_size = icon_size;

            System.Array.Copy(icon_colours, player.icon_colours, 6);


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

            player.weaponsHandsUsed = weaponsHandsUsed;
            player.field_186 = (sbyte)field_101;
            player.weight = weight;

            player.health_status = (Status)health_status;
            player.in_combat = field_10D != 0;
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

            player.attack1_DamageBonus = (sbyte)attack1_DamageBonus;
            player.attack2_DamageBonus = attack2_DamageBonus;
            player.hit_point_current = hit_point_current;
            player.movement = (byte)movement;

            return player;
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
    }
}
