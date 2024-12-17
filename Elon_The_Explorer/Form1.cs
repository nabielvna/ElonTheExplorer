using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Elon_The_Explorer
{
    public partial class Form1 : Form
    {
        //############# CUSTOM METHODS #############

        void ClearScreen()
        {
            g.FillRectangle(blackBrush, 0, 0, dimension, dimension);
        }

        public Form1()
        {
            InitializeComponent();
        }

        //int level = 1, stage = 1;
        int dimension; //=800
        Bitmap bmp;
        Graphics g;
        Brush blackBrush = new SolidBrush(Color.Black);
        Brush mainBrush;
        Game game;
        bool start = true;
        bool pause = false;
        Player player = null;
        string playerName;


        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle rect = System.Windows.Forms.Screen.FromControl(this).Bounds;
            int smallerDim = (rect.Width < rect.Height) ? rect.Width : rect.Height;
            dimension = (int)Math.Round(smallerDim * 0.8);
            this.Width = dimension;
            this.Height = dimension;
            this.CenterToScreen();
            this.MaximumSize = new Size(dimension,dimension);
            this.MinimumSize = new Size(dimension, dimension);
            bmp = new Bitmap(this.Width, this.Height);
            g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;
            ClearScreen();
            mainBrush = new SolidBrush(Color.White);
            LoadPlayerName();
            MoveComboBox();
            MoveLabel2();
            MoveTextbox();
            MoveLabel1();

            game = new Game(g, dimension, timer_start_screen, timer_game);
            game.StartScreen();
            pictureBox1.Refresh();
        }
        void LoadPlayerName()
        {
             playerName = System.IO.File.ReadAllLines(@"name.txt")[0];
             textBox1.Text = playerName;
        }
        void SavePlayerName()
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"name.txt", false))
            {
                file.WriteLine(playerName);
            }
        }
        void MoveComboBox()
        {
            Point nextLoc = new Point();
            nextLoc.X = dimension / 3;
            nextLoc.Y = dimension * 2 / 3;
            nextLoc.X -= comboBox1.Width / 2;
            comboBox1.Location = nextLoc;
        }
        void MoveLabel2()
        {
            Point nextLoc = new Point();
            nextLoc.X = dimension / 3;
            nextLoc.Y = dimension * 2 / 3 - 20;
            nextLoc.X -= label2.Width / 2;
            label2.Location = nextLoc;
        }
        void MoveTextbox()
        {
            textBox1.Width = dimension / 4;
            Point nextLoc = new Point();
            nextLoc.X = 2 * dimension / 3;
            nextLoc.Y = dimension * 2 / 3;
            nextLoc.X -= textBox1.Width / 2;
            textBox1.Location = nextLoc;
        }
        void MoveLabel1()
        {
            Point nextLoc = new Point();
            nextLoc.X = 2 * dimension / 3;
            nextLoc.Y = dimension * 2 / 3 - 20;
            nextLoc.X -= label1.Width / 2;
            label1.Location = nextLoc;
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void Timer_game_Tick(object sender, EventArgs e)
        {
            game.play();
            pictureBox1.Refresh();
        }

        private void Timer_start_screen_Tick(object sender, EventArgs e)
        {
            game.Blink();
            pictureBox1.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.M)
            {
                game.MuteOrUnmute();
            }
            switch (game.GetScreen())
            {
                case Screen.start:
                    if (!(comboBox1.SelectedIndex > -1) || textBox1.Text.Trim() == "")
                    {
                        return;
                    }
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            game.SetDiff(Difficulty.EASY);
                            break;
                        case 1:
                            game.SetDiff(Difficulty.MEDIUM);
                            break;
                        case 2:
                            game.SetDiff(Difficulty.HARD);
                            break;
                    }
                    SavePlayerName();
                    label2.Hide();
                    comboBox1.Hide();
                    label1.Hide();
                    textBox1.Hide();
                    game.StartGame();
                    player = game.GetPlayer();
                    SavePlayerName();
                    game.SetPlayerName(playerName);
                    pictureBox1.Refresh();
                    break;
                case Screen.game:
                    switch (e.KeyCode)
                    {
                        case Keys.P:
                            game.PauseOrUnpause();
                            break;
                        //case Keys.M:
                        //    game.MuteOrUnmute();
                        //    break;
                        default:
                            if (!game.isPaused)
                            {
                                player.resolveKey(e.KeyCode);
                            }
                            break;
                    }
                    break;
                case Screen.leaderboard:
                    switch (e.KeyCode)
                    {
                        case Keys.R:
                            game.StartScreen();
                            label2.Show();
                            comboBox1.Show();
                            label1.Show();
                            textBox1.Show();
                            break;
                        case Keys.Enter:
                            game.Credits();
                            pictureBox1.Refresh();
                            break;
                    }
                    break;
                case Screen.credits:
                    this.Close();
                    break;
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (player != null)
            {
                player.stopMoving(e.KeyCode);
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            playerName = textBox1.Text;
        }

        private void TextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
                KeyEventArgs keys = new KeyEventArgs(Keys.Enter);
                Form1_KeyDown(sender, keys);
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
