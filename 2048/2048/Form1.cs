using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace _2048
{
    public partial class Form1 : Form
    {
        public Form1()
        { 
            InitializeComponent();
            KeyPreview = true;
            lose = false;
            this.BackColor = Color.FromArgb(187, 173, 160);
            changeColors();
            initializeLabels();
            startNewGame();
            updateScore();
            initializeButton();
        }

        int d = 12;
        int startPointY, direction, times;
        bool lose, moved;
        int[,] movement = new int[N, N];
        int[,] finalBoard = new int[N, N];
        const int labelWidth = 100, N = 4;
        RoundLabel[,] labels = new RoundLabel[N, N];
        Font smallFont = new Font("Microsoft Sans Serif", 26), bigFont = new Font("Microsoft Sans Serif", 28);
        int score, tile, best = 0;
        /*int[] colorR = new int[11] { 238, 237, 242, 245, 246, 246, 237, 237, 237, 237, 237 };
        int[] colorG = new int[11] { 228, 224, 177, 149, 124, 94, 207, 204, 200, 197, 194 };
        int[] colorB = new int[11] { 218, 200, 121, 99, 95, 59, 114, 97, 80, 63, 46 };*/
        Color[] TileColors = new Color[11] { Color.FromArgb(238, 228, 218), Color.FromArgb(237, 224, 200), Color.FromArgb(242, 177, 121), Color.FromArgb(245, 149, 99), Color.FromArgb(246, 124, 95), Color.FromArgb(246, 94, 59), Color.FromArgb(237, 207, 114), Color.FromArgb(237, 204, 97), Color.FromArgb(237, 200, 80), Color.FromArgb(237, 197, 63), Color.FromArgb(237, 194, 46) };
        Random rand = new Random();

        protected override bool IsInputKey(Keys keydata)
        {
            return (keydata == Keys.Up || keydata == Keys.Down || keydata == Keys.Left || keydata == Keys.Right) ? true : base.IsInputKey(keydata);
        }

        public GraphicsPath GetRoundPath(RectangleF rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2.0F;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(rectangle.Location, sizeF);

            path.AddArc(arc, 180, 90);
            arc.X = rectangle.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rectangle.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rectangle.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        public void drawLabelBack()
        {
            RectangleF rect1 = new RectangleF(label1.Location.X - 12.5f, label1.Location.Y - 3, Math.Max(label1.Width, label2.Width) + 25, label1.Height + ScoreLabel.Height + 6);
            RectangleF rect2 = new RectangleF(label2.Location.X - Math.Abs(label1.Width - label2.Width) / 2 - 12.5f, label2.Location.Y - 3, Math.Max(label1.Width, label2.Width) + 25, label2.Height + BestLabel.Height + 6);

            this.CreateGraphics().FillPath(new SolidBrush(Color.FromArgb(187, 173, 160)), GetRoundPath(rect1, 3));
            this.CreateGraphics().FillPath(new SolidBrush(Color.FromArgb(187, 173, 160)), GetRoundPath(rect2, 3));
        }

        public void changeColors()
        {
            //BackColors
            this.BackColor = Color.FromArgb(251, 248, 239);
            BestLabel.BackColor = Color.FromArgb(187, 173, 160);
            ScoreLabel.BackColor = Color.FromArgb(187, 173, 160);
            label1.BackColor = Color.FromArgb(187, 173, 160);
            label2.BackColor = Color.FromArgb(187, 173, 160);

            //ForeColors
            label1.ForeColor = Color.FromArgb(238, 228, 214);
            label2.ForeColor = Color.FromArgb(238, 228, 214);
            label3.ForeColor = Color.FromArgb(122, 114, 103);
            label4.ForeColor = Color.FromArgb(122, 114, 103);
            label5.ForeColor = Color.FromArgb(122, 114, 103);
            label6.ForeColor = Color.FromArgb(122, 114, 103);
            ScoreLabel.ForeColor = Color.White;
            BestLabel.ForeColor = Color.White;
        }

        public void initializeButton()
        {
            RoundButton button1 = new RoundButton();
            Font buttonFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            button1.BackColor = Color.FromArgb(143, 122, 101);
            button1.ForeColor = Color.White;
            button1.Font = buttonFont;
            button1.Text = "New Game";
            button1.Size = new Size(120, 50);
            button1.Location = new Point((this.Width - 3 * d - 4 * labelWidth) / 2 - d + 4 * labelWidth + 5 * d - 109, 75);
            button1.TextAlign = ContentAlignment.MiddleCenter;
            button1.Click += new EventHandler(this.button1_Click);
            button1.MouseEnter += (s, e) => button1.Cursor = Cursors.Hand;
            button1.MouseLeave += (s, e) => button1.Cursor = Cursors.Arrow;

            this.Controls.Add(button1);
        }

        public void button1_Click(object sender, EventArgs e)
        {
            lose = false;
            startNewGame();
            changeColor();
            updateScore();
        }

        public void initializeLabels()
        {
            startPointY = label4.Location.Y + label4.Height + 40;
            Point firstPoint = new Point((this.Width - 3 * d - 4 * labelWidth) /2, startPointY);

            for (int i = 0; i < N * N; i++)
            {
                RoundLabel lbl = new RoundLabel();
                int r = i / N;
                int c = i % N;
                lbl.Text = "";
                lbl.Height = labelWidth;
                lbl.Width = labelWidth;
                lbl.BackColor = Color.FromArgb(205, 193, 179);
                lbl.ForeColor = Color.FromArgb(122, 114, 103);
                lbl.Visible = true;
                lbl.Font = bigFont;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Location = new Point(firstPoint.X + r * (labelWidth + d), firstPoint.Y + c * (labelWidth + d));

                labels[r, c] = lbl;
                this.Controls.Add(lbl);
            }
        }

        public void drawBackground()
        {
            startPointY = label4.Location.Y + label4.Height + 40;
            Point firstPoint = new Point((this.Width - 3 * d - 4 * labelWidth) / 2 - d, startPointY - d);

            RectangleF background = new RectangleF(firstPoint.X - 10, firstPoint.Y - 10, 4 * labelWidth + 5 * d + 20, 4 * labelWidth + 5 * d + 20);
            this.CreateGraphics().FillPath(new SolidBrush(Color.FromArgb(187, 173, 160)), GetRoundPath(background, 10));
            //this.CreateGraphics().FillRectangle(new SolidBrush(Color.FromArgb(187, 173, 160)), firstPoint.X, firstPoint.Y, 4 * labelWidth + 5 * d, 4 * labelWidth + 5 * d);

            firstPoint = new Point((this.Width - 3 * d - 4 * labelWidth) / 2, startPointY);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    this.CreateGraphics().FillPath(new SolidBrush(Color.FromArgb(205, 193, 179)), GetRoundPath(new RectangleF(new Point(firstPoint.X + i * (labelWidth + d), firstPoint.Y + j * (labelWidth + d)), new Size(labelWidth, labelWidth)), 4));
        }

        public void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            moved = false;
            if (!lose)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                    case Keys.Up:
                        PressedUp();
                        break;
                    case Keys.A:
                    case Keys.Left:
                        PressedLeft();
                        break;
                    case Keys.S:
                    case Keys.Down:
                        PressedDown();
                        break;
                    case Keys.D:
                    case Keys.Right:
                        PressedRight();
                        break;
                    default:
                        break;
                }
            }
        }

        public void startNewGame()
        {
            //Initialize score and label settings
            ScoreLabel.Text = 0.ToString();
            BestLabel.Text = best.ToString();
            score = 0;

            //Clear labels
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    labels[i, j].Text = "";
                }
            }

            //Spawn two random blocks
            spawnRandom();
            spawnRandom();
            changeColor();
        }

        public bool spawnRandom()
        {
            //Check for empty boxes
            List<int> emptyBlocks = new List<int>();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (labels[i, j].Text == "") emptyBlocks.Add(10*i+j);
                }
            }

            if (emptyBlocks.Count == 0)
                return false;
            else
            {
                //Generate random 2 or 4
                int twoFour = (rand.NextDouble() < 0.9) ? 2 : 4;

                //Change random block
                int chosenBlock = rand.Next(emptyBlocks.Count);

                labels[(int)Math.Floor((decimal)emptyBlocks[chosenBlock] / 10), emptyBlocks[chosenBlock] % 10].Text = twoFour.ToString();
                
                return true;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawBackground();
            changeColor();
            drawLabelBack();
        }

        public void PressedUp()
        {
            for (int i = 0; i < N; i++)
            {
                //Initialize movement
                for (int j = 0; j < N; j++) movement[j, i] = 0;

                //Read same column
                int[] temp = new int[4];
                int[] result = new int[4] { 0, 0, 0, 0 };
                for (int j = 0; j < N; j++)
                    temp[j] = (labels[i, j].Text == "") ? 0 : Convert.ToInt16(labels[i, j].Text);

                //Combine and move to front
                int slot = 0, count = 0, countPos = 0;
                for (int j = 0; j < N; j++)
                {
                    if (temp[j] != 0)
                    {
                        if (count == 0)
                        {
                            count = temp[j];
                            countPos = j;
                        }
                        else
                        {
                            if (temp[j] == count)
                            {
                                result[slot] = 2 * temp[j];
                                movement[j, i] = (j - slot) * (labelWidth + d);
                                movement[countPos, i] = (countPos - slot) * (labelWidth + d);
                                count = 0;
                                score += result[slot];
                            }
                            else
                            {
                                result[slot] = count;
                                movement[countPos, i] = (countPos - slot) * (labelWidth + d);
                                count = temp[j];
                                countPos = j;
                            }
                            slot++;
                        }
                    }
                }
                if (slot < N && count != 0)
                {
                    result[slot] = count;
                    movement[countPos, i] = (countPos - slot) * (labelWidth + d);
                }

                //Check if moved
                if (!moved)
                    for (int j = 0; j < N; j++)
                        if (temp[j] != result[j])
                        {
                            direction = 0;
                            moved = true;
                            break;
                        }

                //Put in board
                for (int j = 0; j < N; j++) finalBoard[i, j] = result[j];
            }
            if (moved)
            {
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (labels[i, j].Text != "")
                            labels[i, j].BringToFront();
                        else
                            labels[i, j].SendToBack();

                times = 0;
                lose = true;
                timer1.Start();
            }

            /*//Console write movement
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                    Console.Write(movement[i, j] + " ");
                Console.WriteLine("");
            }
            Console.WriteLine("");*/
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            times++;
            switch (direction)
            {
                case 0:
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < N; j++)
                        {
                            labels[i, j].Location = new Point(labels[i, j].Location.X, labels[i, j].Location.Y - movement[j, i] / 3);
                        }
                    break;
                case 1:
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < N; j++)
                        {
                            labels[i, j].Location = new Point(labels[i, j].Location.X - movement[j, i] / 3, labels[i, j].Location.Y);
                        }
                    break;
                default:
                    break;
            }

            if (times == 3)
            {
                timer1.Stop();
                Point firstPoint = new Point((this.Width - 3 * d - 4 * labelWidth) / 2, startPointY);
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                    {
                        labels[i, j].Location = new Point(firstPoint.X + i * (labelWidth + d), firstPoint.Y + j * (labelWidth + d));
                        labels[i, j].Text = (finalBoard[i, j] == 0) ? "" : finalBoard[i, j].ToString();
                    }

                spawnRandom();
                changeColor();
                updateScore();
                lose = false;
                if (checkIfLose())
                {
                    lose = true;
                    MessageBox.Show("Game Over!");
                }
            }
        }

        public void PressedLeft()
        {
            for (int i = 0; i < N; i++)
            {
                //Initialize movement
                for (int j = 0; j < N; j++) movement[i, j] = 0;

                //Read same row
                int[] temp = new int[4];
                int[] result = new int[4] { 0, 0, 0, 0 };
                for (int j = 0; j < N; j++)
                    temp[j] = (labels[j, i].Text == "") ? 0 : Convert.ToInt16(labels[j, i].Text);

                //Combine and move to front
                int slot = 0, count = 0, countPos = 0;
                for (int j = 0; j < N; j++)
                {
                    if (temp[j] != 0)
                    {
                        if (count == 0)
                        {
                            count = temp[j];
                            countPos = j;
                        }
                        else
                        {
                            if (temp[j] == count)
                            {
                                result[slot] = 2 * temp[j];
                                movement[i, j] = (j - slot) * (labelWidth + d);
                                movement[i, countPos] = (countPos - slot) * (labelWidth + d);
                                count = 0;
                                score += result[slot];
                            }
                            else
                            {
                                result[slot] = count;
                                movement[i, countPos] = (countPos - slot) * (labelWidth + d);
                                count = temp[j];
                                countPos = j;
                            }
                            slot++;
                        }
                    }
                }
                if (slot < N && count != 0)
                {
                    result[slot] = count;
                    movement[i, countPos] = (countPos - slot) * (labelWidth + d);
                }

                //Check if moved
                for (int j = 0; j < N; j++)
                    if (temp[j] != result[j])
                    {
                        direction = 1;
                        moved = true;
                        break;
                    }
                //Put in board
                for (int j = 0; j < N; j++) finalBoard[j, i] = result[j];
            }
            if (moved)
            {
                //Front back
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (labels[i, j].Text != "")
                            labels[i, j].BringToFront();
                        else
                            labels[i, j].SendToBack();

                /*//Console write movement
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                        Console.Write(movement[i, j] + " ");
                    Console.WriteLine("");
                }
                Console.WriteLine("");*/

                //Move animation
                times = 0;
                lose = true;
                timer1.Start();
            }
        }

        public void PressedDown()
        {
            for (int i = 0; i < N; i++)
            {
                //Initialize movement
                for (int j = 0; j < N; j++) movement[j, i] = 0;

                //Read same column
                int[] temp = new int[4];
                int[] result = new int[4] { 0, 0, 0, 0 };
                for (int j = 0; j < N; j++)
                    temp[j] = (labels[i, j].Text == "") ? 0 : Convert.ToInt16(labels[i, j].Text);

                //Combine and move to front
                int slot = N - 1, count = 0, countPos = 0;
                for (int j = N - 1; j > -1; j--)
                {
                    if (temp[j] != 0)
                    {
                        if (count == 0)
                        {
                            count = temp[j];
                            countPos = j;
                        }
                        else
                        {
                            if (temp[j] == count)
                            {
                                result[slot] = 2 * temp[j];
                                movement[j, i] = (j - slot) * (labelWidth + d);
                                movement[countPos, i] = (countPos - slot) * (labelWidth + d);
                                count = 0;
                                score += result[slot];
                            }
                            else
                            {
                                result[slot] = count;
                                movement[countPos, i] = (countPos - slot) * (labelWidth + d);
                                count = temp[j];
                                countPos = j;
                            }
                            slot--;
                        }
                    }
                }
                if (slot > -1 && count != 0)
                {
                    result[slot] = count;
                    movement[countPos, i] = (countPos - slot) * (labelWidth + d);
                }

                //Check if moved
                if (!moved)
                    for (int j = 0; j < N; j++)
                        if (temp[j] != result[j])
                        {
                            direction = 0;
                            moved = true;
                            break;
                        }

                //Put in board
                for (int j = 0; j < N; j++) finalBoard[i, j] = result[j];
            }

            if (moved)
            {
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (labels[i, j].Text != "")
                            labels[i, j].BringToFront();
                        else
                            labels[i, j].SendToBack();

                times = 0;
                lose = true;
                timer1.Start();
            }
        }

        public void PressedRight()
        {
            for (int i = 0; i < N; i++)
            {
                //Initialize movement
                for (int j = 0; j < N; j++) movement[i, j] = 0;
                
                //Read same column
                int[] temp = new int[4];
                int[] result = new int[4] { 0, 0, 0, 0 };
                for (int j = 0; j < N; j++)
                    temp[j] = (labels[j, i].Text == "") ? 0 : Convert.ToInt16(labels[j, i].Text);

                //Combine and move to front
                int slot = N - 1, count = 0, countPos = 0;
                for (int j = N - 1; j > -1; j--)
                {
                    if (temp[j] != 0)
                    {
                        if (count == 0)
                        {
                            count = temp[j];
                            countPos = j;
                        }
                        else
                        {
                            if (temp[j] == count)
                            {
                                result[slot] = 2 * temp[j];
                                movement[i, j] = (j - slot) * (labelWidth + d);
                                movement[i, countPos] = (countPos - slot) * (labelWidth + d);
                                count = 0;
                                score += result[slot];
                            }
                            else
                            {
                                result[slot] = count;
                                movement[i, countPos] = (countPos - slot) * (labelWidth + d);
                                count = temp[j];
                                countPos = j;
                            }
                            slot--;
                        }
                    }
                }
                if (slot > -1 && count != 0)
                {
                    result[slot] = count;
                    movement[i, countPos] = (countPos - slot) * (labelWidth + d);
                }

                //Check if moved
                if (!moved)
                    for (int j = 0; j < N; j++)
                        if (temp[j] != result[j])
                        {
                            direction = 1;
                            moved = true;
                            break;
                        }
                //Put in board
                for (int j = 0; j < N; j++) finalBoard[j, i] = result[j];
            }

            if (moved)
            {
                //Front back
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (labels[i, j].Text != "")
                            labels[i, j].BringToFront();
                        else
                            labels[i, j].SendToBack();

                /*//Console write movement
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                        Console.Write(movement[i, j] + " ");
                    Console.WriteLine("");
                }
                Console.WriteLine("");*/

                //Move animation
                times = 0;
                lose = true;
                timer1.Start();
            }
        }

        public void changeColor()
        {
            startPointY = label4.Location.Y + label4.Height + 40;
            Point firstPoint = new Point((this.Width - 3 * d - 4 * labelWidth) / 2, startPointY);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    tile = ((int)Math.Log((labels[i, j].Text == "") ? 0 : Convert.ToInt16(labels[i, j].Text), 2) - 1);
                    labels[i, j].ForeColor = (tile < 2) ? Color.FromArgb(122, 114, 103) : Color.FromArgb(245, 246, 251);
                    labels[i, j].Font = (tile < 9) ? bigFont : smallFont;
                    labels[i, j].BackColor = (labels[i, j].Text == "")? Color.FromArgb(205, 193, 179) : TileColors[tile];
                    //this.CreateGraphics().DrawRectangle(new Pen(Color.Black, 2), firstPoint.X + i * (labelWidth + d) - 2, firstPoint.Y + j * (labelWidth + d) - 2, labelWidth + 4, labelWidth + 4);
                }
            }
        }

        public bool checkIfLose()
        {
            for (int r = 0; r < N; r++)
            {
                for (int c = 0; c < N; c++)
                {
                    if (labels[r, c].Text == "") return false;

                    //check same column
                    for (int i = r - 1; i < r + 2; i += 2)
                        if (i >= 0 && i < N)
                            if (labels[i, c].Text == labels[r, c].Text) return false;

                    //check same row
                    for (int i = c - 1; i < c + 2; i += 2)
                        if (i >= 0 && i < N)
                            if (labels[r, i].Text == labels[r, c].Text) return false;
                }
            }

            return true;
        }

        public void updateScore()
        {
            ScoreLabel.Text = score.ToString();

            //put score in middle
            int scorex = (int)(label1.Location.X + label1.Width / 2 - ScoreLabel.Width / 2);
            int scorey = ScoreLabel.Location.Y;
            Point scorePoint = new Point(scorex, scorey);
            ScoreLabel.Location = scorePoint;
            
            if (score > best)
            {
                best = score;
                BestLabel.Text = best.ToString();

                //put in middle
                scorex = (int)(label2.Location.X + label2.Width / 2 - BestLabel.Width / 2);
                scorey = BestLabel.Location.Y;
                scorePoint = new Point(scorex, scorey);
                BestLabel.Location = scorePoint;
            }
        }
    }
}
