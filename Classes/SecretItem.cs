using static Classes.Item;

namespace Classes
{
    public class SecretItem
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

        public const int StructSize = 0x43;

        static BiLookup<SilverTypes, ItemType> type_map;
        static BiLookup<SilverNames, ItemNames> mapping;

        static void InitMapping()
        {
            if (type_map != null && mapping != null) { return; }

            type_map = new BiLookup<SilverTypes, ItemType>();

            type_map.Add(SilverTypes.Type_0, ItemType.Type_0);
            type_map.Add(SilverTypes.BattleAxe, ItemType.BattleAxe);
            type_map.Add(SilverTypes.HandAxe, ItemType.HandAxe);
            type_map.Add(SilverTypes.Club, ItemType.Club);
            type_map.Add(SilverTypes.Dagger, ItemType.Dagger);
            type_map.Add(SilverTypes.Dart, ItemType.Dart);
            type_map.Add(SilverTypes.Hammer, ItemType.Hammer);
            type_map.Add(SilverTypes.Javelin, ItemType.Javelin);
            type_map.Add(SilverTypes.Mace, ItemType.Mace);
            type_map.Add(SilverTypes.MorningStar, ItemType.MorningStar);
            type_map.Add(SilverTypes.MilitaryPick, ItemType.MilitaryPick);
            type_map.Add(SilverTypes.AwlPike, ItemType.AwlPike);
            type_map.Add(SilverTypes.Quarrel, ItemType.Quarrel);
            type_map.Add(SilverTypes.Scimitar, ItemType.Scimitar);
            type_map.Add(SilverTypes.Spear, ItemType.Spear);
            type_map.Add(SilverTypes.QuarterStaff, ItemType.QuarterStaff);
            type_map.Add(SilverTypes.BastardSword, ItemType.BastardSword);
            type_map.Add(SilverTypes.BroadSword, ItemType.BroadSword);
            type_map.Add(SilverTypes.LongSword, ItemType.LongSword);
            type_map.Add(SilverTypes.ShortSword, ItemType.ShortSword);
            type_map.Add(SilverTypes.TwoHandedSword, ItemType.TwoHandedSword);
            type_map.Add(SilverTypes.Trident, ItemType.Trident);
            type_map.Add(SilverTypes.CompositeLongBow, ItemType.CompositeLongBow);
            type_map.Add(SilverTypes.CompositeShortBow, ItemType.CompositeShortBow);
            type_map.Add(SilverTypes.LongBow, ItemType.LongBow);
            type_map.Add(SilverTypes.ShortBow, ItemType.ShortBow);
            type_map.Add(SilverTypes.LightCrossbow, ItemType.LightCrossbow);
            type_map.Add(SilverTypes.Sling, ItemType.Sling);
            type_map.Add(SilverTypes.StaffSling, ItemType.StaffSling);
            type_map.Add(SilverTypes.Arrow, ItemType.Arrow);
            type_map.Add(SilverTypes.LeatherArmor, ItemType.LeatherArmor);
            type_map.Add(SilverTypes.RingMail, ItemType.RingMail);
            type_map.Add(SilverTypes.ScaleMail, ItemType.ScaleMail);
            type_map.Add(SilverTypes.ChainMail, ItemType.ChainMail);
            type_map.Add(SilverTypes.BandedMail, ItemType.BandedMail);
            type_map.Add(SilverTypes.PlateMail, ItemType.PlateMail);
            type_map.Add(SilverTypes.Shield, ItemType.Shield);
            type_map.Add(SilverTypes.ScrollOfProt, ItemType.ScrollOfProt);
            type_map.Add(SilverTypes.MUScroll, ItemType.MUScroll);
            type_map.Add(SilverTypes.ClerSCroll, ItemType.ClrcScroll);
            type_map.Add(SilverTypes.Girdle, ItemType.Girdle);
            type_map.Add(SilverTypes.Boots, ItemType.Boots);
            type_map.Add(SilverTypes.Ring, ItemType.Ring);
            type_map.Add(SilverTypes.Potion, ItemType.Potion);
            type_map.Add(SilverTypes.Mirror, ItemType.Commodities);
            type_map.Add(SilverTypes.Bracers, ItemType.Bracers);
            type_map.Add(SilverTypes.WandA, ItemType.WandA);
            type_map.Add(SilverTypes.WandB, ItemType.WandB);
            type_map.Add(SilverTypes.PotionOfGiantStr, ItemType.PotionOfGiantStr);
            type_map.Add(SilverTypes.Canary, ItemType.Pass);
            type_map.Add(SilverTypes.Cloak, ItemType.Cloak);
            type_map.Add(SilverTypes.RingOfProt, ItemType.RingOfProt);
            type_map.Add(SilverTypes.RingOfWizardry, ItemType.RingOfWizardry);
            type_map.Add(SilverTypes.DartOfHornetsNest, ItemType.DartOfHornetsNest);
            type_map.Add(SilverTypes.Flail, ItemType.Flail);
            type_map.Add(SilverTypes.Halberd, ItemType.Halberd);
            type_map.Add(SilverTypes.Gauntlets, ItemType.Gauntlets);

            mapping = new BiLookup<SilverNames, ItemNames>();

            mapping.Add(0, 0);
            mapping.Add(SilverNames.WEAPONBattle_Axe, ItemNames.WEAPONBattle_Axe);
            mapping.Add(SilverNames.WEAPONHand_Axe, ItemNames.WEAPONHand_Axe);
            mapping.Add(SilverNames.WEAPONClub, ItemNames.WEAPONClub);
            mapping.Add(SilverNames.WEAPONDagger, ItemNames.WEAPONDagger);
            mapping.Add(SilverNames.WEAPONDart, ItemNames.WEAPONDart);
            mapping.Add(SilverNames.WEAPONFlail, ItemNames.WEAPONFlail);
            mapping.Add(SilverNames.WEAPONHalberd, ItemNames.WEAPONHalberd);
            mapping.Add(SilverNames.WEAPONHammer, ItemNames.WEAPONHammer);
            mapping.Add(SilverNames.WEAPONJavelin, ItemNames.WEAPONJavelin);
            mapping.Add(SilverNames.WEAPONMace, ItemNames.WEAPONMace);
            mapping.Add(SilverNames.WEAPONMorning_Star, ItemNames.WEAPONMorning_Star);
            mapping.Add(SilverNames.WEAPONMilitary_Pick, ItemNames.WEAPONMilitary_Pick);
            mapping.Add(SilverNames.WEAPONAwl_Pike, ItemNames.WEAPONAwl_Pike);
            mapping.Add(SilverNames.WEAPONQuarrel, ItemNames.WEAPONQuarrel);
            mapping.Add(SilverNames.WEAPONScimitar, ItemNames.WEAPONScimitar);
            mapping.Add(SilverNames.WEAPONSpear, ItemNames.WEAPONSpear);
            mapping.Add(SilverNames.WEAPONQuarter_Staff, ItemNames.WEAPONQuarter_Staff);
            mapping.Add(SilverNames.WEAPONBastard_Sword, ItemNames.WEAPONBastard_Sword);
            mapping.Add(SilverNames.WEAPONBroad_Sword, ItemNames.WEAPONBroad_Sword);
            mapping.Add(SilverNames.WEAPONLong_Sword, ItemNames.WEAPONLong_Sword);
            mapping.Add(SilverNames.WEAPONShort_Sword, ItemNames.WEAPONShort_Sword);
            mapping.Add(SilverNames.WEAPONTwoMINUSHanded_Sword, ItemNames.WEAPONTwoMINUSHanded_Sword);
            mapping.Add(SilverNames.WEAPONTrident, ItemNames.WEAPONTrident);
            mapping.Add(SilverNames.WEAPONComposite_Long_Bow, ItemNames.WEAPONComposite_Long_Bow);
            mapping.Add(SilverNames.WEAPONComposite_Short_Bow, ItemNames.WEAPONComposite_Short_Bow);
            mapping.Add(SilverNames.WEAPONLong_Bow, ItemNames.WEAPONLong_Bow);
            mapping.Add(SilverNames.WEAPONShort_Bow, ItemNames.WEAPONShort_Bow);
            mapping.Add(SilverNames.WEAPONLight_Crossbow, ItemNames.WEAPONLight_Crossbow);
            mapping.Add(SilverNames.WEAPONSling, ItemNames.WEAPONSling);
            mapping.Add(SilverNames.ARMORMail, ItemNames.ARMORMail);
            mapping.Add(SilverNames.ARMORArmor, ItemNames.ARMORArmor);
            mapping.Add(SilverNames.ARMORLeather, ItemNames.ARMORLeather);
            mapping.Add(SilverNames.ARMORRing, ItemNames.ARMORRing);
            mapping.Add(SilverNames.ARMORScale, ItemNames.ARMORScale);
            mapping.Add(SilverNames.ARMORChain, ItemNames.ARMORChain);
            mapping.Add(SilverNames.ARMORBanded, ItemNames.ARMORBanded);
            mapping.Add(SilverNames.ARMORPlate, ItemNames.ARMORPlate);
            mapping.Add(SilverNames.ARMORShield, ItemNames.ARMORShield);
            mapping.Add(SilverNames.WEAPONArrow, ItemNames.WEAPONArrow);
            mapping.Add(SilverNames.Potion, ItemNames.Potion);
            mapping.Add(SilverNames.Scroll, ItemNames.Scroll);
            mapping.Add(SilverNames.Ring, ItemNames.Ring);
            mapping.Add(SilverNames.Wand, ItemNames.Wand);
            mapping.Add(SilverNames.Dragon_Breath, ItemNames.Dragon_Breath);
            mapping.Add(SilverNames.Ice_Storm, ItemNames.Ice_Storm);
            mapping.Add(SilverNames.Boots, ItemNames.Boots);
            mapping.Add(SilverNames.Hornets_Nest, ItemNames.Hornets_Nest);
            mapping.Add(SilverNames.Bracers, ItemNames.Bracers);
            mapping.Add(SilverNames.Elven_Chain, ItemNames.Elfin_Chain);
            mapping.Add(SilverNames.Wizardry, ItemNames.Wizardry);
            mapping.Add(SilverNames.Cloak, ItemNames.Cloak);
            mapping.Add(SilverNames.Gauntlets, ItemNames.Gauntlets);
            mapping.Add(SilverNames.Gem, ItemNames.Gem);
            mapping.Add(SilverNames.Girdle, ItemNames.Girdle);
            mapping.Add(SilverNames.Helm, ItemNames.Helm);
            mapping.Add(SilverNames.Stone, ItemNames.Stone);
            mapping.Add(SilverNames.Mirror, ItemNames.Mirror);
            mapping.Add(SilverNames.Necklace, ItemNames.Necklace);
            mapping.Add(SilverNames.Robe, ItemNames.Robe);
            mapping.Add(SilverNames.Dragon, ItemNames.Dragon);
            mapping.Add(SilverNames.Lightning, ItemNames.Lightning);
            mapping.Add(SilverNames.Staff, ItemNames.Staff);
            mapping.Add(SilverNames.Drow, ItemNames.Drow);
            mapping.Add(SilverNames.PLUS1, ItemNames.PLUS1);
            mapping.Add(SilverNames.PLUS2, ItemNames.PLUS2);
            mapping.Add(SilverNames.PLUS3, ItemNames.PLUS3);
            mapping.Add(SilverNames.PLUS4, ItemNames.PLUS4);
            mapping.Add(SilverNames.PLUS5, ItemNames.PLUS5);
            mapping.Add(SilverNames.of, ItemNames.of);
            mapping.Add(SilverNames.Vulnerability, ItemNames.Vulnerability);
            mapping.Add(SilverNames.Displacement, ItemNames.Displacement);
            mapping.Add(SilverNames.Speed, ItemNames.Speed);
            mapping.Add(SilverNames.Silver, ItemNames.Silver);
            mapping.Add(SilverNames.Healing, ItemNames.Healing);
            mapping.Add(SilverNames.Extra, ItemNames.Extra);
            mapping.Add(SilverNames.Fear, ItemNames.Fear);
            mapping.Add(SilverNames.MINUS1, ItemNames.MINUS1);
            mapping.Add(SilverNames.MINUS2, ItemNames.MINUS2);
            mapping.Add(SilverNames.MINUS3, ItemNames.MINUS3);
            mapping.Add(SilverNames.Fire_Resistance, ItemNames.Fire_Resistance);
            mapping.Add(SilverNames.Magic_Missiles, ItemNames.Magic_Missiles);
            mapping.Add(SilverNames.Cler, ItemNames.Clrc_Scroll);
            mapping.Add(SilverNames.Mage, ItemNames.MU_Scroll);
            mapping.Add(SilverNames.With_1_Spell, ItemNames.With_1_Spell);
            mapping.Add(SilverNames.With_2_Spells, ItemNames.With_2_Spells);
            mapping.Add(SilverNames.With_3_Spells, ItemNames.With_3_Spells);
            mapping.Add(SilverNames.Jewelry, ItemNames.Jewelry);
            mapping.Add(SilverNames.Fine, ItemNames.Fine);
            mapping.Add(SilverNames.AC_10, ItemNames.ac10);
            mapping.Add(SilverNames.AC_2, ItemNames.AC_2);
            mapping.Add(SilverNames.AC_6, ItemNames.AC_6);
            mapping.Add(SilverNames.AC_4, ItemNames.AC_4);
            mapping.Add(SilverNames.AC_3, ItemNames.AC_3);
            mapping.Add(SilverNames.Of_ProtDOT, ItemNames.Of_ProtDOT);
            mapping.Add(SilverNames.Paralyzation, ItemNames.Paralyzation);
            mapping.Add(SilverNames.Ogre_Power, ItemNames.Ogre_Power);
            mapping.Add(SilverNames.Invisibility, ItemNames.Invisibility);
            mapping.Add(SilverNames.Missiles, ItemNames.Missiles);
            mapping.Add(SilverNames.Giant_Strength, ItemNames.Giant_Strength);
            mapping.Add(SilverNames.Fireballs, ItemNames.Fireballs);
            mapping.Add(SilverNames.Spiritual, ItemNames.Spiritual);
            mapping.Add(SilverNames.Boulder, ItemNames.Boulder);
            mapping.Add(SilverNames.Diamond, ItemNames.Diamond);
            mapping.Add(SilverNames.Emerald, ItemNames.Emerald);
            mapping.Add(SilverNames.Saphire, ItemNames.Saphire);
            mapping.Add(SilverNames.Periapt, ItemNames.Periapt);
            mapping.Add(SilverNames.Good_Luck, ItemNames.Good_Luck);
            mapping.Add(SilverNames.Health, ItemNames.Health);
        }

        public SecretItem(byte[] data, int offset)
        {
            DataIO.ReadObject(this, data, offset);
        }

        public SecretItem(Item item)
        {
            if (mapping == null) { InitMapping(); }

            name = item.name;
            type = (byte)type_map[item.type][0];
            namenum[0] = (byte)mapping[item.namenum[0]][0];
            namenum[1] = (byte)mapping[item.namenum[1]][0];
            namenum[2] = (byte)mapping[item.namenum[2]][0];
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

            if (item.affect_3 == Affects.item_affect)
            {
                affect_2 = (byte)SecretAffect.Map(item.affect_2);
            }
        }

        public Item Load()
        {
            if (mapping == null) { InitMapping(); }

            Item item = new Item()
            {
                name = name,
                type = type_map[(SilverTypes)type][0],
                itemData = gbl.ItemDataTable[type],
                namenum = new ItemNames[3] { mapping[(SilverNames)namenum[0]][0], mapping[(SilverNames)namenum[1]][0], mapping[(SilverNames)namenum[2]][0] },
                plus = plus,
                plus_save = plus_save,
                readied = readied != 0,
                hidden_names_flag = hidden_names_flag,
                cursed = cursed != 0,
                weight = weight,
                count = count,
                _value = _value,
                affect_1 = (Affects)affect_1,
                affect_2 = (Affects)affect_2,
                affect_3 = (Affects)affect_3
            };

            if (item.affect_3 == Affects.item_affect)
            {
                item.affect_2 = SecretAffect.Map((SecretAffect.Affects)affect_2);
            }

            ItemLibrary.Add(item);

            return item;
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }

        private enum SilverTypes
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

        private enum SilverNames
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