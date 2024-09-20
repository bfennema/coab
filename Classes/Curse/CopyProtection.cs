using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes.Curse
{
    internal class CopyProtection
    {
        static string[] codeWheel = {
            "CWLNRTESSCEDCSHSISERRRNSHSSTSSNNHSHN",
            "LAASRDAIILIDSUGADAEEOEGRLSELIITESOIO",
            "LRUNIMMORIIGRRIUPTIIUELIMLHMIXACGRIL",
            "Z0LIOHEUVNODSGEOGXYWISIOCRARLRARRHOI",
            "AMTELRLUIYNAEOOITOUELRREREUIMADPPFAB",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
        };

        static System.Random randomNumber;

        static CopyProtection()
        {
            randomNumber = new System.Random(unchecked((int)System.DateTime.Now.Ticks));
        }
        public static void DrawProtection()
        {
            gbl.Load24x24Set(0x1A, 0, 1, "tiles");
            gbl.Load24x24Set(0x16, 0x1A, 2, "tiles");

            gbl.game.DrawFrame_Outer();

            gbl.displayString("Align the espruar and dethek runes", 0, 10, 2, 3);
            gbl.displayString("shown below, on translation wheel", 0, 10, 3, 3);
            gbl.displayString("like this:", 0, 10, 4, 3);
        }
        public static (string,string) CheckProtection()
        {
            string code_path_str;
            int var_6 = randomNumber.Next() % 26;
            int var_7 = randomNumber.Next() % 22;

            gbl.DrawIsoTile(var_6, 3, 0x11);
            gbl.DrawIsoTile(var_7 + 0x1a, 7, 0x11);

            int code_path = randomNumber.Next() % 3;

            switch (code_path)
            {
                case 0:
                    code_path_str = "-..-..-..";
                    break;

                case 1:
                    code_path_str = "- - - - -";
                    break;

                case 2:
                    code_path_str = ".........";
                    break;

                default:
                    code_path_str = string.Empty;
                    break;
            }

            int code_row = randomNumber.Next() % 6;

            string text = "Type the character in box number " + (6 - code_row);

            gbl.displayString(text, 0, 10, 12, 3);

            gbl.displayString("under the ", 0, 10, 13, 3);
            gbl.displayString(code_path_str, 0, 15, 13, 14);
            gbl.displayString("path.", 0, 10, 13, 0x19);

            int code_index = var_6 + 0x22 - var_7 + (code_path * 12) + ((5 - code_row) << 1);

            while (code_index < 0)
            {
                code_index += 36;
            }

            while (code_index > 35)
            {
                code_index -= 36;
            }

            var input_expected = codeWheel[code_row][code_index];

            return (String.Format("{0}", input_expected), "type character and press return: ");
        }
    }
}
