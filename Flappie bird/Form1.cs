using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace Flappie_bird
{
    public partial class Form1 : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        public struct RECT
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }
        public struct POINT
        {
            public int X;
            public int Y;
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        int ScreenHeigh = Screen.PrimaryScreen.Bounds.Height;
        int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
        float gravity = 1f;
        float velocity = 0f;
        int score = 0;
        bool[] pipeScored = new bool[5];
        List<Pipe> TopPipes = new List<Pipe>();
        List<Pipe> BottomPipes = new List<Pipe>();
        Random rand = new Random();
        private string[] fakeAds = new string[]
        {
            "Ad space empty \n (just like my wallet)",
            "Error 404: \n Budget Not Found",
            "Buy ad space here \n for 1 pizza slice",
            "Your ad here \n (Please, I need \n college funds)",
            "AdBlocker.exe \n cannot block \n this poverty",
            "Sponsored by: \n Nobody. \n Absolutely nobody.",
            "This space is empty, \n like my bank account"
        };

        public Form1()
        {
            InitializeComponent();
            label2.Text = "Record score:" + Properties.Settings.Default.HightestScore;
            label1.Text = "Score: " + score.ToString();
            label3.Visible = false;
            label1.Visible = false;
            for (int i = 0; i < 5; i++)
            {
                Pipe newTop = new Pipe();
                Pipe newBottom = new Pipe();
                newTop.StartPosition = FormStartPosition.Manual;
                newBottom.StartPosition = FormStartPosition.Manual;
                newTop.TopMost = true;
                newBottom.TopMost = true;
                newTop.Left = ScreenWidth + (i * 500);
                newBottom.Left = ScreenWidth + (i * 500);
                TopPipes.Add(newTop);
                BottomPipes.Add(newBottom);
                TopPipes[i].Width = 200;
                BottomPipes[i].Width = 200;
                TopPipes[i].Top = 0;
                int randomHeigh = rand.Next(200, ScreenHeigh - 300);
                TopPipes[i].Height = randomHeigh;
                BottomPipes[i].Height = ScreenHeigh - TopPipes[i].Height - 150;
                TopPipes[i].SetAdText(fakeAds[rand.Next(fakeAds.Length)]);
                BottomPipes[i].SetAdText(fakeAds[rand.Next(fakeAds.Length)]);
                BottomPipes[i].Top = ScreenHeigh - BottomPipes[i].Height;
            }
            label1.BringToFront();
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            label2.Show();
            button1.Show();
        }

       
        private async void timer1_Tick(object sender, EventArgs e)
        {
            POINT currentMouse = new POINT();
            GetCursorPos(out currentMouse);
            velocity += gravity;;
            currentMouse.Y += (int)velocity;
            SetCursorPos(700,currentMouse.Y);
            for (int i = 0; i < TopPipes.Count; i++)
            {
                TopPipes[i].Left -= 5;
                BottomPipes[i].Left -= 5;
                if (TopPipes[i].Right <= 0)
                {
                    pipeScored[i] = false;
                    TopPipes[i].Width = 200;
                    BottomPipes[i].Width = 200;
                    TopPipes[i].Top = 0;
                    int randomHeigh = rand.Next(200, ScreenHeigh - 300);
                    TopPipes[i].Height = randomHeigh;
                    BottomPipes[i].Height = ScreenHeigh - TopPipes[i].Height - 150;
                    TopPipes[i].SetAdText(fakeAds[rand.Next(fakeAds.Length)]);
                    BottomPipes[i].SetAdText(fakeAds[rand.Next(fakeAds.Length)]);
                    BottomPipes[i].Top = ScreenHeigh - BottomPipes[i].Height;
                    TopPipes[i].Left += 2500;
                    BottomPipes[i].Left += 2500;
                }
                Rectangle hitBoxTop = new Rectangle(TopPipes[i].Left, TopPipes[i].Top, TopPipes[i].Width, TopPipes[i].Height);
                Rectangle hitBoxBottom = new Rectangle(BottomPipes[i].Left, BottomPipes[i].Top, BottomPipes[i].Width, BottomPipes[i].Height);
                if (hitBoxTop.Contains(700, currentMouse.Y) || hitBoxBottom.Contains(700, currentMouse.Y))
                    {
                    if (Properties.Settings.Default.HightestScore < score)
                    {
                        Properties.Settings.Default.HightestScore = score;
                        label2.Text = "Record score" + Properties.Settings.Default.HightestScore;
                        Properties.Settings.Default.Save();
                    }
                    timer1.Stop();
                    await Task.Delay(1000);
                    DialogResult answer = MessageBox.Show("Cursor crashed into the Window, Try again? (Press cancel to quit game)", "you had score of: " + score, MessageBoxButtons.YesNoCancel);
                    if (answer == DialogResult.Yes)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            TopPipes[j].Left = ScreenWidth + (j * 500);
                            BottomPipes[j].Left = ScreenWidth + (j * 500);
                        }
                        score = 0;
                        label1.Text = "Score: " + score.ToString();
                        velocity = 0;
                        timer1.Start();
                    }
                    if (answer == DialogResult.No)
                    {
                        score = 0;
                        label1.Text = "Score: " + score.ToString();
                        label2.Show();
                        button1.Show();
                        label1.Visible = false;
                        for (int a = 0; a < 5; a++)
                        {
                            TopPipes[a].Visible = false;
                            BottomPipes[a].Visible = false;
                            label2.Text = "Record score: " + Properties.Settings.Default.HightestScore;
                        }

                    }
                    if (answer == DialogResult.Cancel)
                    {
                        Application.Exit();
                        score = 0;
                        label1.Text = "Score: " + score.ToString();
                    }
                }
                if (TopPipes[i].Right < 700 && pipeScored[i] == false)
                {
                    score += 1;
                    label1.Text = "Score: " + score;
                    pipeScored[i] = true;
                }
            }
            if (currentMouse.Y <= 0 || currentMouse.Y >= ScreenHeigh)
            {
                if (Properties.Settings.Default.HightestScore < score)
                    {
                        Properties.Settings.Default.HightestScore = score;
                        label2.Text = "Record score" + Properties.Settings.Default.HightestScore;
                        Properties.Settings.Default.Save();
                    }
                timer1.Stop();
                await Task.Delay(1000);
                DialogResult answer = MessageBox.Show("Cursor crashed into the Edge, Try again? (Press cancel to quit game)", "you had score of: " + score, MessageBoxButtons.YesNoCancel);
                if (answer == DialogResult.Yes)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        TopPipes[j].Left = ScreenWidth + (j * 500);
                        BottomPipes[j].Left = ScreenWidth + (j * 500);
                    }
                    score = 0;
                    label1.Text = "Score: " + score.ToString();
                    velocity = 0;
                    timer1.Start();
                }
                if (answer == DialogResult.No)
                {
                    score = 0;
                    label1.Text = "Score: " + score.ToString();
                    label2.Show();
                    button1.Show();
                    label1.Visible = false;
                    for (int a = 0; a < 5; a++)
                    {
                        TopPipes[a].Visible = false;
                        BottomPipes[a].Visible = false;
                        label2.Text = "Record score: " + Properties.Settings.Default.HightestScore;
                    }

                }
                if (answer == DialogResult.Cancel)
                {   
                    Application.Exit();
                    score = 0;
                    label1.Text = "Score: " + score.ToString();
                }
            }
            
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space || e.KeyCode == Keys.Shift || e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            { velocity = -10;}
            if (e.KeyCode == Keys.Escape)
            {
                if (timer1.Enabled == true)
                {
                    timer1.Stop();
                    label3.Visible = true;
                }
                else
                {
                    timer1.Start();
                    label3.Visible = false;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            for (int k = 0; k < 5; k++)
            {
                TopPipes[k].TopMost = true;
                BottomPipes[k].TopMost= true;
                
            }
            
            this.TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Visible == true)
            {
                label1.Visible = true;
                button1.Visible = false;
                label2.Visible = false;

                score = 0;
                label2.Text = "Record score" + Properties.Settings.Default.HightestScore;
                label1.Text = "Score: " + score.ToString();
                for (int i = 0; i < 5; i++)
                {
                    TopPipes[i].Visible = true;
                    BottomPipes[i].Visible= true;
                    TopPipes[i].Left = ScreenWidth + (i * 500);
                    BottomPipes[i].Left = ScreenWidth + (i * 500);
                    pipeScored[i] = false;   
                }
                timer2.Start();
                label1.BringToFront();
                TopMost = true;
                WindowState = FormWindowState.Maximized;
                DoubleBuffered = true;
                timer1.Start();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
