namespace Classes.PoolRad
{
    internal class Tiles
    {
        readonly static Tile[] background =
        {
            new(1, 0, 0xff, 0),
            // DUNGCOM [0-24]
            new(   1, 2,  0 ), // [1]
            new(   1, 2,  1 ),
            new(   1, 2,  2 ),
            new(   1, 2,  3 ),
            new(1, 1, 0,  4 ),
            new(   1, 2,  5 ),
            new(   1, 2,  6 ),
            new(   1, 2,  7 ),
            new(1, 1, 0,  8 ),
            new(   1, 2,  9 ),
            new(1, 1, 0, 10 ),
            new(   1, 2, 11 ),
            new(1, 1, 0, 12 ),
            new(   1, 2, 13 ),
            new(1, 1, 0, 14 ),
            new(   1, 2, 15 ),
            new(1, 1, 0, 16 ),
            new(   1, 2, 17 ),
            new(   1, 2, 18 ),
            new(   1, 2, 19 ),
            new(   1, 2, 20 ),
            new(   1, 2, 21 ),
            new(1, 1, 0, 22 ), // [23] (empty)
            new(1, 1, 0, 23 ),
            new(   1, 2, 24 ),
            // RANDCOM [0-5]
            new(2, 2, 0, 0xff ), // [26] TABLE
            new(1, 1, 0, 0xff ), // [27] CHAIR
            new(1, 1, 0, 0xff ), // [28] CLOUDKILL
            new(1, 1, 0, 0xff ), // [29] EMPTY
            new(1, 1, 0, 38 ), // [30] STINKING CLOUD
            new(1, 1, 0, 39 ), // [31] DEAD BODY
            // WILDCOM [0-33]
            new(   1, 2,  0 ), // [32] TREE TOP 0
            new(   1, 2,  1 ), // [33] TREE TOP 1
            new(   1, 2,  2 ), // [34] TREE TOP 2
            new(   1, 2,  3 ), // [35] TREE TOP 3
            new(   1, 2, 32 ), // [36] TREE TOP 4
            new(1, 1, 0,  4 ), // [37] TREE BOTTOM 0
            new(1, 1, 0,  5 ), // [38] TREE BOTTOM 1
            new(1, 1, 0,  6 ), // [39] TREE BOTTOM 2
            new(1, 1, 0,  7 ), // [40] TREE BOTTOM 3
            new(1, 1, 0, 33 ), // [41] TREE BOTTOM 4
            new(   1, 2,  8 ), // [42] LOG 0
            new(   1, 2,  9 ), // [43] LOG 1
            new(1, 1, 0, 12 ), // [44] SWAMP 0
            new(1, 1, 0, 13 ), // [45] SWAMP 1
            new(1, 1, 0, 14 ), // [46] SWAMP 2
            new(1, 1, 0, 15 ), // [47] SWAMP 3
            new(2, 1, 0, 16 ), // [48] BUSH 0
            new(2, 1, 0, 17 ), // [49] BUSH 1
            new(2, 1, 0, 18 ), // [50] SHELL 0
            new(2, 1, 0, 19 ), // [51] SHELL 1
            new(2, 1, 0, 20 ), // [52] SHELL 2
            new(2, 1, 0, 21 ), // [53] SHELL 3
            new(1, 1, 0, 22 ), // [54] (empty)
            new(1, 1, 0, 23 ), // [55] GROUND 0
            new(1, 1, 0, 24 ), // [56] GROUND 1
            new(1, 1, 0, 25 ), // [57] GROUND 2
            new(2, 1, 0, 26 ), // [58] BOG 0
            new(2, 1, 0, 27 ), // [59] BOG 1
            new(   0, 0, 18 ), // [60] RIVER WEST BANK 0
            new(   0, 0, 18 ), // [61] RIVER WEST BANK 1
            new(   0, 0, 19 ), // [62] RIVER EAST BANK 0
            new(   0, 0, 19 ), // [63] RIVER EAST BANK 1
            new(1, 1, 0, 21 ), // [64] RIVER EAST BANK
            new(1, 1, 0, 20 ), // [65] RIVER WEST BANK
        };
        public static Tile[] Background { get => background; }
        public static void SetupBackground()
        {
            if (gbl.area_ptr.inDungeon != 0)
            {

                gbl.Load24x24Set(25, 0, 1, "DungCom");
            }
            else
            {
                gbl.Load24x24Set(34, 0, 1, "WildCom");
            }

            gbl.Load24x24Set(6, 25, 1, "RandCom");

            gbl.mapToBackGroundTile = new Struct_1D1BC();

            gbl.mapToBackGroundTile.drawTargetCursor = false;
            gbl.mapToBackGroundTile.size = 1;
            gbl.mapToBackGroundTile.ignoreWalls = false;
        }
    }
}
