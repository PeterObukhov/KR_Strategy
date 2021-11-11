using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_Strategy
{
    public partial class BaseDialog : Form
    {
        public string ans;
        public BaseDialog()
        {
            InitializeComponent();
            ToolTip fighterTip = new ToolTip();
            fighterTip.SetToolTip(button1, $"Истребитель.\n Стоимость: 50 газа, 20 минералов. \n Базовое перемещение: 5 \n Базовый урон: 50 \n Здоровье: 100");
            ToolTip cruiserTip = new ToolTip();
            cruiserTip.SetToolTip(button3, $"Крейсер.\n Стоимость: 150 газа, 100 минералов. \n Базовое перемещение: 2 \n Базовый урон: 80 \n Здоровье: 150");
            ToolTip droneTip = new ToolTip();
            droneTip.SetToolTip(button4, $"Дрон.\n Стоимость: 40 газа, 10 минералов. \n Базовое перемещение: 3 \n Базовый урон: 40 \n Здоровье: 50");
            ToolTip tankTip = new ToolTip();
            tankTip.SetToolTip(button5, $"Танк.\n Стоимость: 200 газа, 100 минералов. \n Базовое перемещение: 3 \n Базовый урон: 60 \n Здоровье: 200");
            ToolTip carTip = new ToolTip();
            carTip.SetToolTip(button6, $"Боевая машина.\n Стоимость: 70 газа, 10 минералов. \n Базовое перемещение: 4 \n Базовый урон: 30 \n Здоровье: 50");
            ToolTip rocketTip = new ToolTip();
            rocketTip.SetToolTip(button7, $"Ракетная установка.\n Стоимость: 150 газа, 100 минералов. \n Базовое перемещение: 2 \n Базовый урон: 150 \n Здоровье: 50");
            ToolTip boatTip = new ToolTip();
            boatTip.SetToolTip(button8, $"Катер.\n Стоимость: 70 газа, 10 минералов. \n Базовое перемещение: 4 \n Базовый урон: 100 \n Здоровье: 50");
            ToolTip shipTip = new ToolTip();
            shipTip.SetToolTip(button9, $"Корабль.\n Стоимость: 300 газа, 200 минералов. \n Базовое перемещение: 3 \n Базовый урон: 120 \n Здоровье: 200");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ans = "Fighter";
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BaseDialog_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ans = "Cruiser";
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ans = "Drone";
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ans = "Tank";
            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ans = "Car";
            Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ans = "Rocket";
            Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ans = "Boat";
            Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ans = "Ship";
            Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ans = "Infantry";
            Close();
        }
    }
}
