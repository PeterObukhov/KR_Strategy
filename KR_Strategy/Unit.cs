using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Unit
    {
        public double damage { get; set; }
        public int move { get; set; }
        public double health { get; set; }
        public Unit(double dmg, double hp, int mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
        public virtual int CalcMove(ITile tile) { return move; }
        public virtual void Attack(Unit target, ITile tile) { }
        public static void OnClick(string unit, PictureBox pictureBox1, MouseEventArgs firstClick)
        {
            UnitDialog ud = new UnitDialog();
            ud.ShowDialog();
            string dialogRes = ud.ans;
            switch (dialogRes)
            {
                case "Attack":
                    pictureBox1.MouseClick += (sender, secondClick) =>
                    {
                        double dist = Field.GetDistance(firstClick.Location, secondClick.Location);
                        if (dist <= 1)
                        {

                        }
                    };
                    break;
                case "Move":

                    break;
            }
        }
    }
   
    class AirUnit : Unit
    {
        private string[] groundUnits = new string[3] { "Car", "Tank", "RocketLauncher" };
        private string[] waterUnits = new string[2] { "Ship", "Boat" };
        private string[] airUnits = new string[3] { "Fighter", "Cruiser", "Drone" };
        public AirUnit(double dmg, double hp, int mv) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
        public override void Attack(Unit target, ITile tile)
        {
            double tempDamage = damage;
            if (groundUnits.Contains<string>(target.GetType().Name) || waterUnits.Contains<string>(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Sea" || tile.GetType().Name == "Desert") tempDamage += 10;
            if (tile.GetType().Name == "River") tempDamage += 20;
            target.health -= tempDamage;
        }

        public override int CalcMove(ITile tile)
        {
            if (tile.GetType().Name == "Flat") return move + 1;
            if (tile.GetType().Name == "Mountain") return move - 1;
            return move;
        }
    }
    class Fighter : AirUnit
    {
        public Fighter(double dmg = 10, double hp = 100, int mv = 5) : base(dmg, hp, mv)
        {
            damage = 10;
            health = 100;
            move = 5;
        }
    }
    class Cruiser : AirUnit
    {
        public Cruiser(double dmg = 20, double hp = 150, int mv = 2) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
    }
    class Drone : AirUnit
    {
        public Drone(double dmg = 10, double hp = 50, int mv = 3) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
    }

    class GroundUnit : Unit
    {
        public GroundUnit(double dmg, double hp, int mv) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
        private string[] groundUnits = new string[3] { "Car", "Tank", "RocketLauncher" };
        private string[] waterUnits = new string[2] { "Ship", "Boat" };
        private string[] airUnits = new string[3] { "Fighter", "Cruiser", "Drone" };
        public override void Attack(Unit target, ITile tile)
        {
            double tempDamage = damage;
            if (airUnits.Contains<string>(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Forest" || tile.GetType().Name == "Mountain") tempDamage += 10;
            target.health -= tempDamage;
        }

        public override int CalcMove(ITile tile)
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
        public Tank(double dmg = 20, double hp = 200, int mv = 3) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
    }
    class Car : GroundUnit
    {
        public Car(double dmg = 5, double hp = 50, int mv = 4) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
    }
    class RocketLauncher : GroundUnit
    {
        public RocketLauncher(double dmg = 150, double hp = 50, int mv = 2) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
    }
    class WaterUnit : Unit
    {
        public WaterUnit(double dmg, double hp, int mv) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
        private string[] groundUnits = new string[3] { "Car", "Tank", "RocketLauncher" };
        private string[] waterUnits = new string[2] { "Ship", "Boat" };
        private string[] airUnits = new string[3] { "Fighter", "Cruiser", "Drone" };
        public override void Attack(Unit target, ITile tile)
        {
            double tempDamage = damage;
            if (groundUnits.Contains<string>(target.GetType().Name)) tempDamage += 10;
            if (tile.GetType().Name == "Sea") tempDamage += 10;
            if (tile.GetType().Name == "River") tempDamage += 20;
            target.health -= tempDamage;
        }

        public override int CalcMove(ITile tile)
        {
            return move;
        }
    }
    class Boat : WaterUnit
    {
        public Boat(double dmg = 100, double hp = 100, int mv = 4) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
    }
    class Ship : WaterUnit
    {
        public Ship(double dmg = 200, double hp = 200, int mv = 3) : base(dmg, hp, mv)
        {
            damage = dmg;
            health = hp;
            move = mv;
        }
    }
}
