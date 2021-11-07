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
            Field field = new Field();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(Color.Black, 3);
            Field.DrawHexGrid(graphics, pen, 0, pictureBox1.ClientSize.Width, 0, pictureBox1.ClientSize.Height, 80);
            Field.SetUnit("Base", new Point(0, 0));
            Base base1 = new Base(new Point(0, 0));
            Field.DrawUnits(graphics);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Field.FieldClick(e, pictureBox1);
            pictureBox1.Invalidate();
        }
    }
}
