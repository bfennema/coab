using Classes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using static Classes.Item;

namespace Classes
{
    public class PoolRadItem
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

        static BiLookup<PoolRadNames, ItemNames> mapping;

        static void InitMapping()
        {
            if (mapping != null) { return; }

            mapping = new BiLookup<PoolRadNames, ItemNames>();

            mapping.Add(0, 0);
            mapping.Add(PoolRadNames.WEAPONBattle_Axe, ItemNames.WEAPONBattle_Axe);
            mapping.Add(PoolRadNames.WEAPONHand_Axe, ItemNames.WEAPONHand_Axe);
            mapping.Add(PoolRadNames.WEAPONBardiche, ItemNames.WEAPONBardiche);
            mapping.Add(PoolRadNames.WEAPONBec_De_Corbin, ItemNames.WEAPONBec_De_Corbin);
            mapping.Add(PoolRadNames.WEAPONBillMINUSGuisarme, ItemNames.WEAPONBillMINUSGuisarme);
            mapping.Add(PoolRadNames.WEAPONBo_Stick, ItemNames.WEAPONBo_Stick);
            mapping.Add(PoolRadNames.WEAPONClub, ItemNames.WEAPONClub);
            mapping.Add(PoolRadNames.WEAPONDagger, ItemNames.WEAPONDagger);
            mapping.Add(PoolRadNames.WEAPONDart, ItemNames.WEAPONDart);
            mapping.Add(PoolRadNames.WEAPONFauchard, ItemNames.WEAPONFauchard);
            mapping.Add(PoolRadNames.WEAPONFauchardMINUSFork, ItemNames.WEAPONFauchardMINUSFork);
            mapping.Add(PoolRadNames.WEAPONFlail, ItemNames.WEAPONFlail);
            mapping.Add(PoolRadNames.WEAPONMilitary_Fork, ItemNames.WEAPONMilitary_Fork);
            mapping.Add(PoolRadNames.WEAPONGlaive, ItemNames.WEAPONGlaive);
            mapping.Add(PoolRadNames.WEAPONGlaiveMINUSGuisarme, ItemNames.WEAPONGlaiveMINUSGuisarme);
            mapping.Add(PoolRadNames.WEAPONGuisarme, ItemNames.WEAPONGuisarme);
            mapping.Add(PoolRadNames.WEAPONGuisarmeMINUSVoulge, ItemNames.WEAPONGuisarmeMINUSVoulge);
            mapping.Add(PoolRadNames.WEAPONHalberd, ItemNames.WEAPONHalberd);
            mapping.Add(PoolRadNames.WEAPONLucern_Hammer, ItemNames.WEAPONLucern_Hammer);
            mapping.Add(PoolRadNames.WEAPONHammer, ItemNames.WEAPONHammer);
            mapping.Add(PoolRadNames.WEAPONJavelin, ItemNames.WEAPONJavelin);
            mapping.Add(PoolRadNames.WEAPONJo_Stick, ItemNames.WEAPONJo_Stick);
            mapping.Add(PoolRadNames.WEAPONMace, ItemNames.WEAPONMace);
            mapping.Add(PoolRadNames.WEAPONMorning_Star, ItemNames.WEAPONMorning_Star);
            mapping.Add(PoolRadNames.WEAPONPartisan, ItemNames.WEAPONPartisan);
            mapping.Add(PoolRadNames.WEAPONMilitary_Pick, ItemNames.WEAPONMilitary_Pick);
            mapping.Add(PoolRadNames.WEAPONAwl_Pike, ItemNames.WEAPONAwl_Pike);
            mapping.Add(PoolRadNames.WEAPONQuarrel, ItemNames.WEAPONQuarrel);
            mapping.Add(PoolRadNames.WEAPONRanseur, ItemNames.WEAPONRanseur);
            mapping.Add(PoolRadNames.WEAPONScimitar, ItemNames.WEAPONScimitar);
            mapping.Add(PoolRadNames.WEAPONSpear, ItemNames.WEAPONSpear);
            mapping.Add(PoolRadNames.WEAPONSpetum, ItemNames.WEAPONSpetum);
            mapping.Add(PoolRadNames.WEAPONQuarter_Staff, ItemNames.WEAPONQuarter_Staff);
            mapping.Add(PoolRadNames.WEAPONBastard_Sword, ItemNames.WEAPONBastard_Sword);
            mapping.Add(PoolRadNames.WEAPONBroad_Sword, ItemNames.WEAPONBroad_Sword);
            mapping.Add(PoolRadNames.WEAPONLong_Sword, ItemNames.WEAPONLong_Sword);
            mapping.Add(PoolRadNames.WEAPONShort_Sword, ItemNames.WEAPONShort_Sword);
            mapping.Add(PoolRadNames.WEAPONTwoMINUSHanded_Sword, ItemNames.WEAPONTwoMINUSHanded_Sword);
            mapping.Add(PoolRadNames.WEAPONTrident, ItemNames.WEAPONTrident);
            mapping.Add(PoolRadNames.WEAPONVoulge, ItemNames.WEAPONVoulge);
            mapping.Add(PoolRadNames.WEAPONComposite_Long_Bow, ItemNames.WEAPONComposite_Long_Bow);
            mapping.Add(PoolRadNames.WEAPONComposite_Short_Bow, ItemNames.WEAPONComposite_Short_Bow);
            mapping.Add(PoolRadNames.WEAPONLong_Bow, ItemNames.WEAPONLong_Bow);
            mapping.Add(PoolRadNames.WEAPONShort_Bow, ItemNames.WEAPONShort_Bow);
            mapping.Add(PoolRadNames.WEAPONHeavy_Crossbow, ItemNames.WEAPONHeavy_Crossbow);
            mapping.Add(PoolRadNames.WEAPONLight_Crossbow, ItemNames.WEAPONLight_Crossbow);
            mapping.Add(PoolRadNames.WEAPONSling, ItemNames.WEAPONSling);
            mapping.Add(PoolRadNames.ARMORMail, ItemNames.ARMORMail);
            mapping.Add(PoolRadNames.ARMORArmor, ItemNames.ARMORArmor);
            mapping.Add(PoolRadNames.ARMORLeather, ItemNames.ARMORLeather);
            mapping.Add(PoolRadNames.ARMORPadded, ItemNames.ARMORPadded);
            mapping.Add(PoolRadNames.ARMORStudded, ItemNames.ARMORStudded);
            mapping.Add(PoolRadNames.ARMORRing, ItemNames.ARMORRing);
            mapping.Add(PoolRadNames.ARMORScale, ItemNames.ARMORScale);
            mapping.Add(PoolRadNames.ARMORChain, ItemNames.ARMORChain);
            mapping.Add(PoolRadNames.ARMORSplint, ItemNames.ARMORSplint);
            mapping.Add(PoolRadNames.ARMORBanded, ItemNames.ARMORBanded);
            mapping.Add(PoolRadNames.ARMORPlate, ItemNames.ARMORPlate);
            mapping.Add(PoolRadNames.ARMORShield, ItemNames.ARMORShield);
            mapping.Add(PoolRadNames.Woods, ItemNames.Woods);
            mapping.Add(PoolRadNames.WEAPONArrow, ItemNames.WEAPONArrow);
            mapping.Add(PoolRadNames.Potion, ItemNames.Potion);
            mapping.Add(PoolRadNames.Scroll, ItemNames.Scroll);
            mapping.Add(PoolRadNames.Ring, ItemNames.Ring);
            mapping.Add(PoolRadNames.Rod, ItemNames.Rod);
            mapping.Add(PoolRadNames.Stave, ItemNames.Stave);
            mapping.Add(PoolRadNames.Wand, ItemNames.Wand);
            mapping.Add(PoolRadNames.Jug, ItemNames.Jug);
            mapping.Add(PoolRadNames.Amulet, ItemNames.Amulet);
            mapping.Add(PoolRadNames.Apparatus, ItemNames.Apparatus);
            mapping.Add(PoolRadNames.Bag, ItemNames.Bag);
            mapping.Add(PoolRadNames.Beaker, ItemNames.Beaker);
            mapping.Add(PoolRadNames.Boat, ItemNames.Boat);
            mapping.Add(PoolRadNames.Book, ItemNames.Book);
            mapping.Add(PoolRadNames.Boots, ItemNames.Boots);
            mapping.Add(PoolRadNames.Bowl, ItemNames.Bowl);
            mapping.Add(PoolRadNames.Bracers, ItemNames.Bracers);
            mapping.Add(PoolRadNames.Brazier, ItemNames.Brazier);
            mapping.Add(PoolRadNames.Brooch, ItemNames.Brooch);
            mapping.Add(PoolRadNames.Broom, ItemNames.Broom);
            mapping.Add(PoolRadNames.Purse, ItemNames.Purse);
            mapping.Add(PoolRadNames.Candle, ItemNames.Candle);
            mapping.Add(PoolRadNames.Carpet, ItemNames.Carpet);
            mapping.Add(PoolRadNames.Censer, ItemNames.Censer);
            mapping.Add(PoolRadNames.Chime, ItemNames.Chime);
            mapping.Add(PoolRadNames.Cloak, ItemNames.Cloak);
            mapping.Add(PoolRadNames.Crystal, ItemNames.Crystal);
            mapping.Add(PoolRadNames.Cube, ItemNames.Cube);
            mapping.Add(PoolRadNames.Cubic, ItemNames.Cubic);
            mapping.Add(PoolRadNames.Fortress, ItemNames.Fortress);
            mapping.Add(PoolRadNames.Decanter, ItemNames.Decanter);
            mapping.Add(PoolRadNames.Deck, ItemNames.Deck);
            mapping.Add(PoolRadNames.Drums, ItemNames.Drums);
            mapping.Add(PoolRadNames.Dust, ItemNames.Dust);
            mapping.Add(PoolRadNames.Eyes, ItemNames.Eyes);
            mapping.Add(PoolRadNames.Figurine, ItemNames.Figurine);
            mapping.Add(PoolRadNames.Flask, ItemNames.Flask);
            mapping.Add(PoolRadNames.Gauntlets, ItemNames.Gauntlets);
            mapping.Add(PoolRadNames.Gem, ItemNames.Gem);
            mapping.Add(PoolRadNames.Girdle, ItemNames.Girdle);
            mapping.Add(PoolRadNames.Helm, ItemNames.Helm);
            mapping.Add(PoolRadNames.Horn, ItemNames.Horn);
            mapping.Add(PoolRadNames.Horseshoes, ItemNames.Horseshoes);
            mapping.Add(PoolRadNames.Incense, ItemNames.Incense);
            mapping.Add(PoolRadNames.Stone, ItemNames.Stone);
            mapping.Add(PoolRadNames.Instrument, ItemNames.Instrument);
            mapping.Add(PoolRadNames.Javelin, ItemNames.Javelin);
            mapping.Add(PoolRadNames.Jewel, ItemNames.Jewel);
            mapping.Add(PoolRadNames.Ointment, ItemNames.Ointment);
            mapping.Add(PoolRadNames.Libram, ItemNames.Libram);
            mapping.Add(PoolRadNames.Lyre, ItemNames.Lyre);
            mapping.Add(PoolRadNames.Manual, ItemNames.Manual);
            mapping.Add(PoolRadNames.Mattock, ItemNames.Mattock);
            mapping.Add(PoolRadNames.Maul, ItemNames.Maul);
            mapping.Add(PoolRadNames.Medallion, ItemNames.Medallion);
            mapping.Add(PoolRadNames.Mirror, ItemNames.Mirror);
            mapping.Add(PoolRadNames.Necklace, ItemNames.Necklace);
            mapping.Add(PoolRadNames.Net, ItemNames.Net);
            mapping.Add(PoolRadNames.Pigment, ItemNames.Pigment);
            mapping.Add(PoolRadNames.Pearl, ItemNames.Pearl);
            mapping.Add(PoolRadNames.Periapt, ItemNames.Periapt);
            mapping.Add(PoolRadNames.Phylactery, ItemNames.Phylactery);
            mapping.Add(PoolRadNames.Pipes, ItemNames.Pipes);
            mapping.Add(PoolRadNames.Hole, ItemNames.Hole);
            mapping.Add(PoolRadNames.Token, ItemNames.Token);
            mapping.Add(PoolRadNames.Robe, ItemNames.Robe);
            mapping.Add(PoolRadNames.Rope, ItemNames.Rope);
            mapping.Add(PoolRadNames.Rug, ItemNames.Rug);
            mapping.Add(PoolRadNames.Saw, ItemNames.Saw);
            mapping.Add(PoolRadNames.Scarab, ItemNames.Scarab);
            mapping.Add(PoolRadNames.Spade, ItemNames.Spade);
            mapping.Add(PoolRadNames.Sphere, ItemNames.Sphere);
            mapping.Add(PoolRadNames.Talisman, ItemNames.Talisman);
            mapping.Add(PoolRadNames.Tome, ItemNames.Tome);
            mapping.Add(PoolRadNames.Trident, ItemNames.Trident);
            mapping.Add(PoolRadNames.Grimoire, ItemNames.Grimoire);
            mapping.Add(PoolRadNames.Well, ItemNames.Well);
            mapping.Add(PoolRadNames.Wings, ItemNames.Wings);
            mapping.Add(PoolRadNames.Vial, ItemNames.Vial);
            mapping.Add(PoolRadNames.Lantern, ItemNames.Lantern);
            mapping.Add(PoolRadNames.ONE0_ftDOT_Pole, ItemNames.ONE0_ftDOT_Pole);
            mapping.Add(PoolRadNames.FIVE0_ftDOT_Rope, ItemNames.FIVE0_ftDOT_Rope);
            mapping.Add(PoolRadNames.Iron, ItemNames.Iron);
            mapping.Add(PoolRadNames.Thf_Prickly_Tools, ItemNames.Thf_Prickly_Tools);
            mapping.Add(PoolRadNames.Iron_Rations, ItemNames.Iron_Rations);
            mapping.Add(PoolRadNames.Standard_Rations, ItemNames.Standard_Rations);
            mapping.Add(PoolRadNames.Holy_Symbol, ItemNames.Holy_Symbol);
            mapping.Add(PoolRadNames.Holy_Water_vial, ItemNames.Holy_Water_vial);
            mapping.Add(PoolRadNames.Unholy_Water_vial, ItemNames.Unholy_Water_vial);
            mapping.Add(PoolRadNames.Barding, ItemNames.Barding);
            mapping.Add(PoolRadNames.Dragon, ItemNames.Dragon);
            mapping.Add(PoolRadNames.Lightning, ItemNames.Lightning);
            mapping.Add(PoolRadNames.Saddle, ItemNames.Saddle);
            mapping.Add(PoolRadNames.Small_Raft, ItemNames.Small_Raft);
            mapping.Add(PoolRadNames.Cart, ItemNames.Cart);
            mapping.Add(PoolRadNames.Wagon, ItemNames.Wagon);
            mapping.Add(PoolRadNames.PLUS1, ItemNames.PLUS1);
            mapping.Add(PoolRadNames.PLUS2, ItemNames.PLUS2);
            mapping.Add(PoolRadNames.PLUS3, ItemNames.PLUS3);
            mapping.Add(PoolRadNames.PLUS4, ItemNames.PLUS4);
            mapping.Add(PoolRadNames.PLUS5, ItemNames.PLUS5);
            mapping.Add(PoolRadNames.of, ItemNames.of);
            mapping.Add(PoolRadNames.Displacement, ItemNames.Displacement);
            mapping.Add(PoolRadNames.Torches, ItemNames.Torches);
            mapping.Add(PoolRadNames.Oil, ItemNames.Oil);
            mapping.Add(PoolRadNames.Speed, ItemNames.Speed);
            mapping.Add(PoolRadNames.Tapestry, ItemNames.Tapestry);
            mapping.Add(PoolRadNames.Bodily_Health, ItemNames.Bodily_Health);
            mapping.Add(PoolRadNames.Copper, ItemNames.Copper);
            mapping.Add(PoolRadNames.Silver, ItemNames.Silver);
            mapping.Add(PoolRadNames.Electrum, ItemNames.Electrum);
            mapping.Add(PoolRadNames.Gold, ItemNames.Gold);
            mapping.Add(PoolRadNames.Platinum, ItemNames.Platinum);
            mapping.Add(PoolRadNames.KeoghtumAPOSs, ItemNames.KeoghtumAPOSs);
            mapping.Add(PoolRadNames.Sheet, ItemNames.Sheet);
            mapping.Add(PoolRadNames.Strength, ItemNames.Strength);
            mapping.Add(PoolRadNames.Healing, ItemNames.Healing);
            mapping.Add(PoolRadNames.Holding, ItemNames.Holding);
            mapping.Add(PoolRadNames.Extra, ItemNames.Extra);
            mapping.Add(PoolRadNames.Gaseous_Form, ItemNames.Gaseous_Form);
            mapping.Add(PoolRadNames.Slipperiness, ItemNames.Slipperiness);
            mapping.Add(PoolRadNames.Jewelled, ItemNames.Jewelled);
            mapping.Add(PoolRadNames.Flying, ItemNames.Flying);
            mapping.Add(PoolRadNames.Treasure_Finding, ItemNames.Treasure_Finding);
            mapping.Add(PoolRadNames.Fear, ItemNames.Fear);
            mapping.Add(PoolRadNames.Disappearance, ItemNames.Disappearance);
            mapping.Add(PoolRadNames.Statuette, ItemNames.Statuette);
            mapping.Add(PoolRadNames.Fungus, ItemNames.Fungus);
            mapping.Add(PoolRadNames.Chain, ItemNames.Chain);
            mapping.Add(PoolRadNames.Pendant, ItemNames.Pendant);
            mapping.Add(PoolRadNames.Broach, ItemNames.Broach);
            mapping.Add(PoolRadNames.Of_Seeking, ItemNames.Of_Seeking);
            mapping.Add(PoolRadNames.MINUS1, ItemNames.MINUS1);
            mapping.Add(PoolRadNames.MINUS2, ItemNames.MINUS2);
            mapping.Add(PoolRadNames.MINUS3, ItemNames.MINUS3);
            mapping.Add(PoolRadNames.Lightning_Bolt, ItemNames.Lightning_Bolt);
            mapping.Add(PoolRadNames.Fire_Resistance, ItemNames.Fire_Resistance);
            mapping.Add(PoolRadNames.Magic_Missiles, ItemNames.Magic_Missiles);
            mapping.Add(PoolRadNames.Save, ItemNames.Save);
            mapping.Add(PoolRadNames.Clerical_Scroll, ItemNames.Clrc_Scroll);
            mapping.Add(PoolRadNames.Magic_User_Scroll, ItemNames.MU_Scroll);
            mapping.Add(PoolRadNames.With_1_Spell, ItemNames.With_1_Spell);
            mapping.Add(PoolRadNames.With_2_Spells, ItemNames.With_2_Spells);
            mapping.Add(PoolRadNames.With_3_Spells, ItemNames.With_3_Spells);
            mapping.Add(PoolRadNames.Protection_Scroll, ItemNames.ProtDOT_Scroll);
            mapping.Add(PoolRadNames.Jewelry, ItemNames.Jewelry);
            mapping.Add(PoolRadNames.Fine, ItemNames.Fine);
            mapping.Add(PoolRadNames.Huge, ItemNames.Huge);
            mapping.Add(PoolRadNames.Bone, ItemNames.Bone);
            mapping.Add(PoolRadNames.Brass, ItemNames.Brass);
            mapping.Add(PoolRadNames.Key, ItemNames.Key);
            mapping.Add(PoolRadNames.AC_2, ItemNames.AC_2);
            mapping.Add(PoolRadNames.AC_6, ItemNames.AC_6);
            mapping.Add(PoolRadNames.AC_4, ItemNames.AC_4);
            mapping.Add(PoolRadNames.AC_3, ItemNames.AC_3);
            mapping.Add(PoolRadNames.Of_Protection, ItemNames.Of_ProtDOT);
            mapping.Add(PoolRadNames.Paralyzation, ItemNames.Paralyzation);
            mapping.Add(PoolRadNames.Ogre_Power, ItemNames.Ogre_Power);
            mapping.Add(PoolRadNames.Invisibility, ItemNames.Invisibility);
            mapping.Add(PoolRadNames.Missiles, ItemNames.Missiles);
            mapping.Add(PoolRadNames.Elvenkind, ItemNames.Elvenkind);
            mapping.Add(PoolRadNames.Rotting, ItemNames.Rotting);
            mapping.Add(PoolRadNames.Covered, ItemNames.Covered);
            mapping.Add(PoolRadNames.Efreeti, ItemNames.Efreeti);
            mapping.Add(PoolRadNames.Bottle, ItemNames.Bottle);
            mapping.Add(PoolRadNames.Missile_Attractor, ItemNames.Missile_Attractor);
            mapping.Add(PoolRadNames.Of_Maglubiyet, ItemNames.Of_Maglubiyet);
            mapping.Add(PoolRadNames.Secr_Door_AND_Trap_Det, ItemNames.Secr_Door_AND_Trap_Det);
            mapping.Add(PoolRadNames.Gd_Dragon_Control, ItemNames.Gd_Dragon_Control);
            mapping.Add(PoolRadNames.Feather_Falling, ItemNames.Feather_Falling);
            mapping.Add(PoolRadNames.Giant_Strength, ItemNames.Giant_Strength);
            mapping.Add(PoolRadNames.Restoring_LevelOPsCP, ItemNames.Restoring_LevelOPsCP);
            mapping.Add(PoolRadNames.Flame_Tongue, ItemNames.Flame_Tongue);
            mapping.Add(PoolRadNames.Fireballs, ItemNames.Fireballs);
            mapping.Add(PoolRadNames.Spiritual, ItemNames.Spiritual);
            mapping.Add(PoolRadNames.Boulder, ItemNames.Boulder);
            mapping.Add(PoolRadNames.Diamond, ItemNames.Diamond);
            mapping.Add(PoolRadNames.Emerald, ItemNames.Emerald);
            mapping.Add(PoolRadNames.Opal, ItemNames.Opal);
            mapping.Add(PoolRadNames.Saphire, ItemNames.Saphire);
            mapping.Add(PoolRadNames.Of_Tyr, ItemNames.Of_Tyr);
            mapping.Add(PoolRadNames.Of_Tempus, ItemNames.Of_Tempus);
            mapping.Add(PoolRadNames.Of_Sune, ItemNames.Of_Sune);
            mapping.Add(PoolRadNames.Wooden, ItemNames.Wooden);
            mapping.Add(PoolRadNames.PLUS3_vs_Undead, ItemNames.PLUS3_vs_Undead);
            mapping.Add(PoolRadNames.Pass, ItemNames.Pass);
            mapping.Add(PoolRadNames.Cursed, ItemNames.Cursed);
        }

        public PoolRadItem(byte[] data, int offset)
        {
            DataIO.ReadObject(this, data, offset);
        }

        public PoolRadItem(Item item)
        {
            if (mapping == null) { InitMapping(); }

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
            if (item.type == ItemType.Gauntlets && item.affect_3 == (Affects)131)
            {
                affect_2 = (byte)38;
                affect_3 = (byte)131;
            }
            else
            {
                affect_2 = (byte)item.affect_2;
                affect_3 = (byte)item.affect_3;
            }
        }

        public Item Load()
        {
            if (mapping == null) { InitMapping(); }

            Item item = new Item()
            {
                name = name,
                type = (ItemType)type,
                namenum = new ItemNames[3] { mapping[(PoolRadNames)namenum[0]][0], mapping[(PoolRadNames)namenum[1]][0], mapping[(PoolRadNames)namenum[2]][0] },
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

            if (item.type == ItemType.Gauntlets && item.affect_2 == (Affects)38 && item.affect_3 == (Affects)131)
            {
                item.affect_2 = 0;
            }

            ItemLibrary.Add(item);

            return item;
        }

        public byte[] Save(Player player, List<PoolRadAffect> affects)
        {
            byte[] data = new byte[StructSize];

            if (readied > 0 && type == (byte)ItemType.Gauntlets && affect_2 == 38 && affect_3 == 131)
            {
                byte affect_data;
                if (player.stats2.Str.cur == 18)
                {
                    affect_data = (byte)(player.stats2.Str00.cur + 1);
                }
                else
                {
                    affect_data = (byte)(player.stats2.Str.cur + 100);
                }
                PoolRadAffect affect = new PoolRadAffect(PoolRadAffects.strength, 0, affect_data, true);
                affects.Add(affect);
            }

            DataIO.WriteObject(this, data);

            return data;
        }

        private enum PoolRadNames
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
            Apparatus = 0x48,
            Bag = 0x49,
            Beaker = 0x4A,
            Boat = 0x4B,

            Book = 0x4C,
            Boots = 0x4D,
            Bowl = 0x4E,
            Bracers = 0x4F,
            Brazier = 0x50,

            Brooch = 0x51,
            Broom = 0x52,
            Purse = 0x53,
            Candle = 0x54,
            Carpet = 0x55,

            Censer = 0x56,
            Chime = 0x57,
            Cloak = 0x58,
            Crystal = 0x59,
            Cube = 0x5A,

            Cubic = 0x5B,
            Fortress = 0x5C,
            Decanter = 0x5D,
            Deck = 0x5E,
            Drums = 0x5F,

            Dust = 0x60,
            Eyes = 0x61,
            Figurine = 0x62,
            Flask = 0x63,
            Gauntlets = 0x64,

            Gem = 0x65,
            Girdle = 0x66,
            Helm = 0x67,
            Horn = 0x68,
            Horseshoes = 0x69,

            Incense = 0x6A,
            Stone = 0x6B,
            Instrument = 0x6C,
            Javelin = 0x6D,
            Jewel = 0x6E,

            Ointment = 0x6F,
            Libram = 0x70,
            Lyre = 0x71,
            Manual = 0x72,
            Mattock = 0x73,

            Maul = 0x74,
            Medallion = 0x75,
            Mirror = 0x76,
            Necklace = 0x77,
            Net = 0x78,

            Pigment = 0x79,
            Pearl = 0x7A,
            Periapt = 0x7B,
            Phylactery = 0x7C,
            Pipes = 0x7D,

            Hole = 0x7E,
            Token = 0x7F,
            Robe = 0x80,
            Rope = 0x81,
            Rug = 0x82,

            Saw = 0x83,
            Scarab = 0x84,
            Spade = 0x85,
            Sphere = 0x86,
            // 0x87,

            Talisman = 0x88,
            Tome = 0x89,
            Trident = 0x8A,
            Grimoire = 0x8B,
            Well = 0x8C,

            Wings = 0x8D,
            Vial = 0x8E,
            Lantern = 0x8F,
            // 0x90,
            // Oil = 0x91,

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
            Small_Raft = 0x9F,
            Cart = 0xA0,

            Wagon = 0xA1,
            PLUS1 = 0xA2,
            PLUS2 = 0xA3,
            PLUS3 = 0xA4,
            PLUS4 = 0xA5,

            PLUS5 = 0xA6,
            of = 0xA7,
            // 0xA8,
            //Cloak = 0xA9,
            Displacement = 0xAA,

            Torches = 0xAB,
            Oil = 0xAC,
            Speed = 0xAD,
            Tapestry = 0xAE,
            Bodily_Health = 0xAF,

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
            Clerical_Scroll = 0xD0,
            Magic_User_Scroll = 0xD1,
            With_1_Spell = 0xD2,

            With_2_Spells = 0xD3,
            With_3_Spells = 0xD4,
            Protection_Scroll = 0xD5,
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
            Of_Protection = 0xE0,
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