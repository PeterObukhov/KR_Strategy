using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Field
    {
        //Высота шестиугольника
        private static readonly float height = 80;
        //Массив типов клеток
        public static int[,] tileTypes = new int[4, 9];
        //Массив клеток с ресурсами
        public static int[,] resourceTiles = new int[4, 9];
        //Массив баз обоих игроков
        public static Base[,] baseTiles = new Base[4, 9];
        //Массив юнитов обоих игроков
        public static Unit[,] unitTiles = new Unit[4, 9];
        //Массив мин
        public static Mine[,] mines = new Mine[4, 9];
        //Список шестиугольников
        private static List<PointF> Hexagons = new List<PointF>();
        //Словарь идентификаторов клеток и их названий
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
        //Заполнение поля случайными клетками и расстановка ресурсов на них
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
        //Обработчик события нажатия на карту
        public static void FieldClick(MouseEventArgs e, PictureBox pb, Player player1, Player player2)
        {
            PointToHex(e.X, e.Y, out int row, out int col);
            try
            {
                //Если на выбранной клетке одновременно находится юнит и база, обработка нажатия на юнит
                if (player1.playerBases[row, col] != null && player1.playerUnits[row, col] != null)
                {
                    player1.playerUnits[row, col].OnClick(player1.playerUnits[row, col], pb, e, tiles[tileTypes[row, col]], player1, player2);
                }
                //Если на выбранной клетке находится база, обработка нажатия на базу
                else if (player1.playerBases[row, col] != null) player1.playerBases[row, col].OnClick(new System.Drawing.Point(row, col), player1);
                //Если на выбранной клетке находится юнит, обработка нажатия на юнит
                else
                {
                    //Если юнит принадлежит текущему игроку
                    if (player1.playerUnits[row, col] != null)
                    {
                        player1.playerUnits[row, col].OnClick(player1.playerUnits[row, col], pb, e, tiles[tileTypes[row, col]], player1, player2);
                    }
                }
            }
            catch { }
        }
        //Добавление юнита, базы или мины на карту
        public static void SetUnit(Unit unit, int row, int col, Player player)
        {
            switch (unit.GetType().Name)
            {
                //Добавление базы
                case "Base":
                    if(baseTiles[row, col] == null)
                    {
                        Base nb = new Base();
                        //Если у текущего игрока достаточно ресурсов для создания базы
                        if (player.gasAmount >= nb.costGas && player.mineralsAmount >= nb.costMinerals)
                        {
                            //Добавление базы в массив баз игрока
                            player.playerBases[row, col] = nb;
                            //Добавление базы в общий массив
                            baseTiles[row, col] = nb;
                            player.gasAmount -= nb.costGas;
                            player.mineralsAmount -= nb.costMinerals;
                        }
                        else MessageBox.Show("Недостаточно ресурсов!");
                    }
                    break;
                //Добавление мины
                case "Mine":
                    mines[row, col] = new Mine();
                    break;
                //Добавление юнита
                default:
                    unitTiles[row, col] = unit;
                    //Добавление юнита в массив юнитов игрока
                    player.playerUnits[row, col] = unit;
                    break;
            }
        }
        //Отображение юнитов, баз и мин на карте
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
        //Подсчет ширины шестиугольника
        private static float HexWidth()
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }
        //Получение массива точек, образующих шестиугольник на выбранной позиции
        public static PointF[] HexToPoints(float row, float col)
        {
            //Начать с верхнего левого угла
            float width = HexWidth();
            float y = height / 2;
            float x = 0;

            //Сдвинуться вниз на требуемое количество рядов
            y += row * height;

            //Если столбец нечетный, сдвинуться вниз еще на половину шестиугольника
            if (col % 2 == 1) y += height / 2;

            //Сдвинуться на требуемое количество столбцов
            x += col * (width * 0.75f);

            //Генерация точек
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
        //Отображение различных типов клетов и ресурсов на них
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
        //Отображение шестиугольников
        public static void DrawHexGrid(Graphics gr, Pen pen, float xmax, float ymax)
        {
            for (int row = 0; row < 4; row++)
            {
                //Получить точки для первого гексагона
                PointF[] points = HexToPoints(row, 0);

                //Если новый гексагон не влезает, остановить отрисовку
                if (points[4].Y > ymax) break;

                //Отобразить строку
                for (int col = 0; col < 10; col++)
                {
                    //Получить точки для следующего гексагона в строке
                    points = HexToPoints(row, col);

                    //Если не влезает горизонтально, перейти к следующей строке
                    if (points[3].X > xmax) break;

                    //Если влезает вертикально, отрисовать гексагон
                    if (points[4].Y <= ymax)
                    {
                        DrawTile(row, col, gr);
                        gr.DrawPolygon(pen, points);
                        Hexagons.Add(new PointF(row, col));
                    }
                }
            }
        }
        //Отобразить изображение в клетке
        private static void DrawImageInPolygon(Graphics gr, PointF[] points, Image image)
        {
            float wid = HexWidth();
            float hgt = height;
            float cx = (points[0].X + points[3].X) / 2f;
            float cy = (points[5].Y + points[1].Y) / 2f;

            //Вычислить масштаб, чтобы заполнить клетку выбранным изображением
            float xscale = wid / image.Width;
            float yscale = hgt / image.Height;
            float scale = Math.Max(xscale, yscale);

            //Вычислить размер изображения после масштабирования
            float imgWidth = image.Width * scale;
            float imgHeight = image.Height * scale;
            float rx = imgWidth / 2f;
            float ry = imgHeight / 2f;

            //Найти нужный гексагон и точки назначения
            RectangleF src_rect = new RectangleF(0, 0, image.Width, image.Height);
            PointF[] dest_points =
            {
                new PointF(cx - rx,  cy - ry),
                new PointF(cx + rx,  cy - ry),
                new PointF(cx - rx,  cy + ry),
            };

            //Отобразить изображение в выбранном шестиугольнике
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(points);
            GraphicsState state = gr.Save();
            gr.DrawImage(image, dest_points, src_rect, GraphicsUnit.Pixel);
            gr.Restore(state);
        }
        //Получить координаты гексагона, к которому принадлежит точка
        public static void PointToHex(float x, float y, out int row, out int col)
        {
            //Найти тестовый шестиугольник, к которому принадлежит точка
            float width = HexWidth();
            col = (int)(x / (width * 0.75f));

            if (col % 2 == 0)
                row = (int)Math.Floor(y / height);
            else
                row = (int)Math.Floor((y - height / 2) / height);

            //Найти тестовую зону
            float testx = col * width * 0.75f;
            float testy = row * height;
            if (col % 2 != 0) testy += height / 2;

            // Проверить, если точка выше или ниже шестиугольника слева
            bool is_above = false, is_below = false;
            float dx = x - testx;
            if (dx < width / 4)
            {
                float dy = y - (testy + height / 2);
                if (dx < 0.001)
                {
                    //Точка на левой грани тестового шестиугольника
                    if (dy < 0) is_above = true;
                    if (dy > 0) is_below = true;
                }
                else if (dy < 0)
                {
                    //Точка выше тестового шестиугольника
                    if (-dy / dx > Math.Sqrt(3)) is_above = true;
                }
                else
                {
                    //Точка ниже тестового шестиугольника
                    if (dy / dx > Math.Sqrt(3)) is_below = true;
                }
            }

            //Подстроить ряд и строку при необходимости
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
        //Алгоритм подсчета расстояния между двумя выбранными шестиугольниками
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

        //Проверка победы
        public static bool WinCheck(Player player)
        {
            bool win = true;
            //Если у игрока нет баз, он проиграл
            foreach (Base playerBase in player.playerBases)
            {
                if (playerBase != null) win = false;
            }
            return win;
        }

        //Отобразить радиус перемещения выбранного юнита из текущей клетки
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
