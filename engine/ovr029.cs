using Avalonia.Animation;
using Classes;
using System;
using System.Threading.Tasks;

namespace engine
{
    class ovr029
    {
        //static int[] sky_colours = [0x00, 0x0F, 0x04, 0x0B, 0x0D, 0x02, 0x09, 0x0E, 0x00, 0x0F, 0x04, 0x0B, 0x0D, 0x02, 0x09, 0x0E];
        //static int[] sky_colours = new int[]{ /* seg600:0A8A unk_16D9A*/
        //0x00, 0x0F, 0x04, 0x0B, 0x0D, 0x02, 0x09, 0x0E, 0x00, 0x0F, 0x04, 0x0B, 0x0D, 0x02 , 0x09, 0x0E};

        /*  0C7C:0C26		  db	0
            0C7C:0C27 db  0Fh
            0C7C:0C28 db  0Ch
            0C7C:0C29 db  0Bh
            0C7C:0C2A db  0Dh
            0C7C:0C2B db  0Ah
            0C7C:0C2C db	9
            0C7C:0C2D db  0Eh
            0C7C:0C2E db	0
            0C7C:0C2F db  0Fh */
        /* 0x00, 0x0F, 0x0C, 0x0B, 0x0D, 0x0A, 0x09, 0x0E, 0x00, 0x0F */
        static int[] sky_colours = [0x00, 0x0F, 0x0C, 0x0B, 0x0D, 0x0A, 0x09, 0x0E, 0x00, 0x0F, 0x0C, 0x0B, 0x0D, 0x0A, 0x09, 0x0E];

        /**
          * Maps background graphic attributes to valid CGA color indices.
          * * @param asset_attr The graphics asset or terrain block parameter.
          * @return A 16-bit register value containing the mapped color index (0x00 or 0x0F).
          */
        internal static byte sub_6F014(byte asset_attr)
        {
            byte color_index;

            // Evaluate input asset markers against engine color constants
            if (asset_attr == 0x08)
            {
                color_index = 0x00; // Map to Black
            }
            else if (asset_attr == 0x41)
            { // 'A'
                color_index = 0x00; // Map to Black
            }
            else if (asset_attr == 0xE8)
            { // 'è'
                color_index = 0x0F; // Map to Intense White
            }
            else
            {
                // Fallback catch-all default color index
                color_index = 0x0F; // Map to Intense White
            }

            // Returns lower 8 bits in AL register, upper AH register cleared/undefined
            return color_index;
        }

        /**
          * Evaluates asset tags to determine the secondary color register mapping for CGA layouts.
          * * @param asset_attr The graphics asset or terrain block parameter passed from sub_6F0BA.
          * @return A 16-bit register value containing the mapped color index (0x07 or 0x08).
          */
        internal static byte sub_6F048(byte asset_attr)
        {
            byte color_index;

            // Map input attributes to mid-tone hardware palettes
            if (asset_attr == 0x08)
            {
                color_index = 0x08; // Map to Dark Gray / High-intensity Black
            }
            else if (asset_attr == 0x41)
            { // 'A'
                color_index = 0x07; // Map to Light Gray
            }
            else if (asset_attr == 0xE8)
            { // 'è'
                color_index = 0x07; // Map to Light Gray
            }
            else
            {
                // Fallback catch-all default color index
                color_index = 0x07; // Map to Light Gray
            }

            // Lower 8 bits returned in AL; upper bits are uninitialized/cleared by the register state
            return color_index;
        }

        /**
          * Evaluates primary asset attribute layers to determine the third CGA palette slot mapping.
          * * @param asset_attr The primary graphic layer parameter passed from byte_1D912.
          * @return A 16-bit register value containing the mapped CGA color index (0x01, 0x06, or 0x08).
          */
        internal static byte sub_6F07C(byte asset_attr)
        {
            byte color_index;

            // Evaluate structural tile data codes 
            switch (asset_attr)
            {
                case 0x0B:
                    color_index = 0x08; // Map to Dark Gray
                    break;

                case 0x09:
                case 0xDB:              // 'Û' (Commonly used for solid walls/doors)
                    color_index = 0x06; // Map to Brown
                    break;

                case 0x06:
                    color_index = 0x01; // Map to Blue (Often used for skies/water/magic barriers)
                    break;

                default:
                    // Fallback catch-all default color index
                    color_index = 0x06; // Map to Brown
                    break;
            }

            // Lower 8 bits returned in AL register; upper bits uninitialized/cleared by calling convention
            return color_index;
        }

        internal static byte[] wilderness_offset = { 0, 0x0D, 0x1A };

        internal static async Task<bool> RedrawView() /* sub_6F0BA */
        {
            byte view_mode = gbl.wilderness_area;

            if (gbl.lastDaxBlockId == 0x50)
            {
                gbl.can_draw_bigpic = false;
            }

            if (gbl.party_killed == false)
            {
                if (gbl.area_ptr.inDungeon != 0)
                {
                    view_mode = 1;
                }

                if (view_mode == 1)
                {
                    gbl.mapWallRoof = ovr031.get_wall_x2(gbl.mapPosY, gbl.mapPosX);

                    if (gbl.mapWallRoof > 0x7F)
                    {
                        // indoor  
                        gbl.sky_colour = sky_colours[gbl.area_ptr.indoor_sky_colour];
                    }
                    else
                    {
                        // outdoors
                        gbl.sky_colour = sky_colours[gbl.area_ptr.outdoor_sky_colour];
                    }

                    // Apply environmental palettes if flag is on
                    if (gbl.paletteChanged == true) // CoAB: byte_1EE91 POR: byte_14CA5
                    {
                        /*
                        POR:
                            byte_13221 = sub_44CD7(byte_13729, (int16_t)bp);
                            byte_13222 = sub_44D0B(byte_13729, (int16_t)bp);
                            byte_13223 = sub_44D3F(byte_13728, (int16_t)bp);
                        */
                        gbl.byte_1D535 = sub_6F014(gbl.byte_1D913);
                        gbl.byte_1D536 = sub_6F048(gbl.byte_1D913);
                        gbl.byte_1D537 = sub_6F07C(gbl.byte_1D912);
                    }
                    /*
                    // Commit texturing choices to the renderer
                    sub_3D75(byte_13220, byte_13221, byte_13222, byte_13223);
                    */
                    // Commit texturing choices to the renderer
                    sub_71165((byte)gbl.sky_colour, gbl.byte_1D535, gbl.byte_1D536, gbl.byte_1D537);

                    if (gbl.area_ptr.block_area_view != 0 &&
                        Cheats.always_show_areamap == false)
                    {
                        gbl.mapAreaDisplay = false;
                    }

                    ovr031.Draw3dWorld(gbl.mapDirection, gbl.mapPosY, gbl.mapPosX);
                }
                else if (view_mode >= 2 && view_mode <= 4)
                {
                    short modifier_offset = wilderness_offset[view_mode - 2];
                    short base_x_origin = gbl.area_ptr.field_186;

                    // Calculate centered X camera position and clamp to [0, 39] (0x27)
                    short raw_calc_x = (short)(modifier_offset + base_x_origin - 2);
                    byte clamped_x = (byte)raw_calc_x;
                    if (clamped_x < 0) clamped_x = 0;
                    if (clamped_x > 0x27) clamped_x = 0x27;

                    short base_y_origin = gbl.area_ptr.field_188;

                    // Calculate centered Y camera position and clamp to [0, 31] (0x1F)
                    short raw_calc_y = (short)(base_y_origin - 2);
                    var clamped_y = (short)raw_calc_y;
                    if (clamped_y < 0) clamped_y = 0;
                    if (clamped_y > 0x1F) clamped_y = 0x1F;

                    // Perform tactical block transformation blitting onto video surface
                    Display.UpdateStop();
                    ovr031.SetupWilderness((short)clamped_x, (short)clamped_y);
                    await ovr031.AnimateHorse();
                    Display.UpdateStart();
                    //sub_44C10();


                    /*
                    // --- BRANCH 2: OVERLAND WILDERNESS MAP RENDERING ---
                    int16_t modifier_offset = (int16_t)(*(uint8_t*)(view_mode + 0x3C76));
                    int16_t base_x_origin = *(uint16_t far *)((char far *)area_ptr + 0x186);

                    // Calculate centered X camera position and clamp to [0, 39] (0x27)
                    int16_t raw_calc_x = modifier_offset + base_x_origin - 2;
                    clamped_x = (int8_t)raw_calc_x;
                    if (clamped_x < 0) clamped_x = 0;
                    if (clamped_x > 0x27) clamped_x = 0x27;

                    int16_t base_y_origin = *(uint16_t far *)((char far *)area_ptr + 0x188);

                    // Calculate centered Y camera position and clamp to [0, 31] (0x1F)
                    int16_t raw_calc_y = base_y_origin - 2;
                    clamped_y = (int8_t)raw_calc_y;
                    if (clamped_y < 0) clamped_y = 0;
                    if (clamped_y > 0x1F) clamped_y = 0x1F;

                    // Perform tactical block transformation blitting onto video surface
                    sub_3CF5((int16_t)clamped_x, (int16_t)clamped_y);
                    sub_44C10();
                    */
                    //await ovr031.DrawWildernessMap();
                }
                else if (gbl.can_draw_bigpic == true)
                {
                    ovr030.draw_bigpic();
                }

                gbl.can_draw_bigpic = false;
            }

            return true;
        }
        /**
          * Commits calculated local UI color states directly to global video registers.
          * Matches the 'call sub_71165' block orchestrated inside sub_6F0BA.
          * * @param color1 Pushed first; maps to the primary asset visual index.
          * @param color2 Pushed second; maps to the first CGA calculation cache.
          * @param color3 Pushed third; maps to the second CGA calculation cache.
          * @param color4 Pushed fourth; maps to the final terrain structural color index.
          */
        static internal void sub_71165(byte color1, byte color2, byte color3, byte color4)
        {
            // Commit the stack arguments sequentially to the engine's active hardware palette state
            gbl.sky_colour = color1;
            gbl.byte_1D535 = color2;
            gbl.byte_1D536 = color3;
            gbl.byte_1D537 = color4;
        }
    }
}
