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
        public static int[,] tileTypes = new int[4, 9];
        public static int[,] resourceTiles = new int[4, 9];
        public static Base[,] baseTiles = new Base[4, 9];
        public static Unit[,] unitTiles = new Unit[4, 9];
        public static Mine[,] mines = new Mine[4, 9];
        private static List<PointF> Hexagons = new List<PointF>();
        public static readonly Dictionary<int, string> tiles = new Dictionary<int, string> 
        { 
            { 0, "Plain" },
            { 1, "Forest" },
            { 2, "Mountain" },
            { 3, "Sea" },
            { 4, "Desert" },
            { 5, "Swamp" },
            { 6, "River" },
        };
        Random rnd = new Random();
        public void CreateField()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tileTypes[i, j] = rnd.Next(0, 7);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                int row = rnd.Next(0, 4);
                int col = rnd.Next(0, 9);
                if (baseTiles[row, col] == null && resourceTiles[row, col] == 0) resourceTiles[row, col] = 1;
                else i -= 1;
            }
            for (int i = 0; i < 3; i++)
            {
                int row = rnd.Next(0, 4);
                int col = rnd.Next(0, 9);
                if (baseTiles[row, col] == null && resourceTiles[row, col] == 0) resourceTiles[row, col] = 2;
                else i -= 1;
            }
        }
        public static void FieldClick(MouseEventArgs e, PictureBox pb, Player player1, Player player2)
        {
            PointToHex(e.X, e.Y, out int row, out int col);
            try
            {
                if (player1.playerBases[row, col] != null && player1.playerUnits[row, col] != null)
                {
                    player1.playerUnits[row, col].OnClick(player1.playerUnits[row, col], pb, e, tiles[tileTypes[row, col]], player1, player2);
                }
                else if (player1.playerBases[row, col] != null) player1.playerBases[row, col].OnClick(new System.Drawing.Point(row, col), player1);
                else
                {
                    if (player1.playerUnits[row, col] != null)
                    {
                        player1.playerUnits[row, col].OnClick(player1.playerUnits[row, col], pb, e, tiles[tileTypes[row, col]], player1, player2);
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
                case "Mine":
                    mines[row, col] = new Mine();
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
                    if (mines[row, col] != null) DrawImageInPolygon(gr, HexToPoints(row, col), Image.FromFile("mine.png"));
                    if (player.playerBases[row, col] != null) DrawImageInPolygon(gr, HexToPoints(row, col), Image.FromFile($"base{player.color}.png"));
                    if (player.playerUnits[row, col] != null) DrawImageInPolygon(gr, HexToPoints(row, col), Image.FromFile($"{player.playerUnits[row, col].GetType().Name}{player.color}.png"));
                }
            }
        }
        private static float HexWidth()
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }
        public static PointF[] HexToPoints(float row, float col)
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
                    gr.FillPolygon(Brushes.LightGreen, HexToPoints(row, col));
                    break;
                case 1:
                    gr.FillPolygon(Brushes.Green, HexToPoints(row, col));
                    break;
                case 2:
                    gr.FillPolygon(Brushes.Gray, HexToPoints(row, col));
                    break;
                case 3:
                    gr.FillPolygon(Brushes.DarkBlue, HexToPoints(row, col));
                    break;
                case 4:
                    gr.FillPolygon(Brushes.Yellow, HexToPoints(row, col));
                    break;
                case 5:
                    gr.FillPolygon(Brushes.SaddleBrown, HexToPoints(row, col));
                    break;
                case 6:
                    gr.FillPolygon(Brushes.Cyan, HexToPoints(row, col));
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
        public static int HexDistance(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            int ax = p1.X - Floor2(p1.Y);
            int ay = p1.X + Ceil2(p1.Y);
            int bx = p2.X - Floor2(p2.Y);
            int by = p2.X + Ceil2(p2.Y);
            int dx = bx - ax;
            int dy = by - ay;
            if (Math.Sign(dx) == Math.Sign(dy))
            {
                return Math.Max(Math.Abs(dx), Math.Abs(dy));
            }
            return Math.Abs(dx) + Math.Abs(dy);
        }

        private static int Floor2(int x)
        {
            return ((x >= 0) ? (x >> 1) : (x - 1) / 2);
        }
        private static int Ceil2(int x)
        {
            return ((x >= 0) ? ((x + 1) >> 1) : x / 2);
        }
        public static bool WinCheck(Player player, Player otherPlayer)
        {
            bool win = true;
            foreach (Base playerBase in player.playerBases)
            {
                if (playerBase != null) win = false;
            }
            return win;
        }
        public static void ShowPath(int move, int row, int col, Graphics gr)
        {
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (HexDistance(new System.Drawing.Point(row, col), new System.Drawing.Point(i, j)) <= move)
                    {
                        PointF[] points = HexToPoints(i, j);
                        gr.DrawPolygon(new Pen(Color.Blue, 3), points);
                    }
                }
            }
        }

    }
}
