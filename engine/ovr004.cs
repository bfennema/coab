using Classes;

namespace engine
{
    class ovr004
    {
        static string[] codeWheel = { 
								 "CWLNRTESSCEDCSHSISERRRNSHSSTSSNNHSHN",
								 "LAASRDAIILIDSUGADAEEOEGRLSELIITESOIO",
								 "LRUNIMMORIIGRRIUPTIIUELIMLHMIXACGRIL",
								 "Z0LIOHEUVNODSGEOGXYWISIOCRARLRARRHOI",
								 "AMTELRLUIYNAEOOITOUELRREREUIMADPPFAB",
								 "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
							 };

        static char[,] espruarTable =
        {
            {'\x41', '\x42' }, {'\x43', '\x44' }, {'\x45', '\x46' }, {'\x47', '\x48' }, {'\x49', '\x4A' },
            {'\x4B', '\x4C' }, {'\x4D', '\x4E' }, {'\x4F', '\x50' }, {'\x51', '\x52' }, {'\x53', '\x54' },
            {'\x55', '\x56' }, {'\x57', '\x58' }, {'\x59', '\x5A' }, {'\x5B', '\x5C' }, {'\x5D', '\x5E' },
            {'\x62', '\x63' }, {'\x64', '\x65' }, {'\x66', '\x67' }, {'\x68', '\x69' }, {'\x6A', '\x6B' },
            {'\x6C', '\x6D' }, {'\x6E', '\x6F' }, {'\x70', '\x71' }, {'\x72', '\x73' }, {'\x74', '\x75' },
            {'\x76', '\x77' }, {'\x95', '\x96' }, {'\x97', '\x98' }, {'\x99', '\x9A' }, {'\x9B', '\x9C' },
            {'\x9D', '\x9E' }, {'\x9F', '\xA0' }, {'\xA1', '\xA2' }, {'\xA3', '\xA4' }, {'\xA5', '\xA6' },
        };

        static char[,] dethekTable =
        {
            {'\xAC', '\xB0' }, {'\xAF', '\x00' }, {'\xAE', '\x00' }, {'\xAD', '\x00' }, {'\xAC', '\x00' },
            {'\xB0', '\x00' }, {'\xAA', '\x00' }, {'\xA9', '\x00' }, {'\xA8', '\x00' }, {'\x93', '\x00' },
            {'\x92', '\x00' }, {'\x91', '\x00' }, {'\x90', '\x00' }, {'\x8F', '\x00' }, {'\x8E', '\x00' },
            {'\x8D', '\x00' }, {'\x8C', '\x00' }, {'\x8B', '\x00' }, {'\x8A', '\x00' }, {'\x89', '\x00' },
            {'\x88', '\x00' }, {'\x87', '\x00' }, {'\x86', '\x00' }, {'\x85', '\x00' }, {'\x84', '\x00' },
            {'\x83', '\x00' }, {'\x82', '\x00' }, {'\x81', '\x00' }, {'\x80', '\x00' }, {'\x7F', '\x00' },
            {'\x7E', '\x00' }, {'\x7D', '\x00' }, {'\x7C', '\x00' }, {'\x7B', '\x00' }, {'\x7A', '\x00' },
        };

        static string[] porCodeWheel =
        {
            "SXERNTDEHIETTSWADGAREHNNKAEYDIRANSST",
            "AAREOENLSSLHMXOWROIOLRARHTIRCAEGWTIU",
            "MIAPGEEGAVGGGSNALHDIPDCEESBKLSNNOASO",
            "OEWPARIORYNILSTTOOUVMCLVSFMKOAIURNAR",
            "SXEORFROAXUNQLOPOUHAEIUYRUOGIMNGBGOT",
            "0ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789",
        };

        internal static void copy_protection()
        {
            int attempt = 0;
            string input_expected;
            string input;

            gbl.game.DrawProtection();

            do
            {
                (input_expected, string text) = gbl.game.CheckProtection();

                input = seg041.getUserInputString((byte)input_expected.Length, 0, 13, text);

                attempt++;

                if (input != input_expected)
                {
                    seg041.DisplayStatusText(0, 14, "Sorry, that's incorrect.");
                }
            } while (gbl.Exit == false && input != input_expected && attempt < 3);

            if (gbl.Exit == false && attempt >= 3)
            {
                seg044.PlaySound(Sound.sound_1);
                seg044.PlaySound(Sound.sound_5);
                gbl.game_speed_var = 9;
                seg041.DisplayStatusText(0, 14, "An unseen force hurls you into the abyss!");
                seg049.SysDelay(0x3E8);
                seg043.print_and_exit();
            }
        }
    }
}
