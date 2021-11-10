using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Field
    {
        private static readonly float height = 80;
        private static int[,] tileTypes = new int[4, 9];
        public static int[,] resourceTiles = new int[4, 9];
        public static Base[,] baseTiles = new Base[4, 9];
        public static Unit[,] unitTiles = new Unit[4, 9];
        private static List<PointF> Hexagons = new List<PointF>();
        Random rnd = new Random();
        public void CreateField()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tileTypes[i, j] = rnd.Next(0, 5);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                int row = rnd.Next(0, 4);
                int col = rnd.Next(0, 8);
                if (baseTiles[row, col] == null && resourceTiles[row, col] == 0) resourceTiles[row, col] = 1;
                else i -= 1;
            }
            for (int i = 0; i < 3; i++)
            {
                int row = rnd.Next(0, 4);
                int col = rnd.Next(0, 8);
                if (baseTiles[row, col] == null && resourceTiles[row, col] == 0) resourceTiles[row, col] = 2;
                else i -= 1;
            }
        }
        public static void FieldClick(MouseEventArgs e, PictureBox pb, Player player1, Field field, Player player2)
        {
            PointToHex(e.X, e.Y, out int row, out int col);
            try
            {
                if (player1.playerBases[row, col] != null && player1.playerUnits[row, col] != null)
                {
                    player1.playerUnits[row, col].OnClick(player1.playerUnits[row, col], pb, e, "Flat", player1, player2);
                }
                else if (player1.playerBases[row, col] != null) player1.playerBases[row, col].OnClick(new System.Drawing.Point(row, col), player1);
                else
                {
                    if (player1.playerUnits[row, col] != null)
                    {
                        player1.playerUnits[row, col].OnClick(player1.playerUnits[row, col], pb, e, "Flat", player1, player2);
                    }
                }
            }
            catch { }
        }
        public static void SetUnit(Unit unit, int row, int col, Player player)
        {
            switch (unit.GetType().Name)
            {
                case "Base":
                    if(baseTiles[row, col] == null)
                    {
                        Base nb = new Base();
                        if (player.gasAmount >= nb.costGas && player.mineralsAmount >= nb.costMinerals)
                        {
                            player.playerBases[row, col] = nb;
                            baseTiles[row, col] = nb;
                            player.gasAmount -= nb.costGas;
                            player.mineralsAmount -= nb.costMinerals;
                        }
                        else MessageBox.Show("Недостаточно ресурсов!");
                    }
                    break;
                default:
                    unitTiles[row, col] = unit;
                    player.playerUnits[row, col] = unit;
                    break;
            }
        }
        public static void DrawUnits(Graphics gr, Player player)
        {
            for(int row = 0; row < 4; row++)
            {
                for(int col = 0; col < 9; col++)
                {
                    if(player.playerBases[row, col] != null) DrawImageInPolygon(gr, HexToPoints(row, col), Image.FromFile($"base{player.color}.png"));
                    if (player.playerUnits[row, col] != null)
                    {
                        switch (player.playerUnits[row, col].GetType().Name)
                        {
                            case "Fighter":
                                DrawImageInPolygon(gr, HexToPoints(row, col), Image.FromFile($"fighter{player.color}.png"));
                                break;
                        }
                    }
                }
            }
        }
        private static float HexWidth()
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }
        private static PointF[] HexToPoints(float row, float col)
        {
            // Start with the leftmost corner of the upper left hexagon.
            float width = HexWidth();
            float y = height / 2;
            float x = 0;

            // Move down the required number of rows.
            y += row * height;

            // If the column is odd, move down half a hex more.
            if (col % 2 == 1) y += height / 2;

            // Move over for the column number.
            x += col * (width * 0.75f);

            // Generate the points.
            return new PointF[]
            {
                new PointF(x, y),
                new PointF(x + width * 0.25f, y - height / 2),
                new PointF(x + width * 0.75f, y - height / 2),
                new PointF(x + width, y),
                new PointF(x + width * 0.75f, y + height / 2),
                new PointF(x + width * 0.25f, y + height / 2),
            };
        }
        private static void DrawTile(int row, int col, Graphics gr)
        {
            switch(tileTypes[row, col])
            {
                case 0:
                    gr.FillPolygon(Brushes.LightBlue, HexToPoints(row, col));
                    break;
                case 1:
                    gr.FillPolygon(Brushes.Red, HexToPoints(row, col));
                    break;
                case 2:
                    gr.FillPolygon(Brushes.Green, HexToPoints(row, col));
                    break;
                case 3:
                    gr.FillPolygon(Brushes.Yellow, HexToPoints(row, col));
                    break;
                case 4:
                    gr.FillPolygon(Brushes.Orange, HexToPoints(row, col));
                    break;
            }
            switch (resourceTiles[row, col])
            {
                case 1:
                    DrawImageInPolygon(gr, HexToPoints(row, col), Image.FromFile("gas.png"));
                    break;
                case 2:
                    DrawImageInPolygon(gr, HexToPoints(row, col), Image.FromFile("mineral.png"));
                    break;
            }
        }
        public static void DrawHexGrid(Graphics gr, Pen pen, float xmin, float xmax, float ymin, float ymax, float height)
        {
            // Loop until a hexagon won't fit.
            for (int row = 0; row < 4; row++)
            {
                // Get the points for the row's first hexagon.
                PointF[] points = HexToPoints(row, 0);

                // If it doesn't fit, we're done.
                if (points[4].Y > ymax) break;

                // Draw the row.
                for (int col = 0; col < 10; col++)
                {
                    // Get the points for the row's next hexagon.
                    points = HexToPoints(row, col);

                    // If it doesn't fit horizontally,
                    // we're done with this row.
                    if (points[3].X > xmax) break;

                    // If it fits vertically, draw it.
                    if (points[4].Y <= ymax)
                    {
                        DrawTile(row, col, gr);
                        gr.DrawPolygon(pen, points);
                        Hexagons.Add(new PointF(row, col));
                    }
                }
            }
        }
        private static void DrawImageInPolygon(Graphics gr, PointF[] points, Image image)
        {
            float wid = HexWidth();
            float hgt = height;
            float cx = (points[0].X + points[3].X) / 2f;
            float cy = (points[5].Y + points[1].Y) / 2f;

            // Calculate the scale needed to make
            // the image fill the polygon's bounds.
            float xscale = wid / image.Width;
            float yscale = hgt / image.Height;
            float scale = Math.Max(xscale, yscale);

            // Calculate the image's scaled size.
            float imgWidth = image.Width * scale;
            float imgHeight = image.Height * scale;
            float rx = imgWidth / 2f;
            float ry = imgHeight / 2f;

            // Find the source rectangle and destination points.
            RectangleF src_rect = new RectangleF(0, 0, image.Width, image.Height);
            PointF[] dest_points =
            {
                new PointF(cx - rx,  cy - ry),
                new PointF(cx + rx,  cy - ry),
                new PointF(cx - rx,  cy + ry),
            };

            // Clip the drawing area to the polygon and draw the image.
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(points);
            GraphicsState state = gr.Save();
            //gr.SetClip(path);   // Comment out to not clip.
            gr.DrawImage(image, dest_points, src_rect, GraphicsUnit.Pixel);
            gr.Restore(state);
        }
        public static void PointToHex(float x, float y, out int row, out int col)
        {
            // Find the test rectangle containing the point.
            float width = HexWidth();
            col = (int)(x / (width * 0.75f));

            if (col % 2 == 0)
                row = (int)Math.Floor(y / height);
            else
                row = (int)Math.Floor((y - height / 2) / height);

            // Find the test area.
            float testx = col * width * 0.75f;
            float testy = row * height;
            if (col % 2 != 0) testy += height / 2;

            // See if the point is above or
            // below the test hexagon on the left.
            bool is_above = false, is_below = false;
            float dx = x - testx;
            if (dx < width / 4)
            {
                float dy = y - (testy + height / 2);
                if (dx < 0.001)
                {
                    // The point is on the left edge of the test rectangle.
                    if (dy < 0) is_above = true;
                    if (dy > 0) is_below = true;
                }
                else if (dy < 0)
                {
                    // See if the point is above the test hexagon.
                    if (-dy / dx > Math.Sqrt(3)) is_above = true;
                }
                else
                {
                    // See if the point is below the test hexagon.
                    if (dy / dx > Math.Sqrt(3)) is_below = true;
                }
            }

            // Adjust the row and column if necessary.
            if (is_above)
            {
                if (col % 2 == 0) row--;
                col--;
            }
            else if (is_below)
            {
                if (col % 2 != 0) row++;
                col--;
            }
        }
        public static int GetDistance(System.Drawing.Point start, System.Drawing.Point end)
        {
            PointToHex(start.X, start.Y, out int startRow, out int startCol);
            PointToHex(end.X, end.Y, out int endRow, out int endCol);
            Vector vect = new Vector(endCol - startCol, endRow - startRow);
            return (int)vect.Length;
        }
        public static void WinCheck(Player player1, Player otherPlayer)
        {
            bool win = true;
            foreach (Base playerBase in player1.playerBases)
            {
                if (playerBase != null) win = false;
            }
            if (win) MessageBox.Show($"{otherPlayer.name} победил!");
        }
        public static void ShowPath(int move, int row, int col, Graphics gr)
        {
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Vector vect = new Vector(row - i, col - j);
                    int a = (int)vect.Length;
                    if (a <= move)
                    {
                        PointF[] points = HexToPoints(i, j);
                        gr.DrawPolygon(new Pen(Color.LightBlue, 3), points);
                    }
                }
            }
        }
    }
}
