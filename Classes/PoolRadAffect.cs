using System.Collections.Generic;

namespace Classes
{
    public enum PoolRadAffects
    {
        gnome_vs_goblin_kobold = 0x12,
        dwarf_vs_orc = 0x1a,
        dwarf_and_gnome_vs_giants = 0x2f,
        gnome_vs_gnoll = 0x30,
        con_saving_bonus = 0x61,
        elf_resist_sleep = 0x6b,
        halfelf_resistance = 0x7c,
    }

    /// <summary>
    /// Summary description for Affect.
    /// </summary>
    public class PoolRadAffect
    {
        public const int StructSize = 9;

        static BiLookup<PoolRadAffects, Affects> mapping;

        static void InitMapping()
        {
            if (mapping != null) { return; }

            mapping = new BiLookup<PoolRadAffects, Affects>();

            mapping.Add(PoolRadAffects.gnome_vs_goblin_kobold, Affects.gnome_vs_goblin_kobold);
            mapping.Add(PoolRadAffects.dwarf_vs_orc, Affects.dwarf_vs_orc);
            mapping.Add(PoolRadAffects.dwarf_and_gnome_vs_giants, Affects.dwarf_and_gnome_vs_giants);
            mapping.Add(PoolRadAffects.gnome_vs_gnoll, Affects.gnome_vs_gnoll);
            mapping.Add(PoolRadAffects.con_saving_bonus, Affects.con_saving_bonus);
            mapping.Add(PoolRadAffects.elf_resist_sleep, Affects.elf_resist_sleep);
            mapping.Add(PoolRadAffects.halfelf_resistance, Affects.halfelf_resistance);
        }

        public PoolRadAffect(PoolRadAffects _type, ushort _minutes, byte _affect_data, bool _call_spell_jump_list)
        {
            type = _type;
            minutes = _minutes;
            affect_data = _affect_data;
            callAffectTable = _call_spell_jump_list;
        }

        public PoolRadAffect(byte[] data, int offset)
        {
            type = (PoolRadAffects)data[offset + 0x0];
            minutes = Sys.ArrayToUshort(data, offset + 0x1);
            affect_data = data[offset + 0x3];
            callAffectTable = (data[offset + 0x4] != 0);
        }

        public void Load(Player player)
        {
            if (mapping == null) { InitMapping(); }

            IList<Affects> new_type = mapping[type];

            if (new_type.Count == 1)
            {
                Affect affect = new Affect(new_type[0], minutes, affect_data, callAffectTable);
                player.affects.Add(affect);
            }
        }

        [DataOffset(0x00, DataType.IByte)]
        public PoolRadAffects type;
        [DataOffset(0x01, DataType.Word)]
        public ushort minutes;
        [DataOffset(0x03, DataType.Byte)]
        public byte affect_data;
        [DataOffset(0x04, DataType.Bool)]
        public bool callAffectTable;

        public byte[] ToByteArray()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }
    }
}
