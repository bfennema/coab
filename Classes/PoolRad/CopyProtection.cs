using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes.PoolRad
{
    internal class CopyProtection
    {
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

        static string[] codeWheel =
        {
            "SXERNTDEHIETTSWADGAREHNNKAEYDIRANSST",
            "AAREOENLSSLHMXOWROIOLRARHTIRCAEGWTIU",
            "MIAPGEEGAVGGGSNALHDIPDCEESBKLSNNOASO",
            "OEWPARIORYNILSTTOOUVMCLVSFMKOAIURNAR",
            "SXEORFROAXUNQLOPOUHAEIUYRUOGIMNGBGOT",
            "0ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789",
        };

        static System.Random randomNumber;

        static CopyProtection()
        {
            randomNumber = new System.Random(unchecked((int)System.DateTime.Now.Ticks));
        }
        public static void DrawProtection()
        {
            gbl.game.DrawFrame_Outer();

            gbl.displayString("Use the translation wheel to decipher", 0, 10, 1, 1);
            gbl.displayString("the code word.", 0, 10, 2, 1);
        }
        public static (string,string) CheckProtection()
        {
            string code_path_str;
            string input;
            string input_expected;

            gbl.displayString("Match the espruar rune (outer ring)", 0, 10, 5, 1);

            int espruar = randomNumber.Next() % 35;

            for (int i = 0; i < 8; i++)
            {
                gbl.monoCharData[i] = gbl.dax_8x8d1_201[espruarTable[espruar, 0], i];
            }
            Display.DisplayMono8x8(8, 6, gbl.monoCharData, 0, 12);

            for (int i = 0; i < 8; i++)
            {
                gbl.monoCharData[i] = gbl.dax_8x8d1_201[espruarTable[espruar, 1], i];
            }
            Display.DisplayMono8x8(9, 6, gbl.monoCharData, 0, 12);

            gbl.displayString("With the dethek ring (inner ring)", 0, 10, 8, 1);

            int dethek = randomNumber.Next() % 35;

            for (int i = 0; i < 8; i++)
            {
                gbl.monoCharData[i] = gbl.dax_8x8d1_201[dethekTable[dethek, 0], i];
            }
            Display.DisplayMono8x8(8, 9, gbl.monoCharData, 0, 12);

            for (int i = 0; i < 8; i++)
            {
                if (dethekTable[dethek, 1] > 0)
                {
                    gbl.monoCharData[i] = gbl.dax_8x8d1_201[dethekTable[dethek, 1], i];
                }
                else
                {
                    gbl.monoCharData[i] = gbl.dax_8x8d1_201['\xA7', i];
                }
            }
            Display.DisplayMono8x8(9, 9, gbl.monoCharData, 0, 12);

            gbl.displayString("And read the code word under the path:", 0, 10, 11, 1);

            int code_path = randomNumber.Next() % 3;

            switch (code_path)
            {
                case 0:
                    code_path_str = ".........";
                    break;

                case 1:
                    code_path_str = "-..-..-..";
                    break;

                case 2:
                    code_path_str = "- - - - -";
                    break;

                default:
                    code_path_str = string.Empty;
                    break;
            }

            gbl.displayString(code_path_str, 0, 12, 13, 6);

            gbl.displayString("Read from the inside to the outside.", 0, 10, 14, 1);

            var delta = espruar - dethek;
            if (delta < 0)
            {
                delta += 36;
            }
            delta += (code_path * 12);
            if (delta > 36)
            {
                delta -= 36;
            }

            input_expected = String.Format("{0}{1}{2}{3}{4}{5}", codeWheel[5][delta], codeWheel[4][delta], codeWheel[3][delta], codeWheel[2][delta], codeWheel[1][delta], codeWheel[0][delta]);

            return (input_expected, "Input the code word:  ");
        }
    }
}
