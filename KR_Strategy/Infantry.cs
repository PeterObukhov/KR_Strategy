using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Infantry : GroundUnit
    {
        public new double damage;
        public new double health;
        public new int move;
        public new int costGas;
        public new int costMinerals;
        public Infantry(double dmg = 40, double hp = 80, int mv = 4, int cg = 80, int cm = 30) : base(dmg, hp, mv, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            costGas = cg;
            costMinerals = cm;
        }
        public static void UpgradeHealth(Unit unit, Player player)
        {
            if (player.gasAmount >= 100 && player.mineralsAmount >= 50)
            {
                unit.health += 30;
                player.gasAmount -= 100;
                player.mineralsAmount -= 50;
            }
            else MessageBox.Show("Недостаточно ресурсов!");
        }
        public static void UpgradeDamage(Unit unit, Player player)
        {
            if (player.gasAmount >= 100 && player.mineralsAmount >= 50)
            {
                unit.damage += 30;
                player.gasAmount -= 100;
                player.mineralsAmount -= 50;
            }
            else MessageBox.Show("Недостаточно ресурсов!");
        }
        public static void UpgradeMove(Unit unit, Player player)
        {
            if (player.gasAmount >= 100 && player.mineralsAmount >= 50)
            {
                unit.move += 1;
                player.gasAmount -= 100;
                player.mineralsAmount -= 50;
            }
            else MessageBox.Show("Недостаточно ресурсов!");
        }
    }
}
