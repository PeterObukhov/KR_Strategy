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
        static Player player1;
        static Player player2;
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
            if (comboBox1.SelectedIndex == 1)
            {
                player1 = new Player("Player 1", "Blue");
                player2 = new AI("AI", "Red", 1);
                comboBox2.Visible = true;
                label2.Visible = true;
                button3.Enabled = false;
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                player1 = new AI("AI Blue", "Blue", 1);
                player2 = new AI("AI Red", "Red", 1);
                comboBox2.Visible = true;
                label2.Visible = true;
                button3.Enabled = false;
            }
            else
            {
                player1 = new Player("Player 1", "Blue");
                player2 = new Player("Player 2", "Red");
                comboBox2.Visible = false;
                label2.Visible = false;
                button3.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    if(comboBox1.SelectedIndex == 1) player2 = new AI("AI", "Red", 1);
                    if(comboBox1.SelectedIndex == 2)
                    {
                        player1 = new AI("AI Blue", "Blue", 1);
                        player2 = new AI("AI Red", "Red", 1);
                    }
                    break;
                case 1:
                    if (comboBox1.SelectedIndex == 1) player2 = new AI("AI", "Red", 2);
                    if (comboBox1.SelectedIndex == 2)
                    {
                        player1 = new AI("AI Blue", "Blue", 2);
                        player2 = new AI("AI Red", "Red", 2);
                    }
                    break;
                case 3:
                    if (comboBox1.SelectedIndex == 1) player2 = new AI("AI", "Red", 3);
                    if (comboBox1.SelectedIndex == 2)
                    {
                        player1 = new AI("AI Blue", "Blue", 3);
                        player2 = new AI("AI Red", "Red", 3);
                    }
                    break;
            }
            button3.Enabled = true;
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
            button6.Visible = true;
            button7.Visible = true;
            Field.SetUnit(new Base(), 0, 0, player1);
            Field.SetUnit(new Base(), 3, 8, player2);
            field.CreateField();
        }

        public void pictureBox1_Paint(object sender, PaintEventArgs e)
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
            if (Field.WinCheck(player1, player2))
            {
                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                MessageBox.Show($"{player2.name} победил!");
            }
            if (Field.WinCheck(player2, player1))
            {
                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                MessageBox.Show($"{player1.name} победил!");
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(count % 2 == 0) Field.FieldClick(e, pictureBox1, player1, player2);
            else Field.FieldClick(e, pictureBox1, player2, player1);
            pictureBox1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            count += 1;
            button5.Enabled = false;
            button4.Enabled = true;
            button7.Enabled = true;
            button6.Enabled = false;
            foreach (Unit unit in player2.playerUnits)
            {
                if (unit != null)
                {
                    unit.hasActed = false;
                    unit.hasMoved = false;
                }
            }
            if (player1.GetType().Name == "AI")
            {
                AI.AiTurn(player1, player2, pictureBox1);
                Timer timer = new Timer();
                timer.Interval = 500;
                timer.Start();
                timer.Tick += (s, ee) =>
                {
                    timer.Stop();
                    button4.PerformClick();
                };
                pictureBox1.Invalidate();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            count += 1;
            button4.Enabled = false;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = false;
            foreach (Unit unit in player1.playerUnits) 
            {
                if(unit != null)
                {
                    unit.hasActed = false;
                    unit.hasMoved = false;
                }
            }
            if (player2.GetType().Name == "AI")
            {
                AI.AiTurn(player2, player1, pictureBox1);
                Timer timer = new Timer();
                timer.Interval = 500;
                timer.Start();
                timer.Tick += (s, ee) =>
                {
                    timer.Stop();
                    button5.PerformClick();
                };
                pictureBox1.Invalidate();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for(int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    player1.playerBases[row, col] = null;
                }
            }
            pictureBox1.Invalidate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    player2.playerBases[row, col] = null;
                }
            }
            pictureBox1.Invalidate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            HintDialog hd = new HintDialog();
            hd.Show();
        }
    }
}
