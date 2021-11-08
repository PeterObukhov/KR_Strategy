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
                    Fighter nf = new Fighter();
                    if (nf.costMinerals <= player.mineralsAmount && nf.costGas <= player.gasAmount)
                    {
                        Field.SetUnit(new Fighter(), coordinates.X, coordinates.Y, player);
                        player.mineralsAmount -= nf.costMinerals;
                        player.gasAmount -= nf.costGas;
                    }
                    else MessageBox.Show("Недостаточно материалов!");
                    break;
            }
        }
    }
}
