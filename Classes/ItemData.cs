using System;
using System.IO;

namespace Classes
{
    [FlagsAttribute]
    public enum ItemDataFlags : byte
    {
        None = 0,
        arrows = 0x01,
        flag_02 = 0x02,
        melee = 0x04,
        flag_08 = 0x08,
        flag_10 = 0x10,
        flag_20 = 0x20,
        flag_40 = 0x40,
        quarrels = 0x80,
    }

    public enum ItemSlot
    {
        Weapon = 0,
        Shield = 1,
        Armor = 2,
        Gauntlets = 3,
        Helm = 4,
        Belt = 5,
        Robe = 6,
        Cloak = 7,
        Boots = 8,
        Ring1 = 9,
        Ring2 = 10,
        Arrow = 11,
        Quarrel = 12,
        slot_13 = 13
    }

    public class ItemDataTable
    {
        ItemData[] table;

        public ItemDataTable(string fileName)
        {
            table = new ItemData[0x81];
            Read(fileName);
        }

        public async void Read(string fileName)
        {
            var stream = await gbl.file.Open(gbl.DataPath, fileName);

            stream.Seek(2, SeekOrigin.Begin);
            byte[] data = new byte[0x810];
            stream.Read(data, 0, 0x810);

            for (int i = 0; i < 0x81; i++)
            {
                table[i] = new ItemData(data, i * 0x10);
            }

            stream.Close();
        }

        public ItemData this[Item.Type index]
        {
            get { return table[(int)index]; }
            set { table[(int)index] = value; }
        }
    }

    /// <summary>
    /// Summary description for Struct_1C020.
    /// </summary>
    public class ItemData
    {
        public ItemSlot item_slot; //seg600:5D10 unk_1C020 - field_0
        public byte handsCount; //seg600:5D11 unk_1C021
        public byte diceCountLarge; //seg600:5D12 unk_1C022
        public byte diceSizeLarge; //seg600:5D13 unk_1C023
        public sbyte bonusLarge; //seg600:5D14
        public int numberAttacks; //seg600:5D15
        public byte field_6; //seg600:5D16 unk_1C026
        public byte field_7; //seg600:5D17 unk_1C027
        public byte field_8; //seg600:5D18
        public byte diceCountNormal; //seg600:5D19 field_9 maybe ranged 
        public byte diceSizeNormal; //seg600:5D1A field_A  maybe ranged
        public sbyte bonusNormal; //seg600:5D1B
        public int range; //seg600:5D1C unk_1C02C
        public byte classFlags; //seg600:5D1D field_D
        public ItemDataFlags field_E; //seg600:5D1E unk_1C02E
        public byte field_F; //seg600:5D1F 


        public ItemData(byte[] data, int offset)
        {
            item_slot = (ItemSlot)data[offset + 0];
            handsCount = data[offset + 1];
            diceCountLarge = data[offset + 2];
            diceSizeLarge = data[offset + 3];
            bonusLarge = (sbyte)data[offset + 4];
            numberAttacks = data[offset + 5];
            field_6 = data[offset + 6];
            field_7 = data[offset + 7];
            field_8 = data[offset + 8];
            diceCountNormal = data[offset + 9];
            diceSizeNormal = data[offset + 0xa];
            bonusNormal = (sbyte)data[offset + 0xb];
            range = data[offset + 0xc];
            classFlags = data[offset + 0xd];
            field_E = (ItemDataFlags)data[offset + 0xe];
            field_F = data[offset + 0xf];
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}",
                   item_slot, handsCount, diceCountLarge, diceSizeLarge, bonusLarge, numberAttacks, field_6, field_7, field_8,
                   diceCountNormal, diceSizeNormal, bonusNormal, range, classFlags, field_E, field_F);
        }
    }
}
