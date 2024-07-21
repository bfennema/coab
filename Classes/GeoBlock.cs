using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class GeoBlock
    {
        byte[] data;
        public MapInfo[,] maps;

        public void LoadData(byte[] _data, int offset = 2, int max_x = 16, int max_y = 16)
        {
            data = new byte[max_x * max_y * 4];
            System.Array.Copy(_data, offset, data, 0, max_x * max_y * 4);

            maps = new MapInfo[max_y, max_x];

            for (int y = 0; y < max_y; y++)
            {
                for (int x = 0; x < max_x; x++)
                {
                    maps[y, x] = new MapInfo(data, x, y * max_x);
                }
            }
        }
    }

    public class WallDefs
    {
        const int maxBlocks = 3;
        public WallDefBlock[] blocks = new WallDefBlock[maxBlocks];

        public WallDefs()
        {
            blocks[0] = new WallDefBlock();
            blocks[1] = new WallDefBlock();
            blocks[2] = new WallDefBlock();
        }

        public void LoadData(int baseSet, byte[] _data)
        {
            int offset = 0;
            for (int i = 0; i < (_data.Length / 780); i++)
            {
                blocks[baseSet + i - 1].LoadData(_data, offset);

                offset += 780;
            }
        }

        public void BlockOffset(int set, int offset)
        {
            blocks[set - 1].Offset(offset);
        }
    }

    public class WallDefBlock
    {
        byte[,] data = new byte[5, 156];

        public void LoadData(byte[] _data, int offset)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 156; x++)
                {
                    data[y, x] = _data[offset++];
                }
            }
        }

        public int Id(int y, int x)
        {
            return data[y, x];
        }

        public void Offset(int off)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 156; x++)
                {
                    if (data[y, x] >= 0x2D)
                    {
                        data[y, x] += (byte)off;
                    }
                }
            }
        }
    }

    public class MapInfo
    {
        public byte wall_type_dir_0;
        public byte wall_type_dir_2;
        public byte wall_type_dir_4;
        public byte wall_type_dir_6;

        public byte x2;

        public byte x3_dir_0;
        public byte x3_dir_2;
        public byte x3_dir_4;
        public byte x3_dir_6;

        internal MapInfo(byte[] data, int map_x, int map_y_offset)
        {
            wall_type_dir_0 = (byte)((data[map_x + map_y_offset] >> 4) & 0x0f);
            wall_type_dir_2 = (byte)((data[map_x + map_y_offset]) & 0x0f);
            wall_type_dir_4 = (byte)((data[0x100 + map_x + map_y_offset] >> 4) & 0x0f);
            wall_type_dir_6 = (byte)((data[0x100 + map_x + map_y_offset]) & 0x0f);

            x2 = data[0x200 + map_y_offset + map_x];

            byte b = data[0x300 + map_y_offset + map_x];

            x3_dir_6 = (byte)((b >> 6) & 3);
            x3_dir_4 = (byte)((b >> 4) & 3);
            x3_dir_2 = (byte)((b >> 2) & 3);
            x3_dir_0 = (byte)(b & 3);
        }
    }
}
