using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;


namespace Classes
{
    public struct StatValue : IDataIO
    {
        readonly int[, ,] raceSexMinMax;
        readonly int[] classMin;
        readonly int[] ageEffects;
        readonly int min;
        readonly int max;

        public StatValue(int[, ,] _raceSexMinMax, int[] _classMin, int[] _ageEffects, int _cur_offset = 0, int _full_offset = 1, int _min = 3, int _max = 25)
        {
            raceSexMinMax = _raceSexMinMax;
            classMin = _classMin;
            ageEffects = _ageEffects;
            cur = full = 0;
            cur_offset = _cur_offset;
            full_offset = _full_offset;
            min = _min;
            max = _max;
        }

        public StatValue(StatValue old, int[,,] _raceSexMinMax, int[] _classMin, int[] _ageEffects, int _cur_offset = 0, int _full_offset = 1, int _min = 3, int _max = 25)
            : this(_raceSexMinMax, _classMin, _ageEffects, _cur_offset, _full_offset, _min, _max)
        {
            cur = old.cur;
            full = old.full;
        }

        public int cur;
        public int full;
        readonly int cur_offset;
        readonly int full_offset;

        public void Load(int val)
        {
            full = cur = val;
        }

        public void Assign(StatValue sv)
        {
            full = sv.full;
            cur = sv.cur;
        }

        public void Inc()
        {
            if (cur < max)
            {
                cur += 1;
                full += 1;
            }
        }

        public void Dec()
        {
            if (cur > min)
            {
                cur -= 1;
                full -= 1;
            }
        }

        public void EnforceRaceSexLimits(Race race, int sex)
        {
            int delta = full - cur;
            if( raceSexMinMax != null )
            {
                cur = Math.Min(raceSexMinMax[(int)race, 1, sex], cur);
                cur = Math.Max(raceSexMinMax[(int)race, 0, sex], cur);
            }
            full = cur + delta;
        }

        public void EnforceClassLimits(ClassId _class)
        {
            int delta = full - cur;
            if (classMin != null)
            {
                cur = Math.Max(classMin[(int)_class], cur);
            }
            full = cur + delta;
        }

        public void AgeEffects(Race race, int age)
        {
            int delta = full - cur;
            for (int i = 0; i < 5; i++)
            {
                if (Limits.RaceAgeBrackets[(int)race, i] < age)
                {
                    cur += ageEffects[i];
                }
            }
            full = cur + delta;
        }

        public void Write(byte[] data, int offset)
        {
            data[offset + cur_offset] = (byte)cur;
            data[offset + full_offset] = (byte)full;
        }

        public void Read(byte[] data, int offset)
        {
            // enforce values in valid range
            cur = Math.Max(Math.Min((int)data[offset + cur_offset], max), min);
            full = Math.Max(Math.Min((int)data[offset + full_offset], max), min);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", cur, full);
        }
    }

    public class PlayerStats : IDataIO
    {
        public StatValue Str = new StatValue(Limits.StrRaceSexMinMax, Limits.StrClassMin, Limits.StrAgeEffect);
        public StatValue Str00 = new StatValue(Limits.Str00RaceSexMinMax, Limits.Str00ClassMin, Limits.Str00AgeEffect, 1, 0, 0, 100);
        public StatValue Con = new StatValue(Limits.ConRaceSexMinMax, Limits.ConClassMin, Limits.ConAgeEffect);
        public StatValue Dex = new StatValue(Limits.DexRaceSexMinMax, Limits.DexClassMin, Limits.DexAgeEffect);
        public StatValue Int = new StatValue(Limits.IntRaceSexMinMax, Limits.IntClassMin, Limits.IntAgeEffect);
        public StatValue Wis = new StatValue(Limits.WisRaceSexMinMax, Limits.WisClassMin, Limits.WisAgeEffect);
        public StatValue Cha = new StatValue(Limits.ChaRaceSexMinMax, Limits.ChaClassMin, Limits.ChaAgeEffect);

        public void ReInit()
        {
            Str = new StatValue(Str, Limits.StrRaceSexMinMax, Limits.StrClassMin, Limits.StrAgeEffect);
            Str00 = new StatValue(Str00, Limits.Str00RaceSexMinMax, Limits.Str00ClassMin, Limits.Str00AgeEffect, 1, 0, 0, 100);
            Con = new StatValue(Con, Limits.ConRaceSexMinMax, Limits.ConClassMin, Limits.ConAgeEffect);
            Dex = new StatValue(Dex, Limits.DexRaceSexMinMax, Limits.DexClassMin, Limits.DexAgeEffect);
            Int = new StatValue(Int, Limits.IntRaceSexMinMax, Limits.IntClassMin, Limits.IntAgeEffect);
            Wis = new StatValue(Wis, Limits.WisRaceSexMinMax, Limits.WisClassMin, Limits.WisAgeEffect);
            Cha = new StatValue(Cha, Limits.ChaRaceSexMinMax, Limits.ChaClassMin, Limits.ChaAgeEffect);
    }

        void IDataIO.Write(byte[] data, int offset)
        {
            Str.Write(data, offset + 0x00);
            Int.Write(data, offset + 0x02);
            Wis.Write(data, offset + 0x04);
            Dex.Write(data, offset + 0x06);
            Con.Write(data, offset + 0x08);
            Cha.Write(data, offset + 0x0a);
            Str00.Write(data, offset + 0x0c);
        }

        void IDataIO.Read(byte[] data, int offset)
        {
            Str.Read(data, offset + 0x00);
            Int.Read(data, offset + 0x02);
            Wis.Read(data, offset + 0x04);
            Dex.Read(data, offset + 0x06);
            Con.Read(data, offset + 0x08);
            Cha.Read(data, offset + 0x0a);
            Str00.Read(data, offset + 0x0c);
        }

        public void Save(byte[] data)
        {
            Str.Write(data, 0x00);
            Int.Write(data, 0x02);
            Wis.Write(data, 0x04);
            Dex.Write(data, 0x06);
            Con.Write(data, 0x08);
            Cha.Write(data, 0x0a);
            Str00.Write(data, 0x0c);
        }

        public void Load(byte[] data)
        {
            Str.Read(data, 0x00);
            Int.Read(data, 0x02);
            Wis.Read(data, 0x04);
            Dex.Read(data, 0x06);
            Con.Read(data, 0x08);
            Cha.Read(data, 0x0a);
            Str00.Read(data, 0x0c);
        }

        public void Assign(PlayerStats ps)
        {
            Str.Assign(ps.Str);
            Int.Assign(ps.Int);
            Wis.Assign(ps.Wis);
            Dex.Assign(ps.Dex);
            Con.Assign(ps.Con);
            Cha.Assign(ps.Cha);
            Str00.Assign(ps.Str00);
        }

		public void Dec(int idx)
		{
			switch (idx)
			{
				case 0: Str.Dec(); break;
				case 1: Int.Dec(); break;
				case 2: Wis.Dec(); break;
				case 3: Dex.Dec(); break;
				case 4: Con.Dec(); break;
				case 5: Cha.Dec(); break;
				default: throw new IndexOutOfRangeException(string.Format("idx {0} not in [0-5]", idx));
			}
		}

		public void Inc(int idx)
		{
			switch (idx)
			{
				case 0: Str.Inc(); break;
				case 1: Int.Inc(); break;
				case 2: Wis.Inc(); break;
				case 3: Dex.Inc(); break;
				case 4: Con.Inc(); break;
				case 5: Cha.Inc(); break;
				default: throw new IndexOutOfRangeException(string.Format("idx {0} not in [0-5]", idx));
			}
		}

        public StatValue this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return Str;
                    case 1: return Int;
                    case 2: return Wis;
                    case 3: return Dex;
                    case 4: return Con;
                    case 5: return Cha;
                    default: throw new IndexOutOfRangeException(string.Format("idx {0} not in [0-5]", idx));
                }
            }
        }

        public override string ToString()
        {
            return string.Format("S:{0} ({6}),I:{1},W:{2},C:{3},D:{4},Ch:{5}", Str, Int, Wis, Con, Dex, Cha, Str00);
        }
    }

    public class SpellBook : IDataIO
    {
        public List<Spells> spellBook = new List<Spells>();
        void IDataIO.Write(byte[] data, int offset)
        {
            foreach (SpellEntry spell in gbl.spellCastingTable)
            {
                if (spell != null && spell.spellIdx <= (byte)Spells.bestow_curse_MU)
                {
                    data[offset + spell.spellIdx - 1] = (byte)(spellBook.Contains((Spells)spell.spellIdx) ? 1 : 0);
                }
            }
        }

        void IDataIO.Read(byte[] data, int offset)
        {
            for (int i = 0; i < 100; i++)
            {
                if (data[offset + i] != 0)
                {
                    if (!spellBook.Contains((Spells)(i+1)))
                    {
                        spellBook.Add((Spells)(i + 1));
                    }
                }
            }
        }

        public void Save(byte[] data, int length)
        {
            for (int i = 0; i < length; i++)
            {
                data[i] = (byte)(spellBook.Contains((Spells)(i + 1)) ? 1 : 0);
            }
        }

        public void Load(byte[] data, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (data[i] != 0)
                {
                    spellBook.Add((Spells)(i + 1));
                }
            }
        }

        public bool KnowsSpell(Spells spell)
        {
            return spellBook.Contains(spell);
        }

        public void LearnSpell(Spells spell)
        {
            if (!spellBook.Contains(spell))
            {
                spellBook.Add(spell);
            }
        }

        public void UnlearnSpell(Spells spell)
        {
            spellBook.Remove(spell);
        }
    }

    public struct ClassLevels
    {

    }

    public class ActiveItems
    {
        const int ItemSlots = 13;
        Item[] itemArray = new Item[ItemSlots]; // 0x151[]

        public byte PrimaryWeaponHandCount()
        {
            if (primaryWeapon == null) return 0;

            return primaryWeapon.HandsCount();
        }

        public byte SecondaryWeaponHandCount()
        {
            if (secondaryWeapon == null) return 0;

            return secondaryWeapon.HandsCount();
        }     

        public Item primaryWeapon // field_151
        {// 0x151
            get { return itemArray[0]; }
            set { itemArray[0] = value; }
        }
        public Item secondaryWeapon // field_155
        { // 0x155 
            get { return itemArray[1]; }
            set { itemArray[1] = value; }
        }

        public Item armor // field_159
        { // 0x159 
            get { return itemArray[2]; }
            set { itemArray[2] = value; }
        }

        public Item field_15D
        { // 0x15D
            get { return itemArray[3]; }
            set { itemArray[3] = value; }
        }
        public Item field_161
        { // 0x161
            get { return itemArray[4]; }
            set { itemArray[4] = value; }
        }
        public Item field_165
        { // 0x165
            get { return itemArray[5]; }
            set { itemArray[5] = value; }
        }
        public Item field_169
        { // 0x169 
            get { return itemArray[6]; }
            set { itemArray[6] = value; }
        }
        public Item field_16D
        { // 0x16D
            get { return itemArray[7]; }
            set { itemArray[7] = value; }
        }
        public Item field_171
        { // 0x171
            get { return itemArray[8]; }
            set { itemArray[8] = value; }
        }
        public Item Item_ptr_01
        { // 0x175
            get { return itemArray[9]; }
            set { itemArray[9] = value; }
        }
        public Item Item_ptr_02
        {// 0x179
            get { return itemArray[10]; }
            set { itemArray[10] = value; }
        }
        public Item arrows
        {// 0x17d
            get { return itemArray[11]; }
            set { itemArray[11] = value; }
        }
        public Item quarrels
        { // 0x181
            get { return itemArray[12]; }
            set { itemArray[12] = value; }
        }

        public Item this[ItemSlot slot]
        {
            get
            {
                return itemArray[(int)slot];
            }
            set
            {
                itemArray[(int)slot] = value;
            }
        }

        public void Reset()
        {
            for (int slot = 0; slot < ItemSlots; slot++)
            {
                itemArray[slot] = null;
            }
        }

        public void UndreadyAll(int classFlags)
        {
            for (int item_slot = 0; item_slot < ItemSlots; item_slot++)
            {
                if (itemArray[item_slot] != null &&
                    (gbl.ItemDataTable[itemArray[item_slot].type].classFlags & classFlags) == 0 &&
                    itemArray[item_slot].cursed == false)
                {
                    itemArray[item_slot].readied = false;
                }
            }
        }
    }


    public class Control
    {
        public const byte PC_Base = 0;
        public const byte PC_Mask = 0x7F;
        public const byte NPC_Base = 0x80;
        public const byte NPC_Berserk = 0xB2;
        public const byte PC_Berserk = 0xB3;
    }



    /// <summary>
    /// Summary description for Player.
    /// </summary>
    public class Player
    {
        public string name;

        public PlayerStats stats2;

        public SpellList spellList; // ox1e was spell_list

        public byte spell_to_learn_count; // 0x72;
        public sbyte thac0; // 0x73; field_73

        public Race race; // 0x74;

        public ClassId _class; // 0x75;
        public short age; // 0x76;

        public byte hit_point_max; // 0x78;

        public SpellBook spellBook; // 0x79 - 0xDC

        public byte attackLevel; // 0xdd; field_DD
        public byte icon_dimensions; // 0xde;
        public byte[] saveVerse = new byte[5]; // 0xdf; field_DF 

        public byte base_movement; // 0xe4;
        public byte HitDice; // 0xe5; HitDice?
        public byte multiclassLevel; // 0xe6;
        public byte lost_lvls; // 0xe7;
        public byte lost_hp; // 0xe8;
        public byte level_undead; // 0xe9;
        public byte[] thief_skills = new byte[8]; // 0xeA; [] was 1 offset @ 0xe9, pick_pockets, open_locks, find_remove_traps, move_silently, hide_in_shadows, hear_noise, climb_walls, read_languages
        public List<Affect> affects; // f2 - affect_ptr

        public byte field_F6; // 0xf6; field_F6
        public byte control_morale; // 0xf7; field_F7
        public byte npcTreasureShareCount; // 0xf8;
        public byte field_F9; // 0xf9
        public byte field_FA; // 0xfa;

        public MoneySet Money;

        public byte[] ClassLevel = new byte[8]; /* Skill_A_lvl */

        [XmlIgnore]
        public byte cleric_lvl // 0x109;
        {
            get { return ClassLevel[0]; }
            set { ClassLevel[0] = value; }
        }
        [XmlIgnore]
        public byte druid_lvl // 0x10a;
        {
            get { return ClassLevel[1]; }
            set { ClassLevel[1] = value; }
        }
        [XmlIgnore]
        public byte fighter_lvl // 0x10b;
        {
            get { return ClassLevel[2]; }
            set { ClassLevel[2] = value; }
        }
        [XmlIgnore]
        public byte paladin_lvl // 0x10c;
        {
            get { return ClassLevel[3]; }
            set { ClassLevel[3] = value; }
        }
        [XmlIgnore]
        public byte ranger_lvl // 0x10d;
        {
            get { return ClassLevel[4]; }
            set { ClassLevel[4] = value; }
        }
        [XmlIgnore]
        public byte magic_user_lvl // 0x10e;
        {
            get { return ClassLevel[5]; }
            set { ClassLevel[5] = value; }
        }
        [XmlIgnore]
        public byte thief_lvl // 0x10f;
        {
            get { return ClassLevel[6]; }
            set { ClassLevel[6] = value; }
        }
        [XmlIgnore]
        public byte monk_lvl // 0x110;
        {
            get { return ClassLevel[7]; }
            set { ClassLevel[7] = value; }
        }

        public byte[] ClassLevelsOld = new byte[8];


        [XmlIgnore]
        public byte cleric_old_lvl // 0x111;
        {
            get { return ClassLevelsOld[0]; }
            set { ClassLevelsOld[0] = value; }
        }
        [XmlIgnore]
        public byte druid_old_lvl // 0x112;
        {
            get { return ClassLevelsOld[1]; }
            set { ClassLevelsOld[1] = value; }
        }
        [XmlIgnore]
        public byte fighter_old_lvl // 0x113;
        {
            get { return ClassLevelsOld[2]; }
            set { ClassLevelsOld[2] = value; }
        }
        [XmlIgnore]
        public byte paladin_old_lvl // 0x114;
        {
            get { return ClassLevelsOld[3]; }
            set { ClassLevelsOld[3] = value; }
        }
        [XmlIgnore]
        public byte ranger_old_lvl // 0x115;
        {
            get { return ClassLevelsOld[4]; }
            set { ClassLevelsOld[4] = value; }
        }
        [XmlIgnore]
        public byte magic_user_old_lvl // 0x116;
        {
            get { return ClassLevelsOld[5]; }
            set { ClassLevelsOld[5] = value; }
        }
        [XmlIgnore]
        public byte thief_old_lvl // 0x117;
        {
            get { return ClassLevelsOld[6]; }
            set { ClassLevelsOld[6] = value; }
        }
        //public byte monk_old_level // 0x118;
        //{
        //    get { return ClassLevelsOld[7]; }
        //    set { ClassLevelsOld[7] = value; }
        //}

        public int SkillCount()
        {
            int skill_count = 0;
            for (SkillType skill = SkillType.Cleric; skill <= SkillType.Monk; skill++)
            {
                if (ClassLevel[(int)skill] > 0)
                {
                    skill_count++;
                }
            }
            return skill_count;
        }

        public int SkillLevel(params SkillType[] skills)
        {
            int level = 0;

            foreach (SkillType skill in skills)
            {
                level = System.Math.Max(level,
                    ClassLevel[(int)skill] + (ClassLevelsOld[(int)skill] * DualClassExceedsPreviousLevel()));
            }

            return level;
        }

        public int TurnLevel()
        {
            // paladins turn at level - 2
            return System.Math.Max(SkillLevel(SkillType.Cleric), SkillLevel(SkillType.Paladin) - 2);
        }

        public byte sex; // 0x119;
        public byte alignment; // 0x11b;
        /// <summary>
        /// half-attacks count
        /// </summary>
        public byte attacksCount; // 0x11c;
        public byte baseHalfMoves; // 0x11d;
        public byte attack1_DiceCountBase; // 0x11e;
        public byte attack2_DiceCountBase; // 0x11f;
        public byte attack1_DiceSizeBase; // 0x120;
        public byte attack2_DiceSizeBase; // 0x121;
        public byte attack1_DamageBonusBase; // 0x122;
        public byte attack2_DamageBonusBase; // 0x123;
        public byte base_ac; // 0x124;
        public byte field_125; // 0x125;
        public byte mod_id; // 0x126; field_126
        public int exp; // 0x127
        public byte classFlags; // 0x12b;
        public byte hit_point_rolled; // 0x12c;

        //[XmlIgnore]
        //public byte[,] spellCastCount = new byte[3, 5]; // 0x12d - field_12D
        public byte[][] spellCastCount = new byte[3][];

        public short field_13C; // 0x13c
        public byte field_13E; // 0x13e;
        public byte field_13F; // 0x13f;
        public byte field_140; // 0x140;
        public byte head_icon; // 0x141;
        public byte weapon_icon; // 0x142;
        public byte icon_id; // 0x143;
        public byte icon_size; // 0x144; field_144  1 small 2 normal
        public byte[] icon_colours = new byte[6]; // 0x145 = field_144[1] // byte[6]
        public Flags flags; // 0x14b;

        //[DataOffset(0x14c, DataType.Byte)]
        //public byte field_14C; // 0x14c; // items.Count
        public const int MaxItems = 16;


        public List<Item> items; // 0x14d
        //public Item itemsPtr; // 0x14d

        public ActiveItems activeItems = new ActiveItems();


        public byte weaponsHandsUsed; // 0x185;
        public sbyte field_186; // 0x186;
        public short weight; // 0x187

        public Action actions; // 0x18d
        public byte paladinCuresLeft; // 0x191 field_191
        public byte field_192; // 0x192
        public byte field_193; // 0x193
        public byte field_194; // 0x194
        public Status health_status; // 0x195
        public bool in_combat; // 0x196
        public CombatTeam combat_team; // 0x197 0 - our team, 1 - enemy
        public QuickFight quick_fight; // 0x198
        public int hitBonus; // 0x199 field_199
        public byte ac; // 0x19a
        public int DisplayAc { get { return 60 - ac; } }

        public byte ac_behind; // 0x19b field_19B

        public byte AttacksLeft(int index)
        {
            switch (index)
            {
                case 1:
                    return attack1_AttacksLeft;
                case 2:
                    return attack2_AttacksLeft;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public void AttacksLeftSet(int index, byte value)
        {
            switch (index)
            {
                case 1:
                    attack1_AttacksLeft = value;
                    break;
                case 2:
                    attack2_AttacksLeft = value;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public void AttacksLeftDec(int index)
        {
            switch (index)
            {
                case 1:
                    attack1_AttacksLeft -= 1;
                    break;
                case 2:
                    attack2_AttacksLeft -= 1;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public byte attack1_AttacksLeft; // 0x19c - field_19C
        public byte attack2_AttacksLeft; // 0x19d - field_19D

        public byte attackDiceCount(int index)
        {
            switch (index)
            {
                case 1:
                    return attack1_DiceCount;
                case 2:
                    return attack2_DiceCount;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public byte attack1_DiceCount; // 0x19e field_19E
        public byte attack2_DiceCount; // 0x19f

        public byte attackDiceSize(int index)
        {
            switch (index)
            {
                case 1:
                    return attack1_DiceSize;
                case 2:
                    return attack2_DiceSize;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public byte attack1_DiceSize; // 0x1a0 field_1A0
        public byte attack2_DiceSize; // 0x1a1

        public byte attackDamageBonus(int index)
        {
            switch (index)
            {
                case 1:
                    return (byte)attack1_DamageBonus;
                case 2:
                    return attack2_DamageBonus;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public sbyte attack1_DamageBonus; // 0x1a2
        public byte attack2_DamageBonus; // 0x1a3

        public byte hit_point_current; // 0x1a4

        public byte movement; // 0x1a5 initiative

        public const int StructSize = 0x1A6;

        public Player()
        {
            spellCastCount = new byte[3][];
            for (int i=0; i<3; i++)
            {
                spellCastCount[i] = new byte[5];
            }

            stats2 = new PlayerStats();

            spellBook = new SpellBook();

            name = string.Empty;
            items = new List<Item>();
            affects = new List<Affect>();

            actions = null;
            Money = new MoneySet();
            spellList = new SpellList();

        }


        public Player ShallowClone()
        {
            Player p = (Player)this.MemberwiseClone();
            p.stats2.Assign(this.stats2);
            p.spellBook = this.spellBook;
            return p;
        }


        public override string ToString()
        {
            return name;
        }


        public bool CanDuelClass()
        {
            if (race != Race.human)
            {
                return false;
            }

            for (ClassId index = ClassId.cleric; index <= ClassId.monk; index++)
            {
                if (ClassLevelsOld[(int)index] > 0)
                {
                    return false;
                }
            }

            return true;
        }

        int DualClassExceedsPreviousLevel() // sub_6B3D1
        {
            if (DuelClassCurrentLevel() > multiclassLevel)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        int DuelClassCurrentLevel()
        {
            if (race != Race.human)
            {
                return 0;
            }

            int loop_var = 0;

            while (loop_var < 7 &&
                ClassLevel[loop_var] == 0)
            {
                loop_var++;
            }

            return ClassLevel[loop_var];
        }

        public CombatTeam OppositeTeam()
        {
            return (combat_team == CombatTeam.Ours) ? CombatTeam.Enemy : CombatTeam.Ours;
        }

        public bool HasAffect(Affects type)
        {
            return affects.Exists(aff => aff.type == type);
        }

        public Affect GetAffect(Affects type)
        {
            return affects.Find(aff => aff.type == type);
        }

        static Affects[] held_affects = { Affects.snake_charm, Affects.paralyze, Affects.sleep, Affects.helpless };

        public bool IsHeld()
        {
            return Array.Exists(held_affects, affect => HasAffect(affect));
        }


        public void RemoveWeight(int amount)
        {
            weight -= (short)amount;
        }


        public void AddWeight(int amount)
        {
            weight += (short)amount;
        }

    }
}
