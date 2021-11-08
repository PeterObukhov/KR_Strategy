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
    public partial class Form1 : Form
    {
        static Player player1 = new Player("Игрок 1", "Blue");
        static Player player2 = new Player("Игрок 2", "Red");
        static int count = 0;
        static Field field = new Field();
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            button1.Visible = false;
            button2.Visible = false;
            label1.Visible = true;
            comboBox1.Visible = true;
            button3.Visible = true;
            form.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
            {
                comboBox2.Visible = true;
                label2.Visible = true;
            }
            else
            {
                comboBox2.Visible = false;
                label2.Visible = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            comboBox1.Visible = false;
            button3.Visible = false;
            comboBox2.Visible = false;
            label2.Visible = false;
            pictureBox1.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            Field.SetUnit(new Base(), 0, 0, player1);
            Field.SetUnit(new Base(), 3, 8, player2);
            field.CreateField();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(Color.Black, 3);
            label4.Text = player1.mineralsAmount.ToString();
            label6.Text = player1.gasAmount.ToString();
            label8.Text = player2.mineralsAmount.ToString();
            label10.Text = player2.gasAmount.ToString();
            Field.DrawHexGrid(graphics, pen, 0, pictureBox1.ClientSize.Width, 0, pictureBox1.ClientSize.Height, 80);
            Field.DrawUnits(graphics, player1);
            Field.DrawUnits(graphics, player2);
            Field.WinCheck(player1, player2);
            Field.WinCheck(player2, player1);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(count % 2 == 0) Field.FieldClick(e, pictureBox1, player1, field, player2);
            else Field.FieldClick(e, pictureBox1, player2, field, player2);
            pictureBox1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            count += 1;
            button5.Enabled = false;
            button4.Enabled = true;
            foreach (Unit unit in player2.playerUnits)
            {
                if (unit != null)
                {
                    unit.hasActed = false;
                    unit.hasMoved = false;
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            count += 1;
            button4.Enabled = false;
            button5.Enabled = true;
            foreach (Unit unit in player1.playerUnits) 
            {
                if(unit != null)
                {
                    unit.hasActed = false;
                    unit.hasMoved = false;
                }
            }
        }
    }
}
