namespace Classes.Secret
{
    public class Item
    {
        [DataOffset(0x00, DataType.PString, 42)]
        public string name; // 0x0 - 0x2A
        [DataOffset(0x2E, DataType.Byte)]
        public byte type; // 0x2E
        [DataOffset(0x2F, DataType.ByteArray, 3)]
        public byte[] namenum = new byte[3]; // 0x2F - 0x31
        [DataOffset(0x32, DataType.Byte)]
        public byte plus; // 0x32
        [DataOffset(0x33, DataType.Byte)]
        public byte plus_save; // 0x33
        [DataOffset(0x34, DataType.Byte)]
        public byte readied; // 0x34
        [DataOffset(0x35, DataType.Byte)]
        public byte hidden_names_flag; // 0x35
        [DataOffset(0x36, DataType.Byte)]
        public byte cursed; // 0x36
        [DataOffset(0x37, DataType.SWord)]
        public short weight; // 0x37
        [DataOffset(0x39, DataType.Byte)]
        public byte count; // 0x39
        [DataOffset(0x3A, DataType.SWord)]
        public short _value; // 0x3A
        [DataOffset(0x3C, DataType.Byte)]
        public byte affect_1; // 0x3C
        [DataOffset(0x3D, DataType.Byte)]
        public byte affect_2; // 0x3D
        [DataOffset(0x3E, DataType.Byte)]
        public byte affect_3; // 0x3E

        public const int StructSize = 0x3F;
        public const int StructSizeSave = 0x43;

        readonly static BiLookup<Names, Classes.Item.Names> names_map = InitNamesMap();
        readonly static BiLookup<Type, Classes.Item.Type> type_map = InitTypeMap();

        private static BiLookup<Names, Classes.Item.Names> InitNamesMap()
        {
            BiLookup<Names, Classes.Item.Names> map = new();

            map.Add(0, Classes.Item.Names.empty);
            map.Add(Names.WEAPONBattle_Axe, Classes.Item.Names.WEAPONBattle_Axe);
            map.Add(Names.WEAPONHand_Axe, Classes.Item.Names.WEAPONHand_Axe);
            map.Add(Names.WEAPONClub, Classes.Item.Names.WEAPONClub);
            map.Add(Names.WEAPONDagger, Classes.Item.Names.WEAPONDagger);
            map.Add(Names.WEAPONDart, Classes.Item.Names.WEAPONDart);
            map.Add(Names.WEAPONFlail, Classes.Item.Names.WEAPONFlail);
            map.Add(Names.WEAPONHalberd, Classes.Item.Names.WEAPONHalberd);
            map.Add(Names.WEAPONHammer, Classes.Item.Names.WEAPONHammer);
            map.Add(Names.WEAPONJavelin, Classes.Item.Names.WEAPONJavelin);
            map.Add(Names.WEAPONMace, Classes.Item.Names.WEAPONMace);
            map.Add(Names.WEAPONMorning_Star, Classes.Item.Names.WEAPONMorning_Star);
            map.Add(Names.WEAPONMilitary_Pick, Classes.Item.Names.WEAPONMilitary_Pick);
            map.Add(Names.WEAPONAwl_Pike, Classes.Item.Names.WEAPONAwl_Pike);
            map.Add(Names.WEAPONQuarrel, Classes.Item.Names.WEAPONQuarrel);
            map.Add(Names.WEAPONScimitar, Classes.Item.Names.WEAPONScimitar);
            map.Add(Names.WEAPONSpear, Classes.Item.Names.WEAPONSpear);
            map.Add(Names.WEAPONQuarter_Staff, Classes.Item.Names.WEAPONQuarter_Staff);
            map.Add(Names.WEAPONBastard_Sword, Classes.Item.Names.WEAPONBastard_Sword);
            map.Add(Names.WEAPONBroad_Sword, Classes.Item.Names.WEAPONBroad_Sword);
            map.Add(Names.WEAPONLong_Sword, Classes.Item.Names.WEAPONLong_Sword);
            map.Add(Names.WEAPONShort_Sword, Classes.Item.Names.WEAPONShort_Sword);
            map.Add(Names.WEAPONTwoMINUSHanded_Sword, Classes.Item.Names.WEAPONTwoMINUSHanded_Sword);
            map.Add(Names.WEAPONTrident, Classes.Item.Names.WEAPONTrident);
            map.Add(Names.WEAPONComposite_Long_Bow, Classes.Item.Names.WEAPONComposite_Long_Bow);
            map.Add(Names.WEAPONComposite_Short_Bow, Classes.Item.Names.WEAPONComposite_Short_Bow);
            map.Add(Names.WEAPONLong_Bow, Classes.Item.Names.WEAPONLong_Bow);
            map.Add(Names.WEAPONShort_Bow, Classes.Item.Names.WEAPONShort_Bow);
            map.Add(Names.WEAPONLight_Crossbow, Classes.Item.Names.WEAPONLight_Crossbow);
            map.Add(Names.WEAPONSling, Classes.Item.Names.WEAPONSling);
            map.Add(Names.ARMORMail, Classes.Item.Names.ARMORMail);
            map.Add(Names.ARMORArmor, Classes.Item.Names.ARMORArmor);
            map.Add(Names.ARMORLeather, Classes.Item.Names.ARMORLeather);
            map.Add(Names.ARMORRing, Classes.Item.Names.ARMORRing);
            map.Add(Names.ARMORScale, Classes.Item.Names.ARMORScale);
            map.Add(Names.ARMORChain, Classes.Item.Names.ARMORChain);
            map.Add(Names.ARMORBanded, Classes.Item.Names.ARMORBanded);
            map.Add(Names.ARMORPlate, Classes.Item.Names.ARMORPlate);
            map.Add(Names.ARMORShield, Classes.Item.Names.ARMORShield);
            map.Add(Names.WEAPONArrow, Classes.Item.Names.WEAPONArrow);
            map.Add(Names.Potion, Classes.Item.Names.Potion);
            map.Add(Names.Scroll, Classes.Item.Names.Scroll);
            map.Add(Names.Ring, Classes.Item.Names.Ring);
            map.Add(Names.Wand, Classes.Item.Names.Wand);
            map.Add(Names.Dragon_Breath, Classes.Item.Names.Dragon_Breath);
            map.Add(Names.Ice_Storm, Classes.Item.Names.Ice_Storm);
            map.Add(Names.Boots, Classes.Item.Names.Boots);
            map.Add(Names.Hornets_Nest, Classes.Item.Names.Hornets_Nest);
            map.Add(Names.Bracers, Classes.Item.Names.Bracers);
            map.Add(Names.Elven_Chain, Classes.Item.Names.Elfin_Chain);
            map.Add(Names.Wizardry, Classes.Item.Names.Wizardry);
            map.Add(Names.Cloak, Classes.Item.Names.Cloak);
            map.Add(Names.Gauntlets, Classes.Item.Names.Gauntlets);
            map.Add(Names.Gem, Classes.Item.Names.Gem);
            map.Add(Names.Girdle, Classes.Item.Names.Girdle);
            map.Add(Names.Helm, Classes.Item.Names.Helm);
            map.Add(Names.Stone, Classes.Item.Names.Stone);
            map.Add(Names.Mirror, Classes.Item.Names.Mirror);
            map.Add(Names.Necklace, Classes.Item.Names.Necklace);
            map.Add(Names.Robe, Classes.Item.Names.Robe);
            map.Add(Names.Dragon, Classes.Item.Names.Dragon);
            map.Add(Names.Lightning, Classes.Item.Names.Lightning);
            map.Add(Names.Staff, Classes.Item.Names.Staff);
            map.Add(Names.Drow, Classes.Item.Names.Drow);
            map.Add(Names.PLUS1, Classes.Item.Names.PLUS1);
            map.Add(Names.PLUS2, Classes.Item.Names.PLUS2);
            map.Add(Names.PLUS3, Classes.Item.Names.PLUS3);
            map.Add(Names.PLUS4, Classes.Item.Names.PLUS4);
            map.Add(Names.PLUS5, Classes.Item.Names.PLUS5);
            map.Add(Names.of, Classes.Item.Names.of);
            map.Add(Names.Vulnerability, Classes.Item.Names.Vulnerability);
            map.Add(Names.Displacement, Classes.Item.Names.Displacement);
            map.Add(Names.Speed, Classes.Item.Names.Speed);
            map.Add(Names.Silver, Classes.Item.Names.Silver);
            map.Add(Names.Healing, Classes.Item.Names.Healing);
            map.Add(Names.Extra, Classes.Item.Names.Extra);
            map.Add(Names.Fear, Classes.Item.Names.Fear);
            map.Add(Names.MINUS1, Classes.Item.Names.MINUS1);
            map.Add(Names.MINUS2, Classes.Item.Names.MINUS2);
            map.Add(Names.MINUS3, Classes.Item.Names.MINUS3);
            map.Add(Names.Fire_Resistance, Classes.Item.Names.Fire_Resistance);
            map.Add(Names.Magic_Missiles, Classes.Item.Names.Magic_Missiles);
            map.Add(Names.Cler, Classes.Item.Names.Clrc_Scroll);
            map.Add(Names.Mage, Classes.Item.Names.MU_Scroll);
            map.Add(Names.With_1_Spell, Classes.Item.Names.With_1_Spell);
            map.Add(Names.With_2_Spells, Classes.Item.Names.With_2_Spells);
            map.Add(Names.With_3_Spells, Classes.Item.Names.With_3_Spells);
            map.Add(Names.Jewelry, Classes.Item.Names.Jewelry);
            map.Add(Names.Fine, Classes.Item.Names.Fine);
            map.Add(Names.AC_10, Classes.Item.Names.ac10);
            map.Add(Names.AC_2, Classes.Item.Names.AC_2);
            map.Add(Names.AC_6, Classes.Item.Names.AC_6);
            map.Add(Names.AC_4, Classes.Item.Names.AC_4);
            map.Add(Names.AC_3, Classes.Item.Names.AC_3);
            map.Add(Names.Of_ProtDOT, Classes.Item.Names.of_ProtDOT);
            map.Add(Names.Paralyzation, Classes.Item.Names.Paralyzation);
            map.Add(Names.Ogre_Power, Classes.Item.Names.Ogre_Power);
            map.Add(Names.Invisibility, Classes.Item.Names.Invisibility);
            map.Add(Names.Missiles, Classes.Item.Names.Missiles);
            map.Add(Names.Giant_Strength, Classes.Item.Names.Giant_Strength);
            map.Add(Names.Fireballs, Classes.Item.Names.Fireballs);
            map.Add(Names.Spiritual, Classes.Item.Names.Spiritual);
            map.Add(Names.Boulder, Classes.Item.Names.Boulder);
            map.Add(Names.Diamond, Classes.Item.Names.Diamond);
            map.Add(Names.Emerald, Classes.Item.Names.Emerald);
            map.Add(Names.Saphire, Classes.Item.Names.Saphire);
            map.Add(Names.Periapt, Classes.Item.Names.Periapt);
            map.Add(Names.Good_Luck, Classes.Item.Names.Good_Luck);
            map.Add(Names.Health, Classes.Item.Names.Health);

            return map;
        }
        private static BiLookup<Type, Classes.Item.Type> InitTypeMap()
        {
            BiLookup<Type, Classes.Item.Type> map = new();

            map.Add(Type.Type_0, Classes.Item.Type.Type_0);
            map.Add(Type.BattleAxe, Classes.Item.Type.BattleAxe);
            map.Add(Type.HandAxe, Classes.Item.Type.HandAxe);
            map.Add(Type.Club, Classes.Item.Type.Club);
            map.Add(Type.Dagger, Classes.Item.Type.Dagger);
            map.Add(Type.Dart, Classes.Item.Type.Dart);
            map.Add(Type.Hammer, Classes.Item.Type.Hammer);
            map.Add(Type.Javelin, Classes.Item.Type.Javelin);
            map.Add(Type.Mace, Classes.Item.Type.Mace);
            map.Add(Type.MorningStar, Classes.Item.Type.MorningStar);
            map.Add(Type.MilitaryPick, Classes.Item.Type.MilitaryPick);
            map.Add(Type.AwlPike, Classes.Item.Type.AwlPike);
            map.Add(Type.Quarrel, Classes.Item.Type.Quarrel);
            map.Add(Type.Scimitar, Classes.Item.Type.Scimitar);
            map.Add(Type.Spear, Classes.Item.Type.Spear);
            map.Add(Type.QuarterStaff, Classes.Item.Type.QuarterStaff);
            map.Add(Type.BastardSword, Classes.Item.Type.BastardSword);
            map.Add(Type.BroadSword, Classes.Item.Type.BroadSword);
            map.Add(Type.LongSword, Classes.Item.Type.LongSword);
            map.Add(Type.ShortSword, Classes.Item.Type.ShortSword);
            map.Add(Type.TwoHandedSword, Classes.Item.Type.TwoHandedSword);
            map.Add(Type.Trident, Classes.Item.Type.Trident);
            map.Add(Type.CompositeLongBow, Classes.Item.Type.CompositeLongBow);
            map.Add(Type.CompositeShortBow, Classes.Item.Type.CompositeShortBow);
            map.Add(Type.LongBow, Classes.Item.Type.LongBow);
            map.Add(Type.ShortBow, Classes.Item.Type.ShortBow);
            map.Add(Type.LightCrossbow, Classes.Item.Type.LightCrossbow);
            map.Add(Type.Sling, Classes.Item.Type.Sling);
            map.Add(Type.StaffSling, Classes.Item.Type.StaffSling);
            map.Add(Type.Arrow, Classes.Item.Type.Arrow);
            map.Add(Type.LeatherArmor, Classes.Item.Type.LeatherArmor);
            map.Add(Type.RingMail, Classes.Item.Type.RingMail);
            map.Add(Type.ScaleMail, Classes.Item.Type.ScaleMail);
            map.Add(Type.ChainMail, Classes.Item.Type.ChainMail);
            map.Add(Type.BandedMail, Classes.Item.Type.BandedMail);
            map.Add(Type.PlateMail, Classes.Item.Type.PlateMail);
            map.Add(Type.Shield, Classes.Item.Type.Shield);
            map.Add(Type.ScrollOfProt, Classes.Item.Type.ScrollOfProt);
            map.Add(Type.MUScroll, Classes.Item.Type.MUScroll);
            map.Add(Type.ClerSCroll, Classes.Item.Type.ClrcScroll);
            map.Add(Type.Girdle, Classes.Item.Type.Girdle);
            map.Add(Type.Boots, Classes.Item.Type.Boots);
            map.Add(Type.Ring, Classes.Item.Type.Ring);
            map.Add(Type.Potion, Classes.Item.Type.Potion);
            map.Add(Type.Mirror, Classes.Item.Type.Commodities);
            map.Add(Type.Bracers, Classes.Item.Type.Bracers);
            map.Add(Type.WandA, Classes.Item.Type.WandA);
            map.Add(Type.WandB, Classes.Item.Type.WandB);
            map.Add(Type.PotionOfGiantStr, Classes.Item.Type.PotionOfGiantStr);
            map.Add(Type.Canary, Classes.Item.Type.Pass);
            map.Add(Type.Cloak, Classes.Item.Type.Cloak);
            map.Add(Type.RingOfProt, Classes.Item.Type.RingOfProt);
            map.Add(Type.RingOfWizardry, Classes.Item.Type.RingOfWizardry);
            map.Add(Type.DartOfHornetsNest, Classes.Item.Type.DartOfHornetsNest);
            map.Add(Type.Flail, Classes.Item.Type.Flail);
            map.Add(Type.Halberd, Classes.Item.Type.Halberd);
            map.Add(Type.Gauntlets, Classes.Item.Type.Gauntlets);

            return map;
        }

        public Item(byte[] data, int offset)
        {
            DataIO.ReadObject(this, data, offset);
        }

        public Item(Classes.Item item)
        {
            name = item.name;
            type = (byte)type_map[item.type][0];
            namenum[0] = (byte)names_map[item.namenum[0]][0];
            namenum[1] = (byte)names_map[item.namenum[1]][0];
            namenum[2] = (byte)names_map[item.namenum[2]][0];
            plus = (byte)item.plus;
            plus_save = item.plus_save;
            readied = (byte)(item.readied ? 1 : 0);
            hidden_names_flag = item.hidden_names_flag;
            cursed = (byte)(item.cursed ? 1 : 0);
            weight = item.weight;
            count = (byte)item.count;
            _value = item._value;
            affect_1 = (byte)item.affect_1;
            affect_2 = (byte)item.affect_2;
            affect_3 = (byte)item.affect_3;
        }

        public Classes.Item Load()
        {
            Classes.Item item = new()
            {
                name = name,
                type = type_map[(Type)type][0],
                itemData = gbl.ItemDataTable[type],
                namenum = new Classes.Item.Names[3] { names_map[(Names)namenum[0]][0], names_map[(Names)namenum[1]][0], names_map[(Names)namenum[2]][0] },
                plus = plus,
                plus_save = plus_save,
                readied = readied != 0,
                hidden_names_flag = hidden_names_flag,
                cursed = cursed != 0,
                weight = weight,
                count = count,
                _value = _value,
                affect_1 = affect_1,
                affect_2 = affect_2,
                affect_3 = affect_3
            };

            ItemLibrary.Add(item);

            return item;
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSizeSave];

            DataIO.WriteObject(this, data);

            return data;
        }

        private enum Type
        {
            Type_0 = 0,
            BattleAxe = 1,
            HandAxe = 2,
            Club = 3,
            Dagger = 4,
            Dart = 5,
            Hammer = 6,
            Javelin = 7,
            Mace = 8,
            MorningStar = 9,
            MilitaryPick = 10,
            AwlPike = 11,
            Quarrel = 12,
            Scimitar = 13,
            Spear = 14,
            QuarterStaff = 15,
            BastardSword = 16,
            BroadSword = 17,
            LongSword = 18,
            ShortSword = 19,
            TwoHandedSword = 20,
            Trident = 21,
            CompositeLongBow = 22,
            CompositeShortBow = 23,
            LongBow = 24,
            ShortBow = 25,
            FineLongBow = 26,
            LightCrossbow = 27,
            Sling = 28,
            StaffSling = 29,
            Arrow = 30,
            LeatherArmor = 31,
            RingMail = 32,
            ScaleMail = 33,
            ChainMail = 34,
            BandedMail = 35,
            PlateMail = 36,
            Shield = 37,
            ScrollOfProt = 38,
            MUScroll = 39,
            ClerSCroll = 40,
            Girdle = 42,
            Boots = 45,
            Ring = 46,
            Potion = 47, // Necklace
            Mirror = 48,
            Bracers = 50,
            WandA = 51,
            WandB = 52,
            PotionOfGiantStr = 53,
            Canary = 54,
            Cloak = 58,
            RingOfProt = 59,
            RingOfWizardry = 65,
            DartOfHornetsNest = 66,
            Flail = 70,
            Halberd = 71,
            Gauntlets = 72,
        };

        private enum Names
        {
            WEAPONBattle_Axe = 0x01,
            WEAPONHand_Axe = 0x02,
            WEAPONClub = 0x03,
            WEAPONDagger = 0x04,
            WEAPONDart = 0x05,

            WEAPONHammer = 006,
            WEAPONJavelin = 0x07,
            WEAPONMace = 0x08,
            WEAPONMorning_Star = 0x09,
            WEAPONMilitary_Pick = 0x0A,

            WEAPONAwl_Pike = 0x0B,
            WEAPONQuarrel = 0x0C,
            WEAPONScimitar = 0x0D,
            WEAPONSpear = 0x0E,
            WEAPONQuarter_Staff = 0x0F,

            WEAPONBastard_Sword = 0x10,
            WEAPONBroad_Sword = 0x11,
            WEAPONLong_Sword = 0x12,
            WEAPONShort_Sword = 0x13,
            WEAPONTwoMINUSHanded_Sword = 0x14,

            WEAPONTrident = 0x15,
            WEAPONComposite_Long_Bow = 0x16,
            WEAPONComposite_Short_Bow = 0x17,
            WEAPONLong_Bow = 0x18,
            WEAPONShort_Bow = 0x19,

            Fine = 0x1A,
            WEAPONLight_Crossbow = 0x1B,
            WEAPONSling = 0x1C,
            Staff = 0x1D,
            WEAPONArrow = 0x1E,

            ARMORLeather = 0x1F,
            ARMORRing = 0x20,
            ARMORScale = 0x21,
            ARMORChain = 0x22,
            ARMORBanded = 0x23,

            ARMORPlate = 0x24,
            ARMORShield = 0x25,
            Cler = 0x26,
            Scroll = 0x27,
            Mage = 0x28,

            Helm = 0x29,
            Belt = 0x2A,
            Robe = 0x2B,
            Cloak = 0x2C,
            Boots = 0x2D,

            Ring = 0x2E,
            ARMORMail = 0x2F,
            ARMORArmor = 0x30,
            Of_ProtDOT = 0x31,
            Bracers = 0x32,

            Wand = 0x33,
            Elixer = 0x34,
            Potion = 0x35,
            Youth = 0x36,
            Ruby = 0x37,

            Boulder = 0x38,
            Dragon_Breath = 0x39,
            Displacement = 0x3A,
            Eyes = 0x3B,
            Drow = 0x3C,

            Elven_Chain = 0x3D,
            Ice_Storm = 0x3E,
            Saphire = 0x3F,
            Emerald = 0x40,
            Wizardry = 0x41,

            Hornets_Nest = 0x42,
            Fire_Resistance = 0x43,
            Stone = 0x44,
            Good_Luck = 0x45,
            WEAPONFlail = 0x46,

            WEAPONHalberd = 0x47,
            Gauntlets = 0x48,
            Periapt = 0x49,
            Health = 0x4A,
            // 0x4B,

            // 0x4C,
            Bundle_of = 0x4D,
            Ogre_Power = 0x4E,
            Girdle = 0x4F,
            Giant_Strength = 0x50,

            Mirror = 0x51,
            Necklace = 0x52,
            Dragon = 0x53,
            vs_Giants = 0x54,
            Diamond = 0x55,

            // 0x56,
            // 0x57,
            Lightning = 0x58,
            Fireballs = 0x59,
            of = 0x5A,

            Vulnerability = 0x5B,
            Speed = 0x5C,
            Silver = 0x5D,
            Extra = 0x5E,
            Healing = 0x5F,

            Charming = 0x60,
            Fear = 0x61,
            Magic_Missiles = 0x62,
            Missiles = 0x63,
            With_1_Spell = 0x64,

            With_2_Spells = 0x65,
            With_3_Spells = 0x66,
            Paralyzation = 0x67,
            Invisibility = 0x68,
            Cute_Yellow_Canary = 0x69,

            AC_10 = 0x6A,
            AC_6 = 0x6B,
            AC_4 = 0x6C,
            AC_3 = 0x6D,
            AC_2 = 0x6E,

            PLUS1 = 0x6F,
            PLUS2 = 0x70,
            PLUS3 = 0x71,
            PLUS4 = 0x72,
            PLUS5 = 0x73,

            MINUS1 = 0x74,
            MINUS2 = 0x75,
            MINUS3 = 0x76,
            Spiritual = 0x77,
            Gem = 0x78,

            Jewelry = 0x79,
        };
    }
}