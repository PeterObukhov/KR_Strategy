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
        //Здоровье базы
        public double hp;
        //Стоимость базы в газе
        public new int costGas = 700;
        //Стоимость базы в минералах
        public new int costMinerals = 500;
        public Base(double dmg = 0, double hp = 200, int mv = 0, int cg = 0, int cm = 0) : base(dmg, hp, mv, cg, cm)
        {
            this.hp = hp;
        }
        //Обработка нажатия на базу
        public void OnClick(Point coordinates, Player player)
        {
            //Вызов диалогового окна для выбора юнита
            BaseDialog bd = new BaseDialog();
            bd.ShowDialog();
            string dialAns = bd.ans;
            //Создание юнита на клетке с базой, в зависимости от выбора игрока
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
                case "Infantry":
                    CreateUnit(new Infantry(), player, coordinates);
                    break;
            }
        }
        //Метод создания юнита для текущего игрока
        public void CreateUnit(Unit unit, Player player, Point coordinates)
        {
            //Если хватает ресурсов на создание юнита
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
