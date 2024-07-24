using Classes;
using Logging;
using System;

namespace engine
{
    class ovr002
    {
        static void delay_or_key(int seconds)
        {
            seg043.clear_keyboard();

            var timeEnd = DateTime.Now.AddSeconds(seconds);

            while (seg049.KEYPRESSED() == false &&
                DateTime.Now < timeEnd)
            {
                seg049.SysDelay(100);
            }

            seg043.clear_keyboard();
        }


        static void credits()
        {
            Display.UpdateStop();

            seg037.DrawCredits();

            if (gbl.game == Game.PoolOfRadiance)
            {
                seg041.displayString("scenario created by:", 0, 10, 1, 5);
                seg041.displayString("tsr, inc.", 0, 14, 1, 26);
                seg041.displayString("jim ward", 0, 10, 2, 15);
                seg041.displayString("david cook,steve winter,mike breault", 0, 10, 3, 2);
                seg041.displayString("game created by:", 0, 10, 5, 1);
                seg041.displayString("ssi special projects", 0, 14, 5, 18);
                seg041.displayString("version 1.3", 0, 11, 6, 14);
                seg041.displayString("programming:", 0, 14, 7, 2);
                seg041.displayString("original", 0, 14, 7, 20);
                seg041.displayString("scot bayless", 0, 11, 8, 2);
                seg041.displayString("programming:", 0, 14, 8, 20);
                seg041.displayString("brad byers", 0, 11, 9, 2);
                seg041.displayString("keith brors", 0, 11, 9, 20);
                seg041.displayString("russ brown", 0, 11, 10, 2);
                seg041.displayString("brad myers", 0, 11, 10, 20);
                seg041.displayString("ted greer", 0, 11, 11, 2);
                seg041.displayString("graphic arts:", 0, 14, 13, 2);
                seg041.displayString("encounter coding:", 0, 14, 13, 20);
                seg041.displayString("tom wahl", 0, 11, 14, 2);
                seg041.displayString("paul murray", 0, 11, 14, 20);
                seg041.displayString("fred butts", 0, 11, 15, 2);
                seg041.displayString("russ brown", 0, 11, 15, 20);
                seg041.displayString("darla marasco", 0, 11, 16, 2);
                seg041.displayString("victor penman", 0, 11, 16, 20);
                seg041.displayString("susan halbleib", 0, 11, 17, 2);
                seg041.displayString("dave shelley", 0, 11, 17, 20);
                seg041.displayString("project manager:", 0, 14, 19, 2);
                seg041.displayString("developer:", 0, 14, 19, 20);
                seg041.displayString("victor penman", 0, 11, 20, 2);
                seg041.displayString("george mac donald", 0, 11, 20, 20);
                seg041.displayString("testing:", 0, 14, 22, 2);
                seg041.displayString("joel billings,steve salyer", 0, 11, 22, 11);
                seg041.displayString("james kucera,robert daly,rick white", 0, 11, 23, 2);
            }
            else if (gbl.game == Game.CurseOfTheAzureBonds)
            {
                seg041.displayString("based on the tsr novel 'azure bonds'", 0, 10, 1, 2);
                seg041.displayString("by:", 0, 10, 2, 6);
                seg041.displayString("kate novak", 0, 11, 2, 9);
                seg041.displayString("and", 0, 10, 2, 0x14);
                seg041.displayString("jeff grubb", 0, 11, 2, 0x18);
                seg041.displayString("scenario created by:", 0, 10, 4, 0x0a);
                seg041.displayString("tsr, inc.", 0, 0x0e, 5, 0x0b);
                seg041.displayString("and", 0, 0x0a, 5, 0x15);
                seg041.displayString("ssi", 0, 0x0e, 5, 0x19);
                seg041.displayString("jeff grubb", 0, 0x0b, 6, 0x0e);
                seg041.displayString("george mac donald", 0x0, 0x0B, 0x7, 0x0B);
                seg041.displayString("game created by:", 0x0, 0x0A, 0x9, 0x1);
                seg041.displayString("ssi special projects", 0x0, 0x0E, 0x9, 0x12);
                seg041.displayString("project leader:", 0x0, 0x0E, 0x0B, 0x2);
                seg041.displayString("george mac donald", 0x0, 0x0B, 0x0C, 0x2);
                seg041.displayString("programming:", 0x0, 0x0E, 0x0E, 0x2);
                seg041.displayString("scot bayless", 0x0, 0x0B, 0x0F, 0x2);
                seg041.displayString("russ brown", 0x0, 0x0B, 0x10, 0x2);
                seg041.displayString("michael mancuso", 0x0, 0x0B, 0x11, 0x2);
                seg041.displayString("development:", 0x0, 0x0E, 0x13, 0x2);
                seg041.displayString("david shelley", 0x0, 0x0B, 0x14, 0x2);
                seg041.displayString("michael mancuso", 0x0, 0x0B, 0x15, 0x2);
                seg041.displayString("oran kangas", 0x0, 0x0B, 0x16, 0x2);
                seg041.displayString("graphic arts:", 0x0, 0x0E, 0x0B, 0x16);
                seg041.displayString("tom wahl", 0x0, 0x0B, 0x0C, 0x16);
                seg041.displayString("fred butts", 0x0, 0x0B, 0x0D, 0x16);
                seg041.displayString("susan manley", 0x0, 0x0B, 0x0E, 0x16);
                seg041.displayString("mark johnson", 0x0, 0x0B, 0x0F, 0x16);
                seg041.displayString("cyrus lum", 0x0, 0x0B, 0x10, 0x16);
                seg041.displayString("playtesting:", 0x0, 0x0E, 0x12, 0x16);
                seg041.displayString("jim jennings", 0x0, 0x0B, 0x13, 0x16);
                seg041.displayString("james kucera", 0x0, 0x0B, 0x14, 0x16);
                seg041.displayString("rick white", 0x0, 0x0B, 0x15, 0x16);
                seg041.displayString("robert daly", 0x0, 0x0B, 0x16, 0x16);
            }
            else // if (gbl.game == Game.SecretOfTheSilverBlades)
            {
                seg041.displayString("secret of the silver blades", 0, 5, 0, 7);
                seg041.displayString("game created by:", 0, 5, 3, 2);
                seg041.displayString("story and", 0, 5, 3, 22);
                seg041.displayString("ssi special", 0, 14, 4, 2);
                seg041.displayString("development by:", 0, 5, 4, 22);
                seg041.displayString("projects team", 0, 14, 5, 2);
                seg041.displayString("dave shelley", 0, 14, 5, 22);
                seg041.displayString("programming:", 0, 5, 7, 2);
                seg041.displayString("graphic arts", 0, 5, 7, 22);
                seg041.displayString("ken nicholson", 0, 14, 8, 2);
                seg041.displayString("mark johnson", 0, 14, 8, 22);
                seg041.displayString("russ brown", 0, 14, 9, 2);
                seg041.displayString("laura bowen", 0, 14, 9, 22);
                seg041.displayString("fred butts", 0, 14, 10, 22);
                seg041.displayString("encounter design:", 0, 5, 11, 2);
                seg041.displayString("cyrus lum", 0, 14, 11, 22);
                seg041.displayString("dave shelley", 0, 14, 12, 2);
                seg041.displayString("susan manley", 0, 14, 12, 22);
                seg041.displayString("michael mancuso", 0, 14, 13, 2);
                seg041.displayString("mike provenza", 0, 14, 13, 22);
                seg041.displayString("ken humphries", 0, 14, 14, 2);
                seg041.displayString("tom wahl", 0, 14, 14, 22);
                seg041.displayString("graeme bayless", 0, 14, 15, 2);
                seg041.displayString("rick white", 0, 14, 16, 2);
                seg041.displayString("musical score:", 0, 5, 16, 22);
                seg041.displayString("john halbleib", 0, 14, 17, 22);
                seg041.displayString("playtest:", 0, 5, 19, 2);
                seg041.displayString("rick white,don mc clure,", 0, 14, 19, 12);
                seg041.displayString("dave lucca,cliff mann,rick wilson,", 0, 14, 20, 2);
                seg041.displayString("erik flom,james young,larry webber", 0, 14, 21, 2);
            }

            Display.UpdateStart();
        }


        internal static void title_screen()
        {
            DaxBlock dax_ptr;

            dax_ptr = seg040.LoadDax(0, 0, 1, "Title");
            seg040.draw_picture(dax_ptr, 0, 0, 0);

            delay_or_key(5);

            dax_ptr = seg040.LoadDax(0, 0, 2, "Title");
            seg040.draw_picture(dax_ptr, 0, 0, 0);

            if (gbl.game == Game.CurseOfTheAzureBonds)
            {
                dax_ptr = seg040.LoadDax(0, 0, 3, "Title");
                seg040.draw_picture(dax_ptr, 0x0b, 6, 0);
                delay_or_key(10);

                dax_ptr = seg040.LoadDax(0, 0, 4, "Title");

                seg044.PlaySound(Sound.sound_d);

                seg040.draw_picture(dax_ptr, 0x0b, 0, 0);
                delay_or_key(10);
            }
            else if (gbl.game == Game.SecretOfTheSilverBlades)
            {
                delay_or_key(10);
                dax_ptr = seg040.LoadDax(0, 0, 3, "Title");
                seg040.draw_picture(dax_ptr, 5, 2, 0);
                delay_or_key(10);

                dax_ptr = seg040.LoadDax(0, 0, 2, "Title");
                seg040.draw_picture(dax_ptr, 0, 0, 0);
                delay_or_key(10);

                dax_ptr = seg040.LoadDax(0, 0, 4, "Title");
                seg040.draw_picture(dax_ptr, 5, 5, 0);
                delay_or_key(10);
            }

            seg041.ClearScreen();
            credits();
            delay_or_key(10);

            seg041.ClearScreen();
        }
    }
}
