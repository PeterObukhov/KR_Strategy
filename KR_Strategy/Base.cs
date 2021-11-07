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
        private static Point coordinates;
        public Base(Point coords)
        {
            coordinates = coords;
        }
        public static void OnClick(Point coords)
        {
            BaseDialog bd = new BaseDialog();
            bd.ShowDialog();
            string dialAns = bd.ans;
            switch (dialAns)
            {
                case "Fighter":
                    CreateUnit("Fighter");
                    break;
                case "Close":
                    break;
            }
        }
        private static void CreateUnit(string unit)
        {
            switch(unit)
            {
                case "Fighter":
                    Fighter fighter = new Fighter();
                    Field.SetUnit("Fighter", coordinates);
                    break;
            }
        }
    }
}
