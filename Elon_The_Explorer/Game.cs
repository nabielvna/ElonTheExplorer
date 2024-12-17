using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;

namespace Elon_The_Explorer
{
    public class Game
    {
        GameMap gameMap;
        //timer resets in the beginning of every stage and doesn't speed up during boss fight
        Timer timer_game;
        Timer timer_start;
        bool start_show = false;
        int lvl, stage;
        int endScore;
        TimeSpan timeSpan;
        bool won = false;
        readonly int dimension;
        readonly Graphics g;
        SizeF blinkSize;
        Screen screen;
        public bool isPaused { get; private set; }
        public bool isMuted { get; private set; }
        bool? wasMuted = null;
        string playerName;

        Font titleFont = new Font("Times New Roman", 32, FontStyle.Italic);
        Font mainFont = new Font("Ariel", 30);
        Font smallFont = new Font("Ariel", 20);
        Brush mainBrush = new SolidBrush(Color.White);
        Brush blackBrush = new SolidBrush(Color.Black);
        Stopwatch stopwatch;
        Difficulty diff;
        SoundPlayer soundPlayerStart, soundPlayerGame, soundPlayerLeaderboard;
        List<Leaderboard> listLB;

        public Game(Graphics graphics, int dimension, Timer timer_start, Timer timer_game)
        {
            g = graphics;
            this.dimension = dimension;
            this.timer_start = timer_start;
            this.timer_game = timer_game;
            stopwatch = new Stopwatch();
            soundPlayerStart = new SoundPlayer(@"Dystopic-Mayhem.wav");
            soundPlayerStart.Load();
            soundPlayerGame = new SoundPlayer(@"Space-Game-Loop.wav");
            soundPlayerGame.Load();
            soundPlayerLeaderboard = new SoundPlayer(@"8-Bit-Mayhem.wav");
            soundPlayerLeaderboard.Load();
        }
        public void play()
        {
            if (gameMap.IsPlayerAlive())
            {
                gameMap.Update();
                bool eAlive = gameMap.EnemiesAlive();
                bool sExist = gameMap.ShotsExist();
                if (eAlive == false && sExist == false)
                {
                    this.NextStage();
                }
            }
            else
            {
                endScore = gameMap.GetScore();
                screen += 1;
                EndGame();
            }
        }
        public Player GetPlayer()
        {
            return gameMap.GetPlayer();
        }
        public Screen GetScreen()
        {
            return screen;
        }
        public void SetPlayerName(string name)
        {
            playerName = name;
        }
        public void SetDiff(Difficulty diff)
        {
            this.diff = diff;
            switch (this.diff)
            {
                case Difficulty.EASY:
                    timer_game.Interval = (int)Math.Floor(1000 / 30.0);
                    break;
                case Difficulty.MEDIUM:
                    timer_game.Interval = (int)Math.Floor(1000 / 45.0);
                    break;
                case Difficulty.HARD:
                    timer_game.Interval = (int)Math.Floor(1000 / 60.0);
                    break;
            }
        }
        public void StartGame()
        {
            // SOLVE MUTING
            gameMap = new GameMap(g, dimension, diff);
            soundPlayerLeaderboard.Stop();
            soundPlayerStart.Stop();
            soundPlayerGame.PlayLooping();
            stopwatch.Reset();
            lvl = 1;
            stage = 1;
            endScore = 0;
            isPaused = false;
            isMuted = false;
            timeSpan = new TimeSpan(0, 0, 0);
            timer_start.Stop();
            gameMap.LoadPlayer();
            gameMap.LoadStage(lvl, stage);
            screen = Screen.game;
            timer_game.Start();
            stopwatch.Start();
        }
        void ClearScreen()
        {
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, dimension, dimension);
        }
        public void StartScreen()
        {
            //maybe wrap it in object would be nicer, but who really cares..?

            soundPlayerStart.PlayLooping();

            screen = Screen.start;
            string[] s = { "press P to pause, M to mute", "use A and D to move", "use J to shoot" };
            ClearScreen();
            PrintStringCenter("Elon The Explorer", mainBrush, titleFont, dimension / 2, 50);
            for (int i = 0; i < s.Length; i++)
            {
                PrintStringCenter(s[i], mainBrush, mainFont, dimension / 2, 100 + (i + 1) * 50);
            }

            blinkSize = g.MeasureString("Choose difficulity, enter your name and press ENTER to start", smallFont);
            timer_start.Interval = 500;
            timer_start.Start();
        }
        public void Blink()
        {
            if (start_show)
            {
                PrintStringCenter("Choose difficulty, enter your name and press ENTER to start", mainBrush, smallFont, dimension / 2, dimension - 120);
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.Black), 
                    new RectangleF(dimension / 2 - blinkSize.Width / 2, dimension - 120 - blinkSize.Height / 2,
                    blinkSize.Width, blinkSize.Height));
            }
            start_show = !start_show;
        }
        private void Timer_start_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void PrintStringCenter(string s, Brush brush, Font font, int x, int y)
        {
            SizeF size = g.MeasureString(s, font);
            g.DrawString(s, font, brush, (int)Math.Ceiling(x - (size.Width / 2)), (int)Math.Ceiling(y - (size.Height / 2)));
        }
        public void PauseOrUnpause()
        {
            if (isPaused) //resume
            {
                isPaused = !isPaused;
                if (wasMuted.HasValue && !wasMuted.Value)
                {
                    MuteOrUnmute();
                    wasMuted = null;
                }
                stopwatch.Start();
                timer_game.Start();
            }
            else //pause
            {
                wasMuted = isMuted;
                if (!wasMuted.Value)
                {
                    MuteOrUnmute();
                }
                stopwatch.Stop();
                timer_game.Stop();
                isPaused = !isPaused;
            }
        }
        public void MuteOrUnmute()
        {
            if (isPaused)
            {
                return;
            }
            switch (screen)
            {
                case Screen.start:
                    if (isMuted)
                    {
                        soundPlayerStart.PlayLooping();
                    }
                    else
                    {
                        soundPlayerStart.Stop();
                    }
                    isMuted = !isMuted;
                    break;
                case Screen.game:
                    if (isMuted)
                    {
                        soundPlayerGame.PlayLooping();
                    }
                    else
                    {
                        soundPlayerGame.Stop();
                    }
                    isMuted = !isMuted;
                    break;
                case Screen.leaderboard:
                    if (isMuted)
                    {
                        soundPlayerLeaderboard.PlayLooping();
                    }
                    else
                    {
                        soundPlayerLeaderboard.Stop();
                    }
                    isMuted = !isMuted;
                    break;
            }
        }
        void NextStage()
        {
            if (stage == 3)
            {
                if (lvl == 5)
                {
                    won = true;
                    endScore = gameMap.GetScore();
                    screen += 1;
                    EndGame();
                }
                else
                {
                    stage = 1;
                    lvl += 1;
                    gameMap.LoadStage(lvl, stage);
                }
            }
            else
            {
                stage += 1;
                gameMap.LoadStage(lvl, stage);
            }
        }
        void EndGame()
        {
            soundPlayerGame.Stop();
            if (!isMuted)
            {
                soundPlayerLeaderboard.PlayLooping();
            }
            timer_game.Stop();
            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            g.FillRectangle(blackBrush, 0, 0, dimension, dimension);
            if (won)
            {
                this.Celebration();
            }
            else
            {
                this.GameOver();
            }
            screen += 1;
            this.LeaderboardsScreen();
            PrintStringCenter("Press R to restart or press ENTER to end", mainBrush, smallFont, 
                        dimension / 2, 18 * dimension / 20);
        }
        void Celebration()
        {
            PrintStringCenter("CONGRATULATIONS",mainBrush, titleFont, dimension / 2, dimension / 20);
            PrintStringCenter("YOU WON!", mainBrush, titleFont, dimension / 2, 2 * dimension / 20);
            PrintStringCenter("Your time: " + stopwatch.Elapsed, mainBrush, titleFont, dimension / 2, 3 * dimension / 20);
        }
        void GameOver()
        {
            PrintStringCenter("GAME OVER", mainBrush, titleFont, dimension / 2, dimension / 20);
            PrintStringCenter("Your score: " + endScore, mainBrush, titleFont, dimension / 2, 2 * dimension / 20);
        }
        void LeaderboardsScreen()
        {
            Leaderboard thisLeader = new Leaderboard(playerName, timeSpan, endScore.ToString());
            List<string> lines = new List<string>();
            listLB = new List<Leaderboard>();
            int i;
            {
                string[] lineArray = null;
                switch (diff)
                {
                    case Difficulty.EASY:
                        lineArray = System.IO.File.ReadAllLines(@"leaderboard_easy.txt");
                        break;
                    case Difficulty.MEDIUM:
                        lineArray = System.IO.File.ReadAllLines(@"leaderboard_medium.txt");
                        break;
                    case Difficulty.HARD:
                        lineArray = System.IO.File.ReadAllLines(@"leaderboard_hard.txt");
                        break;
                }
                for (i = 0; i < lineArray.Length; i++)
                {
                    lines.Add(lineArray[i]);
                }
            }
            if (lines.Count % 3 != 0)
            {
                Console.WriteLine("Problem with leaderboards file.");
            }
            for (i = 0; i < 30 && i < lines.Count; )
            {
                string tempName = lines[i++];
                string tempTimespanLine = lines[i++];
                string tempScore = lines[i++];
                listLB.Add(new Leaderboard(tempName, tempTimespanLine, tempScore));
            }
            ResortLeaderboard(listLB, thisLeader);
            ShowLeaderboard();
            SaveLeaderboard();
        }
        void ResortLeaderboard(List<Leaderboard> list, Leaderboard playerLB)
        {
            int i;
            int inLB = 1;
            for ( i = 0; i < list.Count; i++) //get desired index for your score
            {
                if ( list[i].score < playerLB.score || (list[i].score == playerLB.score && list[i].time > playerLB.time))
                {
                    break;
                }
                inLB += 1;
            }
            if (inLB <= 10)
            {
                if (list.Count == 10)
                {
                    list.RemoveAt(9);
                }
                list.Add(playerLB);

                Leaderboard temp;
                for (int k = list.Count - 1; k > i; k--) //set on index
                {
                    temp = listLB[k - 1];
                    listLB[k - 1] = list[k];
                    listLB[k] = temp;
                }
            }
        }
        void ShowLeaderboard()
        {
            for (int i = 0; i < listLB.Count; i++)
            {
                Leaderboard temp = listLB[i];
                g.DrawString((i+1) + ". " + temp.name, smallFont, mainBrush, dimension / 5, (5 + i) * dimension / 20);
                g.DrawString(temp.score.ToString(), smallFont, mainBrush, 2 * dimension / 5, (5 + i) * dimension / 20);
                g.DrawString(temp.time.ToString(), smallFont, mainBrush, 3 * dimension / 5, (5 + i) * dimension / 20);
            }
        }
        void SaveLeaderboard()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < listLB.Count; i++)
            {
                Leaderboard temp = listLB[i];
                list.Add(temp.name);
                string tempStr = "";
                tempStr += temp.time.Hours + ",";
                tempStr += temp.time.Minutes + ",";
                tempStr += temp.time.Seconds + ",";
                tempStr += temp.time.Milliseconds;
                list.Add(tempStr);
                list.Add(temp.score.ToString());
            }
            switch (diff)
            {
                case Difficulty.EASY:
                    System.IO.File.WriteAllLines(@"leaderboard_easy.txt", list);
                    break;
                case Difficulty.MEDIUM:
                    System.IO.File.WriteAllLines(@"leaderboard_medium.txt", list);
                    break;
                case Difficulty.HARD:
                    System.IO.File.WriteAllLines(@"leaderboard_hard.txt", list);
                    break;
            }
        }
        public void Credits()
        {
            screen += 1;
            ClearScreen();
            PrintStringCenter("Thank you for playing!", mainBrush, titleFont, 
                dimension / 2, dimension / 20);
            PrintStringCenter("Vidiawan Nabiel Arrasyid 5025221231", mainBrush, mainFont,
                dimension / 2, 3 * dimension / 20);
            PrintStringCenter("Fanza Khairan Pratama 5025221305", mainBrush, mainFont,
                dimension / 2, 5 * dimension / 20);
            PrintStringCenter("Press any key to close", mainBrush, mainFont,
                dimension / 2, 17 * dimension / 20);
        }

    }
}
