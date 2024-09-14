using Classes;

namespace engine
{
    class seg037
    {
        internal static void draw8x8_clear_area(TextRegion region)
        {
            int r = (int)region;
            draw8x8_clear_area(seg041.bounds[r, 0], seg041.bounds[r, 1], seg041.bounds[r, 2], seg041.bounds[r, 3]);
        }

        internal static void draw8x8_clear_area(int yEnd, int xEnd, int yStart, int xStart)
        {
            seg041.DrawRectangle(0, yEnd, xEnd, yStart, xStart);
        }
    }
}
