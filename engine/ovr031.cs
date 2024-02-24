using Classes;
using Logging;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace engine
{
    class ovr031
    {
        const int Column_A = 5; //word_16E08
        const int Column_B = 4; //word_16E0A
        const int Column_C = 6; // word_16E0C
        const int Column_D = 4; // word_16E0E
        const int Column_E = 2; // word_16E10
        const int Column_F = 7; // word_16E12
        const int Column_G = 2; // word_16E14
        const int Column_H = 0; // word_16E16
        const int Column_I = 9; // word_16E18
        const int Column_J = 5; // word_16E1A
        const int Row_A = 4; // byte_16E1C
        const int Row_B = 3; // byte_16E1E
        const int Row_C = 3; // byte_16E20
        const int Row_D = 3; // byte_16E22
        const int Row_E = 1; // byte_16E24
        const int Row_F = 1; // byte_16E26
        const int Row_G = 1; // byte_16E28
        const int Row_H = 0; // byte_16E2A
        const int Row_I = 0; // byte_16E2C
        const int Row_J = 4; // byte_16E2E

        internal static void DrawAreaMap(int partyDir, int partyMapY, int partyMapX)
        { /* sub_7100F */
            const int displayWidth = 11;
            const int halfDisplayWidth = displayWidth / 2;
            const int displayOffset = 2;

            int offsetX = partyMapX - halfDisplayWidth;
            offsetX = System.Math.Max(offsetX, 0);
            offsetX = System.Math.Min(offsetX, halfDisplayWidth);


            int offsetY = partyMapY - halfDisplayWidth;
            offsetY = System.Math.Max(offsetY, 0);
            offsetY = System.Math.Min(offsetY, halfDisplayWidth);


            for (int y = 0; y < displayWidth; y++)
            {
                int mapY = y + offsetY;

                for (int x = 0; x < displayWidth; x++)
                {
                    int mapX = x + offsetX;

                    int symbol_id = 0x104;
                    int door_id = 0x104;

                    MapInfo mi = getMap_XXX(mapY, mapX);
                    if (mi != null)
                    {
                        if (mi.wall_type_dir_0 > 0) symbol_id += 1;
                        if (mi.wall_type_dir_2 > 0) symbol_id += 2;
                        if (mi.wall_type_dir_4 > 0) symbol_id += 4;
                        if (mi.wall_type_dir_6 > 0) symbol_id += 8;

                        if (mi.x3_dir_0 > 0) door_id += 1;
                        if (mi.x3_dir_2 > 0) door_id += 2;
                        if (mi.x3_dir_4 > 0) door_id += 4;
                        if (mi.x3_dir_6 > 0) door_id += 8;
                    }

                    ovr038.Put8x8Symbol(0, true, symbol_id, y + displayOffset, x + displayOffset);

                    if (Cheats.improved_area_map)
                    {
                        seg040.draw_clipped_nodraw(8);
                        seg040.draw_clipped_recolor(7, 1);
                        ovr038.Put8x8Symbol(0, true, door_id, y + displayOffset, x + displayOffset);
                        seg040.draw_clipped_recolor(17, 17);
                        seg040.draw_clipped_nodraw(17);
                    }
                }
            }

            int partyScreenY = partyMapX - offsetX;
            int partyScreenX = partyMapY - offsetY;

            seg040.draw_clipped_nodraw(8);
            ovr038.Put8x8Symbol(0, true, (partyDir >> 1) + 0x100, partyScreenX + displayOffset, partyScreenY + displayOffset);
            seg040.draw_clipped_nodraw(17);
            seg040.DrawOverlay();
        }


        internal static void Draw3dWorldBackground()
        {
            seg040.DrawColorBlock(gbl.sky_colour, 0x2c, 11, 16, 2);
            seg040.DrawColorBlock(0, 2, 11, 0x3c, 2);
            seg040.DrawColorBlock(8, 0x2a, 11, 0x3e, 2);
            if (gbl.game == Game.PoolOfRadiance)
            {
                seg040.DrawColorBlock(6, 0x30, 11, 0x38, 2);
            }
            else // if (gbl.game == Game.CurseOfTheAzureBonds)
            {
                if (get_wall_x2(gbl.mapPosY, gbl.mapPosY) < 0x80 &&
                    gbl.sky_colour == 11)
                {
                    int col_x = 2;
                    int row_y = 2;

                    int hour = gbl.area_ptr.time_hour;

                    if (hour >= 1 && hour <= 5)
                    {
                        if (gbl.mapDirection == 2)
                        {
                            seg040.OverlayBounded(gbl.sky_dax_251, 1, 0, (row_y + 5) - hour, 12 - 3);
                        }
                        else if (gbl.mapDirection == 4 && hour > 2)
                        {
                            seg040.OverlayBounded(gbl.sky_dax_251, 1, 0, (row_y + 5) - hour, (col_x + hour) - 3);
                        }
                    }
                    else if (hour >= 13 && hour <= 18)
                    {
                        if (gbl.mapDirection == 6)
                        {
                            seg040.OverlayBounded(gbl.sky_dax_251, 1, 0, (row_y + hour) - 13, col_x);
                        }
                        else if (gbl.mapDirection == 4 && hour >= 16)
                        {
                            seg040.OverlayBounded(gbl.sky_dax_251, 1, 0, (row_y + hour) - 13, (col_x + hour) - 8);
                        }
                    }

                    if (gbl.mapDirection == 0)
                    {
                        seg040.OverlayBounded(gbl.sky_dax_250, 1, 0, row_y, col_x);
                    }
                }

                seg040.OverlayBounded(gbl.sky_dax_252, 1, 0, 7, 2);
            }
        }


        static byte[] idxOffset = { 0, 2, 6, 10, 22, 38, 54, 110, 132, 154, 1 };   // seg600:0ADA
        static int[] colCount = { 1, 1, 1, 3, 2, 2, 7, 2, 2, 1 };                 // seg600:0AE4
        static int[] rowCount = { 2, 4, 4, 4, 8, 8, 8, 11, 11, 2 };               // seg600:0AEE


        internal static void draw_3D_8x8_titles(int offsetIndex, int arg_2, int rowStart, int colStart) /* sub_71434 */
        {
            int idx = idxOffset[offsetIndex];

            int colMax = colCount[offsetIndex] + colStart;
            int rowMax = rowCount[offsetIndex] + rowStart;

            int wallset = (arg_2 - 1) / 5;
            int slice = (arg_2 - 1) % 5;

            for (int rowY = rowStart; rowY < rowMax; rowY++)
            {
                for (int colX = colStart; colX < colMax; colX++)
                {
                    int symbolId = gbl.wallDef.blocks[wallset].Id(slice, idx);

                    if (rowY >= 0 && rowY <= 10 && colX >= 0 && colX <= 10 && symbolId > 0)
                    {
                        ovr038.Put8x8Symbol(1, true, symbolId, rowY + 2, colX + 2);

                        Display.Update();
                    }

                    idx++;
                }
            }
        }

        const int MapSize = 16; // 16x16 so 0-15

        internal static bool MapCoordIsValid(int mapY, int mapX)
        { /*sub_71542*/
            return (mapX < MapSize && mapX >= 0 && mapY < MapSize && mapX >= 0);
        }


        internal static byte WallDoorFlagsGet(int mapDir, int mapY, int mapX) /*sub_71573*/
        {
            if (MapCoordIsValid(mapY, mapX) == false &&
                (gbl.EclBlockId == 0 || gbl.EclBlockId == 10))
            {
                return 0;
            }

            mapX = Sys.WrapMinMax(mapX, 0, 15);
            mapY = Sys.WrapMinMax(mapY, 0, 15);

            MapInfo mi = gbl.geo_ptr.maps[mapY, mapX];
            byte var_1 = 1;

            switch (mapDir)
            {
                case 6:
                    if (mi.wall_type_dir_6 != 0)
                        var_1 = mi.x3_dir_6;
                    break;

                case 4:
                    if (mi.wall_type_dir_4 != 0)
                        var_1 = mi.x3_dir_4;
                    break;

                case 2:
                    if (mi.wall_type_dir_2 != 0)
                        var_1 = mi.x3_dir_2;
                    break;

                case 0:
                    if (mi.wall_type_dir_0 != 0)
                        var_1 = mi.x3_dir_0;
                    break;
            }

            return var_1;
        }


        internal static byte getMap_wall_type(int direction, int mapY, int mapX)
        {
            byte result = 0;

            MapInfo mi = getMap_XXX(mapY, mapX);

            if (mi != null)
            {
                switch (direction)
                {
                    case 0:
                        result = mi.wall_type_dir_0;
                        break;

                    case 2:
                        result = mi.wall_type_dir_2;
                        break;

                    case 4:
                        result = mi.wall_type_dir_4;
                        break;

                    case 6:
                        result = mi.wall_type_dir_6;
                        break;
                }
            }

            return result;
        }


        internal static MapInfo getMap_XXX(int mapY, int mapX)
        {
            MapInfo mi = null;

            if (MapCoordIsValid(mapY, mapX) == false &&
                (gbl.EclBlockId == 0 || gbl.EclBlockId == 10))
            {
            }
            else
            {
                if (mapX > 0x0F)
                {
                    mapX = 0;
                }
                else if (mapX < 0)
                {
                    mapX = 0x0F;
                }

                if (mapY > 0x0F)
                {
                    mapY = 0;
                }
                else if (mapY < 0)
                {
                    mapY = 0x0F;
                }

                mi = gbl.geo_ptr.maps[mapY, mapX];
            }

            return mi;
        }


        internal static byte get_wall_x2(int mapY, int mapX) /* sub_717A5 */
        {
            if (MapCoordIsValid(mapY, mapX) == false &&
                (gbl.EclBlockId == 0 || gbl.EclBlockId == 10))
            {
                return 0;
            }

            if (mapX > 0x0F)
            {
                mapX = 0;
            }
            if (mapX < 0)
            {
                mapX = 0x0F;
            }

            if (mapY > 0x0F)
            {
                mapY = 0;
            }
            if (mapY < 0)
            {
                mapY = 0x0F;
            }

            MapInfo mi = gbl.geo_ptr.maps[mapY, mapX];

            return mi.x2;
        }


        internal static void Draw3dWorld(byte partyDir, int partyPosY, int partyPosX) /* sub_71820 */
        {
            Display.UpdateStop();

            if (gbl.mapAreaDisplay == true)
            {
                DrawAreaMap(partyDir, partyPosY, partyPosX);
            }
            else
            {
                Draw3dWorldBackground();

                int dir_left = (partyDir + 6) % 8;
                int dir_behind = (partyDir + 4) % 8;
                int dir_right = (partyDir + 2) % 8;

                int drawStep = 2;

                int drawX = partyPosX + (drawStep * gbl.MapDirectionXDelta[partyDir]);
                int drawY = partyPosY + (drawStep * gbl.MapDirectionYDelta[partyDir]);

                do
                {
                    switch (drawStep)
                    {
                        case 2:
                            Draw3dWorldFar(partyDir, dir_left, dir_right, drawX, drawY);
                            break;

                        case 1:
                            Draw3dWorldMid(partyDir, dir_left, dir_right, drawX, drawY);
                            break;

                        case 0:
                            Draw3dWorldNear(partyDir, dir_left, dir_right, drawX, drawY);
                            break;
                    }


                    drawX += gbl.MapDirectionXDelta[dir_behind];
                    drawY += gbl.MapDirectionYDelta[dir_behind];

                    drawStep -= 1;

                } while (drawStep >= 0);
            }

            Display.UpdateStart();
            seg040.DrawOverlay();
        }


        private static void Draw3dWorldFar(byte partyDir, int dir_left, int dir_right, int drawX, int drawY)
        {
            int tmpX = drawX;
            int tmpY = drawY;
            int var_10 = 0;
            int Col = 0;
            byte var_17 = 0;

            while (var_10 < 4)
            {
                byte var_14 = getMap_wall_type(partyDir, tmpY, tmpX);

                if (MapCoordIsValid(tmpY, tmpX) == false &&
                    getMap_wall_type(dir_right, tmpY, tmpX) == 0)
                {
                    var_17 = 0;
                }

                if (var_14 != 0)
                {
                    if (var_17 > 0)
                    {
                        draw_3D_8x8_titles(9, var_17, Row_J, Column_J + Col + 1);
                    }

                    var_17 = var_14;

                    draw_3D_8x8_titles(0, var_14, Row_A, Column_A + Col);
                }
                else
                {
                    if (var_17 > 0 &&
                        getMap_wall_type(dir_left, tmpY - gbl.MapDirectionYDelta[dir_left], tmpX - gbl.MapDirectionXDelta[dir_left]) != 0)
                    {
                        draw_3D_8x8_titles(9, var_17, Row_J, Column_J + Col + 1);
                    }

                    var_17 = 0;
                }

                var_10++;
                Col -= 2;

                tmpX += gbl.MapDirectionXDelta[dir_left];
                tmpY += gbl.MapDirectionYDelta[dir_left];
            }

            tmpX = drawX;
            tmpY = drawY;
            var_10 = 0;
            Col = 0;
            var_17 = 0;

            while (var_10 < 4)
            {
                byte var_14 = getMap_wall_type(partyDir, tmpY, tmpX);

                if (MapCoordIsValid(tmpY, tmpX) == false &&
                    getMap_wall_type(dir_left, tmpY, tmpX) == 0)
                {
                    var_17 = 0;
                }

                if (var_14 != 0)
                {
                    if (var_17 > 0)
                    {
                        draw_3D_8x8_titles(9, var_17, Row_J, Column_J + Col - 1);
                    }

                    var_17 = var_14;
                    draw_3D_8x8_titles(0, var_14, Row_A, Column_A + Col);
                }
                else
                {
                    if (var_17 > 0 &&
                        getMap_wall_type(dir_right, tmpY - gbl.MapDirectionYDelta[dir_right], tmpX - gbl.MapDirectionXDelta[dir_right]) != 0)
                    {
                        draw_3D_8x8_titles(9, var_17, Row_J, Column_J + Col - 1);
                    }

                    var_17 = 0;
                }

                var_10++;
                Col += 2;

                tmpX += gbl.MapDirectionXDelta[dir_right];
                tmpY += gbl.MapDirectionYDelta[dir_right];
            }

            tmpX = drawX;
            tmpY = drawY;
            var_10 = 0;
            Col = 0;

            while (var_10 < 3)
            {
                byte var_15 = getMap_wall_type(dir_left, tmpY, tmpX);

                if (var_15 != 0)
                {
                    if (var_10 == 0)
                    {
                        draw_3D_8x8_titles(1, var_15, Row_B, Column_B + Col);
                    }
                    else
                    {
                        draw_3D_8x8_titles(1, var_15, Row_B, Column_B + Col - 1);
                    }

                }

                var_10++;
                Col -= 2;

                tmpX += gbl.MapDirectionXDelta[dir_left];
                tmpY += gbl.MapDirectionYDelta[dir_left];
            }

            tmpX = drawX;
            tmpY = drawY;
            var_10 = 0;
            Col = 0;

            while (var_10 < 3)
            {
                byte var_15 = getMap_wall_type(dir_right, tmpY, tmpX);

                if (var_15 != 0)
                {
                    if (var_10 == 0)
                    {
                        draw_3D_8x8_titles(2, var_15, Row_C, Column_C + Col);
                    }
                    else
                    {
                        draw_3D_8x8_titles(2, var_15, Row_C, Column_C + Col + 1);
                    }
                }

                var_10++;
                Col += 2;

                tmpX += gbl.MapDirectionXDelta[dir_right];
                tmpY += gbl.MapDirectionYDelta[dir_right];
            }
        }


        private static void Draw3dWorldMid(byte partyDir, int dir_left, int dir_right, int var_5, int var_7)
        {
            int tmpX = gbl.MapDirectionXDelta[dir_left] + var_5 + gbl.MapDirectionXDelta[dir_left];
            int tmpY = gbl.MapDirectionYDelta[dir_left] + var_7 + gbl.MapDirectionYDelta[dir_left];
            int var_10 = 0;
            int var_12 = -6;

            while (var_10 < 3)
            {
                byte var_14 = getMap_wall_type(partyDir, tmpY, tmpX);
                if (var_14 != 0)
                {
                    draw_3D_8x8_titles(3, var_14, Row_D, Column_D + var_12);
                }

                byte var_15 = getMap_wall_type(dir_left, tmpY, tmpX);
                if (var_15 != 0)
                {
                    draw_3D_8x8_titles(4, var_15, Row_E, Column_E + var_12);
                }

                var_10++;
                var_12 += 3;
                tmpX += gbl.MapDirectionXDelta[dir_right];
                tmpY += gbl.MapDirectionYDelta[dir_right];
            }


            tmpX = gbl.MapDirectionXDelta[dir_right] + gbl.MapDirectionXDelta[dir_right] + var_5;
            tmpY = gbl.MapDirectionYDelta[dir_right] + gbl.MapDirectionYDelta[dir_right] + var_7;
            var_10 = 0;
            var_12 = 6;
            while (var_10 < 3)
            {
                byte var_14 = getMap_wall_type(partyDir, tmpY, tmpX);

                if (var_14 != 0)
                {
                    draw_3D_8x8_titles(3, var_14, Row_D, Column_D + var_12);
                }

                byte var_15 = getMap_wall_type(dir_right, tmpY, tmpX);

                if (var_15 != 0)
                {
                    draw_3D_8x8_titles(5, var_15, Row_F, Column_F + var_12);
                }

                var_10++;
                var_12 -= 3;

                tmpX += gbl.MapDirectionXDelta[dir_left];
                tmpY += gbl.MapDirectionYDelta[dir_left];
            }
        }


        private static void Draw3dWorldNear(byte partyDir, int dir_left, int dir_right, int var_5, int var_7)
        {
            int tmpX = gbl.MapDirectionXDelta[dir_left] + var_5;
            int tmpY = gbl.MapDirectionYDelta[dir_left] + var_7;
            int var_10 = 0;
            int var_12 = -7;

            while (var_10 < 2)
            {
                byte var_14 = getMap_wall_type(partyDir, tmpY, tmpX);

                if (var_14 != 0)
                {
                    draw_3D_8x8_titles(6, var_14, Row_G, Column_G + var_12);
                }

                byte var_15 = getMap_wall_type(dir_left, tmpY, tmpX);

                if (var_15 != 0)
                {
                    draw_3D_8x8_titles(7, var_15, Row_H, Column_H + var_12);
                }

                var_10++;

                var_12 += 7;
                tmpX += gbl.MapDirectionXDelta[dir_right];
                tmpY += gbl.MapDirectionYDelta[dir_right];
            }


            tmpX = var_5 + gbl.MapDirectionXDelta[dir_right];
            tmpY = var_7 + gbl.MapDirectionYDelta[dir_right];
            var_10 = 0;
            var_12 = 7;

            while (var_10 < 2)
            {

                byte var_14 = getMap_wall_type(partyDir, tmpY, tmpX);

                if (var_14 != 0)
                {

                    draw_3D_8x8_titles(6, var_14, Row_G, var_12 + Column_G);
                }

                int var_15 = getMap_wall_type(dir_right, tmpY, tmpX);

                if (var_15 != 0)
                {
                    draw_3D_8x8_titles(8, var_15, Row_I, var_12 + Column_I);
                }

                var_10++;
                var_12 -= 7;

                tmpX += gbl.MapDirectionXDelta[dir_left];
                tmpY += gbl.MapDirectionYDelta[dir_left];
            }
        }

        internal static void LoadWalldef(int symbolSet, int block_id)
        {
            if (symbolSet >= 1 && symbolSet < 4)
            {
                string area_text = gbl.game_area.ToString();
                byte[] data;

                ushort decode_size;
                seg042.load_decode_dax(out data, out decode_size, block_id, "WALLDEF" + area_text + ".dax");

                if (decode_size == 0 ||
                    ((decode_size / 0x30C) + symbolSet) > 4)
                {
                    Logger.LogAndExit("Unable to load {0} from WALLDEF{1}.", block_id, area_text);
                }

                int var_A = gbl.symbol_set_fix[symbolSet] - gbl.symbol_set_fix[1];

                gbl.wallDef.LoadData(symbolSet, data);

                int blockCount = decode_size / 0x30C;

                for (int block = 0; block < blockCount; block++)
                {
                    int idx = symbolSet + block;
                    if (idx >= 1 && idx <= 3)
                    {
                        gbl.setBlocks[idx - 1].Reset();

                        gbl.wallDef.BlockOffset(idx, var_A);

                        if (blockCount > 1)
                        {
                            if (block_id == 0)
                            {
                                ovr038.Load8x8D(idx, (10 * 10) + block + 1);
                            }
                            else
                            {
                                ovr038.Load8x8D(idx, (block_id * 10) + block + 1);
                            }
                        }
                        else
                        {
                            ovr038.Load8x8D(idx, block_id);
                        }
                    }
                }

                gbl.setBlocks[symbolSet - 1].blockId = block_id;
                gbl.setBlocks[symbolSet - 1].setId = symbolSet;
            }
        }


        internal static void Load3DMap(int blockId)
        {
            byte[] data;
            ushort bytesRead;

            seg042.load_decode_dax(out data, out bytesRead, blockId, "GEO" + gbl.game_area.ToString() + ".dax");

            if (bytesRead == 0 || bytesRead != 0x402)
            {
                Logger.LogAndExit("Unable to load geo in Load3DMap.");
            }

            gbl.geo_ptr.LoadData(data);

            gbl.area_ptr.current_3DMap_block_id = (byte)blockId;
        }

        internal static byte[,] wilderness =
        {
            // y=0,    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,   18,   19,   20,   21,   22,   23,   24,   25,   26,   27,   28,  29,    30,   31,   32,   33,   34,   35 
            { 0x00, 0x01, 0x00, 0x01, 0x02, 0x0D, 0x0B, 0x0A, 0x21, 0x23, 0x0A, 0x23, 0x21, 0x1A, 0x03, 0x13, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0xB0, 0x4D, 0x4D }, //  0 = x
            { 0x01, 0x00, 0x01, 0x04, 0x06, 0x0E, 0x0A, 0x23, 0x21, 0x0A, 0x21, 0x0B, 0x23, 0x1A, 0x11, 0x14, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0xAC, 0x4D, 0x4D }, //  1
            { 0x00, 0x01, 0x00, 0x05, 0x0C, 0x21, 0x23, 0x21, 0x0B, 0x21, 0x23, 0x0A, 0x21, 0x1B, 0x06, 0x2A, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0xB0, 0x4D, 0x4D }, //  2
            { 0x01, 0x00, 0x00, 0x02, 0x0E, 0x0A, 0x21, 0x23, 0x0A, 0x0B, 0x23, 0x21, 0x23, 0x1C, 0x03, 0x16, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0xFD, 0xAC, 0x4D, 0x4D }, //  3
            { 0x00, 0x01, 0x01, 0x0C, 0x0B, 0x21, 0x23, 0x0B, 0x21, 0x0A, 0x21, 0x23, 0x21, 0x1D, 0x11, 0x17, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0xA9, 0x4D, 0x4D }, //  4
            { 0x01, 0x00, 0x04, 0x0E, 0x0A, 0x0B, 0x21, 0x0A, 0x23, 0x21, 0x23, 0x21, 0x0A, 0x0B, 0x1C, 0x17, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0xAE, 0x4D, 0x4D }, //  5
            { 0x00, 0x04, 0x11, 0x0D, 0x21, 0x0A, 0x0B, 0x21, 0x23, 0x0B, 0x21, 0x0B, 0x21, 0x0A, 0x1D, 0x18, 0x00, 0x01, 0x00, 0x51, 0x52, 0x53, 0x54, 0x01, 0x00, 0x51, 0x54, 0x01, 0x00, 0x01, 0x00, 0x01, 0xAC, 0x4D, 0x4D, 0x4D }, //  6
            { 0x01, 0x05, 0x06, 0x0E, 0x0B, 0x21, 0x0A, 0x23, 0x21, 0x0A, 0x21, 0x0A, 0x23, 0x21, 0x1E, 0x13, 0x01, 0x00, 0x01, 0x5B, 0x4E, 0x4E, 0x4E, 0x53, 0x52, 0x4E, 0x4E, 0x53, 0x52, 0x53, 0x54, 0x00, 0x01, 0xAB, 0x4D, 0x4D }, //  7
            { 0x00, 0x07, 0x0C, 0x0B, 0x0A, 0x23, 0x21, 0x21, 0x0B, 0x21, 0x23, 0x21, 0x21, 0x1E, 0x03, 0x14, 0x00, 0x01, 0x00, 0x01, 0x5A, 0x4F, 0x5A, 0x5B, 0x4E, 0x4E, 0x50, 0x4E, 0x4E, 0x50, 0x55, 0x01, 0x00, 0xB0, 0x4D, 0x4D }, //  8
            { 0x01, 0x05, 0x20, 0x0A, 0x21, 0x23, 0x1E, 0x20, 0x0A, 0x23, 0x21, 0x23, 0x1E, 0x03, 0x11, 0x16, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x5A, 0x4F, 0x01, 0x5A, 0x4F, 0x00, 0x01, 0x00, 0x01, 0xAC, 0x4D, 0x4D }, //  9
            { 0x00, 0x07, 0x03, 0x09, 0x0A, 0x1E, 0x06, 0x11, 0xC8, 0xCA, 0xCC, 0xCF, 0x03, 0x11, 0x06, 0x17, 0x00, 0x85, 0x85, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0xA9, 0x4D, 0x4D }, // 10
            { 0x01, 0x08, 0x11, 0x22, 0x1E, 0x06, 0x03, 0x0D, 0xC9, 0xCB, 0xCD, 0xD0, 0x11, 0x06, 0x03, 0x14, 0x01, 0x88, 0x87, 0x7C, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0xAE, 0x4D, 0x4D }, // 11
            { 0x00, 0x01, 0x07, 0x06, 0x11, 0x03, 0x11, 0x0E, 0x21, 0x23, 0xCE, 0xD1, 0xD2, 0x03, 0x13, 0x00, 0x89, 0x7A, 0x90, 0x87, 0x7C, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0xAF, 0x4D, 0x4D, 0x4D }, // 12
            { 0x01, 0x00, 0x08, 0x03, 0x06, 0x0C, 0x0D, 0x21, 0x0B, 0x21, 0x23, 0x1B, 0xD3, 0x11, 0x17, 0x01, 0x88, 0x90, 0x7A, 0x91, 0x87, 0x7C, 0x85, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0xAF, 0x4D, 0x4D, 0x4D, 0x4D }, // 13
            { 0x00, 0x01, 0x04, 0x0C, 0x0D, 0x21, 0x0B, 0x23, 0x0A, 0x0B, 0x1E, 0x11, 0xEE, 0x06, 0x14, 0x89, 0x92, 0x91, 0x91, 0x92, 0x90, 0x7A, 0x87, 0x7C, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0xA9, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 14
            { 0x01, 0x00, 0x05, 0x0D, 0x21, 0x23, 0x0A, 0x21, 0x23, 0x0A, 0x1C, 0x06, 0xD4, 0xD9, 0xED, 0x90, 0x7A, 0x92, 0x92, 0x78, 0x7B, 0x78, 0xA6, 0x8B, 0x01, 0x85, 0x01, 0x00, 0x01, 0x00, 0xAA, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 15
            { 0x00, 0x01, 0x07, 0x0E, 0x0B, 0x21, 0x23, 0x0A, 0x21, 0x23, 0x1B, 0x03, 0xD5, 0xD8, 0xDA, 0x78, 0x90, 0x78, 0x78, 0x7A, 0x8D, 0x8C, 0x8B, 0x01, 0x89, 0x79, 0x7C, 0x85, 0x00, 0x01, 0xAC, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 16
            { 0x01, 0x00, 0x05, 0x22, 0x0A, 0x0B, 0x21, 0x23, 0x0B, 0x1F, 0x03, 0x11, 0xD6, 0x78, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF, 0x8E, 0x01, 0x00, 0x01, 0x89, 0x90, 0x8F, 0x79, 0x8F, 0x7C, 0x00, 0xA9, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 17
            { 0x00, 0x01, 0x02, 0x11, 0x09, 0x0A, 0x23, 0x21, 0x0A, 0x1A, 0x11, 0xD7, 0x78, 0x7B, 0x78, 0xE6, 0xE8, 0xE0, 0x6C, 0x75, 0x76, 0xF5, 0x89, 0x90, 0x7A, 0x90, 0x7A, 0x90, 0x87, 0x7C, 0xAE, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 18
            { 0x01, 0x04, 0x11, 0x0C, 0x0B, 0x23, 0x21, 0x23, 0x21, 0x1B, 0x12, 0x93, 0x79, 0x78, 0x79, 0xE5, 0xE7, 0xE1, 0x71, 0x67, 0x68, 0x69, 0x58, 0x57, 0x90, 0x7A, 0x90, 0x7A, 0x91, 0x87, 0x7D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 19
            { 0x00, 0x05, 0x06, 0x20, 0x0A, 0x21, 0x0B, 0x21, 0x1E, 0x15, 0x00, 0x01, 0x94, 0x79, 0x78, 0xE4, 0xE3, 0xE2, 0x65, 0x66, 0x6C, 0x6A, 0x69, 0x6C, 0x58, 0x57, 0x91, 0x91, 0x92, 0x91, 0x7E, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 20
            { 0x01, 0x07, 0x03, 0x03, 0x20, 0x23, 0x0A, 0x1E, 0x11, 0x03, 0x16, 0x00, 0x01, 0x94, 0x90, 0x7C, 0x00, 0x6E, 0x6F, 0x70, 0x6F, 0x72, 0x6A, 0x65, 0x69, 0x6C, 0x58, 0x92, 0x78, 0x7B, 0x7F, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 21
            { 0x00, 0x08, 0x11, 0x11, 0x03, 0x20, 0x1E, 0x03, 0x06, 0x11, 0x14, 0x01, 0x00, 0x01, 0x94, 0x87, 0x7C, 0x00, 0x01, 0x00, 0x01, 0x00, 0x73, 0x6C, 0x6A, 0x69, 0x6C, 0x59, 0x7A, 0x90, 0x80, 0x81, 0x4D, 0x4D, 0x4D, 0x4D }, // 22
            { 0x01, 0x00, 0x0F, 0x06, 0x11, 0x03, 0x06, 0x11, 0x10, 0x12, 0x01, 0x00, 0x01, 0x00, 0x85, 0x88, 0x87, 0x7C, 0x00, 0x01, 0x00, 0x01, 0x00, 0x73, 0x6C, 0x6B, 0x6C, 0x58, 0x79, 0x92, 0x82, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 23
            { 0x00, 0x01, 0x00, 0x0F, 0x06, 0x11, 0x03, 0x15, 0x00, 0x01, 0x00, 0x01, 0x00, 0x89, 0x79, 0x78, 0x79, 0x87, 0x7C, 0x89, 0x87, 0x7C, 0x01, 0x00, 0x74, 0x6A, 0x65, 0xF3, 0x56, 0x84, 0x83, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 24
            { 0x01, 0x00, 0x01, 0x00, 0x0F, 0x10, 0x10, 0x12, 0x01, 0x00, 0x01, 0x00, 0x89, 0x90, 0x8F, 0x79, 0x8F, 0x91, 0x90, 0x90, 0x8F, 0x79, 0xA5, 0x7C, 0x00, 0x74, 0x6C, 0xFC, 0xF4, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 25
            { 0x00, 0x24, 0x25, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x94, 0x7A, 0x90, 0x7A, 0x90, 0x92, 0x91, 0x7A, 0x90, 0x7A, 0x90, 0x92, 0xA5, 0x7C, 0x74, 0x6D, 0xAD, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 26
            { 0x27, 0x2C, 0x19, 0x26, 0x01, 0x28, 0x01, 0x28, 0x01, 0x00, 0x01, 0x00, 0x01, 0x94, 0x7A, 0x90, 0x7A, 0x91, 0x92, 0x90, 0x7A, 0x90, 0x7A, 0x91, 0x92, 0x87, 0x7C, 0x00, 0xAC, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 27
            { 0x9C, 0x2D, 0x61, 0x32, 0x27, 0x9B, 0x27, 0x9A, 0x26, 0x28, 0x00, 0x01, 0x00, 0x01, 0x94, 0x91, 0x91, 0x92, 0x90, 0x7A, 0x92, 0x91, 0x91, 0x92, 0x90, 0x7A, 0x87, 0x7C, 0x00, 0xAB, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 28
            { 0x9D, 0x9B, 0x2E, 0x64, 0x37, 0x36, 0x32, 0x32, 0x9B, 0x36, 0x29, 0x00, 0x01, 0x00, 0x01, 0x95, 0x92, 0x78, 0x7B, 0x90, 0x7A, 0x92, 0x92, 0x78, 0x7B, 0x78, 0xA6, 0x8B, 0x01, 0xA9, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 29
            { 0x9A, 0x2C, 0x61, 0x63, 0x64, 0x61, 0x63, 0x61, 0x34, 0x61, 0x63, 0x29, 0x00, 0x01, 0x00, 0x01, 0x94, 0x7A, 0x90, 0x78, 0x90, 0x78, 0x78, 0x7A, 0x90, 0x7A, 0x8B, 0x01, 0x00, 0xA8, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 30
            { 0x9B, 0x2F, 0x61, 0x5D, 0x33, 0x62, 0x5D, 0x5D, 0x64, 0x62, 0x5D, 0x35, 0x2A, 0x00, 0x01, 0x28, 0x01, 0x94, 0x8D, 0x8C, 0x92, 0x79, 0x96, 0x96, 0x96, 0x92, 0xA5, 0x7C, 0x01, 0xA8, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 31
            { 0x34, 0x61, 0x35, 0x9A, 0x9C, 0x31, 0x9A, 0x9C, 0x31, 0x31, 0x9A, 0x2B, 0x00, 0x01, 0x3F, 0xA4, 0x26, 0x01, 0x00, 0x01, 0x94, 0x8B, 0x99, 0x9B, 0x9B, 0x97, 0x91, 0x92, 0xA5, 0xA7, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 32
            { 0x64, 0x62, 0x5E, 0x9B, 0x9D, 0x9C, 0x9B, 0x9D, 0x43, 0x48, 0x36, 0x61, 0x44, 0x38, 0x9A, 0x2E, 0xB9, 0xB8, 0x01, 0x00, 0x01, 0x00, 0x99, 0x9C, 0xC1, 0xC4, 0xBF, 0xBE, 0xBD, 0xBB, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 33
            { 0x30, 0x63, 0x5E, 0x9C, 0x9A, 0x36, 0x47, 0x42, 0x41, 0x40, 0x61, 0x45, 0x9A, 0x9C, 0x2C, 0x61, 0x63, 0xB7, 0xBA, 0x28, 0x98, 0x9E, 0x9A, 0x9D, 0xC2, 0xA3, 0x01, 0xA2, 0xBC, 0xE9, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 34
            { 0x9A, 0x31, 0x9A, 0x43, 0x42, 0x62, 0x64, 0x41, 0x92, 0x87, 0x40, 0x46, 0x9B, 0x9D, 0x2F, 0x49, 0x4A, 0x2D, 0xB6, 0xB5, 0xC7, 0xC6, 0xC5, 0xC4, 0xC3, 0x00, 0x89, 0xA0, 0xEA, 0xEB, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 35
            { 0x32, 0x9B, 0x36, 0x64, 0x61, 0x63, 0x39, 0x7A, 0x91, 0x92, 0x3E, 0x61, 0x47, 0x42, 0x35, 0x9A, 0x9C, 0x2C, 0x47, 0x5C, 0xB4, 0x9B, 0x43, 0x5C, 0xA3, 0x00, 0x88, 0xA1, 0xEC, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 36
            { 0x61, 0x34, 0x5D, 0x33, 0x62, 0x64, 0x61, 0x3C, 0x92, 0xA6, 0x3D, 0x62, 0x61, 0x35, 0x9D, 0x9B, 0x9D, 0x4B, 0x62, 0x64, 0x47, 0x42, 0x61, 0xF0, 0x00, 0x01, 0x88, 0x91, 0x82, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 37
            { 0x5D, 0x35, 0x9C, 0x36, 0x61, 0x63, 0x39, 0x92, 0xA6, 0x3D, 0x61, 0x63, 0x62, 0x32, 0x43, 0x5C, 0x9A, 0x9B, 0x4C, 0x5F, 0x5F, 0x30, 0xF1, 0x00, 0x01, 0x89, 0x91, 0x9F, 0x83, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 38
            { 0x9C, 0x43, 0x42, 0x33, 0x62, 0x64, 0x61, 0x3A, 0x3B, 0x64, 0x62, 0x64, 0x63, 0x61, 0x63, 0x64, 0x47, 0x5C, 0x9C, 0x9A, 0x9C, 0x9A, 0x4C, 0x29, 0x00, 0x88, 0x92, 0x82, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 39
            { 0x34, 0x49, 0x4A, 0x9B, 0x4B, 0x61, 0x62, 0x64, 0x63, 0x61, 0x63, 0x61, 0x33, 0x35, 0x4B, 0x62, 0x49, 0x4A, 0x9D, 0x9B, 0x9D, 0x43, 0x60, 0x5C, 0xF2, 0xB3, 0x86, 0x83, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 40
            { 0x35, 0x9C, 0x9A, 0x9C, 0x9A, 0x31, 0x4C, 0x4A, 0x4C, 0x62, 0x64, 0x35, 0x9A, 0x9C, 0x9B, 0x31, 0x9A, 0x43, 0x60, 0x60, 0x42, 0x49, 0x5F, 0x30, 0x61, 0x63, 0xB1, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 41
            { 0x9D, 0x37, 0x9B, 0x9D, 0x9B, 0x9C, 0x9D, 0x43, 0x42, 0x63, 0x35, 0x9D, 0x9B, 0x9D, 0x43, 0x60, 0x42, 0x61, 0x63, 0x61, 0x35, 0x9C, 0x9B, 0x9A, 0x4B, 0x64, 0xB2, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 42
            { 0x36, 0x62, 0x32, 0x2C, 0x47, 0x42, 0x34, 0x62, 0x64, 0x35, 0x9B, 0x9A, 0x9C, 0x2C, 0x61, 0x62, 0x63, 0x62, 0x64, 0x35, 0x9A, 0x9D, 0x43, 0x42, 0x34, 0xB1, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D, 0x4D }, // 43
        };

        internal static byte[] wilderness_impassable =
        {
            0x4D, 0x7D, 0x7E, 0x7F, 0x81, 0x82, 0x83, 0xA7, 0xA8, 0xA9, 0xAA, 0xAB, 0xAE, 0xAF, 0xB0, 0xB1, 0xBB, 0xDC, 0xDD, 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE8, 0xF4
        };

        internal static bool TerrainImpassable()
        {
            byte x = (byte)gbl.word_1D914;
            byte y = (byte)gbl.word_1D916;

            if (gbl.EclBlockId == 25)
            {
                x += 0;
            }
            else if (gbl.EclBlockId == 26)
            {
                x += 13;
            }
            else if (gbl.EclBlockId == 27)
            {
                x += 26;
            }

            byte terrain = wilderness[x, y];

            if (wilderness_impassable.Contains(terrain))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void DrawWildernessMap()
        {
            byte x = gbl.area_ptr.field_186;
            byte y = gbl.area_ptr.field_188;
            DaxBlock sqrpaci = new DaxBlock(0, 256, 3, 24);
            Display.UpdateStop();
            seg037.DrawFrame_Wilderness();
            ovr025.PartySummary(gbl.SelectedPlayer);
            ovr025.display_map_position_time();

            DaxBlock tmp_block = seg040.LoadDax(0, 0, 1, "SQRPACI");
            System.Array.Copy(tmp_block.data, 0, sqrpaci.data, 0, tmp_block.item_count * tmp_block.bpp);
            int dataLength = tmp_block.item_count * tmp_block.bpp;

            tmp_block = seg040.LoadDax(0, 0, 1, "BACPAC");
            System.Array.Copy(tmp_block.data, 0, sqrpaci.data, 0, tmp_block.item_count * tmp_block.bpp);

            tmp_block = seg040.LoadDax(0, 0, 2, "SQRPACI");
            System.Array.Copy(tmp_block.data, 0, sqrpaci.data, dataLength, tmp_block.item_count * tmp_block.bpp);

            for (int i = 0; i < 3; i++)
            {
                ovr034.chead_cbody_comspr_icon((byte)(26 + i), i, "ICON");
            }

            if (gbl.EclBlockId == 25)
            {
                x += 0;
            }
            else if (gbl.EclBlockId == 26)
            {
                x += 13;
            }
            else if (gbl.EclBlockId == 27)
            {
                x += 26;
            }

            if (gbl.area_ptr.field_340 > 0)
            {
                wilderness[32, 15] = 0xFB;
            }
            else
            {
                wilderness[32, 15] = 0xA4;
            }

            if (gbl.area_ptr.field_366 >= 254)
            {
                wilderness[25, 27] = 0xFF;
            }
            else
            {
                wilderness[25, 27] = 0xFC;
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    seg040.OverlayUnbounded(sqrpaci, 0, wilderness[x - 2 + i, y - 2 + j], j * 3, i * 3);
                }
            }

            ovr034.draw_combat_icon(26 + (gbl.worldIcon >> 1), (Classes.Combat.Icon)(gbl.worldIcon & 0x1), gbl.mapDirection, 2, 2);
            gbl.worldIcon = (byte)((gbl.worldIcon + 1) % 6);
            Display.UpdateStart();
        }
    }
}