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
    public partial class UnitDialog : Form
    {
        public string ans;
        public UnitDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ans = "Attack";
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ans = "Move";
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ans = "Dig";
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ans = "Build";
            Close();
        }
    }
}
