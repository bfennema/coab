namespace Classes.PoolRad
{
    internal class Draw
    {
        readonly static byte[] outer_frame_horizontal /*unk_16EB0*/ =
            { 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 };

        readonly static byte[] outer_frame_horizontal_portrait =
            { 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 };

        readonly static byte[] inner_frame_horizontal /*unk_16ED6*/ =
            { 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 };

        readonly static byte[] frame_middle_vertical /*unk_16F0A*/ =
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 };

        readonly static byte[] frame_horizontal /*x8x8_07*/ =
            { 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 };

        readonly static byte[] outer_frame_vertical  /*unk_16EF2*/ =
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 };

        readonly static byte[] outer_frame_vertical_portrait =
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 };

        readonly static byte[] outer_frame_vertical_temple =
            { 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0 };

        readonly static int[] outer_frame_vertical_credits  =
            { 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 };

        readonly static byte[] inner_frame_vertical /*unk_16F31*/ =
            { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 };
        readonly static byte[] combat_frame_vertical /*unk_16F4D*/ =
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 };

        internal static void Frame_Outer() /* draw8x8_01 */
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(23, 39, 0, 0);

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 0, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 23, col_x);
            }

            for (int row_y = 0; row_y <= 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical[row_y] + 0x114, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical[row_y] + 0x114, row_y, 39);
            }

            Display.UpdateStart();
        }

        internal static void Credits() /* draw8x8_02 */
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(24, 39, 0, 0);

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 0, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 4, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 24, col_x);
            }

            for (int row_y = 0; row_y <= 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_credits[row_y] + 0x114, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_credits[row_y] + 0x114, row_y, 39);
            }

            gbl.displayString("scenario created by:", 0, 10, 1, 5);
            gbl.displayString("tsr, inc.", 0, 14, 1, 26);
            gbl.displayString("jim ward", 0, 10, 2, 15);
            gbl.displayString("david cook,steve winter,mike breault", 0, 10, 3, 2);
            gbl.displayString("game created by:", 0, 10, 5, 1);
            gbl.displayString("ssi special projects", 0, 14, 5, 18);
            gbl.displayString("version 1.3", 0, 11, 6, 14);
            gbl.displayString("programming:", 0, 14, 7, 2);
            gbl.displayString("original", 0, 14, 7, 20);
            gbl.displayString("scot bayless", 0, 11, 8, 2);
            gbl.displayString("programming:", 0, 14, 8, 20);
            gbl.displayString("brad byers", 0, 11, 9, 2);
            gbl.displayString("keith brors", 0, 11, 9, 20);
            gbl.displayString("russ brown", 0, 11, 10, 2);
            gbl.displayString("brad myers", 0, 11, 10, 20);
            gbl.displayString("ted greer", 0, 11, 11, 2);
            gbl.displayString("graphic arts:", 0, 14, 13, 2);
            gbl.displayString("encounter coding:", 0, 14, 13, 20);
            gbl.displayString("tom wahl", 0, 11, 14, 2);
            gbl.displayString("paul murray", 0, 11, 14, 20);
            gbl.displayString("fred butts", 0, 11, 15, 2);
            gbl.displayString("russ brown", 0, 11, 15, 20);
            gbl.displayString("darla marasco", 0, 11, 16, 2);
            gbl.displayString("victor penman", 0, 11, 16, 20);
            gbl.displayString("susan halbleib", 0, 11, 17, 2);
            gbl.displayString("dave shelley", 0, 11, 17, 20);
            gbl.displayString("project manager:", 0, 14, 19, 2);
            gbl.displayString("developer:", 0, 14, 19, 20);
            gbl.displayString("victor penman", 0, 11, 20, 2);
            gbl.displayString("george mac donald", 0, 11, 20, 20);
            gbl.displayString("testing:", 0, 14, 22, 2);
            gbl.displayString("joel billings,steve salyer", 0, 11, 22, 11);
            gbl.displayString("james kucera,robert daly,rick white", 0, 11, 23, 2);

            Display.UpdateStart();
        }
        internal static void Frame_Dungeon() // draw8x8_03
        {
            Display.UpdateStop();

            Frame_Outer();

            for (int colX = 0; colX <= 39; colX++)
            {
                gbl.Put8x8Symbol(0, false, frame_horizontal[colX] + 0x114, 16, colX);
            }

            for (int rowY = 0; rowY <= 16; rowY++)
            {
                gbl.Put8x8Symbol(0, false, frame_middle_vertical[rowY] + 0x114, rowY, 16);
            }

            for (int col_x = 2; col_x <= 14; col_x++)
            {
                gbl.Put8x8Symbol(0, false, inner_frame_horizontal[col_x] + 0x114, 2, col_x);
                gbl.Put8x8Symbol(0, false, inner_frame_horizontal[col_x] + 0x114, 14, col_x);
            }

            for (int row_y = 2; row_y <= 14; row_y++)
            {
                gbl.Put8x8Symbol(0, false, inner_frame_vertical[row_y] + 0x114, row_y, 2);
                gbl.Put8x8Symbol(0, false, inner_frame_vertical[row_y] + 0x114, row_y, 14);
            }

            Display.UpdateStart();
        }
        internal static void Frame_Wilderness() // draw8x8_04
        {
            Display.UpdateStop();

            Frame_Outer();

            for (int colX = 0; colX <= 39; colX++)
            {
                gbl.Put8x8Symbol(0, false, frame_horizontal[colX] + 0x114, 16, colX);
            }

            for (int rowY = 0; rowY <= 16; rowY++)
            {
                gbl.Put8x8Symbol(0, false, frame_middle_vertical[rowY] + 0x114, rowY, 16);
            }

            Display.UpdateStart();
        }
        internal static void Frame_Memorize() // draw8x8_05
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(16, 38, 0, 0);

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 0, col_x);
            }

            for (int row_y = 0; row_y <= 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical[row_y] + 0x114, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical[row_y] + 0x114, row_y, 39);
            }

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 2, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 16, col_x);
            }

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 23, col_x);
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
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 0, col_x);
            }

            // Three Vert Bars
            for (int row_y = 0; row_y <= 0x16; row_y++)
            {
                gbl.Put8x8Symbol(0, false, combat_frame_vertical[row_y] + 0x114, row_y, 0);
                gbl.Put8x8Symbol(0, false, combat_frame_vertical[row_y] + 0x114, row_y, 22);
                gbl.Put8x8Symbol(0, false, combat_frame_vertical[row_y] + 0x114, row_y, 39);
            }

            // Bottom Bar
            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 22, col_x);
            }

            Display.UpdateStart();
        }
        internal static void Frame_List() // draw8x8_07
        {
            Display.UpdateStop();

            Frame_Outer();

            for (int col_x = 0; col_x <= 0x27; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 2, col_x);
            }

            Display.UpdateStart();
        }
        internal static void Frame_Portrait()
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(23, 39, 0, 0);

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal_portrait[col_x] + 0x114, 0, col_x);
            }

            for (int col_x = 27; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal_portrait[col_x] + 0x114, 12, col_x);
            }

            for (int row_y = 0; row_y <= 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical[row_y] + 0x114, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_portrait[row_y] + 0x114, row_y, 39);
            }

            for (int row_y = 0; row_y <= 11; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_portrait[row_y] + 0x114, row_y, 27);
            }

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 23, col_x);
            }

            Display.UpdateStart();
        }

        internal static void Frame_Temple()
        {
            Display.UpdateStop();

            gbl.draw8x8_clear_area(23, 39, 0, 0);

            for (int col_x = 0; col_x <= 39; col_x++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 0, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 2, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 16, col_x);
                gbl.Put8x8Symbol(0, false, outer_frame_horizontal[col_x] + 0x114, 23, col_x);
            }

            for (int row_y = 0; row_y <= 23; row_y++)
            {
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_temple[row_y] + 0x114, row_y, 0);
                gbl.Put8x8Symbol(0, false, outer_frame_vertical_temple[row_y] + 0x114, row_y, 39);
            }

            Display.UpdateStart();
        }
    }
}
