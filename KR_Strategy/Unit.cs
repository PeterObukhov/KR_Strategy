﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Unit
    {
        public bool hasMoved;
        public bool hasActed;
        public double damage { get; set; }
        public int move { get; set; }
        public double health { get; set; }
        public int costGas { get; set; }
        public int costMinerals { get; set; }
        public Unit(double dmg, double hp, int mv, int cg, int cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
            hasMoved = false;
            hasActed = false;
        }
        public virtual int CalcMove(string tile) { return move; }
        public virtual void Attack(Unit target, string tile) { }
        public void OnClick(Unit unit, PictureBox pictureBox1, MouseEventArgs firstClick, string tile, Player attacker, Player defender)
        {
            UnitDialog ud = new UnitDialog();
            ud.ShowDialog();
            string dialogRes = ud.ans;
            switch (dialogRes)
            {
                case "Attack":
                    if (unit.hasActed == false)
                    {
                        MouseEventHandler attackClickHandler = null;
                        attackClickHandler = delegate (object sender, MouseEventArgs secondClick)
                        {
                            double dist = Field.GetDistance(firstClick.Location, secondClick.Location);
                            if (dist <= 1)
                            {
                                Field.PointToHex(secondClick.Location.X, secondClick.Location.Y, out int row, out int col);
                                if (Field.unitTiles[row, col] != null)
                                {
                                    Unit target = Field.unitTiles[row, col];
                                    Attack(target, tile);
                                    if (target.health <= 0)
                                    {
                                        Field.unitTiles[row, col] = null;
                                        defender.playerUnits[row, col] = null;
                                        pictureBox1.Invalidate();
                                    }
                                }
                                else if (Field.baseTiles[row, col] != null)
                                {
                                    Unit target = Field.baseTiles[row, col];
                                    Attack(target, tile);
                                    if (target.health <= 0)
                                    {
                                        Field.baseTiles[row, col] = null;
                                        defender.playerBases[row, col] = null;
                                        pictureBox1.Invalidate();
                                    }
                                }
                                unit.hasActed = true;
                            }
                            pictureBox1.MouseClick -= attackClickHandler;
                        };
                        pictureBox1.MouseClick += attackClickHandler;
                        unit.hasActed = true;
                    }
                    else MessageBox.Show("Этот юнит уже сделал действие в этом ходу!");
                    break;

                case "Move":
                    if (unit.hasMoved == false)
                    {
                        int mv = CalcMove("Flat");
                        MouseEventHandler moveClickHandler = null;
                        moveClickHandler = delegate (object sender, MouseEventArgs secondClick)
                        {
                            try
                            {
                                double dist = Field.GetDistance(firstClick.Location, secondClick.Location);
                                if (dist <= mv)
                                {
                                    Field.PointToHex(secondClick.Location.X, secondClick.Location.Y, out int rowEnd, out int colEnd);
                                    Field.unitTiles[rowEnd, colEnd] = unit;
                                    attacker.playerUnits[rowEnd, colEnd] = unit;
                                    Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int rowStart, out int colStart);
                                    Field.unitTiles[rowStart, colStart] = null;
                                    attacker.playerUnits[rowStart, colStart] = null;
                                }
                                pictureBox1.MouseClick -= moveClickHandler;
                            }
                            catch { }
                        };
                        pictureBox1.MouseClick += moveClickHandler;
                        unit.hasMoved = true;
                    }
                    else MessageBox.Show("Этот юнит уже перемещался в этом ходу!");
                    break;

                case "Dig":
                    if (unit.hasActed == false)
                    {
                        Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int unitRow, out int unitCol);
                        if (Field.resourceTiles[unitRow, unitCol] == 1)
                        {
                            attacker.gasAmount += 100;
                            unit.hasActed = true;
                        }
                        else if (Field.resourceTiles[unitRow, unitCol] == 2)
                        {
                            attacker.mineralsAmount += 100; 
                            unit.hasActed = true;
                        }
                        else MessageBox.Show("В данном месте нет ресурсов для добычи!");
                    }
                    else MessageBox.Show("Этот юнит уже сделал действие в этом ходу!");
                    break;

                case "Build":
                    if (unit.hasActed == false)
                    {
                        Field.PointToHex(firstClick.Location.X, firstClick.Location.Y, out int currRow, out int currCol);
                        if (Field.resourceTiles[currRow, currCol] == 0 && Field.baseTiles[currRow, currCol] == null)
                        {
                            Field.SetUnit(new Base(), currRow, currCol, attacker);
                            unit.hasActed = true;
                        }
                        else MessageBox.Show("Невозможно поставить базу");
                    }
                    else MessageBox.Show("Этот юнит уже сделал действие в этом ходу!");
                    break;
            }
        }
    }
   
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
            if (groundUnits.Contains<string>(target.GetType().Name) || waterUnits.Contains<string>(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Sea" || tile.GetType().Name == "Desert") tempDamage += 10;
            if (tile.GetType().Name == "River") tempDamage += 20;
            target.health -= tempDamage;
            MessageBox.Show($"Попадание! Нанесенный урон: {tempDamage}. Оставшиеся жизни цели: {target.health}");
        }

        public override int CalcMove(string tile)
        {
            if (tile.GetType().Name == "Flat") return move + 1;
            if (tile.GetType().Name == "Mountain") return move - 1;
            return move;
        }
    }
    class Fighter : AirUnit
    {
        public Fighter(double dmg = 50, double hp = 100, int mv = 5, int cg = 50, int cm = 20) : base(dmg, hp, mv, cg, cm)
        {
            damage = 50;
            health = 100;
            move = 5;
            costGas = 50;
            costMinerals = 20;
        }
    }
    class Cruiser : AirUnit
    {
        public Cruiser(double dmg = 20, double hp = 150, int mv = 2, int cg = 150, int cm = 100) : base(dmg, hp, mv, cg, cm)
        {
            damage = 20;
            health = 150;
            move = 2;
            costGas = 150;
            costMinerals = 100;
        }
    }
    class Drone : AirUnit
    {
        public Drone(double dmg = 10, double hp = 50, int mv = 3, int cg = 40, int cm = 10) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }

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
            if (airUnits.Contains<string>(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Forest" || tile.GetType().Name == "Mountain") tempDamage += 10;
            target.health -= tempDamage;
        }

        public override int CalcMove(string tile)
        {
            if (tile.GetType().Name == "Flat") return move + 1;
            if (tile.GetType().Name == "Mountain" || tile.GetType().Name == "Forest") return move - 1;
            if (tile.GetType().Name == "River") return move - 2;
            if (tile.GetType().Name == "Sea") return 0;
            return move;
        }
    }
    class Tank : GroundUnit
    {
        public Tank(double dmg = 20, double hp = 200, int mv = 3, int cg = 200, int cm = 100) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    class Car : GroundUnit
    {
        public Car(double dmg = 5, double hp = 50, int mv = 4, int cg = 70, int cm = 10) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    class RocketLauncher : GroundUnit
    {
        public RocketLauncher(double dmg = 150, double hp = 50, int mv = 2, int cg = 150, int cm = 100) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
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
            if (groundUnits.Contains<string>(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Sea") tempDamage += 10;
            if (tile.GetType().Name == "River") tempDamage += 20;
            target.health -= tempDamage;
        }

        public override int CalcMove(string tile)
        {
            return move;
        }
    }
    class Boat : WaterUnit
    {
        public Boat(double dmg = 100, double hp = 100, int mv = 4, int cg = 70, int cm = 10) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
    class Ship : WaterUnit
    {
        public Ship(double dmg = 200, double hp = 200, int mv = 3, int cg = 200, int cm = 150) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
    }
}
