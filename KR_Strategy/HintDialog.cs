using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    public partial class HintDialog : Form
    {
        public HintDialog()
        {
            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            for(int i = 0; i < 9; i++)
            {
                Hint.DrawTile(i, graphics);
            }
        }
    }
    class Hint
    {
        private static readonly int height = 45;
        private static readonly int[] tileTypes = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        public static void DrawTile(int num, Graphics gr)
        {
            switch (tileTypes[num])
            {
                case 0:
                    gr.FillPolygon(Brushes.LightGreen, HexToPoints(num, 0));
                    break;
                case 1:
                    gr.FillPolygon(Brushes.Green, HexToPoints(num, 0));
                    break;
                case 2:
                    gr.FillPolygon(Brushes.Gray, HexToPoints(num, 0));
                    break;
                case 3:
                    gr.FillPolygon(Brushes.DarkBlue, HexToPoints(num, 0));
                    break;
                case 4:
                    gr.FillPolygon(Brushes.Yellow, HexToPoints(num, 0));
                    break;
                case 5:
                    gr.FillPolygon(Brushes.SaddleBrown, HexToPoints(num, 0));
                    break;
                case 6:
                    gr.FillPolygon(Brushes.Cyan, HexToPoints(num, 0));
                    break;
                case 7:
                    DrawImageInPolygon(gr, HexToPoints(num, 0), Image.FromFile("gas.png"));
                    break;
                case 8:
                    DrawImageInPolygon(gr, HexToPoints(num, 0), Image.FromFile("mineral.png"));
                    break;
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
    }
}
