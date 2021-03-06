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
        //Игрок или ИИ 1
        static Player player1;
        //Игрок или ИИ 2
        static Player player2;
        //Счетчик ходов
        static int count = 0;
        //Игровое поле
        static Field field = new Field();
        public Form1()
        {
            InitializeComponent();
        }

        //Кнопка "Закрыть игру"
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Кнопка "Играть"
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            //Отображение элементов управления настройками игры (режим игры)
            button1.Visible = false;
            button2.Visible = false;
            label1.Visible = true;
            comboBox1.Visible = true;
            button3.Visible = true;
            form.Invalidate();
        }
        //Выбор режима игры
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                //Если выбран режим "Игрок против компьютера", создается один игрок
                player1 = new Player("Player 1", "Blue");
                comboBox2.Visible = true;
                label2.Visible = true;
                button3.Enabled = false;
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                //Если выбран режим "Компьютер против компьютера", игроков не создается
                comboBox2.Visible = true;
                label2.Visible = true;
                button3.Enabled = false;
            }
            else
            {
                //Если выбран режим "Игрок против игрока", создаются оба игрока
                player1 = new Player("Player 1", "Blue");
                player2 = new Player("Player 2", "Red");
                comboBox2.Visible = false;
                label2.Visible = false;
                button3.Enabled = true;
            }
        }
        //Выбор сложности компьютера
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    //Колчество создаваемых ботов зависит от режима игры, выбранного ранее
                    //Если выбрана легкая сложность, агрессивность ботов равна 1
                    if (comboBox1.SelectedIndex == 1) player2 = new AI("AI", "Red", 1);
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
                case 2:
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
        //Кнопка "Старт"
        private void button3_Click(object sender, EventArgs e)
        {
            //Сокрытие и отображение элементов управления и интерфейса
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
            //Установка начальных баз первого и второго игроков
            Field.SetUnit(new Base(), 0, 0, player1);
            Field.SetUnit(new Base(), 3, 8, player2);
            //Заполнение игрового поля
            field.CreateField();
            //Если игру начинает бот, то сразу делается ход первого игрока
            if(player1.GetType().Name == "AI")
            {
                AI.AiTurn(player1, player2, pictureBox1);
                button4.PerformClick();
            }
        }

        public void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(Color.Black, 3);
            //Вывод количества ресурсов для обоих игроков
            label4.Text = player1.mineralsAmount.ToString();
            label6.Text = player1.gasAmount.ToString();
            label8.Text = player2.mineralsAmount.ToString();
            label10.Text = player2.gasAmount.ToString();
            //Отрисовка сетки шестиугольникова
            Field.DrawHexGrid(graphics, pen, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            //Отображения баз и юнитов обоих игроков
            Field.DrawUnits(graphics, player1);
            Field.DrawUnits(graphics, player2);
            //Проверка победы второго игрока
            if (player1.WinCheck())
            {
                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                MessageBox.Show($"{player2.name} победил!");
            }
            //Проверка победы первого игрока
            if (player2.WinCheck())
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

        //"Завершить ход" для второго игрока
        private void button5_Click(object sender, EventArgs e)
        {
            count += 1;
            button5.Enabled = false;
            button4.Enabled = true;
            button7.Enabled = true;
            button6.Enabled = false;
            //Сброс информации о перемещении и действии всех юнитов второго игрока
            foreach (Unit unit in player2.playerUnits)
            {
                if (unit != null)
                {
                    unit.hasActed = false;
                    unit.hasMoved = false;
                }
            }
            //Если другой игрок является ботом
            if (player1.GetType().Name == "AI")
            {
                //Ход бота
                AI.AiTurn(player1, player2, pictureBox1);
                Timer timer = new Timer();
                timer.Interval = 500;
                timer.Start();
                //Задержка в полсекунды перед нажатием ботом кнопки "Завершить ход" для корректного отображения состояния поля
                timer.Tick += (s, ee) =>
                {
                    timer.Stop();
                    button4.PerformClick();
                };
                pictureBox1.Invalidate();
            }

        }

        //"Завершить ход" для первого игрока
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

        //Кнопка "Сдаться" для первого игрока
        private void button7_Click(object sender, EventArgs e)
        {
            //Удаление всех баз первого игрока
            for(int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    player1.playerBases[row, col] = null;
                }
            }
            //Перерисовка поля, в которой вызывается проверка на завершение игры
            pictureBox1.Invalidate();
        }

        //Кнопка "Сдаться" для второго игрока
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

        //Кнопка "Справка". Вызов диалогового окна с информацией по игре
        private void button8_Click(object sender, EventArgs e)
        {
            HintDialog hd = new HintDialog();
            hd.Show();
        }
    }
}
