using Classes;
using Logging;
using System.Threading.Tasks;

namespace engine
{
    class ovr038
    {
        internal static void Put8x8Symbol(byte arg_0, bool use_overlay, int symbol_id, int rowY, int colX)
        {
            byte symbol_set = 0; /*HACK to make compiler happy*/

            if (symbol_id >= 1 && symbol_id <= 0x2d)
            {
                symbol_set = 0;
            }
            else if (symbol_id >= 0x2E && symbol_id <= 0x73)
            {
                symbol_set = 1;
            }
            else if (symbol_id >= 0x74 && symbol_id <= 0x0B9)
            {
                symbol_set = 2;
            }
            else if (symbol_id >= 0x0BA && symbol_id <= 0x0FF)
            {
                symbol_set = 3;
            }
            else if (symbol_id >= 0x100 && symbol_id <= 0x164)
            {
                symbol_set = 4;
            }
            else if (symbol_id == 0 || (symbol_id >= 0x165 && symbol_id <= 0x7FFF))
            {
                throw new System.ApplicationException("Bad symbol number in Put8x8Symbol." + symbol_id);
            }

            if (gbl.symbol_8x8_set[symbol_set] != null)
            {
                symbol_id -= gbl.symbol_set_fix[symbol_set];

                if (use_overlay)
                {
                    seg040.OverlayUnbounded(gbl.symbol_8x8_set[symbol_set], arg_0, symbol_id, rowY, colX);
                }
                else
                {
                    DaxBlock var_6 = gbl.symbol_8x8_set[symbol_set];

                    int offset = symbol_id * var_6.bpp;
                    System.Array.Copy(var_6.data, offset, gbl.cursor_bkup.data, 0, var_6.bpp);

                    seg040.draw_picture(gbl.cursor_bkup, rowY, colX, 0);
                }
            }
        }
    }
}
