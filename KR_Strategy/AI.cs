using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    class AI : Player
    {
        //Параметр агрессиваности бота, в зависимости от сложности
        public static int aggressiveness;
        public AI(string name, string color, int _aggressiveness) : base(name, color)
        {
            this.name = name;
            this.color = color;
            aggressiveness = _aggressiveness;
        }
        //Получение списка координат клеток, доступных юниту для перемещения
        static public List<Point> FindCells(int move, int row, int col)
        {
            List<Point> avaliablePoints = new List<Point>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Field.HexDistance(new System.Drawing.Point(row, col), new System.Drawing.Point(i, j)) <= move)
                    {
                        if (Field.unitTiles[i, j] == null && Field.baseTiles[i, j] == null && Field.tiles[Field.tileTypes[i, j]] != "Sea") avaliablePoints.Add(new Point(i, j));
                    }
                }
            }
            return avaliablePoints;
        }

        //Получение списка юнитов или баз, доступных для атаки, для выбранного юнита
        static public List<Point> FindEnemies(Player ai, int row, int col)
        {
            List<Point> enemyUnits = new List<Point>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Field.HexDistance(new System.Drawing.Point(row, col), new System.Drawing.Point(i, j)) == 1)
                    {
                        //Если юнит или база принадлежит врагу
                        if ((Field.unitTiles[i, j] != null || Field.baseTiles[i, j] != null) && ai.playerBases[i, j] == null && ai.playerUnits[i, j] == null) enemyUnits.Add(new Point(i, j));
                    }
                }
            }
            return enemyUnits;
        }

        //Перемещение юнита
        static void MoveUnits(Player ai, int i, int j)
        {
            string tile = Field.tiles[Field.tileTypes[i, j]];
            //Если на клетке присутсвует юнит, его перемещение не равно нулю, и он не двигался
            if (ai.playerUnits[i, j] != null && ai.playerUnits[i, j].CalcMove(tile) != 0 && ai.playerUnits[i, j].hasMoved == false)
            {
                Random rnd = new Random();
                List<Point> choice = FindCells(ai.playerUnits[i, j].CalcMove(tile), i, j);
                //Если есть клетки, доступные для перемещения
                if (choice.Count != 0)
                {
                    Point temp = choice[rnd.Next(0, choice.Count)];
                    while (new Point(i, j) == temp)
                    {
                        temp = choice[rnd.Next(0, choice.Count)];
                    }
                    ai.playerUnits[temp.X, temp.Y] = ai.playerUnits[i, j];
                    Field.unitTiles[temp.X, temp.Y] = ai.playerUnits[i, j];
                    ai.playerUnits[i, j] = null;
                    Field.unitTiles[i, j] = null;
                    ai.playerUnits[temp.X, temp.Y].hasMoved = true;
                    //Если наземный юнит переместился на морскую клетку, он утонул
                    if (Field.tiles[Field.tileTypes[temp.X, temp.Y]] == "Sea" &&
                        (ai.playerUnits[temp.X, temp.Y].GetType().Name != "Ship" ||
                        ai.playerUnits[temp.X, temp.Y].GetType().Name != "Boat" ||
                        ai.playerUnits[temp.X, temp.Y].GetType().Name != "Fighter" ||
                        ai.playerUnits[temp.X, temp.Y].GetType().Name != "Cruiser" ||
                        ai.playerUnits[temp.X, temp.Y].GetType().Name != "Drone"))
                    {
                        ai.playerUnits[temp.X, temp.Y] = null;
                    }
                }
            }
        }
        //Установка базы
        static void SetBase(Player ai, int i, int j)
        {
            if (ai.playerUnits[i, j] != null && ai.playerUnits[i, j].hasActed == false)
            {
                if (Field.resourceTiles[i, j] == 0 && Field.baseTiles[i, j] == null)
                {
                    Field.SetUnit(new Base(), i, j, ai);
                    ai.playerUnits[i, j].hasActed = true;
                }
            }
        }
        //Добыча ресурсов
        static void Dig(Player ai, int i, int j)
        {
            if (ai.playerUnits[i, j] != null && ai.playerUnits[i, j].hasActed == false)
            {

                if (Field.resourceTiles[i, j] == 1)
                {
                    ai.gasAmount += 100;
                    ai.playerUnits[i, j].hasActed = true;
                }
                else if (Field.resourceTiles[i, j] == 2)
                {
                    ai.mineralsAmount += 100;
                    ai.playerUnits[i, j].hasActed = true;
                }
            }
        }
        //Атака
        static void Attack(Player ai, int i, int j, string tile, Player defender, PictureBox pictureBox1)
        {
            if (ai.playerUnits[i, j] != null && ai.playerUnits[i, j].hasActed == false)
            {
                List<Point> enemies = FindEnemies(ai, i, j);
                Random rnd = new Random();
                //Выбор случайного противнивника из доступных для атаки
                int choice = rnd.Next(0, enemies.Count);
                Unit target = Field.unitTiles[enemies[choice].X, enemies[choice].Y];
                //Если на выбранной клетке нет юнита, то там стоит база
                if (target == null) target = Field.baseTiles[enemies[choice].X, enemies[choice].Y];
                //Осуществление атаки
                ai.playerUnits[i, j].Attack(target, tile);
                if (target.health <= 0)
                {
                    if(target.GetType().Name != "Base")
                    {
                        Field.unitTiles[enemies[choice].X, enemies[choice].Y] = null;
                        defender.playerUnits[enemies[choice].X, enemies[choice].Y] = null;
                    }
                    else
                    {
                        Field.baseTiles[enemies[choice].X, enemies[choice].Y] = null;
                        defender.playerBases[enemies[choice].X, enemies[choice].Y] = null;
                    }
                    pictureBox1.Invalidate();
                }
                ai.playerUnits[i, j].hasActed = true;
            }
        }
        //Выбор юнита для создания на базе
        static Unit ChooseUnit(Player ai, int row, int col)
        {
            //Массив наземных и летающих юнитов
            Unit[] ground = new Unit[] { new Fighter(), new Cruiser(), new Drone(), new Tank(), new Car(), new RocketLauncher(), new Infantry() };
            //Массив водных и летающих юнитов
            Unit[] water = new Unit[] { new Fighter(), new Cruiser(), new Drone(), new Boat(), new Ship() };
            List<Unit> allowedUnits = new List<Unit>();
            Random rnd = new Random();
            string tile = Field.tiles[Field.tileTypes[row, col]];
            if (tile == "Sea")
            {
                //Если база находится в море, выбор юнита, который может перемещаться по морю
                for (int i = 0; i < 5; i++)
                {
                    Unit pick = water[i];
                    if (ai.gasAmount >= pick.costGas && ai.mineralsAmount >= pick.costMinerals) allowedUnits.Add(pick);
                }
            }
            else
            {
                //Если база находится на суше, выбор юнита, который может перемещаться по суше
                for (int i = 0; i < 7; i++)
                {
                    Unit pick = ground[i];
                    if (ai.gasAmount >= pick.costGas && ai.mineralsAmount >= pick.costMinerals) allowedUnits.Add(pick);
                }
            }
            return allowedUnits[rnd.Next(0, allowedUnits.Count)];
        }
        //Метод совершения хода ботом
        public static void AiTurn(Player ai, Player otherPlayer, PictureBox pictureBox1)
        {
            Random rnd = new Random();
            //Перебор всего поля в поисках юнитов, принадлежащих боту
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    //Если на выбранной клетке находится юнит, совершение одного из действий
                    if (ai.playerUnits[row, col] != null)
                    {
                        float moveChance = 0;
                        //Если юниту есть куда переместиться, генерация шанса на перемещение
                        if(FindCells(ai.playerUnits[row, col].CalcMove(Field.tiles[Field.tileTypes[row, col]]), row, col).Count != 0)
                        {
                            moveChance = rnd.Next(100);
                        }

                        float attackChance = 0;
                        //Если юниту есть кого атаковать, генерация шанса на атаку, в зависимости от агрессивности бота
                        if (FindEnemies(ai, row, col).Count != 0)
                        {
                            attackChance = rnd.Next(100) * aggressiveness;
                        }

                        float digChance = 0;
                        //Если юниту есть что добыть, генерация шанса на добычу
                        if (Field.resourceTiles[row, col] != 0)
                        {
                            digChance = rnd.Next(200);
                        }

                        float baseChance = 0;
                        //Если юнит может поставить базу, генерация шанса на создание базы, в зависимости от агрессивности бота 
                        if (ai.gasAmount >= new Base().costGas && ai.mineralsAmount >= new Base().costMinerals)
                        {
                            baseChance = rnd.Next(100) * aggressiveness;
                        }

                        else if (attackChance == Math.Max(Math.Max(moveChance, attackChance), Math.Max(digChance, baseChance)) && attackChance != 0)
                        {
                            //Если шанс на атаку больше всех остальных, осуществление атаки на случайный вражеский юнит или базу
                            Attack(ai, row, col, Field.tiles[Field.tileTypes[row, col]], otherPlayer, pictureBox1);
                        }
                        if (moveChance == Math.Max(Math.Max(moveChance, attackChance), Math.Max(digChance, baseChance)))
                        {
                            //Если шанс на перемещение больше всех остальных, осуществление перемещения на случайную клетку
                            MoveUnits(ai, row, col);
                        }
                        else if (digChance == Math.Max(Math.Max(moveChance, attackChance), Math.Max(digChance, baseChance)))
                        {
                            //Если шанс на добычу больше всех остальных, осуществление добычи
                            Dig(ai, row, col);
                        }
                        else if (baseChance == Math.Max(Math.Max(moveChance, attackChance), Math.Max(digChance, baseChance)))
                        {
                            //Если шанс на создание базы больше всех остальных, осуществление создания базы
                            SetBase(ai, row, col);
                        }

                    }
                    //Если на выбранной клетке находится база, создание случайного подходящего юнита
                    else if (ai.playerBases[row, col] != null)
                    {
                        try
                        {
                            ai.playerBases[row, col].CreateUnit(ChooseUnit(ai, row, col), ai, new Point(row, col));
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
