namespace Classes
{
    public enum PoolRadAffects
    {
        enlarge = 0xc,
        friends = 0xe,
        gnome_vs_goblin_kobold = 0x12,
        dwarf_vs_orc = 0x1a,
        strength = 0x26,
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

        public PoolRadAffect(Affect affect, Player player)
        {
            if (mapping == null) { InitMapping(); }

            type = mapping[affect.type][0];
            minutes = affect.minutes;
            if (type == PoolRadAffects.enlarge)
            {
                if (player.stats2.Str.cur == 18)
                {
                    affect_data = (byte)(player.stats2.Str00.cur + 1);
                }
                else
                {
                    affect_data = (byte)(player.stats2.Str.cur + 100);
                }
            }
            else if (type == PoolRadAffects.friends)
            {
                affect_data = (byte)player.stats2.Cha.cur;
            }
            affect_data = affect.affect_data;
            callAffectTable = affect.callAffectTable;

        }

        public void Load(Player player)
        {
            if (mapping == null) { InitMapping(); }

            if (mapping[type].Count == 1)
            {
                Affect affect = new Affect(mapping[type][0], minutes, affect_data, callAffectTable);
                player.affects.Add(affect);
            }
            else if (type == PoolRadAffects.friends)
            {
                player.stats2.Cha.cur = affect_data;
            }
            else if (type == PoolRadAffects.strength || type == PoolRadAffects.enlarge)
            {
                int str_00 = 0;
                int str = (int)(affect_data & 0x7F);

                if (str <= 101)
                {
                    str_00 = str - 1;
                    str = 18;
                }
                else
                {
                    str -= 100;
                    str_00 = 0;
                }

                player.stats2.Str.cur = str;
                player.stats2.Str00.cur = str_00;
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

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }
    }
}
