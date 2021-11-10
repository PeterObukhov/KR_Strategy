using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Mine : Unit
    {
        public new double health;
        public new double damage;
        public new int costGas = 80;
        public new int costMinerals = 30;
        public Mine(double dmg = 70, double hp = 10, int mv = 0, int cg = 0, int cm = 0) : base(dmg, hp, mv, cg, cm)
        {
            health = hp;
            damage = dmg;
        }
        public void MineAttack(Unit target)
        {
            target.health -= damage;
            MessageBox.Show("Взрыв на мине!");
        }
    }
}
