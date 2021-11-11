using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Base : Unit
    {
        public double hp;
        public new int costGas = 700;
        public new int costMinerals = 500;
        public Base(double dmg = 0, double hp = 200, int mv = 0, int cg = 0, int cm = 0) : base(dmg, hp, mv, cg, cm)
        {
            this.hp = hp;
        }
        public void OnClick(Point coordinates, Player player)
        {
            BaseDialog bd = new BaseDialog();
            bd.ShowDialog();
            string dialAns = bd.ans;
            switch (dialAns)
            {
                case "Fighter":
                    CreateUnit(new Fighter(), player, coordinates);
                    break;
                case "Cruiser":
                    CreateUnit(new Cruiser(), player, coordinates);
                    break;
                case "Drone":
                    CreateUnit(new Drone(), player, coordinates);
                    break;
                case "Tank":
                    CreateUnit(new Tank(), player, coordinates);
                    break;
                case "Car":
                    CreateUnit(new Car(), player, coordinates);
                    break;
                case "Rocket":
                    CreateUnit(new RocketLauncher(), player, coordinates);
                    break;
                case "Boat":
                    CreateUnit(new Boat(), player, coordinates);
                    break;
                case "Ship":
                    CreateUnit(new Ship(), player, coordinates);
                    break;
            }
        }
        private void CreateUnit(Unit unit, Player player, Point coordinates)
        {
            if (unit.costMinerals <= player.mineralsAmount && unit.costGas <= player.gasAmount)
            {
                Field.SetUnit(unit, coordinates.X, coordinates.Y, player);
                player.mineralsAmount -= unit.costMinerals;
                player.gasAmount -= unit.costGas;
            }
            else MessageBox.Show("Недостаточно материалов!");
        }
    }
}
