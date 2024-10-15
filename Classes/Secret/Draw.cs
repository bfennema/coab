namespace Classes.Secret
{
    internal class Draw
    {
        readonly static byte[] outer_frame_horizontal /*unk_16EB0*/ =
            { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        readonly static byte[] inner_frame_horizontal /*unk_16ED6*/ =
            { 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };

        readonly static byte[] frame_middle_vertical1 /*unk_16F0A*/ =
            { 3, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3 };

        readonly static byte[] outer_frame_top /*byte_16E60*/ =
            { 0, 0, 0, 0, 0, 0, 0, 1, 12, 2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 8, 2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 16, 2, 0, 0, 0, 0, 0, 0, 0, 0 };

        readonly static byte[] outer_frame_vertical  /*unk_16EF2*/ =
            { 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 0 };
        readonly static byte[] outer_frame_vertical_list =
            { 0, 3, 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 0 };
        readonly static byte[] outer_frame_vertical_dungeon =
            { 0, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 0, 3, 4, 4, 4, 4, 5, 0 };
        readonly static int[] outer_frame_vertical1 =
            { 3, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3 };
        readonly static int[] outer_frame_vertical2 =
            { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3 };
        readonly static int[] outer_frame_vertical3 =
            { 3, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 0, 1, 1, 1, 1, 2, 3 };
        readonly static int[] outer_frame_vertical4 =
            { 3, 0, 1, 1, 1, 1, 1, 2, 3, 0, 1, 1, 1, 1, 1, 2, 3, 0, 1, 2, 3, 0, 2, 3 };
        readonly static int[] outer_frame_vertical5 =
            { 3, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3 };

        readonly static byte[] inner_frame_vertical /*unk_16F31*/ =
            { 7, 7, 7, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 7 };
        readonly static byte[] combat_frame_vertical =
            { 3, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3 };
        readonly static byte[] combat_frame_left /*unk_16F4D*/ =
            { 0, 2, 9, 5, 2, 2, 2, 2, 2, 2, 5, 7, 2, 2, 2, 2, 2, 9, 7, 2, 2, 2, 1 };
        readonly static byte[] combat_frame_middle /*unk_16F64*/ =
            { 0, 7, 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7, 5, 2, 2, 2, 2, 2, 2, 5, 2, 4 };
        readonly static byte[] combat_frame_right /*unk_16F7B*/ =
            { 2, 2, 9, 7, 2, 2, 2, 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7, 2, 2, 2, 2, 2 };

        internal static void Frame_Outer() /* draw8x8_01 */
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(23, 39, 0, 0);

            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 0, col_x);
            }

            for (int row_y = 0; row_y < 0x17; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical1[row_y] + 0x134, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical1[row_y] + 0x134, row_y, 39);
            }

            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 23, col_x);
            }

            Display.UpdateStart();
        }

        internal static void Credits() /* draw8x8_02 */
        {
            Display.UpdateStop();

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 1, col_x);
            }

            for (int row_y = 2; row_y < 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical2[row_y] + 0x134, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical2[row_y] + 0x134, row_y, 39);
            }

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 23, col_x);
            }

            gbl.displayString("based on the tsr novel 'azure bonds'", 0, 10, 1, 2);
            gbl.displayString("by:", 0, 10, 2, 6);
            gbl.displayString("kate novak", 0, 11, 2, 9);
            gbl.displayString("and", 0, 10, 2, 0x14);
            gbl.displayString("jeff grubb", 0, 11, 2, 0x18);
            gbl.displayString("scenario created by:", 0, 10, 4, 0x0a);
            gbl.displayString("tsr, inc.", 0, 0x0e, 5, 0x0b);
            gbl.displayString("and", 0, 0x0a, 5, 0x15);
            gbl.displayString("ssi", 0, 0x0e, 5, 0x19);
            gbl.displayString("jeff grubb", 0, 0x0b, 6, 0x0e);
            gbl.displayString("george mac donald", 0x0, 0x0B, 0x7, 0x0B);
            gbl.displayString("game created by:", 0x0, 0x0A, 0x9, 0x1);
            gbl.displayString("ssi special projects", 0x0, 0x0E, 0x9, 0x12);
            gbl.displayString("project leader:", 0x0, 0x0E, 0x0B, 0x2);
            gbl.displayString("george mac donald", 0x0, 0x0B, 0x0C, 0x2);
            gbl.displayString("programming:", 0x0, 0x0E, 0x0E, 0x2);
            gbl.displayString("scot bayless", 0x0, 0x0B, 0x0F, 0x2);
            gbl.displayString("russ brown", 0x0, 0x0B, 0x10, 0x2);
            gbl.displayString("michael mancuso", 0x0, 0x0B, 0x11, 0x2);
            gbl.displayString("development:", 0x0, 0x0E, 0x13, 0x2);
            gbl.displayString("david shelley", 0x0, 0x0B, 0x14, 0x2);
            gbl.displayString("michael mancuso", 0x0, 0x0B, 0x15, 0x2);
            gbl.displayString("oran kangas", 0x0, 0x0B, 0x16, 0x2);
            gbl.displayString("graphic arts:", 0x0, 0x0E, 0x0B, 0x16);
            gbl.displayString("tom wahl", 0x0, 0x0B, 0x0C, 0x16);
            gbl.displayString("fred butts", 0x0, 0x0B, 0x0D, 0x16);
            gbl.displayString("susan manley", 0x0, 0x0B, 0x0E, 0x16);
            gbl.displayString("mark johnson", 0x0, 0x0B, 0x0F, 0x16);
            gbl.displayString("cyrus lum", 0x0, 0x0B, 0x10, 0x16);
            gbl.displayString("playtesting:", 0x0, 0x0E, 0x12, 0x16);
            gbl.displayString("jim jennings", 0x0, 0x0B, 0x13, 0x16);
            gbl.displayString("james kucera", 0x0, 0x0B, 0x14, 0x16);
            gbl.displayString("rick white", 0x0, 0x0B, 0x15, 0x16);
            gbl.displayString("robert daly", 0x0, 0x0B, 0x16, 0x16);

            Display.UpdateStart();
        }
        internal static void Frame_Dungeon() // draw8x8_03
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(23, 39, 0, 0);

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 0, col_x);
            }

            for (int row_y = 1; row_y < 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical3[row_y] + 0x134, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical3[row_y] + 0x134, row_y, 39);
            }

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 23, col_x);
            }

            for (int colX = 0; colX <= 39; colX++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[colX] + 0x134, 16, colX);
            }

            for (int rowY = 0; rowY <= 16; rowY++)
            {
                gbl.Put8x8Symbol(0, false, frame_middle_vertical1[rowY] + 0x134, rowY, 16);
            }

            for (int col_x = 2; col_x <= 14; col_x++)
            {
                gbl.Put8x8Symbol(0, false, inner_frame_horizontal[col_x] + 0x134, 2, col_x);
                gbl.Put8x8Symbol(0, false, inner_frame_horizontal[col_x] + 0x134, 14, col_x);
            }

            for (int row_y = 2; row_y <= 14; row_y++)
            {
                gbl.Put8x8Symbol(0, false, inner_frame_vertical[row_y] + 0x134, row_y, 2);
                gbl.Put8x8Symbol(0, false, inner_frame_vertical[row_y] + 0x134, row_y, 14);
            }

            Display.UpdateStart();
        }
        internal static void Frame_Wilderness() // draw8x8_04
        {
            Display.UpdateStop();

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 0, col_x);
            }

            for (int row_y = 2; row_y < 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical3[row_y] + 0x134, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical3[row_y] + 0x134, row_y, 39);
            }

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 23, col_x);
            }

            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 0x10, col_x);
            }

            Display.UpdateStart();
        }
        internal static void Frame_Memorize() // draw8x8_05
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(0x10, 0x26, 1, 1);

            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 0, col_x);
            }

            for (int row_y = 0; row_y <= 0x17; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical3[row_y] + 0x134, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical3[row_y] + 0x134, row_y, 0x27);
            }

            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 23, col_x);
            }

            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 16, col_x);
            }

            Display.UpdateStart();
        }
        internal static void Frame_Combat() // draw8x8_06
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(0x17, 0x27, 0, 0);

            // Top Bar
            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 0, col_x);
            }

            // Three Vert Bars
            for (int row_y = 0; row_y <= 0x16; row_y++)
            {
                gbl.Put8x8Symbol(0, false, combat_frame_vertical[row_y] + 0x134, row_y, 0);
                gbl.Put8x8Symbol(0, false, combat_frame_vertical[row_y] + 0x134, row_y, 22);
                gbl.Put8x8Symbol(0, false, combat_frame_vertical[row_y] + 0x134, row_y, 39);
            }

            // Bottom Bar
            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x134, 22, col_x);
            }

            Display.UpdateStart();
        }
        internal static void Frame_List() // draw8x8_07
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(23, 39, 0, 0);

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_top[col_x] + 0x114, 0, col_x);
            }

            for (int row_y = 0; row_y <= 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_list[row_y] + 0x114, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_list[row_y] + 0x114, row_y, 0x27);
            }

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 2, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 0x17, col_x);
            }

            Display.UpdateStart();
        }
    }
}
