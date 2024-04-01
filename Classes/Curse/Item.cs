namespace Classes.Curse
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

        readonly static BiLookup<Names, Classes.Item.Names> mapping = InitMapping();

        private static BiLookup<Names, Classes.Item.Names> InitMapping()
        {
            BiLookup<Names, Classes.Item.Names> map = new();

            map.Add(0, Classes.Item.Names.empty);
            map.Add(Names.WEAPONBattle_Axe, Classes.Item.Names.WEAPONBattle_Axe);
            map.Add(Names.WEAPONHand_Axe, Classes.Item.Names.WEAPONHand_Axe);
            map.Add(Names.WEAPONBardiche, Classes.Item.Names.WEAPONBardiche);
            map.Add(Names.WEAPONBec_De_Corbin, Classes.Item.Names.WEAPONBec_De_Corbin);
            map.Add(Names.WEAPONBillMINUSGuisarme, Classes.Item.Names.WEAPONBillMINUSGuisarme);
            map.Add(Names.WEAPONBo_Stick, Classes.Item.Names.WEAPONBo_Stick);
            map.Add(Names.WEAPONClub, Classes.Item.Names.WEAPONClub);
            map.Add(Names.WEAPONDagger, Classes.Item.Names.WEAPONDagger);
            map.Add(Names.WEAPONDart, Classes.Item.Names.WEAPONDart);
            map.Add(Names.WEAPONFauchard, Classes.Item.Names.WEAPONFauchard);
            map.Add(Names.WEAPONFauchardMINUSFork, Classes.Item.Names.WEAPONFauchardMINUSFork);
            map.Add(Names.WEAPONFlail, Classes.Item.Names.WEAPONFlail);
            map.Add(Names.WEAPONMilitary_Fork, Classes.Item.Names.WEAPONMilitary_Fork);
            map.Add(Names.WEAPONGlaive, Classes.Item.Names.WEAPONGlaive);
            map.Add(Names.WEAPONGlaiveMINUSGuisarme, Classes.Item.Names.WEAPONGlaiveMINUSGuisarme);
            map.Add(Names.WEAPONGuisarme, Classes.Item.Names.WEAPONGuisarme);
            map.Add(Names.WEAPONGuisarmeMINUSVoulge, Classes.Item.Names.WEAPONGuisarmeMINUSVoulge);
            map.Add(Names.WEAPONHalberd, Classes.Item.Names.WEAPONHalberd);
            map.Add(Names.WEAPONLucern_Hammer, Classes.Item.Names.WEAPONLucern_Hammer);
            map.Add(Names.WEAPONHammer, Classes.Item.Names.WEAPONHammer);
            map.Add(Names.WEAPONJavelin, Classes.Item.Names.WEAPONJavelin);
            map.Add(Names.WEAPONJo_Stick, Classes.Item.Names.WEAPONJo_Stick);
            map.Add(Names.WEAPONMace, Classes.Item.Names.WEAPONMace);
            map.Add(Names.WEAPONMorning_Star, Classes.Item.Names.WEAPONMorning_Star);
            map.Add(Names.WEAPONPartisan, Classes.Item.Names.WEAPONPartisan);
            map.Add(Names.WEAPONMilitary_Pick, Classes.Item.Names.WEAPONMilitary_Pick);
            map.Add(Names.WEAPONAwl_Pike, Classes.Item.Names.WEAPONAwl_Pike);
            map.Add(Names.WEAPONQuarrel, Classes.Item.Names.WEAPONQuarrel);
            map.Add(Names.WEAPONRanseur, Classes.Item.Names.WEAPONRanseur);
            map.Add(Names.WEAPONScimitar, Classes.Item.Names.WEAPONScimitar);
            map.Add(Names.WEAPONSpear, Classes.Item.Names.WEAPONSpear);
            map.Add(Names.WEAPONSpetum, Classes.Item.Names.WEAPONSpetum);
            map.Add(Names.WEAPONQuarter_Staff, Classes.Item.Names.WEAPONQuarter_Staff);
            map.Add(Names.WEAPONBastard_Sword, Classes.Item.Names.WEAPONBastard_Sword);
            map.Add(Names.WEAPONBroad_Sword, Classes.Item.Names.WEAPONBroad_Sword);
            map.Add(Names.WEAPONLong_Sword, Classes.Item.Names.WEAPONLong_Sword);
            map.Add(Names.WEAPONShort_Sword, Classes.Item.Names.WEAPONShort_Sword);
            map.Add(Names.WEAPONTwoMINUSHanded_Sword, Classes.Item.Names.WEAPONTwoMINUSHanded_Sword);
            map.Add(Names.WEAPONTrident, Classes.Item.Names.WEAPONTrident);
            map.Add(Names.WEAPONVoulge, Classes.Item.Names.WEAPONVoulge);
            map.Add(Names.WEAPONComposite_Long_Bow, Classes.Item.Names.WEAPONComposite_Long_Bow);
            map.Add(Names.WEAPONComposite_Short_Bow, Classes.Item.Names.WEAPONComposite_Short_Bow);
            map.Add(Names.WEAPONLong_Bow, Classes.Item.Names.WEAPONLong_Bow);
            map.Add(Names.WEAPONShort_Bow, Classes.Item.Names.WEAPONShort_Bow);
            map.Add(Names.WEAPONHeavy_Crossbow, Classes.Item.Names.WEAPONHeavy_Crossbow);
            map.Add(Names.WEAPONLight_Crossbow, Classes.Item.Names.WEAPONLight_Crossbow);
            map.Add(Names.WEAPONSling, Classes.Item.Names.WEAPONSling);
            map.Add(Names.ARMORMail, Classes.Item.Names.ARMORMail);
            map.Add(Names.ARMORArmor, Classes.Item.Names.ARMORArmor);
            map.Add(Names.ARMORLeather, Classes.Item.Names.ARMORLeather);
            map.Add(Names.ARMORPadded, Classes.Item.Names.ARMORPadded);
            map.Add(Names.ARMORStudded, Classes.Item.Names.ARMORStudded);
            map.Add(Names.ARMORRing, Classes.Item.Names.ARMORRing);
            map.Add(Names.ARMORScale, Classes.Item.Names.ARMORScale);
            map.Add(Names.ARMORChain, Classes.Item.Names.ARMORChain);
            map.Add(Names.ARMORSplint, Classes.Item.Names.ARMORSplint);
            map.Add(Names.ARMORBanded, Classes.Item.Names.ARMORBanded);
            map.Add(Names.ARMORPlate, Classes.Item.Names.ARMORPlate);
            map.Add(Names.ARMORShield, Classes.Item.Names.ARMORShield);
            map.Add(Names.Woods, Classes.Item.Names.Woods);
            map.Add(Names.WEAPONArrow, Classes.Item.Names.WEAPONArrow);
            map.Add(Names.Potion, Classes.Item.Names.Potion);
            map.Add(Names.Scroll, Classes.Item.Names.Scroll);
            map.Add(Names.Ring, Classes.Item.Names.Ring);
            map.Add(Names.Rod, Classes.Item.Names.Rod);
            map.Add(Names.Stave, Classes.Item.Names.Stave);
            map.Add(Names.Wand, Classes.Item.Names.Wand);
            map.Add(Names.Jug, Classes.Item.Names.Jug);
            map.Add(Names.Amulet, Classes.Item.Names.Amulet);
            map.Add(Names.Dragon_Breath, Classes.Item.Names.Dragon_Breath);
            map.Add(Names.Bag, Classes.Item.Names.Bag);
            map.Add(Names.Defoliation, Classes.Item.Names.Defoliation);
            map.Add(Names.Ice_Storm, Classes.Item.Names.Ice_Storm);
            map.Add(Names.Book, Classes.Item.Names.Book);
            map.Add(Names.Boots, Classes.Item.Names.Boots);
            map.Add(Names.Hornets_Nest, Classes.Item.Names.Hornets_Nest);
            map.Add(Names.Bracers, Classes.Item.Names.Bracers);
            map.Add(Names.Piercing, Classes.Item.Names.Piercing);
            map.Add(Names.Brooch, Classes.Item.Names.Brooch);
            map.Add(Names.Elfin_Chain, Classes.Item.Names.Elfin_Chain);
            map.Add(Names.Wizardry, Classes.Item.Names.Wizardry);
            map.Add(Names.ac10, Classes.Item.Names.ac10);
            map.Add(Names.Dexterity, Classes.Item.Names.Dexterity);
            map.Add(Names.Fumbling, Classes.Item.Names.Fumbling);
            map.Add(Names.Chime, Classes.Item.Names.Chime);
            map.Add(Names.Cloak, Classes.Item.Names.Cloak);
            map.Add(Names.Crystal, Classes.Item.Names.Crystal);
            map.Add(Names.Cube, Classes.Item.Names.Cube);
            map.Add(Names.Cubic, Classes.Item.Names.Cubic);
            map.Add(Names.The_Dwarves, Classes.Item.Names.The_Dwarves);
            map.Add(Names.Decanter, Classes.Item.Names.Decanter);
            map.Add(Names.Gloves, Classes.Item.Names.Gloves);
            map.Add(Names.Drums, Classes.Item.Names.Drums);
            map.Add(Names.Dust, Classes.Item.Names.Dust);
            map.Add(Names.Thievery, Classes.Item.Names.Thievery);
            map.Add(Names.Hat, Classes.Item.Names.Hat);
            map.Add(Names.Flask, Classes.Item.Names.Flask);
            map.Add(Names.Gauntlets, Classes.Item.Names.Gauntlets);
            map.Add(Names.Gem, Classes.Item.Names.Gem);
            map.Add(Names.Girdle, Classes.Item.Names.Girdle);
            map.Add(Names.Helm, Classes.Item.Names.Helm);
            map.Add(Names.Horn, Classes.Item.Names.Horn);
            map.Add(Names.Stupidity, Classes.Item.Names.Stupidity);
            map.Add(Names.Incense, Classes.Item.Names.Incense);
            map.Add(Names.Stone, Classes.Item.Names.Stone);
            map.Add(Names.Ioun_Stone, Classes.Item.Names.Ioun_Stone);
            map.Add(Names.Javelin, Classes.Item.Names.Javelin);
            map.Add(Names.Jewel, Classes.Item.Names.Jewel);
            map.Add(Names.Ointment, Classes.Item.Names.Ointment);
            map.Add(Names.Pale_Blue, Classes.Item.Names.Pale_Blue);
            map.Add(Names.Scarlet_And, Classes.Item.Names.Scarlet_And);
            map.Add(Names.Manual, Classes.Item.Names.Manual);
            map.Add(Names.Incandescent, Classes.Item.Names.Incandescent);
            map.Add(Names.Deep_Red, Classes.Item.Names.Deep_Red);
            map.Add(Names.Pink, Classes.Item.Names.Pink);
            map.Add(Names.Mirror, Classes.Item.Names.Mirror);
            map.Add(Names.Necklace, Classes.Item.Names.Necklace);
            map.Add(Names.And_Green, Classes.Item.Names.And_Green);
            map.Add(Names.Blue, Classes.Item.Names.Blue);
            map.Add(Names.Pearl, Classes.Item.Names.Pearl);
            map.Add(Names.Powerlessness, Classes.Item.Names.Powerlessness);
            map.Add(Names.Vermin, Classes.Item.Names.Vermin);
            map.Add(Names.Pipes, Classes.Item.Names.Pipes);
            map.Add(Names.Hole, Classes.Item.Names.Hole);
            map.Add(Names.Dragon_Slayer, Classes.Item.Names.Dragon_Slayer);
            map.Add(Names.Robe, Classes.Item.Names.Robe);
            map.Add(Names.Rope, Classes.Item.Names.Rope);
            map.Add(Names.Frost_Brand, Classes.Item.Names.Frost_Brand);
            map.Add(Names.Berserker, Classes.Item.Names.Berserker);
            map.Add(Names.Scarab, Classes.Item.Names.Scarab);
            map.Add(Names.Spade, Classes.Item.Names.Spade);
            map.Add(Names.Sphere, Classes.Item.Names.Sphere);
            map.Add(Names.Blessed, Classes.Item.Names.Blessed);
            map.Add(Names.Talisman, Classes.Item.Names.Talisman);
            map.Add(Names.Tome, Classes.Item.Names.Tome);
            map.Add(Names.Trident, Classes.Item.Names.Trident);
            map.Add(Names.Grimoire, Classes.Item.Names.Grimoire);
            map.Add(Names.Well, Classes.Item.Names.Well);
            map.Add(Names.Wings, Classes.Item.Names.Wings);
            map.Add(Names.Vial, Classes.Item.Names.Vial);
            map.Add(Names.Lantern, Classes.Item.Names.Lantern);
            map.Add(Names.Flask_of_Oil, Classes.Item.Names.Flask_of_Oil);
            map.Add(Names.ONE0_ftDOT_Pole, Classes.Item.Names.ONE0_ftDOT_Pole);
            map.Add(Names.FIVE0_ftDOT_Rope, Classes.Item.Names.FIVE0_ftDOT_Rope);
            map.Add(Names.Iron, Classes.Item.Names.Iron);
            map.Add(Names.Thf_Prickly_Tools, Classes.Item.Names.Thf_Prickly_Tools);
            map.Add(Names.Iron_Rations, Classes.Item.Names.Iron_Rations);
            map.Add(Names.Standard_Rations, Classes.Item.Names.Standard_Rations);
            map.Add(Names.Holy_Symbol, Classes.Item.Names.Holy_Symbol);
            map.Add(Names.Holy_Water_vial, Classes.Item.Names.Holy_Water_vial);
            map.Add(Names.Unholy_Water_vial, Classes.Item.Names.Unholy_Water_vial);
            map.Add(Names.Barding, Classes.Item.Names.Barding);
            map.Add(Names.Dragon, Classes.Item.Names.Dragon);
            map.Add(Names.Lightning, Classes.Item.Names.Lightning);
            map.Add(Names.Saddle, Classes.Item.Names.Saddle);
            map.Add(Names.Staff, Classes.Item.Names.Staff);
            map.Add(Names.Drow, Classes.Item.Names.Drow);
            map.Add(Names.Wagon, Classes.Item.Names.Wagon);
            map.Add(Names.PLUS1, Classes.Item.Names.PLUS1);
            map.Add(Names.PLUS2, Classes.Item.Names.PLUS2);
            map.Add(Names.PLUS3, Classes.Item.Names.PLUS3);
            map.Add(Names.PLUS4, Classes.Item.Names.PLUS4);
            map.Add(Names.PLUS5, Classes.Item.Names.PLUS5);
            map.Add(Names.of, Classes.Item.Names.of);
            map.Add(Names.Vulnerability, Classes.Item.Names.Vulnerability);
            map.Add(Names.CloakIGNORE, Classes.Item.Names.CloakIGNORE);
            map.Add(Names.Displacement, Classes.Item.Names.Displacement);
            map.Add(Names.Torches, Classes.Item.Names.Torches);
            map.Add(Names.Oil, Classes.Item.Names.Oil);
            map.Add(Names.Speed, Classes.Item.Names.Speed);
            map.Add(Names.Tapestry, Classes.Item.Names.Tapestry);
            map.Add(Names.Spine, Classes.Item.Names.Spine);
            map.Add(Names.Copper, Classes.Item.Names.Copper);
            map.Add(Names.Silver, Classes.Item.Names.Silver);
            map.Add(Names.Electrum, Classes.Item.Names.Electrum);
            map.Add(Names.Gold, Classes.Item.Names.Gold);
            map.Add(Names.Platinum, Classes.Item.Names.Platinum);
            map.Add(Names.OintmentIGNORE, Classes.Item.Names.OintmentIGNORE);
            map.Add(Names.KeoghtumAPOSs, Classes.Item.Names.KeoghtumAPOSs);
            map.Add(Names.Sheet, Classes.Item.Names.Sheet);
            map.Add(Names.Strength, Classes.Item.Names.Strength);
            map.Add(Names.Healing, Classes.Item.Names.Healing);
            map.Add(Names.Holding, Classes.Item.Names.Holding);
            map.Add(Names.Extra, Classes.Item.Names.Extra);
            map.Add(Names.Gaseous_Form, Classes.Item.Names.Gaseous_Form);
            map.Add(Names.Slipperiness, Classes.Item.Names.Slipperiness);
            map.Add(Names.Jewelled, Classes.Item.Names.Jewelled);
            map.Add(Names.Flying, Classes.Item.Names.Flying);
            map.Add(Names.Treasure_Finding, Classes.Item.Names.Treasure_Finding);
            map.Add(Names.Fear, Classes.Item.Names.Fear);
            map.Add(Names.Disappearance, Classes.Item.Names.Disappearance);
            map.Add(Names.Statuette, Classes.Item.Names.Statuette);
            map.Add(Names.Fungus, Classes.Item.Names.Fungus);
            map.Add(Names.Chain, Classes.Item.Names.Chain);
            map.Add(Names.Pendant, Classes.Item.Names.Pendant);
            map.Add(Names.Broach, Classes.Item.Names.Broach);
            map.Add(Names.Of_Seeking, Classes.Item.Names.Of_Seeking);
            map.Add(Names.MINUS1, Classes.Item.Names.MINUS1);
            map.Add(Names.MINUS2, Classes.Item.Names.MINUS2);
            map.Add(Names.MINUS3, Classes.Item.Names.MINUS3);
            map.Add(Names.Lightning_Bolt, Classes.Item.Names.Lightning_Bolt);
            map.Add(Names.Fire_Resistance, Classes.Item.Names.Fire_Resistance);
            map.Add(Names.Magic_Missiles, Classes.Item.Names.Magic_Missiles);
            map.Add(Names.Save, Classes.Item.Names.Save);
            map.Add(Names.Clrc_Scroll, Classes.Item.Names.Clrc_Scroll);
            map.Add(Names.MU_Scroll, Classes.Item.Names.MU_Scroll);
            map.Add(Names.With_1_Spell, Classes.Item.Names.With_1_Spell);
            map.Add(Names.With_2_Spells, Classes.Item.Names.With_2_Spells);
            map.Add(Names.With_3_Spells, Classes.Item.Names.With_3_Spells);
            map.Add(Names.ProtDOT_Scroll, Classes.Item.Names.ProtDOT_Scroll);
            map.Add(Names.Jewelry, Classes.Item.Names.Jewelry);
            map.Add(Names.Fine, Classes.Item.Names.Fine);
            map.Add(Names.Huge, Classes.Item.Names.Huge);
            map.Add(Names.Bone, Classes.Item.Names.Bone);
            map.Add(Names.Brass, Classes.Item.Names.Brass);
            map.Add(Names.Key, Classes.Item.Names.Key);
            map.Add(Names.AC_2, Classes.Item.Names.AC_2);
            map.Add(Names.AC_6, Classes.Item.Names.AC_6);
            map.Add(Names.AC_4, Classes.Item.Names.AC_4);
            map.Add(Names.AC_3, Classes.Item.Names.AC_3);
            map.Add(Names.Of_ProtDOT, Classes.Item.Names.Of_ProtDOT);
            map.Add(Names.Paralyzation, Classes.Item.Names.Paralyzation);
            map.Add(Names.Ogre_Power, Classes.Item.Names.Ogre_Power);
            map.Add(Names.Invisibility, Classes.Item.Names.Invisibility);
            map.Add(Names.Missiles, Classes.Item.Names.Missiles);
            map.Add(Names.Elvenkind, Classes.Item.Names.Elvenkind);
            map.Add(Names.Rotting, Classes.Item.Names.Rotting);
            map.Add(Names.Covered, Classes.Item.Names.Covered);
            map.Add(Names.Efreeti, Classes.Item.Names.Efreeti);
            map.Add(Names.Bottle, Classes.Item.Names.Bottle);
            map.Add(Names.Missile_Attractor, Classes.Item.Names.Missile_Attractor);
            map.Add(Names.Of_Maglubiyet, Classes.Item.Names.Of_Maglubiyet);
            map.Add(Names.Secr_Door_AND_Trap_Det, Classes.Item.Names.Secr_Door_AND_Trap_Det);
            map.Add(Names.Gd_Dragon_Control, Classes.Item.Names.Gd_Dragon_Control);
            map.Add(Names.Feather_Falling, Classes.Item.Names.Feather_Falling);
            map.Add(Names.Giant_Strength, Classes.Item.Names.Giant_Strength);
            map.Add(Names.Restoring_LevelOPsCP, Classes.Item.Names.Restoring_LevelOPsCP);
            map.Add(Names.Flame_Tongue, Classes.Item.Names.Flame_Tongue);
            map.Add(Names.Fireballs, Classes.Item.Names.Fireballs);
            map.Add(Names.Spiritual, Classes.Item.Names.Spiritual);
            map.Add(Names.Boulder, Classes.Item.Names.Boulder);
            map.Add(Names.Diamond, Classes.Item.Names.Diamond);
            map.Add(Names.Emerald, Classes.Item.Names.Emerald);
            map.Add(Names.Opal, Classes.Item.Names.Opal);
            map.Add(Names.Saphire, Classes.Item.Names.Saphire);
            map.Add(Names.Of_Tyr, Classes.Item.Names.Of_Tyr);
            map.Add(Names.Of_Tempus, Classes.Item.Names.Of_Tempus);
            map.Add(Names.Of_Sune, Classes.Item.Names.Of_Sune);
            map.Add(Names.Wooden, Classes.Item.Names.Wooden);
            map.Add(Names.PLUS3_vs_Undead, Classes.Item.Names.PLUS3_vs_Undead);
            map.Add(Names.Pass, Classes.Item.Names.Pass);
            map.Add(Names.Cursed, Classes.Item.Names.Cursed);

            return map;
        }

        public Item(byte[] data, int offset)
        {
            DataIO.ReadObject(this, data, offset);
        }

        public Item(Classes.Item item)
        {
            name = item.name;
            type = (byte)item.type;
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
        }

        public Classes.Item Load()
        {
            Classes.Item item = new()
            {
                name = name,
                type = (ItemType)type,
                namenum = new Classes.Item.Names[3] { mapping[(Names)namenum[0]][0], mapping[(Names)namenum[1]][0], mapping[(Names)namenum[2]][0] },
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
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }

        private enum Names
        {
            WEAPONBattle_Axe = 0x01,
            WEAPONHand_Axe = 0x02,
            WEAPONBardiche = 0x03,
            WEAPONBec_De_Corbin = 0x04,
            WEAPONBillMINUSGuisarme = 0x05,

            WEAPONBo_Stick = 0x06,
            WEAPONClub = 0x07,
            WEAPONDagger = 0x08,
            WEAPONDart = 0x09,
            WEAPONFauchard = 0x0A,

            WEAPONFauchardMINUSFork = 0x0B,
            WEAPONFlail = 0x0C,
            WEAPONMilitary_Fork = 0x0D,
            WEAPONGlaive = 0x0E,
            WEAPONGlaiveMINUSGuisarme = 0x0F,

            WEAPONGuisarme = 0x10,
            WEAPONGuisarmeMINUSVoulge = 0x11,
            WEAPONHalberd = 0x12,
            WEAPONLucern_Hammer = 0x13,
            WEAPONHammer = 0x14,

            WEAPONJavelin = 0x15,
            WEAPONJo_Stick = 0x16,
            WEAPONMace = 0x17,
            WEAPONMorning_Star = 0x18,
            WEAPONPartisan = 0x19,

            WEAPONMilitary_Pick = 0x1A,
            WEAPONAwl_Pike = 0x1B,
            WEAPONQuarrel = 0x1C,
            WEAPONRanseur = 0x1D,
            WEAPONScimitar = 0x1E,

            WEAPONSpear = 0x1F,
            WEAPONSpetum = 0x20,
            WEAPONQuarter_Staff = 0x21,
            WEAPONBastard_Sword = 0x22,
            WEAPONBroad_Sword = 0x23,

            WEAPONLong_Sword = 0x24,
            WEAPONShort_Sword = 0x25,
            WEAPONTwoMINUSHanded_Sword = 0x26,
            WEAPONTrident = 0x27,
            WEAPONVoulge = 0x28,

            WEAPONComposite_Long_Bow = 0x29,
            WEAPONComposite_Short_Bow = 0x2A,
            WEAPONLong_Bow = 0x2B,
            WEAPONShort_Bow = 0x2C,
            WEAPONHeavy_Crossbow = 0x2D,

            WEAPONLight_Crossbow = 0x2E,
            WEAPONSling = 0x2F,
            ARMORMail = 0x30,
            ARMORArmor = 0x31,
            ARMORLeather = 0x32,

            ARMORPadded = 0x33,
            ARMORStudded = 0x34,
            ARMORRing = 0x35,
            ARMORScale = 0x36,
            ARMORChain = 0x37,

            ARMORSplint = 0x38,
            ARMORBanded = 0x39,
            ARMORPlate = 0x3A,
            ARMORShield = 0x3B,
            Woods = 0x3C,

            WEAPONArrow = 0x3D,
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
            CloakIGNORE = 0xA9,
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

            OintmentIGNORE = 0xB5,
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
        };
    }
}