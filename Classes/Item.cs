using System;
using System.ComponentModel;
using System.Text;

namespace Classes
{
    /// <summary>
    /// Summary description for Item.
    /// </summary>
    [Serializable]
    public class Item
    {
        public string name; // 0x00

        public ItemType type; // 0x2e;
        public byte field_2EArray(int index)
        {
            switch (index)
            {
                case 1: return (byte)namenum[0];
                case 2: return (byte)namenum[1];
                case 3: return (byte)namenum[2];
                default: throw new NotSupportedException();
            }
        }
        public ItemNames[] namenum;

        public int plus; // 0x32 
        public byte plus_save; // 0x33 
        public bool readied; // 0x34
        public byte hidden_names_flag; // 0x35 
        public bool cursed; // 0x36 
        public short weight; // 0x37
        public int count;   // 0x39 
        public short _value; // 0x3A "seams like value is in electrum, as value is doubled.";
        public Affects affect_1; // 0x3C
        public Affects affect_2; // 0x3D
        public Affects affect_3; // 0x3E

        public byte HandsCount()
        {
            return gbl.ItemDataTable[type].handsCount;
        }

        public bool CheckMaskedAffect(int i, byte masked_val)
        {
            return ((int)getAffect(i) > 0x7F && ((int)getAffect(i) & 0x7F) == masked_val);
        }

        public bool IsScroll()
        {
            return (gbl.ItemDataTable[type].item_slot >= ItemSlot.Arrow && gbl.ItemDataTable[type].item_slot <= ItemSlot.slot_13);
        }

        public Affects getAffect(int i)
        {
            switch (i)
            {
                case 1:
                    return affect_1;
                case 2:
                    return affect_2;
                case 3:
                    return affect_3;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
        public void setAffect(int i, Affects value)
        {
            switch (i)
            {
                case 1:
                    affect_1 = value;
                    break;
                case 2:
                    affect_2 = value;
                    break;
                case 3:
                    affect_3 = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        public const int StructSize = 0x3F;

        public Item()
        {
            namenum = new ItemNames[3];
            Clear();
        }


        public Item(Affects _affect_3, Affects _affect_2, Affects _affect_1, short __value, byte _count,
            short _weight, bool _cursed, byte _name_flags, bool _readied, byte _plus_save, sbyte _plus, ItemNames _namenum3,
            ItemNames _namenum2, ItemNames _namenum1, ItemType _type, bool AddToLibrary)
        {
            name = string.Empty;
            type = _type;
            namenum = new ItemNames[3];
            namenum[0] = _namenum1;
            namenum[1] = _namenum2;
            namenum[2] = _namenum3;
            plus = _plus;
            plus_save = _plus_save;
            readied = _readied;
            hidden_names_flag = _name_flags;
            cursed = _cursed;
            weight = _weight;
            count = _count;
            _value = __value;
            affect_1 = _affect_1;
            affect_2 = _affect_2;
            affect_3 = _affect_3;

            if (AddToLibrary)
            {
                ItemLibrary.Add(this);
            }
        }

        public Item(byte _affect_3, Affects _affect_2, Affects _affect_1, short __value, byte _count,
            short _weight, bool _cursed, byte _name_flags, bool _readied, byte _plus_save, sbyte _plus, ItemNames _namenum3,
            ItemNames _namenum2, ItemNames _namenum1, ItemType _type, bool AddToLibrary) :
            this((Affects)_affect_3, _affect_2, _affect_1, __value, _count, _weight, _cursed, _name_flags,
                _readied, _plus_save, _plus, _namenum3, _namenum2, _namenum1, _type, AddToLibrary)
        {
        }

        public Item(short __value, byte _count,
            short _weight, bool _cursed, byte _name_flags, bool _readied, byte _plus_save, sbyte _plus, ItemNames _namenum3,
            ItemNames _namenum2, ItemNames _namenum1, ItemType _type, bool AddToLibrary) :
            this(Affects.none, Affects.none, Affects.none, __value, _count, _weight, _cursed, _name_flags,
                _readied, _plus_save, _plus, _namenum3, _namenum2, _namenum1, _type, AddToLibrary)
        {
        }

        public Item(byte[] data, int offset)
        {
            name = Sys.ArrayToString(data, offset, 0x2a);

            type = (ItemType)data[offset + 0x2e];
            namenum = new ItemNames[3];
            namenum[0] = (ItemNames)data[offset + 0x2f];
            namenum[1] = (ItemNames)data[offset + 0x30];
            namenum[2] = (ItemNames)data[offset + 0x31];
            plus = (sbyte)data[offset + 0x32];
            plus_save = data[offset + 0x33];
            readied = (data[offset + 0x34] != 0);
            hidden_names_flag = data[offset + 0x35];
            cursed = (data[offset + 0x36] != 0);

            weight = Sys.ArrayToShort(data, offset + 0x37);
            count = data[offset + 0x39];
            _value = Sys.ArrayToShort(data, offset + 0x3a);
            affect_1 = (Affects)data[offset + 0x3C];
            affect_2 = (Affects)data[offset + 0x3D];
            affect_3 = (Affects)data[offset + 0x3E];

            ItemLibrary.Add(this);
            //AddItemsText(string.Format("{0},{1},{2},{3},{4}", type, namenum1, namenum2, namenum3, GenerateName(0)));
        }

        public Item ShallowClone()
        {
            Item i = (Item)this.MemberwiseClone();
            return i;
        }

        public void Clear()
        {
            name = string.Empty;

            type = 0;
            namenum[0] = 0;
            namenum[1] = 0;
            namenum[2] = 0;
            plus = 0;
            plus_save = 0;
            readied = false;
            hidden_names_flag = 0;
            cursed = false;
            weight = 0;
            count = 0;
            _value = 0;
            affect_1 = 0;
            affect_2 = 0;
            affect_3 = 0;
        }

        public enum ItemNames
        {
            WEAPONBattle_Axe = ItemType.BattleAxe,
            WEAPONHand_Axe = ItemType.HandAxe,
            WEAPONBardiche = ItemType.Bardiche,
            WEAPONBec_De_Corbin = ItemType.BecDeCorbin,
            WEAPONBillMINUSGuisarme = ItemType.BillGuisarme,
            WEAPONBo_Stick = ItemType.BoStick,
            WEAPONClub = ItemType.Club,
            WEAPONDagger = ItemType.Dagger,
            WEAPONDart = ItemType.Dart,
            WEAPONFauchard = ItemType.Fauchard,
            WEAPONFauchardMINUSFork = ItemType.FauchardFork,
            WEAPONFlail = ItemType.Flail,
            WEAPONMilitary_Fork = ItemType.MilitaryFork,
            WEAPONGlaive = ItemType.Glaive,
            WEAPONGlaiveMINUSGuisarme = ItemType.GlaiveGuisarme,
            WEAPONGuisarme = ItemType.Guisarme,
            WEAPONGuisarmeMINUSVoulge = ItemType.GuisarmeVoulge,
            WEAPONHalberd = ItemType.Halberd,
            WEAPONLucern_Hammer = ItemType.LucernHammer,
            WEAPONHammer = ItemType.Hammer,
            WEAPONJavelin = ItemType.Javelin,
            WEAPONJo_Stick = ItemType.JoStick,
            WEAPONMace = ItemType.Mace,
            WEAPONMorning_Star = ItemType.MorningStar,
            WEAPONPartisan = ItemType.Partisan,
            WEAPONMilitary_Pick = ItemType.MilitaryPick,
            WEAPONAwl_Pike = ItemType.AwlPike,
            WEAPONRanseur = ItemType.Ranseur,
            WEAPONScimitar = ItemType.Scimitar,
            WEAPONSpear = ItemType.Spear,
            WEAPONSpetum = ItemType.Spetum,
            WEAPONQuarter_Staff = ItemType.QuarterStaff,
            WEAPONBastard_Sword = ItemType.BastardSword,
            WEAPONBroad_Sword = ItemType.BroadSword,
            WEAPONLong_Sword = ItemType.LongSword,
            WEAPONShort_Sword = ItemType.ShortSword,
            WEAPONTwoMINUSHanded_Sword = ItemType.TwoHandedSword,
            WEAPONTrident = ItemType.Trident,
            WEAPONVoulge = ItemType.Voulge,

            WEAPONComposite_Long_Bow = ItemType.CompositeLongBow,
            WEAPONComposite_Short_Bow = ItemType.CompositeShortBow,
            WEAPONLong_Bow = ItemType.LongBow,
            WEAPONShort_Bow = ItemType.ShortBow,
            WEAPONHeavy_Crossbow = ItemType.HeavyCrossbow,
            WEAPONLight_Crossbow = ItemType.LightCrossbow,
            WEAPONSling = ItemType.Sling,

            WEAPONArrow = 0x3D,
            WEAPONQuarrel = ItemType.Quarrel,

            ARMORMail = 0x30,
            ARMORArmor = 0x31,

            ARMORLeather = ItemType.LeatherArmor,
            ARMORPadded = ItemType.PaddedArmor,
            ARMORStudded = ItemType.StuddedLeather,
            ARMORRing = ItemType.RingMail,
            ARMORScale = ItemType.ScaleMail,
            ARMORChain = ItemType.ChainMail,
            ARMORSplint = ItemType.SplintMail,
            ARMORBanded = ItemType.BandedMail,
            ARMORPlate = ItemType.PlateMail,
            ARMORShield = ItemType.Shield,

            Woods = 0x3C,
            // 0x3E,
            // 0x3F,
            Potion = 0x40,
            Scroll = 0x41,

            Ring = 0x42,
            Rod = 0x43,
            Stave = 0x44,
            Wand = 0x45,
            Jug = 0x46,

            Amulet = 0x47,
            Dragon_Breath = 0x48,
            Bag = 0x49,
            Defoliation = 0x4A,
            Ice_Storm = 0x4B,

            Book = 0x4C,
            Boots = 0x4D,
            Hornets_Nest = 0x4E,
            Bracers = 0x4F,
            Piercing = 0x50,

            Brooch = 0x51,
            Elfin_Chain = 0x52,
            Wizardry = 0x53,
            ac10 = 0x54,
            Dexterity = 0x55,

            Fumbling = 0x56,
            Chime = 0x57,
            Cloak = 0x58,
            Crystal = 0x59,
            Cube = 0x5A,

            Cubic = 0x5B,
            The_Dwarves = 0x5C,
            Decanter = 0x5D,
            Gloves = 0x5E,
            Drums = 0x5F,

            Dust = 0x60,
            Thievery = 0x61,
            Hat = 0x62,
            Flask = 0x63,
            Gauntlets = 0x64,

            Gem = 0x65,
            Girdle = 0x66,
            Helm = 0x67,
            Horn = 0x68,
            Stupidity = 0x69,

            Incense = 0x6A,
            Stone = 0x6B,
            Ioun_Stone = 0x6C,
            Javelin = 0x6D,
            Jewel = 0x6E,

            Ointment = 0x6F,
            Pale_Blue = 0x70,
            Scarlet_And = 0x71,
            Manual = 0x72,
            Incandescent = 0x73,

            Deep_Red = 0x74,
            Pink = 0x75,
            Mirror = 0x76,
            Necklace = 0x77,
            And_Green = 0x78,

            Blue = 0x79,
            Pearl = 0x7A,
            Powerlessness = 0x7B,
            Vermin = 0x7C,
            Pipes = 0x7D,

            Hole = 0x7E,
            Dragon_Slayer = 0x7F,
            Robe = 0x80,
            Rope = 0x81,
            Frost_Brand = 0x82,

            Berserker = 0x83,
            Scarab = 0x84,
            Spade = 0x85,
            Sphere = 0x86,
            Blessed = 0x87,

            Talisman = 0x88,
            Tome = 0x89,
            Trident = 0x8A,
            Grimoire = 0x8B,
            Well = 0x8C,

            Wings = 0x8D,
            Vial = 0x8E,
            Lantern = 0x8F,
            // 0x90,
            Flask_of_Oil = 0x91,

            ONE0_ftDOT_Pole = 0x92,
            FIVE0_ftDOT_Rope = 0x93,
            Iron = 0x94,
            Thf_Prickly_Tools = 0x95,
            Iron_Rations = 0x96,

            Standard_Rations = 0x97,
            Holy_Symbol = 0x98,
            Holy_Water_vial = 0x99,
            Unholy_Water_vial = 0x9A,
            Barding = 0x9B,

            Dragon = 0x9C,
            Lightning = 0x9D,
            Saddle = 0x9E,
            Staff = 0x9F,
            Drow = 0xA0,

            Wagon = 0xA1,
            PLUS1 = 0xA2,
            PLUS2 = 0xA3,
            PLUS3 = 0xA4,
            PLUS4 = 0xA5,

            PLUS5 = 0xA6,
            of = 0xA7,
            Vulnerability = 0xA8,
            //Cloak = 0xA9,
            Displacement = 0xAA,

            Torches = 0xAB,
            Oil = 0xAC,
            Speed = 0xAD,
            Tapestry = 0xAE,
            Spine = 0xAF,

            Copper = 0xB0,
            Silver = 0xB1,
            Electrum = 0xB2,
            Gold = 0xB3,
            Platinum = 0xB4,

            //Ointment = 0xB5,
            KeoghtumAPOSs = 0xB6,
            Sheet = 0xB7,
            Strength = 0xB8,
            Healing = 0xB9,

            Holding = 0xBA,
            Extra = 0xBB,
            Gaseous_Form = 0xBC,
            Slipperiness = 0xBD,
            Jewelled = 0xBE,

            Flying = 0xBF,
            Treasure_Finding = 0xC0,
            Fear = 0xC1,
            Disappearance = 0xC2,
            Statuette = 0xC3,

            Fungus = 0xC4,
            Chain = 0xC5,
            Pendant = 0xC6,
            Broach = 0xC7,
            Of_Seeking = 0xC8,

            MINUS1 = 0xC9,
            MINUS2 = 0xCA,
            MINUS3 = 0xCB,
            Lightning_Bolt = 0xCC,
            Fire_Resistance = 0xCD,

            Magic_Missiles = 0xCE,
            Save = 0xCF,
            Clrc_Scroll = 0xD0,
            MU_Scroll = 0xD1,
            With_1_Spell = 0xD2,

            With_2_Spells = 0xD3,
            With_3_Spells = 0xD4,
            ProtDOT_Scroll = 0xD5,
            Jewelry = 0xD6,
            Fine = 0xD7,

            Huge = 0xD8,
            Bone = 0xD9,
            Brass = 0xDA,
            Key = 0xDB,
            AC_2 = 0xDC,

            AC_6 = 0xDD,
            AC_4 = 0xDE,
            AC_3 = 0xDF,
            Of_ProtDOT = 0xE0,
            Paralyzation = 0xE1,

            Ogre_Power = 0xE2,
            Invisibility = 0xE3,
            Missiles = 0xE4,
            Elvenkind = 0xE5,
            Rotting = 0xE6,
            
            Covered = 0xE7,
            Efreeti = 0xE8,
            Bottle = 0xE9,
            Missile_Attractor = 0xEA,
            Of_Maglubiyet = 0xEB,

            Secr_Door_AND_Trap_Det = 0xEC,
            Gd_Dragon_Control = 0xED,
            Feather_Falling = 0xEE,
            Giant_Strength = 0xEF,
            Restoring_LevelOPsCP = 0xF0,

            Flame_Tongue = 0xF1,
            Fireballs = 0xF2,
            Spiritual = 0xF3,
            Boulder = 0xF4,
            Diamond = 0xF5,

            Emerald = 0xF6,
            Opal = 0xF7,
            Saphire = 0xF8,
            Of_Tyr = 0xF9,
            Of_Tempus = 0xFA,
            
            Of_Sune = 0xFB,
            Wooden = 0xFC,
            PLUS3_vs_Undead = 0xFD,
            Pass = 0xFE,
            Cursed = 0xFF,

            Apparatus = 0x100,
            Beaker = 0x101,
            Boat = 0x102,
            Bowl = 0x103,
            Brazier = 0x104,

            Broom = 0x105,
            Curse = 0x106,
            Candle = 0x107,
            Carpet = 0x108,
            Censer = 0x109,

            Purse = 0x10A,
            Fortress = 0x10B,
            Deck = 0x10C,
            Eyes = 0x10D,
            Figurine = 0x10E,

            Horseshoes = 0x10F,
            Instrument = 0x110,
            Libram = 0x111,
            Lyre = 0x112,
            Mattock = 0x113,

            Maul = 0x114,
            Medallion = 0x115,
            Net = 0x116,
            Pigment = 0x117,
            Periapt = 0x118,

            Phylactery = 0x119,
            Token = 0x11A,
            Rug = 0x11B,
            Saw = 0x11C,
            Small_Raft = 0x11D,

            Cart = 0x11E,
            Bodily_Health = 0x11F,
        };

        static string GetName(ItemNames item)
        {
            return new StringBuilder(item.ToString()).Replace("MINUS", "-").Replace("PLUS", "+").Replace("DOT", ".").Replace("_", " ").
                Replace("APOS", "'").Replace("ONE", "1").Replace("FIVE", "5").Replace("AND", "&").
                Replace("OP", "(").Replace("CP", ")").Replace("WEAPON", "").Replace("ARMOR", "").ToString();
        }

        public static ItemNames GetItemNamesPlus(int plus)
        {
            switch (plus)
            {
                case 1:
                    return ItemNames.PLUS1;
                case 2:
                    return ItemNames.PLUS2;
                case 3:
                    return ItemNames.PLUS3;
                case 4:
                    return ItemNames.PLUS4;
                case 5:
                    return ItemNames.PLUS5;
                default:
                    throw new ArgumentException("plus out of range 1-5");
            }
        }

        public static ItemNames GetItemNamesSpellCount(int spell_count)
        {
            switch (spell_count)
            {
                case 1:
                    return ItemNames.With_1_Spell;
                case 2:
                    return ItemNames.With_2_Spells;
                case 3:
                    return ItemNames.With_3_Spells;
                default:
                    throw new ArgumentException("spell_count out of range 1-3");
            }
        }

        public string GenerateName(int hidden_names_flag)
        {
            int display_flags = 0;
            display_flags |= (namenum[0] != 0 && (hidden_names_flag & 0x4) == 0) ? 0x1 : 0;
            display_flags |= (namenum[1] != 0 && (hidden_names_flag & 0x2) == 0) ? 0x2 : 0;
            display_flags |= (namenum[2] != 0 && (hidden_names_flag & 0x1) == 0) ? 0x4 : 0;

            bool pural_added = false;
            string name = "";

            for (int var_1 = 3; var_1 >= 1; var_1--)
            {
                if (((display_flags >> (var_1 - 1)) & 1) > 0)
                {
                    name += GetName((ItemNames)namenum[var_1 - 1]);

                    if (count < 2 ||
                        pural_added == true)
                    {
                        name += " ";
                    }
                    else if ((1 << (var_1 - 1) == display_flags) ||
                            (var_1 == 1 && display_flags > 4 && type != ItemType.FlaskOfOil) ||
                            (var_1 == 2 && (display_flags & 1) == 0) ||
                            (var_1 == 3 && type == ItemType.FlaskOfOil) ||
                            (namenum[2] != ItemNames.Blessed && (type == ItemType.Arrow || type == ItemType.Quarrel || type == ItemType.Dart) && namenum[2] != ItemNames.Silver))
                    {
                        name += "s ";
                        pural_added = true;
                    }
                    else
                    {
                        name += " ";
                    }
                }
            }

            return name.Trim();
        }


        public override bool Equals(object obj)
        {
            Item rhs = obj as Item;
            return rhs != null &&
                rhs.type == type &&
                rhs.namenum[0] == namenum[0] &&
                rhs.namenum[1] == namenum[1] &&
                rhs.namenum[2] == namenum[2] &&
                rhs.plus == plus &&
                rhs.plus_save == plus_save &&
                rhs.readied == readied &&
                rhs.hidden_names_flag == hidden_names_flag &&
                rhs.cursed == cursed &&
                rhs.weight == weight &&
                rhs.count == count &&
                rhs._value == _value &&
                rhs.affect_1 == affect_1 &&
                rhs.affect_2 == affect_2 &&
                rhs.affect_3 == affect_3
                ;
        }

        public override string ToString()
        {
            return GenerateName(0);
        }


        public bool IsRanged()
        {
            return gbl.ItemDataTable[type].range > 1;
        }
    }
}
