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
        public void OnClick(Field field, Point coordinates, Player player)
        {
            BaseDialog bd = new BaseDialog();
            bd.ShowDialog();
            string dialAns = bd.ans;
            switch (dialAns)
            {
                case "Fighter":
                    CreateUnit("Fighter", field, coordinates, player);
                    break;
                case "Close":
                    break;
            }
        }
        private static void CreateUnit(string unit, Field field, Point coordinates, Player player)
        {
            switch(unit)
            {
                case "Fighter":
                    field.SetUnit("Fighter", coordinates.X, coordinates.Y, player);
                    break;
            }
        }
    }
}
