namespace Classes.PoolRad
{
    /// <summary>
    /// Summary description for Affect.
    /// </summary>
    public class Affect
    {
        [DataOffset(0x00, DataType.IByte)]
        public Affects type;
        [DataOffset(0x01, DataType.Word)]
        public ushort minutes;
        [DataOffset(0x03, DataType.Byte)]
        public byte affect_data;
        [DataOffset(0x04, DataType.Bool)]
        public bool callAffectTable;

        public const int StructSize = 9;

        readonly static BiLookup<Affects, Classes.Affects> mapping = InitMapping();

        private static BiLookup<Affects, Classes.Affects> InitMapping()
        {
            BiLookup<Affects, Classes.Affects> map = new();

            map.Add(Affects.gnome_vs_goblin_kobold, Classes.Affects.gnome_vs_goblin_kobold);
            map.Add(Affects.dwarf_vs_orc_goblin, Classes.Affects.dwarf_vs_orc_goblin);
            map.Add(Affects.giant_vs_dwarf_gnome, Classes.Affects.giant_vs_dwarf_gnome);
            map.Add(Affects.gnoll_bugbear_vs_gnome, Classes.Affects.gnoll_bugbear_vs_gnome);
            map.Add(Affects.con_saving_bonus, Classes.Affects.con_saving_bonus);
            map.Add(Affects.elf_resist_sleep, Classes.Affects.elf_resist_sleep);
            map.Add(Affects.halfelf_resistance, Classes.Affects.halfelf_resistance);

            return map;
        }

        public Affect(Affects _type, ushort _minutes, byte _affect_data, bool _call_spell_jump_list)
        {
            type = _type;
            minutes = _minutes;
            affect_data = _affect_data;
            callAffectTable = _call_spell_jump_list;
        }

        public Affect(byte[] data, int offset)
        {
            type = (Affects)data[offset + 0x0];
            minutes = Sys.ArrayToUshort(data, offset + 0x1);
            affect_data = data[offset + 0x3];
            callAffectTable = (data[offset + 0x4] != 0);
        }

        public Affect(Classes.Affect affect, Classes.Player player)
        {
            type = mapping[affect.type][0];
            minutes = affect.minutes;
            if (type == Affects.enlarge)
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
            else if (type == Affects.friends)
            {
                affect_data = (byte)player.stats2.Cha.cur;
            }
            affect_data = affect.affect_data;
            callAffectTable = affect.callAffectTable;

        }

        public void Load(Classes.Player player)
        {
            if (mapping[type].Count == 1)
            {
                Classes.Affect affect = new Classes.Affect(mapping[type][0], minutes, affect_data, callAffectTable);
                player.affects.Add(affect);
            }
            else if (type == Affects.friends)
            {
                player.stats2.Cha.cur = affect_data;
            }
            else if (type == Affects.strength || type == Affects.enlarge)
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

        public byte[] Save()
        {
            byte[] data = new byte[StructSize];

            DataIO.WriteObject(this, data);

            return data;
        }

        public enum Affects
        {
            enlarge = 0xc,
            friends = 0xe,
            gnome_vs_goblin_kobold = 0x12,
            dwarf_vs_orc_goblin = 0x1a,
            strength = 0x26,
            giant_vs_dwarf_gnome = 0x2f,
            gnoll_bugbear_vs_gnome = 0x30,
            con_saving_bonus = 0x61,
            elf_resist_sleep = 0x6b,
            halfelf_resistance = 0x7c,
        }
    }
}
