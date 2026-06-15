using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Classes;
using System;

namespace GoldBoxPlayer.Views
{
    public class MapView : Avalonia.Controls.Control
    {
        static MapView()
        {
            AffectsRender<MapView>(BoundsProperty);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var bounds = Bounds;
            double width = bounds.Width;
            double height = bounds.Height;

            // Fill background with a muted/darker tan color representing unexplored area
            var unexploredBrush = new SolidColorBrush(Color.Parse("#8c7653"));
            context.FillRectangle(unexploredBrush, new Rect(0, 0, width, height));

            if (gbl.geo_ptr?.maps == null)
            {
                var promptBrush = new SolidColorBrush(Colors.White);
                var text = new FormattedText(
                    "No Map Loaded",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    16,
                    promptBrush);
                context.DrawText(text, new Avalonia.Point((width - text.Width) / 2, (height - text.Height) / 2));
                return;
            }

            int mapRows = gbl.geo_ptr.maps.GetLength(0);
            int mapCols = gbl.geo_ptr.maps.GetLength(1);

            double cellWidth = width / mapCols;
            double cellHeight = height / mapRows;

            int gameArea = gbl.game_area;
            int blockId = gbl.area_ptr != null ? gbl.area_ptr.current_3DMap_block_id : 0;

            // Draw cell backgrounds: warm tan if explored
            var exploredBrush = new SolidColorBrush(Color.Parse("#bfa374"));
            for (int y = 0; y < mapRows; y++)
            {
                for (int x = 0; x < mapCols; x++)
                {
                    if (MapTracker.IsExplored(gameArea, blockId, x, y))
                    {
                        context.FillRectangle(exploredBrush, new Rect(x * cellWidth, y * cellHeight, cellWidth, cellHeight));
                    }
                }
            }

            // Draw grid lines
            var gridPen = new Pen(new SolidColorBrush(Color.Parse("#6d573d")), 0.5);
            for (int r = 0; r <= mapRows; r++)
            {
                context.DrawLine(gridPen, new Avalonia.Point(0, r * cellHeight), new Avalonia.Point(width, r * cellHeight));
            }
            for (int c = 0; c <= mapCols; c++)
            {
                context.DrawLine(gridPen, new Avalonia.Point(c * cellWidth, 0), new Avalonia.Point(c * cellWidth, height));
            }

            // Draw walls and doors.
            // A wall/door is drawn only if at least one of the cells sharing the boundary is explored.
            for (int y = 0; y < mapRows; y++)
            {
                for (int x = 0; x < mapCols; x++)
                {
                    MapInfo mi = gbl.geo_ptr.maps[y, x];
                    if (mi == null) continue;

                    bool currentExplored = MapTracker.IsExplored(gameArea, blockId, x, y);

                    double cellLeft = x * cellWidth;
                    double cellTop = y * cellHeight;
                    double cellRight = cellLeft + cellWidth;
                    double cellBottom = cellTop + cellHeight;

                    // North boundary: cell (y, x) [Side A] vs cell (y - 1, x) [Side B]
                    if (mi.wall_type_dir_0 != 0)
                    {
                        bool neighborExplored = (y > 0) && MapTracker.IsExplored(gameArea, blockId, x, y - 1);
                        if (currentExplored || neighborExplored)
                        {
                            DrawBoundary(context, cellLeft, cellTop, cellRight, cellTop, mi.x3_dir_0, cellWidth, cellHeight, true, currentExplored, neighborExplored);
                        }
                    }
                    // East boundary: cell (y, x) [Side A] vs cell (y, x + 1) [Side B]
                    if (mi.wall_type_dir_2 != 0)
                    {
                        bool neighborExplored = (x < mapCols - 1) && MapTracker.IsExplored(gameArea, blockId, x + 1, y);
                        if (currentExplored || neighborExplored)
                        {
                            DrawBoundary(context, cellRight, cellTop, cellRight, cellBottom, mi.x3_dir_2, cellWidth, cellHeight, false, currentExplored, neighborExplored);
                        }
                    }
                    // South boundary: cell (y, x) [Side A] vs cell (y + 1, x) [Side B]
                    if (mi.wall_type_dir_4 != 0)
                    {
                        bool neighborExplored = (y < mapRows - 1) && MapTracker.IsExplored(gameArea, blockId, x, y + 1);
                        if (currentExplored || neighborExplored)
                        {
                            DrawBoundary(context, cellLeft, cellBottom, cellRight, cellBottom, mi.x3_dir_4, cellWidth, cellHeight, true, currentExplored, neighborExplored);
                        }
                    }
                    // West boundary: cell (y, x) [Side A] vs cell (y, x - 1) [Side B]
                    if (mi.wall_type_dir_6 != 0)
                    {
                        bool neighborExplored = (x > 0) && MapTracker.IsExplored(gameArea, blockId, x - 1, y);
                        if (currentExplored || neighborExplored)
                        {
                            DrawBoundary(context, cellLeft, cellTop, cellLeft, cellBottom, mi.x3_dir_6, cellWidth, cellHeight, false, currentExplored, neighborExplored);
                        }
                    }
                }
            }

            // Draw player position and orientation as a green triangle
            int px = gbl.mapPosX;
            int py = gbl.mapPosY;
            if (px >= 0 && px < mapCols && py >= 0 && py < mapRows)
            {
                double centerLeft = px * cellWidth + cellWidth / 2;
                double centerTop = py * cellHeight + cellHeight / 2;

                var playerBrush = new SolidColorBrush(Color.Parse("#2e7d32")); // Green
                var playerPen = new Pen(new SolidColorBrush(Colors.White), 1.0);

                double r = Math.Min(cellWidth, cellHeight) * 0.35;
                Avalonia.Point p1, p2, p3;

                if (gbl.mapDirection == 0) // North
                {
                    p1 = new Avalonia.Point(centerLeft, centerTop - r);
                    p2 = new Avalonia.Point(centerLeft - r * 0.8, centerTop + r * 0.6);
                    p3 = new Avalonia.Point(centerLeft + r * 0.8, centerTop + r * 0.6);
                }
                else if (gbl.mapDirection == 2) // East
                {
                    p1 = new Avalonia.Point(centerLeft + r, centerTop);
                    p2 = new Avalonia.Point(centerLeft - r * 0.6, centerTop - r * 0.8);
                    p3 = new Avalonia.Point(centerLeft - r * 0.6, centerTop + r * 0.8);
                }
                else if (gbl.mapDirection == 4) // South
                {
                    p1 = new Avalonia.Point(centerLeft, centerTop + r);
                    p2 = new Avalonia.Point(centerLeft - r * 0.8, centerTop - r * 0.6);
                    p3 = new Avalonia.Point(centerLeft + r * 0.8, centerTop - r * 0.6);
                }
                else // West (6)
                {
                    p1 = new Avalonia.Point(centerLeft - r, centerTop);
                    p2 = new Avalonia.Point(centerLeft + r * 0.6, centerTop - r * 0.8);
                    p3 = new Avalonia.Point(centerLeft + r * 0.6, centerTop + r * 0.8);
                }

                var playerGeometry = new PathGeometry
                {
                    Figures = new PathFigures
                    {
                        new PathFigure
                        {
                            StartPoint = p1,
                            IsClosed = true,
                            Segments = new PathSegments
                            {
                                new LineSegment { Point = p2 },
                                new LineSegment { Point = p3 }
                            }
                        }
                    }
                };

                context.DrawGeometry(playerBrush, playerPen, playerGeometry);
            }
        }

        private void DrawBoundary(DrawingContext context, double x1, double y1, double x2, double y2, byte doorFlag, double cellWidth, double cellHeight, bool isHorizontal, bool sideAExplored, bool sideBExplored)
        {
            var wallPen = new Pen(new SolidColorBrush(Color.Parse("#4a371c")), 3.5);
            var doorBrush = new SolidColorBrush(Colors.White);
            var doorPen = new Pen(new SolidColorBrush(Colors.Black), 1.0);
            var lockedDoorBrush = new SolidColorBrush(Color.Parse("#e65100")); // Orange
            var secretDoorPen = new Pen(new SolidColorBrush(Color.Parse("#b71c1c")), 2.0, dashStyle: DashStyle.Dash); // Dashed red

            if (doorFlag == 0) // Normal wall
            {
                context.DrawLine(wallPen, new Avalonia.Point(x1, y1), new Avalonia.Point(x2, y2));
            }
            else if (doorFlag == 1 || doorFlag == 2) // Door or Locked Door
            {
                double midX = (x1 + x2) / 2;
                double midY = (y1 + y2) / 2;

                double doorWidth = isHorizontal ? cellWidth * 0.35 : cellWidth * 0.12;
                double doorHeight = isHorizontal ? cellHeight * 0.12 : cellHeight * 0.35;

                var brush = (doorFlag == 2) ? lockedDoorBrush : doorBrush;

                if (isHorizontal)
                {
                    // Side A is below (positive Y), Side B is above (negative Y)
                    if (sideAExplored)
                    {
                        // Draw bottom half
                        Rect r = new Rect(midX - doorWidth / 2, midY, doorWidth, doorHeight / 2);
                        context.DrawRectangle(brush, doorPen, r);
                    }
                    if (sideBExplored)
                    {
                        // Draw top half
                        Rect r = new Rect(midX - doorWidth / 2, midY - doorHeight / 2, doorWidth, doorHeight / 2);
                        context.DrawRectangle(brush, doorPen, r);
                    }
                }
                else
                {
                    // Vertical boundary.
                    // For East boundary, Side A is left (negative X), Side B is right (positive X).
                    // For West boundary, Side A is right (positive X), Side B is left (negative X).
                    // To be general, we can look at the relative position of the cell coordinate or just use x1, y1.
                    // Since it's vertical, we can draw the left half if either (East boundary and sideAExplored) or (West boundary and sideBExplored).
                    // To keep it simple: we can draw left half if the left cell is explored, and right half if the right cell is explored!
                    // Let's determine which side is left and right of this vertical line:
                    // The line is at midX. The cell to the left is x - 1, the cell to the right is x.
                    // So if sideAExplored corresponds to cell (y, x) and sideBExplored to (y, x - 1) [for West boundary],
                    // then cell to the right is Side A, cell to the left is Side B.
                    // If Side A is on the right, we draw right half if sideAExplored, and left half if sideBExplored.
                    // If Side A is on the left, we draw left half if sideAExplored, and right half if sideBExplored.
                    // Since East boundary: Side A is left (x), Side B is right (x+1). So Side A is left.
                    // Since West boundary: Side A is right (x), Side B is left (x-1). So Side A is right.

                    // Let's check which boundary it is:
                    // If the boundary is at x1 (which is cellLeft or cellRight):
                    // Let's just find out if the left cell of midX is explored, and right cell of midX is explored.
                    // We can check:
                    // Is the left cell visited? (xCoord = (int)(midX / cellWidth) - 1 if midX is on boundary)
                    // It's much simpler:
                    // For vertical boundary, Side A is either Left (for East boundary) or Right (for West boundary).
                    // But wait, the loop in Render draws boundaries for each cell:
                    // - East boundary of (y, x) is at cellRight. Left cell is current (Side A), Right cell is neighbor (Side B).
                    //   So Left = sideAExplored, Right = sideBExplored.
                    // - West boundary of (y, x) is at cellLeft. Right cell is current (Side A), Left cell is neighbor (Side B).
                    //   So Right = sideAExplored, Left = sideBExplored.
                    // But wait! Since we iterate over all cells, we will draw the East boundary of cell (y, x) and also the West boundary of cell (y, x+1).
                    // Wait, do we double-draw boundaries?
                    // Ah! The original code drew them for every cell, which could double-draw shared boundaries.
                    // To draw them accurately:
                    // If we just check:
                    // Is the cell to the left of midX explored?
                    // Let's compute left cell coordinate: `int leftX = (int)((midX - 0.1) / cellWidth);`
                    // Is the cell to the right of midX explored?
                    // Let's compute right cell coordinate: `int rightX = (int)((midX + 0.1) / cellWidth);`
                    // This is extremely robust and doesn't depend on Side A/B mapping!
                    // Let's do the same for horizontal boundary:
                    // Is the cell above midY explored? `int topY = (int)((midY - 0.1) / cellHeight);`
                    // Is the cell below midY explored? `int bottomY = (int)((midY + 0.1) / cellHeight);`

                    int mapRows = gbl.geo_ptr.maps.GetLength(0);
                    int mapCols = gbl.geo_ptr.maps.GetLength(1);
                    int gameArea = gbl.game_area;
                    int blockId = gbl.area_ptr != null ? gbl.area_ptr.current_3DMap_block_id : 0;

                    if (isHorizontal)
                    {
                        int topY = (int)Math.Floor((midY - cellHeight * 0.1) / cellHeight);
                        int bottomY = (int)Math.Floor((midY + cellHeight * 0.1) / cellHeight);

                        bool topExplored = (topY >= 0 && topY < mapRows) && MapTracker.IsExplored(gameArea, blockId, (int)Math.Floor(midX / cellWidth), topY);
                        bool bottomExplored = (bottomY >= 0 && bottomY < mapRows) && MapTracker.IsExplored(gameArea, blockId, (int)Math.Floor(midX / cellWidth), bottomY);

                        if (bottomExplored)
                        {
                            Rect r = new Rect(midX - doorWidth / 2, midY, doorWidth, doorHeight / 2);
                            context.DrawRectangle(brush, doorPen, r);
                        }
                        if (topExplored)
                        {
                            Rect r = new Rect(midX - doorWidth / 2, midY - doorHeight / 2, doorWidth, doorHeight / 2);
                            context.DrawRectangle(brush, doorPen, r);
                        }
                    }
                    else
                    {
                        int leftX = (int)Math.Floor((midX - cellWidth * 0.1) / cellWidth);
                        int rightX = (int)Math.Floor((midX + cellWidth * 0.1) / cellWidth);

                        bool leftExplored = (leftX >= 0 && leftX < mapCols) && MapTracker.IsExplored(gameArea, blockId, leftX, (int)Math.Floor(midY / cellHeight));
                        bool rightExplored = (rightX >= 0 && rightX < mapCols) && MapTracker.IsExplored(gameArea, blockId, rightX, (int)Math.Floor(midY / cellHeight));

                        if (leftExplored)
                        {
                            Rect r = new Rect(midX - doorWidth / 2, midY - doorHeight / 2, doorWidth / 2, doorHeight);
                            context.DrawRectangle(brush, doorPen, r);
                        }
                        if (rightExplored)
                        {
                            Rect r = new Rect(midX, midY - doorHeight / 2, doorWidth / 2, doorHeight);
                            context.DrawRectangle(brush, doorPen, r);
                        }
                    }
                }
            }
            else if (doorFlag == 3) // Secret door
            {
                context.DrawLine(secretDoorPen, new Avalonia.Point(x1, y1), new Avalonia.Point(x2, y2));
            }
        }
    }
}
