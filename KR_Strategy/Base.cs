using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    class Base
    {
        private static Player owner;
        private static Point coordinates;
        public Base(Point coords, Player player)
        {
            coordinates = coords;
            owner = player;
        }
        public void OnClick(Field field)
        {
            BaseDialog bd = new BaseDialog();
            bd.ShowDialog();
            string dialAns = bd.ans;
            switch (dialAns)
            {
                case "Fighter":
                    CreateUnit("Fighter", field);
                    break;
                case "Close":
                    break;
            }
        }
        private static void CreateUnit(string unit, Field field)
        {
            switch(unit)
            {
                case "Fighter":
                    Fighter fighter = new Fighter();
                    field.SetUnit("Fighter", coordinates, owner);
                    break;
            }
        }
    }
}
