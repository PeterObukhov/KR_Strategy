using System.Windows.Forms;

namespace KR_Strategy
{
    //Класс пехоты, являющийся наследником класса наземного юнита
    class Infantry : GroundUnit
    {
        //Урон юнита
        public new double damage;
        //Здоровье юнита
        public new double health;
        //Аеремещение юнита
        public new int move;
        //Стоимость в газе
        public new int costGas;
        //Стоимость в минералах
        public new int costMinerals;
        public Infantry(double dmg = 40, double hp = 80, int mv = 4, int ar = 1, int cg = 80, int cm = 30) : base(dmg, hp, mv, ar, cg, cm)
        {
            damage = dmg;
            health = hp;
            move = mv;
            attackRange = ar;
            costGas = cg;
            costMinerals = cm;
        }
        //Метод улучшения здоровья на 30 пунктов
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
        //Метод улучшения урона на 30 пунктов
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
        //Метод улучшения перемещения на 1 пункт
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
