using Classes;
using System;

namespace engine
{
    class ovr002
    {
        static void delay_or_key(int seconds)
        {
            seg043.clear_keyboard();

            var timeEnd = DateTime.Now.AddSeconds(seconds);

            while (gbl.Exit == false &&
                seg049.KEYPRESSED() == false &&
                DateTime.Now < timeEnd)
            {
                seg049.SysDelay(100);
            }

            seg043.clear_keyboard();
        }


        static void credits()
        {
            gbl.game.DrawCredits();
        }


        internal static void title_screen()
        {
            DaxBlock dax_ptr;

            dax_ptr = seg040.LoadDax(0, 0, 1, "Title");
            seg040.draw_picture(dax_ptr, 0, 0, 0);

            delay_or_key(5);

            dax_ptr = seg040.LoadDax(0, 0, 2, "Title");
            seg040.draw_picture(dax_ptr, 0, 0, 0);

            dax_ptr = seg040.LoadDax(0, 0, 3, "Title");
            seg040.draw_picture(dax_ptr, 0x0b, 6, 0);
            delay_or_key(10);

            dax_ptr = seg040.LoadDax(0, 0, 4, "Title");

            seg044.PlaySound(Sound.sound_d);

            seg040.draw_picture(dax_ptr, 0x0b, 0, 0);
            delay_or_key(10);

            seg041.ClearScreen();
            credits();
            delay_or_key(10);

            seg041.ClearScreen();
        }
    }
}
