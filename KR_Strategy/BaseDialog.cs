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
    }
}
