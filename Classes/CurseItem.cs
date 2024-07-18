using static Classes.Item;

namespace Classes
{
    public class CurseItem
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

        static BiLookup<CurseTypes, ItemType> type_map;
        static BiLookup<CurseNames, ItemNames> mapping;

        static void InitMapping()
        {
            if (mapping != null) { return; }

            type_map = new BiLookup<CurseTypes, ItemType>();

            type_map.Add(CurseTypes.Type_0, ItemType.Type_0);
            type_map.Add(CurseTypes.BattleAxe, ItemType.BattleAxe);
            type_map.Add(CurseTypes.HandAxe, ItemType.HandAxe);
            type_map.Add(CurseTypes.Bardiche, ItemType.Bardiche);
            type_map.Add(CurseTypes.BecDeCorbin, ItemType.BecDeCorbin);
            type_map.Add(CurseTypes.BillGuisarme, ItemType.BillGuisarme);
            type_map.Add(CurseTypes.BoStick, ItemType.BoStick);
            type_map.Add(CurseTypes.Club, ItemType.Club);
            type_map.Add(CurseTypes.Dagger, ItemType.Dagger);
            type_map.Add(CurseTypes.Dart, ItemType.Dart);
            type_map.Add(CurseTypes.Fauchard, ItemType.Fauchard);
            type_map.Add(CurseTypes.FauchardFork, ItemType.FauchardFork);
            type_map.Add(CurseTypes.Flail, ItemType.Flail);
            type_map.Add(CurseTypes.MilitaryFork, ItemType.MilitaryFork);
            type_map.Add(CurseTypes.Glaive, ItemType.Glaive);
            type_map.Add(CurseTypes.GlaiveGuisarme, ItemType.GlaiveGuisarme);
            type_map.Add(CurseTypes.Guisarme, ItemType.Guisarme);
            type_map.Add(CurseTypes.GuisarmeVoulge, ItemType.GuisarmeVoulge);
            type_map.Add(CurseTypes.Halberd, ItemType.Halberd);
            type_map.Add(CurseTypes.LucernHammer, ItemType.LucernHammer);
            type_map.Add(CurseTypes.Hammer, ItemType.Hammer);
            type_map.Add(CurseTypes.Javelin, ItemType.Javelin);
            type_map.Add(CurseTypes.JoStick, ItemType.JoStick);
            type_map.Add(CurseTypes.Mace, ItemType.Mace);
            type_map.Add(CurseTypes.MorningStar, ItemType.MorningStar);
            type_map.Add(CurseTypes.Partisan, ItemType.Partisan);
            type_map.Add(CurseTypes.MilitaryPick, ItemType.MilitaryPick);
            type_map.Add(CurseTypes.AwlPike, ItemType.AwlPike);
            type_map.Add(CurseTypes.Quarrel, ItemType.Quarrel);
            type_map.Add(CurseTypes.Ranseur, ItemType.Ranseur);
            type_map.Add(CurseTypes.Scimitar, ItemType.Scimitar);
            type_map.Add(CurseTypes.Spear, ItemType.Spear);
            type_map.Add(CurseTypes.Spetum, ItemType.Spetum);
            type_map.Add(CurseTypes.QuarterStaff, ItemType.QuarterStaff);
            type_map.Add(CurseTypes.BastardSword, ItemType.BastardSword);
            type_map.Add(CurseTypes.BroadSword, ItemType.BroadSword);
            type_map.Add(CurseTypes.LongSword, ItemType.LongSword);
            type_map.Add(CurseTypes.ShortSword, ItemType.ShortSword);
            type_map.Add(CurseTypes.TwoHandedSword, ItemType.TwoHandedSword);
            type_map.Add(CurseTypes.Trident, ItemType.Trident);
            type_map.Add(CurseTypes.Voulge, ItemType.Voulge);
            type_map.Add(CurseTypes.CompositeLongBow, ItemType.CompositeLongBow);
            type_map.Add(CurseTypes.CompositeShortBow, ItemType.CompositeShortBow);
            type_map.Add(CurseTypes.LongBow, ItemType.LongBow);
            type_map.Add(CurseTypes.ShortBow, ItemType.ShortBow);
            type_map.Add(CurseTypes.HeavyCrossbow, ItemType.HeavyCrossbow);
            type_map.Add(CurseTypes.LightCrossbow, ItemType.LightCrossbow);
            type_map.Add(CurseTypes.Sling, ItemType.Sling);
            type_map.Add(CurseTypes.Type_48, ItemType.Type_48);
            type_map.Add(CurseTypes.Type_49, ItemType.Type_49);
            type_map.Add(CurseTypes.LeatherArmor, ItemType.LeatherArmor);
            type_map.Add(CurseTypes.PaddedArmor, ItemType.PaddedArmor);
            type_map.Add(CurseTypes.StuddedLeather, ItemType.StuddedLeather);
            type_map.Add(CurseTypes.RingMail, ItemType.RingMail);
            type_map.Add(CurseTypes.ScaleMail, ItemType.ScaleMail);
            type_map.Add(CurseTypes.ChainMail, ItemType.ChainMail);
            type_map.Add(CurseTypes.SplintMail, ItemType.SplintMail);
            type_map.Add(CurseTypes.BandedMail, ItemType.BandedMail);
            type_map.Add(CurseTypes.PlateMail, ItemType.PlateMail);
            type_map.Add(CurseTypes.Shield, ItemType.Shield);
            type_map.Add(CurseTypes.ScrollOfProt, ItemType.ScrollOfProt);
            type_map.Add(CurseTypes.MUScroll, ItemType.MUScroll);
            type_map.Add(CurseTypes.ClrcScroll, ItemType.ClrcScroll);
            type_map.Add(CurseTypes.Gauntlets, ItemType.Gauntlets);
            type_map.Add(CurseTypes.Hat, ItemType.Hat);
            type_map.Add(CurseTypes.Girdle, ItemType.Girdle);
            type_map.Add(CurseTypes.Robe, ItemType.Robe);
            type_map.Add(CurseTypes.CloakOfProt, ItemType.CloakOfProt);
            type_map.Add(CurseTypes.Boots, ItemType.Boots);
            type_map.Add(CurseTypes.Ring, ItemType.Ring);
            type_map.Add(CurseTypes.GemsJewelry, ItemType.GemsJewelry);
            type_map.Add(CurseTypes.Potion, ItemType.Potion);
            type_map.Add(CurseTypes.Type_72, ItemType.Commodities);
            type_map.Add(CurseTypes.Arrow, ItemType.Arrow);
            type_map.Add(CurseTypes.Type_74, ItemType.HolySymbol);
            type_map.Add(CurseTypes.Type_75, ItemType.Key);
            type_map.Add(CurseTypes.Type_76, ItemType.CursedNecklace);
            type_map.Add(CurseTypes.Bracers, ItemType.Bracers);
            type_map.Add(CurseTypes.WandA, ItemType.WandA);
            type_map.Add(CurseTypes.WandB, ItemType.WandB);
            type_map.Add(CurseTypes.Type_80, ItemType.Type_80);
            type_map.Add(CurseTypes.Type_81, ItemType.Type_81);
            type_map.Add(CurseTypes.Type_82, ItemType.EfreetiBottle);
            type_map.Add(CurseTypes.Type_83, ItemType.Type_83);
            type_map.Add(CurseTypes.PotionOfGiantStr, ItemType.PotionOfGiantStr);
            type_map.Add(CurseTypes.Type_85, ItemType.HolyWater);
            type_map.Add(CurseTypes.FlaskOfOil, ItemType.FlaskOfOil);
            type_map.Add(CurseTypes.Type_87, ItemType.HillGiantBoulder);
            type_map.Add(CurseTypes.Type_88, ItemType.CloudGiantBoulder);
            type_map.Add(CurseTypes.Type_89, ItemType.FireGiantBoulder);
            type_map.Add(CurseTypes.Type_90, ItemType.RingOfFeatherFall);
            type_map.Add(CurseTypes.Type_91, ItemType.Pass);
            type_map.Add(CurseTypes.Cloak, ItemType.Cloak);
            type_map.Add(CurseTypes.RingOfProt, ItemType.RingOfProt);
            type_map.Add(CurseTypes.DrowMace, ItemType.DrowMace);
            type_map.Add(CurseTypes.ElvenChain, ItemType.ElvenChain);
            type_map.Add(CurseTypes.DrowChainMail, ItemType.DrowChainMail);
            type_map.Add(CurseTypes.DrowLongSword, ItemType.DrowLongSword);
            type_map.Add(CurseTypes.Type_98, ItemType.Spine);
            type_map.Add(CurseTypes.RingOfWizardry, ItemType.RingOfWizardry);
            type_map.Add(CurseTypes.DartOfHornetsNest, ItemType.DartOfHornetsNest);
            type_map.Add(CurseTypes.StaffSling, ItemType.StaffSling);
            type_map.Add(CurseTypes.Type_102, ItemType.Type_102);
            type_map.Add(CurseTypes.Type_103, ItemType.Type_103);
            type_map.Add(CurseTypes.Type_104, ItemType.Type_104);
            type_map.Add(CurseTypes.Type_105, ItemType.Type_105);
            type_map.Add(CurseTypes.Type_106, ItemType.Type_106);
            type_map.Add(CurseTypes.Type_107, ItemType.Type_107);
            type_map.Add(CurseTypes.Type_108, ItemType.Type_108);
            type_map.Add(CurseTypes.Type_109, ItemType.Type_109);
            type_map.Add(CurseTypes.Type_110, ItemType.Type_110);
            type_map.Add(CurseTypes.Type_128, ItemType.Type_128);

            mapping = new BiLookup<CurseNames, ItemNames>();

            mapping.Add(0, 0);
            mapping.Add(CurseNames.WEAPONBattle_Axe, ItemNames.WEAPONBattle_Axe);
            mapping.Add(CurseNames.WEAPONHand_Axe, ItemNames.WEAPONHand_Axe);
            mapping.Add(CurseNames.WEAPONBardiche, ItemNames.WEAPONBardiche);
            mapping.Add(CurseNames.WEAPONBec_De_Corbin, ItemNames.WEAPONBec_De_Corbin);
            mapping.Add(CurseNames.WEAPONBillMINUSGuisarme, ItemNames.WEAPONBillMINUSGuisarme);
            mapping.Add(CurseNames.WEAPONBo_Stick, ItemNames.WEAPONBo_Stick);
            mapping.Add(CurseNames.WEAPONClub, ItemNames.WEAPONClub);
            mapping.Add(CurseNames.WEAPONDagger, ItemNames.WEAPONDagger);
            mapping.Add(CurseNames.WEAPONDart, ItemNames.WEAPONDart);
            mapping.Add(CurseNames.WEAPONFauchard, ItemNames.WEAPONFauchard);
            mapping.Add(CurseNames.WEAPONFauchardMINUSFork, ItemNames.WEAPONFauchardMINUSFork);
            mapping.Add(CurseNames.WEAPONFlail, ItemNames.WEAPONFlail);
            mapping.Add(CurseNames.WEAPONMilitary_Fork, ItemNames.WEAPONMilitary_Fork);
            mapping.Add(CurseNames.WEAPONGlaive, ItemNames.WEAPONGlaive);
            mapping.Add(CurseNames.WEAPONGlaiveMINUSGuisarme, ItemNames.WEAPONGlaiveMINUSGuisarme);
            mapping.Add(CurseNames.WEAPONGuisarme, ItemNames.WEAPONGuisarme);
            mapping.Add(CurseNames.WEAPONGuisarmeMINUSVoulge, ItemNames.WEAPONGuisarmeMINUSVoulge);
            mapping.Add(CurseNames.WEAPONHalberd, ItemNames.WEAPONHalberd);
            mapping.Add(CurseNames.WEAPONLucern_Hammer, ItemNames.WEAPONLucern_Hammer);
            mapping.Add(CurseNames.WEAPONHammer, ItemNames.WEAPONHammer);
            mapping.Add(CurseNames.WEAPONJavelin, ItemNames.WEAPONJavelin);
            mapping.Add(CurseNames.WEAPONJo_Stick, ItemNames.WEAPONJo_Stick);
            mapping.Add(CurseNames.WEAPONMace, ItemNames.WEAPONMace);
            mapping.Add(CurseNames.WEAPONMorning_Star, ItemNames.WEAPONMorning_Star);
            mapping.Add(CurseNames.WEAPONPartisan, ItemNames.WEAPONPartisan);
            mapping.Add(CurseNames.WEAPONMilitary_Pick, ItemNames.WEAPONMilitary_Pick);
            mapping.Add(CurseNames.WEAPONAwl_Pike, ItemNames.WEAPONAwl_Pike);
            mapping.Add(CurseNames.WEAPONQuarrel, ItemNames.WEAPONQuarrel);
            mapping.Add(CurseNames.WEAPONRanseur, ItemNames.WEAPONRanseur);
            mapping.Add(CurseNames.WEAPONScimitar, ItemNames.WEAPONScimitar);
            mapping.Add(CurseNames.WEAPONSpear, ItemNames.WEAPONSpear);
            mapping.Add(CurseNames.WEAPONSpetum, ItemNames.WEAPONSpetum);
            mapping.Add(CurseNames.WEAPONQuarter_Staff, ItemNames.WEAPONQuarter_Staff);
            mapping.Add(CurseNames.WEAPONBastard_Sword, ItemNames.WEAPONBastard_Sword);
            mapping.Add(CurseNames.WEAPONBroad_Sword, ItemNames.WEAPONBroad_Sword);
            mapping.Add(CurseNames.WEAPONLong_Sword, ItemNames.WEAPONLong_Sword);
            mapping.Add(CurseNames.WEAPONShort_Sword, ItemNames.WEAPONShort_Sword);
            mapping.Add(CurseNames.WEAPONTwoMINUSHanded_Sword, ItemNames.WEAPONTwoMINUSHanded_Sword);
            mapping.Add(CurseNames.WEAPONTrident, ItemNames.WEAPONTrident);
            mapping.Add(CurseNames.WEAPONVoulge, ItemNames.WEAPONVoulge);
            mapping.Add(CurseNames.WEAPONComposite_Long_Bow, ItemNames.WEAPONComposite_Long_Bow);
            mapping.Add(CurseNames.WEAPONComposite_Short_Bow, ItemNames.WEAPONComposite_Short_Bow);
            mapping.Add(CurseNames.WEAPONLong_Bow, ItemNames.WEAPONLong_Bow);
            mapping.Add(CurseNames.WEAPONShort_Bow, ItemNames.WEAPONShort_Bow);
            mapping.Add(CurseNames.WEAPONHeavy_Crossbow, ItemNames.WEAPONHeavy_Crossbow);
            mapping.Add(CurseNames.WEAPONLight_Crossbow, ItemNames.WEAPONLight_Crossbow);
            mapping.Add(CurseNames.WEAPONSling, ItemNames.WEAPONSling);
            mapping.Add(CurseNames.ARMORMail, ItemNames.ARMORMail);
            mapping.Add(CurseNames.ARMORArmor, ItemNames.ARMORArmor);
            mapping.Add(CurseNames.ARMORLeather, ItemNames.ARMORLeather);
            mapping.Add(CurseNames.ARMORPadded, ItemNames.ARMORPadded);
            mapping.Add(CurseNames.ARMORStudded, ItemNames.ARMORStudded);
            mapping.Add(CurseNames.ARMORRing, ItemNames.ARMORRing);
            mapping.Add(CurseNames.ARMORScale, ItemNames.ARMORScale);
            mapping.Add(CurseNames.ARMORChain, ItemNames.ARMORChain);
            mapping.Add(CurseNames.ARMORSplint, ItemNames.ARMORSplint);
            mapping.Add(CurseNames.ARMORBanded, ItemNames.ARMORBanded);
            mapping.Add(CurseNames.ARMORPlate, ItemNames.ARMORPlate);
            mapping.Add(CurseNames.ARMORShield, ItemNames.ARMORShield);
            mapping.Add(CurseNames.Woods, ItemNames.Woods);
            mapping.Add(CurseNames.WEAPONArrow, ItemNames.WEAPONArrow);
            mapping.Add(CurseNames.Potion, ItemNames.Potion);
            mapping.Add(CurseNames.Scroll, ItemNames.Scroll);
            mapping.Add(CurseNames.Ring, ItemNames.Ring);
            mapping.Add(CurseNames.Rod, ItemNames.Rod);
            mapping.Add(CurseNames.Stave, ItemNames.Stave);
            mapping.Add(CurseNames.Wand, ItemNames.Wand);
            mapping.Add(CurseNames.Jug, ItemNames.Jug);
            mapping.Add(CurseNames.Amulet, ItemNames.Amulet);
            mapping.Add(CurseNames.Dragon_Breath, ItemNames.Dragon_Breath);
            mapping.Add(CurseNames.Bag, ItemNames.Bag);
            mapping.Add(CurseNames.Defoliation, ItemNames.Defoliation);
            mapping.Add(CurseNames.Ice_Storm, ItemNames.Ice_Storm);
            mapping.Add(CurseNames.Book, ItemNames.Book);
            mapping.Add(CurseNames.Boots, ItemNames.Boots);
            mapping.Add(CurseNames.Hornets_Nest, ItemNames.Hornets_Nest);
            mapping.Add(CurseNames.Bracers, ItemNames.Bracers);
            mapping.Add(CurseNames.Piercing, ItemNames.Piercing);
            mapping.Add(CurseNames.Brooch, ItemNames.Brooch);
            mapping.Add(CurseNames.Elfin_Chain, ItemNames.Elfin_Chain);
            mapping.Add(CurseNames.Wizardry, ItemNames.Wizardry);
            mapping.Add(CurseNames.ac10, ItemNames.ac10);
            mapping.Add(CurseNames.Dexterity, ItemNames.Dexterity);
            mapping.Add(CurseNames.Fumbling, ItemNames.Fumbling);
            mapping.Add(CurseNames.Chime, ItemNames.Chime);
            mapping.Add(CurseNames.Cloak, ItemNames.Cloak);
            mapping.Add(CurseNames.Crystal, ItemNames.Crystal);
            mapping.Add(CurseNames.Cube, ItemNames.Cube);
            mapping.Add(CurseNames.Cubic, ItemNames.Cubic);
            mapping.Add(CurseNames.The_Dwarves, ItemNames.The_Dwarves);
            mapping.Add(CurseNames.Decanter, ItemNames.Decanter);
            mapping.Add(CurseNames.Gloves, ItemNames.Gloves);
            mapping.Add(CurseNames.Drums, ItemNames.Drums);
            mapping.Add(CurseNames.Dust, ItemNames.Dust);
            mapping.Add(CurseNames.Thievery, ItemNames.Thievery);
            mapping.Add(CurseNames.Hat, ItemNames.Hat);
            mapping.Add(CurseNames.Flask, ItemNames.Flask);
            mapping.Add(CurseNames.Gauntlets, ItemNames.Gauntlets);
            mapping.Add(CurseNames.Gem, ItemNames.Gem);
            mapping.Add(CurseNames.Girdle, ItemNames.Girdle);
            mapping.Add(CurseNames.Helm, ItemNames.Helm);
            mapping.Add(CurseNames.Horn, ItemNames.Horn);
            mapping.Add(CurseNames.Stupidity, ItemNames.Stupidity);
            mapping.Add(CurseNames.Incense, ItemNames.Incense);
            mapping.Add(CurseNames.Stone, ItemNames.Stone);
            mapping.Add(CurseNames.Ioun_Stone, ItemNames.Ioun_Stone);
            mapping.Add(CurseNames.Javelin, ItemNames.Javelin);
            mapping.Add(CurseNames.Jewel, ItemNames.Jewel);
            mapping.Add(CurseNames.Ointment, ItemNames.Ointment);
            mapping.Add(CurseNames.Pale_Blue, ItemNames.Pale_Blue);
            mapping.Add(CurseNames.Scarlet_And, ItemNames.Scarlet_And);
            mapping.Add(CurseNames.Manual, ItemNames.Manual);
            mapping.Add(CurseNames.Incandescent, ItemNames.Incandescent);
            mapping.Add(CurseNames.Deep_Red, ItemNames.Deep_Red);
            mapping.Add(CurseNames.Pink, ItemNames.Pink);
            mapping.Add(CurseNames.Mirror, ItemNames.Mirror);
            mapping.Add(CurseNames.Necklace, ItemNames.Necklace);
            mapping.Add(CurseNames.And_Green, ItemNames.And_Green);
            mapping.Add(CurseNames.Blue, ItemNames.Blue);
            mapping.Add(CurseNames.Pearl, ItemNames.Pearl);
            mapping.Add(CurseNames.Powerlessness, ItemNames.Powerlessness);
            mapping.Add(CurseNames.Vermin, ItemNames.Vermin);
            mapping.Add(CurseNames.Pipes, ItemNames.Pipes);
            mapping.Add(CurseNames.Hole, ItemNames.Hole);
            mapping.Add(CurseNames.Dragon_Slayer, ItemNames.Dragon_Slayer);
            mapping.Add(CurseNames.Robe, ItemNames.Robe);
            mapping.Add(CurseNames.Rope, ItemNames.Rope);
            mapping.Add(CurseNames.Frost_Brand, ItemNames.Frost_Brand);
            mapping.Add(CurseNames.Berserker, ItemNames.Berserker);
            mapping.Add(CurseNames.Scarab, ItemNames.Scarab);
            mapping.Add(CurseNames.Spade, ItemNames.Spade);
            mapping.Add(CurseNames.Sphere, ItemNames.Sphere);
            mapping.Add(CurseNames.Blessed, ItemNames.Blessed);
            mapping.Add(CurseNames.Talisman, ItemNames.Talisman);
            mapping.Add(CurseNames.Tome, ItemNames.Tome);
            mapping.Add(CurseNames.Trident, ItemNames.Trident);
            mapping.Add(CurseNames.Grimoire, ItemNames.Grimoire);
            mapping.Add(CurseNames.Well, ItemNames.Well);
            mapping.Add(CurseNames.Wings, ItemNames.Wings);
            mapping.Add(CurseNames.Vial, ItemNames.Vial);
            mapping.Add(CurseNames.Lantern, ItemNames.Lantern);
            mapping.Add(CurseNames.Flask_of_Oil, ItemNames.Flask_of_Oil);
            mapping.Add(CurseNames.ONE0_ftDOT_Pole, ItemNames.ONE0_ftDOT_Pole);
            mapping.Add(CurseNames.FIVE0_ftDOT_Rope, ItemNames.FIVE0_ftDOT_Rope);
            mapping.Add(CurseNames.Iron, ItemNames.Iron);
            mapping.Add(CurseNames.Thf_Prickly_Tools, ItemNames.Thf_Prickly_Tools);
            mapping.Add(CurseNames.Iron_Rations, ItemNames.Iron_Rations);
            mapping.Add(CurseNames.Standard_Rations, ItemNames.Standard_Rations);
            mapping.Add(CurseNames.Holy_Symbol, ItemNames.Holy_Symbol);
            mapping.Add(CurseNames.Holy_Water_vial, ItemNames.Holy_Water_vial);
            mapping.Add(CurseNames.Unholy_Water_vial, ItemNames.Unholy_Water_vial);
            mapping.Add(CurseNames.Barding, ItemNames.Barding);
            mapping.Add(CurseNames.Dragon, ItemNames.Dragon);
            mapping.Add(CurseNames.Lightning, ItemNames.Lightning);
            mapping.Add(CurseNames.Saddle, ItemNames.Saddle);
            mapping.Add(CurseNames.Staff, ItemNames.Staff);
            mapping.Add(CurseNames.Drow, ItemNames.Drow);
            mapping.Add(CurseNames.Wagon, ItemNames.Wagon);
            mapping.Add(CurseNames.PLUS1, ItemNames.PLUS1);
            mapping.Add(CurseNames.PLUS2, ItemNames.PLUS2);
            mapping.Add(CurseNames.PLUS3, ItemNames.PLUS3);
            mapping.Add(CurseNames.PLUS4, ItemNames.PLUS4);
            mapping.Add(CurseNames.PLUS5, ItemNames.PLUS5);
            mapping.Add(CurseNames.of, ItemNames.of);
            mapping.Add(CurseNames.Vulnerability, ItemNames.Vulnerability);
            mapping.Add(CurseNames.Displacement, ItemNames.Displacement);
            mapping.Add(CurseNames.Torches, ItemNames.Torches);
            mapping.Add(CurseNames.Oil, ItemNames.Oil);
            mapping.Add(CurseNames.Speed, ItemNames.Speed);
            mapping.Add(CurseNames.Tapestry, ItemNames.Tapestry);
            mapping.Add(CurseNames.Spine, ItemNames.Spine);
            mapping.Add(CurseNames.Copper, ItemNames.Copper);
            mapping.Add(CurseNames.Silver, ItemNames.Silver);
            mapping.Add(CurseNames.Electrum, ItemNames.Electrum);
            mapping.Add(CurseNames.Gold, ItemNames.Gold);
            mapping.Add(CurseNames.Platinum, ItemNames.Platinum);
            mapping.Add(CurseNames.KeoghtumAPOSs, ItemNames.KeoghtumAPOSs);
            mapping.Add(CurseNames.Sheet, ItemNames.Sheet);
            mapping.Add(CurseNames.Strength, ItemNames.Strength);
            mapping.Add(CurseNames.Healing, ItemNames.Healing);
            mapping.Add(CurseNames.Holding, ItemNames.Holding);
            mapping.Add(CurseNames.Extra, ItemNames.Extra);
            mapping.Add(CurseNames.Gaseous_Form, ItemNames.Gaseous_Form);
            mapping.Add(CurseNames.Slipperiness, ItemNames.Slipperiness);
            mapping.Add(CurseNames.Jewelled, ItemNames.Jewelled);
            mapping.Add(CurseNames.Flying, ItemNames.Flying);
            mapping.Add(CurseNames.Treasure_Finding, ItemNames.Treasure_Finding);
            mapping.Add(CurseNames.Fear, ItemNames.Fear);
            mapping.Add(CurseNames.Disappearance, ItemNames.Disappearance);
            mapping.Add(CurseNames.Statuette, ItemNames.Statuette);
            mapping.Add(CurseNames.Fungus, ItemNames.Fungus);
            mapping.Add(CurseNames.Chain, ItemNames.Chain);
            mapping.Add(CurseNames.Pendant, ItemNames.Pendant);
            mapping.Add(CurseNames.Broach, ItemNames.Broach);
            mapping.Add(CurseNames.Of_Seeking, ItemNames.Of_Seeking);
            mapping.Add(CurseNames.MINUS1, ItemNames.MINUS1);
            mapping.Add(CurseNames.MINUS2, ItemNames.MINUS2);
            mapping.Add(CurseNames.MINUS3, ItemNames.MINUS3);
            mapping.Add(CurseNames.Lightning_Bolt, ItemNames.Lightning_Bolt);
            mapping.Add(CurseNames.Fire_Resistance, ItemNames.Fire_Resistance);
            mapping.Add(CurseNames.Magic_Missiles, ItemNames.Magic_Missiles);
            mapping.Add(CurseNames.Save, ItemNames.Save);
            mapping.Add(CurseNames.Clrc_Scroll, ItemNames.Clrc_Scroll);
            mapping.Add(CurseNames.MU_Scroll, ItemNames.MU_Scroll);
            mapping.Add(CurseNames.With_1_Spell, ItemNames.With_1_Spell);
            mapping.Add(CurseNames.With_2_Spells, ItemNames.With_2_Spells);
            mapping.Add(CurseNames.With_3_Spells, ItemNames.With_3_Spells);
            mapping.Add(CurseNames.ProtDOT_Scroll, ItemNames.ProtDOT_Scroll);
            mapping.Add(CurseNames.Jewelry, ItemNames.Jewelry);
            mapping.Add(CurseNames.Fine, ItemNames.Fine);
            mapping.Add(CurseNames.Huge, ItemNames.Huge);
            mapping.Add(CurseNames.Bone, ItemNames.Bone);
            mapping.Add(CurseNames.Brass, ItemNames.Brass);
            mapping.Add(CurseNames.Key, ItemNames.Key);
            mapping.Add(CurseNames.AC_2, ItemNames.AC_2);
            mapping.Add(CurseNames.AC_6, ItemNames.AC_6);
            mapping.Add(CurseNames.AC_4, ItemNames.AC_4);
            mapping.Add(CurseNames.AC_3, ItemNames.AC_3);
            mapping.Add(CurseNames.Of_ProtDOT, ItemNames.Of_ProtDOT);
            mapping.Add(CurseNames.Paralyzation, ItemNames.Paralyzation);
            mapping.Add(CurseNames.Ogre_Power, ItemNames.Ogre_Power);
            mapping.Add(CurseNames.Invisibility, ItemNames.Invisibility);
            mapping.Add(CurseNames.Missiles, ItemNames.Missiles);
            mapping.Add(CurseNames.Elvenkind, ItemNames.Elvenkind);
            mapping.Add(CurseNames.Rotting, ItemNames.Rotting);
            mapping.Add(CurseNames.Covered, ItemNames.Covered);
            mapping.Add(CurseNames.Efreeti, ItemNames.Efreeti);
            mapping.Add(CurseNames.Bottle, ItemNames.Bottle);
            mapping.Add(CurseNames.Missile_Attractor, ItemNames.Missile_Attractor);
            mapping.Add(CurseNames.Of_Maglubiyet, ItemNames.Of_Maglubiyet);
            mapping.Add(CurseNames.Secr_Door_AND_Trap_Det, ItemNames.Secr_Door_AND_Trap_Det);
            mapping.Add(CurseNames.Gd_Dragon_Control, ItemNames.Gd_Dragon_Control);
            mapping.Add(CurseNames.Feather_Falling, ItemNames.Feather_Falling);
            mapping.Add(CurseNames.Giant_Strength, ItemNames.Giant_Strength);
            mapping.Add(CurseNames.Restoring_LevelOPsCP, ItemNames.Restoring_LevelOPsCP);
            mapping.Add(CurseNames.Flame_Tongue, ItemNames.Flame_Tongue);
            mapping.Add(CurseNames.Fireballs, ItemNames.Fireballs);
            mapping.Add(CurseNames.Spiritual, ItemNames.Spiritual);
            mapping.Add(CurseNames.Boulder, ItemNames.Boulder);
            mapping.Add(CurseNames.Diamond, ItemNames.Diamond);
            mapping.Add(CurseNames.Emerald, ItemNames.Emerald);
            mapping.Add(CurseNames.Opal, ItemNames.Opal);
            mapping.Add(CurseNames.Saphire, ItemNames.Saphire);
            mapping.Add(CurseNames.Of_Tyr, ItemNames.Of_Tyr);
            mapping.Add(CurseNames.Of_Tempus, ItemNames.Of_Tempus);
            mapping.Add(CurseNames.Of_Sune, ItemNames.Of_Sune);
            mapping.Add(CurseNames.Wooden, ItemNames.Wooden);
            mapping.Add(CurseNames.PLUS3_vs_Undead, ItemNames.PLUS3_vs_Undead);
            mapping.Add(CurseNames.Pass, ItemNames.Pass);
            mapping.Add(CurseNames.Cursed, ItemNames.Cursed);
        }

        public CurseItem(byte[] data, int offset)
        {
            DataIO.ReadObject(this, data, offset);
        }

        public CurseItem(Item item)
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
        }

        public Item Load()
        {
            if (mapping == null) { InitMapping(); }

            Item item = new Item()
            {
                name = name,
                type = type_map[(CurseTypes)type][0],
                itemData = gbl.ItemDataTable[type],
                namenum = new ItemNames[3] { mapping[(CurseNames)namenum[0]][0], mapping[(CurseNames)namenum[1]][0], mapping[(CurseNames)namenum[2]][0] },
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

            ItemLibrary.Add(item);

            return item;
        }

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }

        public enum CurseTypes
        {
            Type_0 = 0,
            BattleAxe = 1,
            HandAxe = 2,
            Bardiche = 3,
            BecDeCorbin = 4,
            BillGuisarme = 5,
            BoStick = 6,
            Club = 7,
            Dagger = 8,
            Dart = 9,
            Fauchard = 10,
            FauchardFork = 11,
            Flail = 12,
            MilitaryFork = 13,
            Glaive = 14,
            GlaiveGuisarme = 15,
            Guisarme = 16,
            GuisarmeVoulge = 17,
            Halberd = 18,
            LucernHammer = 19,
            Hammer = 20,
            Javelin = 21,
            JoStick = 22,
            Mace = 23,
            MorningStar = 24,
            Partisan = 25,
            MilitaryPick = 26,
            AwlPike = 27,
            Quarrel = 28,
            Ranseur = 29,
            Scimitar = 30,
            Spear = 31,
            Spetum = 32,
            QuarterStaff = 33,
            BastardSword = 34,
            BroadSword = 35,
            LongSword = 36,
            ShortSword = 37,
            TwoHandedSword = 38,
            Trident = 39,
            Voulge = 40,
            CompositeLongBow = 41,
            CompositeShortBow = 42,
            LongBow = 43,
            ShortBow = 44,
            HeavyCrossbow = 45,
            LightCrossbow = 46,
            Sling = 47,
            Type_48 = 48,
            Type_49 = 49,
            LeatherArmor = 50,
            PaddedArmor = 51,
            StuddedLeather = 52,
            RingMail = 53,
            ScaleMail = 54,
            ChainMail = 55,
            SplintMail = 56,
            BandedMail = 57,
            PlateMail = 58,
            Shield = 59,
            ScrollOfProt = 60,
            MUScroll = 61,
            ClrcScroll = 62,
            Gauntlets = 63, // Gloves of Thievery, Gauntlets of Dexterity, Gauntlets of Fumbling
            Hat = 64,
            Girdle = 65,
            Robe = 66,
            CloakOfProt = 67,
            Boots = 68,
            Ring = 69,
            GemsJewelry = 70, // Ioun Stone, Dust of Disappearance, Necklace of Missiles
            Potion = 71, // Silver Mirror, Potion of Extra Healing, Potion of Speed, Potion of Invisibility
            Type_72 = 72,
            Arrow = 73,
            Type_74 = 74,
            Type_75 = 75,
            Type_76 = 76,
            Bracers = 77,
            WandA = 78,
            WandB = 79,
            Type_80 = 80,
            Type_81 = 81,
            Type_82 = 82,
            Type_83 = 83,
            PotionOfGiantStr = 84,
            Type_85 = 85,
            FlaskOfOil = 86,
            Type_87 = 87,
            Type_88 = 88,
            Type_89 = 89,
            Type_90 = 90,
            Type_91 = 91,
            Cloak = 92,
            RingOfProt = 93,
            DrowMace = 94,
            ElvenChain = 95,
            DrowChainMail = 96,
            DrowLongSword = 97,
            Type_98 = 98,
            RingOfWizardry = 99,
            DartOfHornetsNest = 100,
            StaffSling = 101,
            Type_102 = 102,
            Type_103 = 103,
            Type_104 = 104,
            Type_105 = 105,
            Type_106 = 106,
            Type_107 = 107,
            Type_108 = 108,
            Type_109 = 109,
            Type_110 = 110,
            Type_128 = 128,
        }

        private enum CurseNames
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
        };
    }
}