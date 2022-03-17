﻿using System.Windows.Forms;

namespace KR_Strategy
{
    //Класс мины наследует класс Unit для корректной работы метода Attack, где целью является мина
    class Mine : Unit
    {
        public new double health;
        public new double damage;
        public new int costGas = 80;
        public new int costMinerals = 30;
        public Mine(double dmg = 70, double hp = 10, int mv = 0, int ar = 0, int cg = 0, int cm = 0) : base(dmg, hp, mv, ar, cg, cm)
        {
            health = hp;
            damage = dmg;
        }
        //Получение юнитом урона от взрыва мины
        public void MineAttack(Unit target)
        {
            target.health -= damage;
            MessageBox.Show("Взрыв на мине!");
        }
    }
}
