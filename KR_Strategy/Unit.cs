using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    //Базовый класс всех юнитов
    class Unit
    {
        //Перемещался ли юнит в этом ходу
        public bool hasMoved;
        //Совершал ли юнит в этом ходу действие
        public bool hasActed;
        //Урон юнита
        public double damage { get; set; }
        //Перемещение юнита
        public int move { get; set; }
        //Здоровье юнита
        public double health { get; set; }
        //Стоимость юнита в газе 
        public int costGas { get; set; }
        //Стоимость юнита в минералах 
        public int costMinerals { get; set; }
        public Unit(double dmg, double hp, int mv, int cg, int cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
            //Задержка на один ход после создания юнита
            hasMoved = true;
            hasActed = true;
        }
        //Подсчет расстояния перемещения юнита в зависимости от клетки
        public virtual int CalcMove(string tile) { return move; }
        //Атака с уроном, зависящим от клетки
        public virtual void Attack(Unit target, string tile) { }
        //Осуществление атаки на выбранный юнит, базу или мину
        public void DoAttack(Unit unit, PictureBox pictureBox1, MouseEventArgs firstClick, string tile, Player defender) 
        {
            Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int rowStart, out int colStart);
            PaintEventHandler attackPaintHandler = null;
            //Отображение радиуса атаки в одну клетку для выбранного юнита
            attackPaintHandler = delegate (object sender, PaintEventArgs paint)
            {
                Field.ShowPath(1, rowStart, colStart, paint.Graphics);
            };
            pictureBox1.Paint += attackPaintHandler;
            
            //Обработчик события нажатия на юнит, выбранный для атаки
            MouseEventHandler attackClickHandler = null;
            attackClickHandler = delegate (object sender, MouseEventArgs secondClick)
            {
                //Получение координат в массиве для атакуемого юнита
                Field.PointToHex(secondClick.Location.X, secondClick.Location.Y, out int rowEnd, out int colEnd);
                double dist = Field.HexDistance(new System.Drawing.Point(rowStart, colStart), new System.Drawing.Point(rowEnd, colEnd));
                if (dist <= 1 && dist != 0)
                {
                    //Если атака на юнит
                    if (Field.unitTiles[rowEnd, colEnd] != null)
                    {
                        Unit target = Field.unitTiles[rowEnd, colEnd];
                        double tempHealth = target.health;
                        //Осуществление атаки
                        Attack(target, tile);
                        MessageBox.Show($"Попадание! Нанесенный урон: {tempHealth - target.health}. Оставшиеся жизни цели: {target.health}");
                        //Если цель уничтожена, убрать ее с поля
                        if (target.health <= 0)
                        {
                            Field.unitTiles[rowEnd, colEnd] = null;
                            defender.playerUnits[rowEnd, colEnd] = null;
                            pictureBox1.Invalidate();
                        }
                        unit.hasActed = true;
                    }
                    //Если атака на базу
                    else if (Field.baseTiles[rowEnd, colEnd] != null)
                    {
                        Unit target = Field.baseTiles[rowEnd, colEnd];
                        double tempHealth = target.health;
                        Attack(target, tile);
                        MessageBox.Show($"Попадание! Нанесенный урон: {tempHealth - target.health}. Оставшиеся жизни цели: {target.health}");
                        if (target.health <= 0)
                        {
                            Field.baseTiles[rowEnd, colEnd] = null;
                            defender.playerBases[rowEnd, colEnd] = null;
                            pictureBox1.Invalidate();
                        }
                        unit.hasActed = true;
                    }
                    //Если атака на мину
                    else if (Field.mines[rowEnd, colEnd] != null)
                    {
                        Unit target = Field.mines[rowEnd, colEnd];
                        Attack(target, tile);
                        if (target.health <= 0)
                        {
                            Field.mines[rowEnd, colEnd] = null;
                            pictureBox1.Invalidate();
                        }
                        unit.hasActed = true;
                    }
                }
                pictureBox1.Paint -= attackPaintHandler;
                pictureBox1.MouseClick -= attackClickHandler;
            };
            pictureBox1.MouseClick += attackClickHandler;
        }
        //Метод перемещения юнита
        public void DoMove(Unit unit, PictureBox pictureBox1, MouseEventArgs firstClick, Player attacker, string tile)
        {
            int mv = CalcMove(tile);
            Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int rowStart, out int colStart);
            //Отображение радиуса перемещения для выбранного юнита
            PaintEventHandler moveHandler = null;
            moveHandler = delegate (object sender, PaintEventArgs paint)
            {
                Field.ShowPath(mv, rowStart, colStart, paint.Graphics);
            };
            if (unit.hasMoved == false)
            {
                pictureBox1.Paint += moveHandler;
                //Обработчик события нажатия на клетку для перемещения
                MouseEventHandler moveClickHandler = null;
                moveClickHandler = delegate (object sender, MouseEventArgs secondClick)
                {
                    try
                    {
                        Field.PointToHex(secondClick.Location.X, secondClick.Location.Y, out int rowEnd, out int colEnd);
                        //Если выбранная клетка пуста
                        if (Field.unitTiles[rowEnd, colEnd] == null && Field.baseTiles[rowEnd, colEnd] == null)
                        {
                            double dist = Field.HexDistance(new System.Drawing.Point(rowStart, colStart), new System.Drawing.Point(rowEnd, colEnd));
                            if (dist <= mv)
                            {
                                //Перемещение юнита в массиве с юнитами текущего игрока и в массиве со всеми юнитами на поле
                                Field.unitTiles[rowEnd, colEnd] = unit;
                                attacker.playerUnits[rowEnd, colEnd] = unit;
                                Field.unitTiles[rowStart, colStart] = null;
                                attacker.playerUnits[rowStart, colStart] = null;
                                unit.hasMoved = true;
                                //Если наземный юнит переместился в клетку с морем, то он моментально погибает
                                if (Field.tiles[Field.tileTypes[rowEnd, colEnd]] == "Sea" &&
                                (attacker.playerUnits[rowEnd, colEnd].GetType().Name != "Ship" ||
                                attacker.playerUnits[rowEnd, colEnd].GetType().Name != "Boat" ||
                                attacker.playerUnits[rowEnd, colEnd].GetType().Name != "Fighter" ||
                                attacker.playerUnits[rowEnd, colEnd].GetType().Name != "Cruiser" ||
                                attacker.playerUnits[rowEnd, colEnd].GetType().Name != "Drone"))
                                {
                                    attacker.playerUnits[rowEnd, colEnd] = null;
                                    Field.unitTiles[rowEnd, colEnd] = null;
                                    MessageBox.Show("Юнит утонул!");
                                    pictureBox1.Invalidate();
                                }
                                //Если юнит переместился на клетку с миной, то он получает урон
                                if (Field.mines[rowEnd, colEnd] != null)
                                {
                                    Field.mines[rowEnd, colEnd].MineAttack(unit);
                                    Field.mines[rowEnd, colEnd] = null;
                                    if (unit.health <= 0)
                                    {
                                        Field.unitTiles[rowEnd, colEnd] = null;
                                        attacker.playerUnits[rowEnd, colEnd] = null;
                                    }
                                    pictureBox1.Invalidate();
                                }
                            }
                        }
                        else MessageBox.Show("Клетка занята!");
                        pictureBox1.MouseClick -= moveClickHandler;
                        pictureBox1.Paint -= moveHandler;
                    }
                    catch { }
                };
                pictureBox1.MouseClick += moveClickHandler;
            }
            else MessageBox.Show("Этот юнит уже перемещался в этом ходу!");
        }
        //Метод добычи ресурсов
        public void DoDig(Unit unit, MouseEventArgs firstClick, Player attacker)
        {
            Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int unitRow, out int unitCol);
            //Добыча газа
            if (Field.resourceTiles[unitRow, unitCol] == 1)
            {
                attacker.gasAmount += 100;
                unit.hasActed = true;
            }
            //Добыча минералов
            else if (Field.resourceTiles[unitRow, unitCol] == 2)
            {
                attacker.mineralsAmount += 100;
                unit.hasActed = true;
            }
            else MessageBox.Show("В данном месте нет ресурсов для добычи!");
        }
        //Постройка базы
        public void DoBuild(Unit unit, MouseEventArgs firstClick, Player attacker)
        {
            Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int currRow, out int currCol);
            //Если текущая клетка свободная, создание базы
            if (Field.resourceTiles[currRow, currCol] == 0 && Field.baseTiles[currRow, currCol] == null)
            {
                Field.SetUnit(new Base(), currRow, currCol, attacker);
                unit.hasActed = true;
            }
            else MessageBox.Show("Невозможно поставить базу");
        }
        //Установка мины
        public void PlaceMine(Unit unit, MouseEventArgs firstClick, Player attacker)
        {
            Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int currRow, out int currCol);
            Field.SetUnit(new Mine(), currRow, currCol, attacker);
            unit.hasActed = true;
        }
        //Обработка нажатия на юнит
        public void OnClick(Unit unit, PictureBox pictureBox1, MouseEventArgs firstClick, string tile, Player attacker, Player defender)
        {
            string dialogRes;
            //Если выбрана пехота, отображение диалогового окна для пехоты
            if (unit.GetType().Name != "Infantry")
            {
                UnitDialog ud = new UnitDialog();
                ud.ShowDialog();
                dialogRes = ud.ans;
            }
            //В ином случае, отображение общего диалогового окна
            else
            {
                InfantryDialog id = new InfantryDialog();
                id.ShowDialog();
                dialogRes = id.ans;
            }
            //Осуществление действия, в зависимости от выбора в диалоговом окне
            switch (dialogRes)
            {
                case "Attack":
                    if (unit.hasActed == false)
                    {
                        DoAttack(unit, pictureBox1, firstClick, tile, defender);
                    }
                    else MessageBox.Show("Этот юнит уже сделал действие в этом ходу!");
                    break;

                case "Move":
                    DoMove(unit, pictureBox1, firstClick, attacker, tile);
                    break;

                case "Dig":
                    if (unit.hasActed == false)
                    {
                        DoDig(unit, firstClick, attacker);
                    }
                    else MessageBox.Show("Этот юнит уже сделал действие в этом ходу!");
                    break;

                case "Build":
                    if (unit.hasActed == false)
                    {
                        DoBuild(unit, firstClick, attacker);
                    }
                    else MessageBox.Show("Этот юнит уже сделал действие в этом ходу!");
                    break;

                case "Mine":
                    if (unit.hasActed == false)
                    {
                        PlaceMine(unit, firstClick, attacker);
                    }
                    else MessageBox.Show("Этот юнит уже сделал действие в этом ходу!");
                    break;

                case "Health":
                    Infantry.UpgradeHealth(unit, attacker);
                    break;

                case "Damage":
                    Infantry.UpgradeDamage(unit, attacker);
                    break;

                case "Moving":
                    Infantry.UpgradeMove(unit, attacker);
                    break;
            }
        }
    }
   
    //Класс летающих юнитов
    class AirUnit : Unit
    {
        private string[] groundUnits = new string[3] { "Car", "Tank", "RocketLauncher" };
        private string[] waterUnits = new string[2] { "Ship", "Boat" };
        private string[] airUnits = new string[3] { "Fighter", "Cruiser", "Drone" };
        public AirUnit(double dmg, double hp, int mv, int cg, int cm) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
        public override void Attack(Unit target, string tile)
        {
            double tempDamage = damage;
            if (groundUnits.Contains(target.GetType().Name) || waterUnits.Contains(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Sea" || tile.GetType().Name == "Desert") tempDamage += 10;
            if (tile.GetType().Name == "River") tempDamage += 20;
            target.health -= tempDamage;
        }

        public override int CalcMove(string tile)
        {
            if (tile == "Plain") return move + 1;
            if (tile == "Mountain") return move - 1;
            else return move;
        }
    }
    //Класс истребителя
    class Fighter : AirUnit
    {
        public Fighter(double dmg = 50, double hp = 100, int mv = 5, int cg = 50, int cm = 20) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    //Класс крейсера
    class Cruiser : AirUnit
    {
        public Cruiser(double dmg = 80, double hp = 150, int mv = 2, int cg = 150, int cm = 100) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    //Класс дрона
    class Drone : AirUnit
    {
        public Drone(double dmg = 40, double hp = 50, int mv = 3, int cg = 40, int cm = 10) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    //Класс наземеых юнитов
    class GroundUnit : Unit
    {
        public GroundUnit(double dmg, double hp, int mv, int cg, int cm) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
        private string[] groundUnits = new string[3] { "Car", "Tank", "RocketLauncher" };
        private string[] waterUnits = new string[2] { "Ship", "Boat" };
        private string[] airUnits = new string[3] { "Fighter", "Cruiser", "Drone" };
        public override void Attack(Unit target, string tile)
        {
            double tempDamage = damage;
            if (airUnits.Contains(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Forest" || tile.GetType().Name == "Mountain") tempDamage += 10;
            target.health -= tempDamage;
        }

        public override int CalcMove(string tile)
        {
            if (tile == "Plain") return move + 1;
            if (tile == "Mountain" || tile == "Forest" || tile == "Swamp") return move - 1;
            if (tile == "River") return move - 2;
            if (tile == "Sea") return 0;
            return move;
        }
    }
    //Класс танка
    class Tank : GroundUnit
    {
        public Tank(double dmg = 60, double hp = 200, int mv = 3, int cg = 200, int cm = 100) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    //Класс боевой машины
    class Car : GroundUnit
    {
        public Car(double dmg = 30, double hp = 50, int mv = 4, int cg = 70, int cm = 10) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    //Класс ракетной установки
    class RocketLauncher : GroundUnit
    {
        public RocketLauncher(double dmg = 150, double hp = 50, int mv = 3, int cg = 150, int cm = 100) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    //Класс водных юнитов
    class WaterUnit : Unit
    {
        public WaterUnit(double dmg, double hp, int mv, int cg, int cm) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
        private string[] groundUnits = new string[3] { "Car", "Tank", "RocketLauncher" };
        private string[] waterUnits = new string[2] { "Ship", "Boat" };
        private string[] airUnits = new string[3] { "Fighter", "Cruiser", "Drone" };
        public override void Attack(Unit target, string tile)
        {
            double tempDamage = damage;
            if (groundUnits.Contains(target.GetType().Name)) tempDamage += 10;
            if (tile == "Sea") tempDamage += 10;
            if (tile == "River") tempDamage += 20;
            target.health -= tempDamage;
        }

        public override int CalcMove(string tile)
        {
            if (tile == "Sea" || tile == "River") return move;
            if (tile == "Swamp") return move - 1;
            else return 0;
        }
    }
    //Класс лодки
    class Boat : WaterUnit
    {
        public Boat(double dmg = 100, double hp = 50, int mv = 4, int cg = 70, int cm = 10) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    //Класс корабля
    class Ship : WaterUnit
    {
        public Ship(double dmg = 120, double hp = 200, int mv = 3, int cg = 300, int cm = 200) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
}
