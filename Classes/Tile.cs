using System;

namespace Classes
{
    /// <summary>
    /// Summary description for Struct_189B4.
    /// </summary>
    public class Tile
    {
        private bool _blocked;
        private byte _moveCost; /* field_0 189B4 */
        private byte _y1; /* 189B5 */
        private byte _y2; /* 189B6 */
        private byte _tileIndex; /* 189B7 field_3 */
        public Tile(byte y1, byte y2, byte TileIndex)
        {
            _blocked = true;
            _moveCost = 0xFF;
            _y1 = y1;
            _y2 = y2;
            _tileIndex = TileIndex;
        }
        public Tile(byte MoveCost, byte y1, byte y2, byte TileIndex)
        {
            _blocked = false;
            _moveCost = MoveCost;
            _y1 = y1;
            _y2 = y2;
            _tileIndex = TileIndex;
        }
        public bool blocked { get => _blocked; }
        public byte move_cost { get { if (_blocked) { return 0xFF; } else { return _moveCost; } } }
        public byte y1 { get => _y1; }
        public byte y2 { get => _y2; }
        public byte tile_index { get => _tileIndex; }
    }
}
