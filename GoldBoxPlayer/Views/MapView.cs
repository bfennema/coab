using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Classes;
using System;

namespace GoldBoxPlayer.Views
{
    public class MapView : Avalonia.Controls.Control
    {
        // ── Zoom / cell‑size property ──────────────────────────────────────
        public static readonly StyledProperty<double> CellSizeProperty =
            AvaloniaProperty.Register<MapView, double>(nameof(CellSize), defaultValue: 25.0);

        public double CellSize
        {
            get => GetValue(CellSizeProperty);
            set => SetValue(CellSizeProperty, Math.Max(20.0, value));
        }

        // ── Debug Show Event Numbers Property ──────────────────────────────
        public static readonly StyledProperty<bool> ShowEventNumbersProperty =
            AvaloniaProperty.Register<MapView, bool>(nameof(ShowEventNumbers), defaultValue: false);

        public bool ShowEventNumbers
        {
            get => GetValue(ShowEventNumbersProperty);
            set => SetValue(ShowEventNumbersProperty, value);
        }

        static MapView()
        {
            AffectsRender<MapView>(BoundsProperty);
            AffectsRender<MapView>(CellSizeProperty);
            AffectsMeasure<MapView>(CellSizeProperty);
            AffectsRender<MapView>(ShowEventNumbersProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (gbl.geo_ptr?.maps == null)
                return new Size(200, 200);

            int mapRows = gbl.geo_ptr.maps.GetLength(0);
            int mapCols = gbl.geo_ptr.maps.GetLength(1);
            const double padding = 12.0;
            double cs = CellSize;
            return new Size(mapCols * cs + 2 * padding, mapRows * cs + 2 * padding);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // Define padding to keep edge squares away from window border
            const double padding = 12.0;

            double cs = CellSize;

            // Fill background within padded area
            var unexploredBrush = new SolidColorBrush(Color.Parse("#8c7653"));

            if (gbl.geo_ptr?.maps == null)
            {
                context.FillRectangle(unexploredBrush, new Rect(0, 0, 200, 200));
                var promptBrush = new SolidColorBrush(Colors.White);
                var text = new FormattedText(
                    "No Map Loaded",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    16,
                    promptBrush);
                context.DrawText(text, new Avalonia.Point((200 - text.Width) / 2, (200 - text.Height) / 2));
                return;
            }

            int mapRows = gbl.geo_ptr.maps.GetLength(0);
            int mapCols = gbl.geo_ptr.maps.GetLength(1);

            double totalWidth  = mapCols * cs + 2 * padding;
            double totalHeight = mapRows * cs + 2 * padding;

            // Fill full background
            context.FillRectangle(unexploredBrush, new Rect(0, 0, totalWidth, totalHeight));

            // Cell dimensions equal CellSize
            double cellWidth  = cs;
            double cellHeight = cs;

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
                        context.FillRectangle(exploredBrush, new Rect(padding + x * cellWidth, padding + y * cellHeight, cellWidth, cellHeight));
                    }
                }
            }

            // Draw grid lines (same color as unexplored cells) with padding offset
            var gridPen = new Pen(new SolidColorBrush(Color.Parse("#8c7653")), 0.5);
            for (int r = 0; r < mapRows; r++)
            {
                double yPos = padding + r * cellHeight;
                context.DrawLine(gridPen, new Avalonia.Point(padding, yPos), new Avalonia.Point(padding + mapCols * cellWidth, yPos));
            }
            for (int c = 0; c < mapCols; c++)
            {
                double xPos = padding + c * cellWidth;
                context.DrawLine(gridPen, new Avalonia.Point(xPos, padding), new Avalonia.Point(xPos, padding + mapRows * cellHeight));
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

                    double cellLeft = padding + x * cellWidth;
                    double cellTop = padding + y * cellHeight;
                    double cellRight = cellLeft + cellWidth;
                    double cellBottom = cellTop + cellHeight;

                    // North boundary: cell (y, x) [Side A] vs cell (y - 1, x) [Side B]
                    if (mi.wall_type_dir_0 != 0)
                    {
                        bool neighborExplored = (y > 0) && MapTracker.IsExplored(gameArea, blockId, x, y - 1);
                        if (currentExplored || neighborExplored)
                        {
                            DrawBoundary(context, cellLeft, cellTop, cellRight, cellTop, mi.x3_dir_0, cellWidth, cellHeight, true, neighborExplored, currentExplored);
                        }
                    }
                    // East boundary: cell (y, x) [Side A] vs cell (y, x + 1) [Side B]
                    if (mi.wall_type_dir_2 != 0)
                    {
                        bool neighborExplored = (x < mapCols - 1) && MapTracker.IsExplored(gameArea, blockId, x + 1, y);
                        if (currentExplored || neighborExplored)
                        {
                            DrawBoundary(context, cellRight, cellTop, cellRight, cellBottom, mi.x3_dir_2, cellWidth, cellHeight, false, neighborExplored, currentExplored);
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
            // Draw small corner squares where two walls meet
            var cornerPen = new Pen(new SolidColorBrush(Color.Parse("#4a371c")), 1.75);
            double cornerSize = 1.75; // match wall thickness
            for (int y = 0; y < mapRows; y++)
            {
                for (int x = 0; x < mapCols; x++)
                {
                    MapInfo mi = gbl.geo_ptr.maps[y, x];
                    if (mi == null) continue;
                    bool explored = MapTracker.IsExplored(gameArea, blockId, x, y);
                    // Determine wall presence with exploration check
                    bool hasNorth = mi.wall_type_dir_0 != 0 && (explored || (y > 0 && MapTracker.IsExplored(gameArea, blockId, x, y - 1)));
                    bool hasEast = mi.wall_type_dir_2 != 0 && (explored || (x < mapCols - 1 && MapTracker.IsExplored(gameArea, blockId, x + 1, y)));
                    bool hasSouth = mi.wall_type_dir_4 != 0 && (explored || (y < mapRows - 1 && MapTracker.IsExplored(gameArea, blockId, x, y + 1)));
                    bool hasWest = mi.wall_type_dir_6 != 0 && (explored || (x > 0 && MapTracker.IsExplored(gameArea, blockId, x - 1, y)));
                    double cellLeft = padding + x * cellWidth;
                    double cellTop = padding + y * cellHeight;
                    // Top‑left corner (north + west)
                    if (hasNorth && hasWest)
                    {
                        var rect = new Rect(cellLeft - cornerSize / 2, cellTop - cornerSize / 2, cornerSize, cornerSize);
                        context.DrawRectangle(null, cornerPen, rect);
                    }
                    // Top‑right corner (north + east)
                    if (hasNorth && hasEast)
                    {
                        var rect = new Rect(cellLeft + cellWidth - cornerSize / 2, cellTop - cornerSize / 2, cornerSize, cornerSize);
                        context.DrawRectangle(null, cornerPen, rect);
                    }
                    // Bottom‑left corner (south + west)
                    if (hasSouth && hasWest)
                    {
                        var rect = new Rect(cellLeft - cornerSize / 2, cellTop + cellHeight - cornerSize / 2, cornerSize, cornerSize);
                        context.DrawRectangle(null, cornerPen, rect);
                    }
                    // Bottom‑right corner (south + east)
                    if (hasSouth && hasEast)
                    {
                        var rect = new Rect(cellLeft + cellWidth - cornerSize / 2, cellTop + cellHeight - cornerSize / 2, cornerSize, cornerSize);
                        context.DrawRectangle(null, cornerPen, rect);
                    }
                }
            }

            // Draw player position and orientation as a green triangle
            int px = gbl.mapPosX;
            int py = gbl.mapPosY;
            if (px >= 0 && px < mapCols && py >= 0 && py < mapRows)
            {
                double centerLeft = padding + px * cellWidth + cellWidth / 2;
                double centerTop = padding + py * cellHeight + cellHeight / 2;

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

            // Draw event numbers if Debug option is checked
            if (ShowEventNumbers)
            {
                var eventBrush = new SolidColorBrush(Colors.Red);
                var textTypeface = new Typeface("Arial", FontStyle.Normal, FontWeight.Bold);

                for (int y = 0; y < mapRows; y++)
                {
                    for (int x = 0; x < mapCols; x++)
                    {
                        MapInfo mi = gbl.geo_ptr.maps[y, x];
                        if (mi == null) continue;
                        int eventNum = mi.x2 & 0x7F;
                        if (eventNum == 0) continue;

                        string textVal = eventNum.ToString();
                        var formattedText = new FormattedText(
                            textVal,
                            System.Globalization.CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            textTypeface,
                            Math.Max(10.0, cellHeight * 0.45),
                            eventBrush);

                        double cellLeft = padding + x * cellWidth;
                        double cellTop = padding + y * cellHeight;

                        // Center the text slightly to the top-left or centered inside the square
                        double tx = cellLeft + (cellWidth - formattedText.Width) / 2;
                        double ty = cellTop + (cellHeight - formattedText.Height) / 2;

                        context.DrawText(formattedText, new Avalonia.Point(tx, ty));
                    }
                }
            }
        }

        private void DrawBoundary(DrawingContext context, double x1, double y1, double x2, double y2, byte doorFlag, double cellWidth, double cellHeight, bool isHorizontal, bool sideAExplored, bool sideBExplored)
        {
            var wallPen = new Pen(new SolidColorBrush(Color.Parse("#4a371c")), 3.5);
            var doorBrush = new SolidColorBrush(Colors.White);
            var doorPen = new Pen(new SolidColorBrush(Color.Parse("#4a371c")), 1.0);
            var lockedDoorBrush = new SolidColorBrush(Color.Parse("#e65100")); // Orange
            var secretDoorPen = new Pen(new SolidColorBrush(Color.Parse("#b71c1c")), 2.0, dashStyle: DashStyle.Dash); // Dashed red

            if (doorFlag == 0) // Normal wall
            {
                context.DrawLine(wallPen, new Avalonia.Point(x1, y1), new Avalonia.Point(x2, y2));
            }
            else if (doorFlag == 1 || doorFlag == 2) // Door or Locked Door
            {
                // Draw wall line first
                context.DrawLine(wallPen, new Avalonia.Point(x1, y1), new Avalonia.Point(x2, y2));
                double midX = (x1 + x2) / 2;
                double midY = (y1 + y2) / 2;

                double doorWidth = isHorizontal ? cellWidth * 0.45 : cellWidth * 0.24;
                double doorHeight = isHorizontal ? cellHeight * 0.24 : cellHeight * 0.45;

                var brush = (doorFlag == 2) ? lockedDoorBrush : doorBrush;

                if (isHorizontal)
                {
                    // Side A is below (positive Y), Side B is above (negative Y)
                    if (sideAExplored && sideBExplored)
                    {
                        // Draw top and bottom half
                        Rect r = new Rect(midX - doorWidth / 2, midY - doorHeight / 2, doorWidth, doorHeight);
                        context.DrawRectangle(brush, doorPen, r);
                    }
                    else if (sideAExplored)
                    {
                        // Draw top half
                        Rect r = new Rect(midX - doorWidth / 2, midY - doorHeight / 2, doorWidth, (doorHeight - 3.5) / 2);
                        context.DrawRectangle(brush, doorPen, r);
                    }
                    else if (sideBExplored)
                    {
                        // Draw bottom half
                        Rect r = new Rect(midX - doorWidth / 2, midY + (3.5 / 2), doorWidth, (doorHeight - 3.5) / 2);
                        context.DrawRectangle(brush, doorPen, r);
                    }
                }
                else
                {
                    // Size A is to the right (positive X), Size B is to the left (negative X)
                    if (sideAExplored && sideBExplored)
                    {
                        // Draw left and right half
                        Rect r = new Rect(midX - doorWidth / 2, midY - doorHeight / 2, doorWidth, doorHeight);
                        context.DrawRectangle(brush, doorPen, r);
                    }
                    else if (sideAExplored)
                    {
                        // Draw right half
                        Rect r = new Rect(midX + (3.5/2), midY - doorHeight / 2, (doorWidth - 3.5)/ 2, doorHeight);
                        context.DrawRectangle(brush, doorPen, r);
                    }
                    else if (sideBExplored)
                    {
                        // Draw left half
                        Rect r = new Rect(midX - doorWidth / 2, midY - (doorHeight + 3.5)/ 2, (doorWidth - 3.5)/ 2, doorHeight);
                        context.DrawRectangle(brush, doorPen, r);
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
