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
    public class GameMap
    {
        List<List<IEnemy>> enemies = null;
        IBoss boss = null;
        Player player;
        Graphics g;
        Brush whiteBrush = new SolidBrush(Color.White);
        Pen myPen = new Pen(Color.White);
        Image enemyImg, bossImg, playerImg;
        Random rand;
        Font smallFont = new Font("Ariel", 15);
        List<Rectangle> barriers;
        Dictionary<int, IShot> enemiesShots;  //because if you kill enemy after he shot, shot will disapear
        Difficulty diff;
        int dim;
        int bounds;
        int enemiesMoveX;
        int step = 0;
        int shotWidth;
        int lvl,stage;
        public GameMap(Graphics g, int dimension, Difficulty diff)
        {
            enemiesShots = new Dictionary<int, IShot>();
            this.g = g;
            dim = dimension;
            this.diff = diff;
            rand = new Random();
            bounds = 30;
            shotWidth = dim / 80;
            enemiesMoveX = 10;
        }
        public bool IsPlayerAlive()
        {
            return player.IsAlive();
        }
        public bool ShotsExist()
        {
            return (enemiesShots.Count > 0) ? true : false;
        }
        public bool EnemiesAlive()
        {
            if (boss == null)
            {
                if (enemies != null && enemies.Count > 0)
                {
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (boss.GetLifes() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        void ClearScreen()
        {
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, dim, dim);
        }
        void CreateBarriers()
        {
            barriers = new List<Rectangle>();
            int barWidth = dim / 40;
            int y = 8 * dim / 10;
            int k;
            k = dim / 4;
            for (int i = -2; i <= 2; i++)
            {
                barriers.Add(new Rectangle(k + i*barWidth - barWidth/2, y, barWidth, barWidth));
            }
            k = dim / 2;
            for (int i = -2; i <= 2; i++)
            {
                barriers.Add(new Rectangle(k + i * barWidth - barWidth / 2, y, barWidth, barWidth));
            }
            k = 3 * dim / 4;
            for (int i = -2; i <= 2; i++)
            {
                barriers.Add(new Rectangle(k + i * barWidth - barWidth / 2, y, barWidth, barWidth));
            }
        }
        public void LoadPlayer()
        {
            playerImg = Image.FromFile(@"player.png");
            int player_height = dim / (10 * 2);
            int player_width = (int)Math.Floor(player_height * Math.E);
            
            player = new Player(dim/2 - player_width, dim - player_height - dim/10,
                                player_width/2, player_height, dim/53);
            int lives = 0;
            switch (diff)
            {
                case Difficulty.EASY:
                    lives = 15;
                    break;
                case Difficulty.MEDIUM:
                    lives = 10;
                    break;
                case Difficulty.HARD:
                    lives = 5;
                    break;
            }
            player.SetLives(lives);
        }
        public void LoadStage(int lvl, int stage)
        {
            ClearScreen();
            enemiesMoveX = 10;
            this.lvl = lvl;
            this.stage = stage;
            Random rand = new Random();
            enemyImg = Image.FromFile(@"" + lvl + ".png");
            enemiesShots.Clear();
            if (stage == 3)
            {
                enemies = null;
                bossImg = Image.FromFile(@"boss_" + lvl + ".png");
                switch (lvl)
                {
                    case 1:
                        boss = new BossEnemy1(dim / 2 - 30, bounds + 20, dim / 6, dim / 10);
                        break;
                    case 2:
                        boss = new BossEnemy2(dim / 2 - 30, bounds + 20, dim / 6, dim / 10);
                        break;
                    case 3:
                        boss = new BossEnemy3(dim / 2 - 30, bounds + 20, dim / 6, dim / 10);
                        break;
                    case 4:
                        boss = new BossEnemy4(dim / 2 - 30, bounds + 20, dim / 6, dim / 10);
                        break;
                    case 5:
                        boss = new BossEnemy5(dim / 2 - 30, bounds + 20, dim / 6, dim / 10);
                        break;
                }
            }
            else
            {
                if (stage == 1)
                {
                    CreateBarriers();
                }
                boss = null;
                enemies = new List<List<IEnemy>>();
                int k = 0;
                string[] enemyStage = System.IO.File.ReadAllLines(@"L-" + lvl + "_S-" + stage + ".txt");
                foreach (string strnum in enemyStage)
                {
                    int num = int.Parse(strnum);
                    enemies.Add(new List<IEnemy>());
                    for (int i = 0; i < num; i++)
                    {
                        enemies[k].Add(new BasicEnemy(lvl, dim/2 - 200 + k*50, 50 + i*50, 40, 30));
                        g.DrawImage(enemyImg, enemies[k][i].GetRect());
                    }
                    k++;
                }
            }
            ShowPlayer();
        }
        void ShowPlayer()
        {
            g.DrawImage(playerImg, player.GetRectangle());
        }
        void ShowShot(IShot shot)
        {
            Point shotPoint = shot.GetPoint();
            switch (shot.GetType())
            {
                case 'b':
                    double angle = shot.GetAngle();
                    g.DrawLine(myPen,
                        (int)Math.Round(shotPoint.X + shotWidth * Math.Cos(angle)), (int)Math.Round(shotPoint.Y - shotWidth * Math.Sin(angle)),
                        (int)Math.Round(shotPoint.X - shotWidth * Math.Cos(angle)), (int)Math.Round(shotPoint.Y + shotWidth * Math.Sin(angle)));
                    break;
                case 'x':
                        g.FillEllipse(whiteBrush, shotPoint.X - shotWidth, shotPoint.Y - shotWidth,
                        2 * shotWidth, 2 * shotWidth);
                    break;
            }
            
        }
        public Player GetPlayer()
        {
            return player;
        }
        public int GetScore()
        {
            return player.score;
        }
        void ShowLives()
        {
            g.DrawString("Lives: " + player.GetLives(), smallFont, whiteBrush, new PointF(bounds,bounds/3));
        }
        void ShowBoss()
        {
            g.DrawImage(bossImg, boss.GetRect());
        }
        void ShowBossHealth()
        {
            int bossMaxLives = boss.GetMaxLifes();
            int bossLives = boss.GetLifes();
            float curW = myPen.Width;
            myPen.Width = 3 * curW;
            int leftSide = dim / 4 + (bossMaxLives - bossLives) * dim / (2* bossMaxLives);
            int rightSide = dim / 2 - (bossMaxLives - bossLives) * dim / (2* bossMaxLives);
            g.FillRectangle(new SolidBrush(Color.Blue), leftSide, bounds/3, rightSide, 2 * bounds / 3);
            g.DrawRectangle(myPen, dim / 4, bounds / 3, dim / 2, 2* bounds / 3);
            myPen.Width = curW;
        }
        void ShowStage()
        {
            g.DrawString("Stage " + lvl + ":" + stage,smallFont, whiteBrush, new PointF(5*dim/6,bounds / 3));
        }
        void ShowScore()
        {
            int score = player.score;
            g.DrawString("Score:" + score, smallFont, whiteBrush, new PointF(dim / 20, Convert.ToSingle(dim - 2.5*bounds)));
        }
        void ShowBarriers()
        {
            Brush brush = new SolidBrush(Color.LightGreen);
            for (int i = 0; i < barriers.Count; i++)
            {
                g.FillRectangle(brush, barriers[i]);
            }
        }
        void PrintOut()
        {
            ClearScreen();
            if (boss == null)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    for (int k = 0; k < enemies[i].Count; k++)
                    {
                        g.DrawImage(enemyImg, enemies[i][k].GetRect());
                    }
                }
            }
            else
            {
                if (boss.GetLifes() != 0)
                {
                    ShowBoss();
                }
                ShowBossHealth();
            }

            foreach (KeyValuePair<int, IShot> entry in enemiesShots)
            {
                ShowShot(entry.Value);
            }

            // Modifikasi untuk render semua shots player
            foreach (var shot in player.GetShots())
            {
                ShowShot(shot);
            }

            ShowScore();
            if (player.Show())
            {
                ShowPlayer();
            }
            ShowBarriers();
            ShowLives();
            ShowStage();
        }

        public void Update()
        {
            Rectangle playerRect;
            step =  (step + 1) % (6-lvl);
            /* ########## MOVING PART ########## */

            // check left and right column of enemies if they need to change direction and move them
            // check every 5th time
            if (boss == null)
            {
                if (step == 0 && enemies.Count > 0)
                {
                    int x_left = enemies[0][0].GetRect().X;
                    int x_right = enemies[enemies.Count - 1][0].GetRect().Right;
                    int y = 0;
                    if (x_right >= dim - bounds || x_left <= bounds) //change of direction
                    {
                        enemiesMoveX = -enemiesMoveX;
                        y = 10;
                    }

                    int i = 0, k;
                    while (i < enemies.Count)
                    {
                        k = 0;
                        while (k < enemies[i].Count)
                        {
                            enemies[i][k].Move(enemiesMoveX, y);
                            k++;
                        }
                        i++;
                    }
                }
            }
            else
            {
                if (step == 0)
                {
                    boss.Move(dim, bounds, player.GetPoint());
                }
            }

            //move player
            switch (player.GetMovement())
            {
                case MovementDirection.dontMove:
                    //do nothing
                    break;
                case MovementDirection.moveLeft:
                    if (! (player.GetRectangle().Left <= bounds))
                    {
                        player.Move(-10);
                    }
                    break;
                case MovementDirection.moveRight:
                    if (!(player.GetRectangle().Right >= dim - bounds))
                    {
                        player.Move(10);
                    }
                    break;
            }

            //move shots(player)
            var playerShots = player.GetShots();
            for (int i = playerShots.Count - 1; i >= 0; i--)
            {
                var shot = playerShots[i];
                shot.Update();
            }

            //move shots(enemies)
            Dictionary<int, IShot> addDict = new Dictionary<int, IShot>();
            List<int> removeList = new List<int>();
            foreach (KeyValuePair<int, IShot> entry in enemiesShots)
            {
                IShot shot = entry.Value;
                switch (shot.GetType())
                {
                    case 'b':
                        shot.Update();
                        break;
                    case 'x':
                        if (shot.Multiply())
                        {
                            removeList.Add(entry.Key);
                            int countBefore = addDict.Count;
                            List<IShot> listS = shot.GetShots();
                            //add them to directory
                            int i = 0; //key
                            int k = 0; //index of shot
                            while (addDict.Count < listS.Count + countBefore)
                            {
                                if (!enemiesShots.ContainsKey(i) && !addDict.ContainsKey(i))
                                {
                                    addDict.Add(i, listS[k]);
                                    k++;
                                }
                                i++;
                            }
                    }
                        else
                        {
                            shot.Update();
                        }
                        break;
                }
            }
            for (int i = 0; i < removeList.Count; i++)
            {
                enemiesShots.Remove(removeList[i]);
            }
            if (addDict.Count > 0)
            {
                foreach (KeyValuePair<int, IShot> entry in addDict)
                {
                    enemiesShots.Add(entry.Key, entry.Value);
                }
            }
            addDict = new Dictionary<int, IShot>();

            /* ########## BOUNDS AND HITBOXS' CHECK ########### */

            //player's shots collision check
            for (int shotIndex = playerShots.Count - 1; shotIndex >= 0; shotIndex--)
            {
                var shot = playerShots[shotIndex];
                if (boss == null) // Normal enemies stage
                {
                    bool breakLoop = false;
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        for (int k = 0; k < enemies[i].Count; k++)
                        {
                            Rectangle rect = enemies[i][k].GetRect();
                            Point shotPoint = shot.GetPoint();
                            bool vertical = shotPoint.Y < rect.Bottom && shotPoint.Y > rect.Top;
                            bool horizontal = shotPoint.X < rect.Right && shotPoint.X > rect.Left;
                            if (vertical && horizontal)
                            {
                                //remove shot
                                player.DestroyShot(shotIndex);
                                player.RaiseScore(20);
                                //destroy enemy
                                enemies[i].RemoveAt(k);
                                if (enemies[i].Count == 0)
                                {
                                    enemies.RemoveAt(i);
                                    i -= 1;
                                    break;
                                }
                                breakLoop = true;
                                break;
                            }
                        }
                        if (breakLoop)
                        {
                            breakLoop = false;
                            break;
                        }
                    }
                }
                else // Boss stage
                {
                    Rectangle bossRect = boss.GetRect();
                    Point shotPoint = shot.GetPoint();
                    bool vertical = shotPoint.Y < bossRect.Bottom && shotPoint.Y > bossRect.Top;
                    bool horizontal = shotPoint.X < bossRect.Right && shotPoint.X > bossRect.Left;

                    // Debug print untuk cek kalkulasi collision
                    Console.WriteLine($"Shot pos: {shotPoint.X}, {shotPoint.Y}");
                    Console.WriteLine($"Boss rect: Left={bossRect.Left}, Right={bossRect.Right}, Top={bossRect.Top}, Bottom={bossRect.Bottom}");
                    Console.WriteLine($"Collision check: vertical={vertical}, horizontal={horizontal}");

                    if (vertical && horizontal)
                    {
                        // Tambah debug print
                        Console.WriteLine("Hit boss!");

                        //remove shot
                        player.DestroyShot(shotIndex);
                        //remove life
                        boss.RemoveLife();
                        Console.WriteLine($"Boss life remaining: {boss.GetLifes()}");

                        if (boss.GetLifes() == 0)
                        {
                            player.RaiseScore(500);
                            Console.WriteLine("Boss defeated!");
                            playerShots.Clear();
                            enemiesShots.Clear();
                        }
                    }
                }
            }

            //enemies' shots colliding with barriers
            List<int> keysToRemove = new List<int>();
            foreach (KeyValuePair<int, IShot> entry in enemiesShots)
            {
                Point tempPoint = entry.Value.GetPoint();
                int? barIndex = null;
                for (int i = 0; i < barriers.Count; i++)
                {
                    Rectangle barRect = barriers[i];
                    bool vertical = tempPoint.Y > barRect.Top && tempPoint.Y < barRect.Bottom;
                    bool horizontal = tempPoint.X >= barRect.Left && tempPoint.X <= barRect.Right;
                    if (vertical && horizontal)
                    {
                        barIndex = i;
                        keysToRemove.Add(entry.Key);
                        break;
                    }
                }
                if (barIndex.HasValue)
                {
                    barriers.RemoveAt(barIndex.Value);
                }
            }
            for (int i = 0; i < keysToRemove.Count; i++)
            {
                enemiesShots.Remove(keysToRemove[i]);
            }

            //player's shots colliding with barriers
            for (int shotIndex = playerShots.Count - 1; shotIndex >= 0; shotIndex--)
            {
                var shot = playerShots[shotIndex];
                Point tempPoint = shot.GetPoint();
                int? barIndex = null;
                for (int i = 0; i < barriers.Count; i++)
                {
                    Rectangle barRect = barriers[i];
                    bool vertical = tempPoint.Y > barRect.Top && tempPoint.Y < barRect.Bottom;
                    bool horizontal = tempPoint.X >= barRect.Left && tempPoint.X <= barRect.Right;
                    if (vertical && horizontal)
                    {
                        barIndex = i;
                        player.DestroyShot(shotIndex);
                        break;
                    }
                }
                if (barIndex.HasValue)
                {
                    barriers.RemoveAt(barIndex.Value);
                }
            }

            //enemies' shots
            playerRect = player.GetRectangle();
            keysToRemove = new List<int>();
            foreach (KeyValuePair<int, IShot> entry in enemiesShots)
            {
                IShot tempShotEnemy = entry.Value;
                Point shotPoint = tempShotEnemy.GetPoint();
                bool vertical = shotPoint.Y < playerRect.Bottom && shotPoint.Y > playerRect.Top;
                bool horizontal = shotPoint.X < playerRect.Right && shotPoint.X > playerRect.Left;
                if (vertical && horizontal)
                {
                    keysToRemove.Add(entry.Key);
                    player.RemoveLife();
                }
            }
            for (int i = 0; i < keysToRemove.Count; i++)
            {
                enemiesShots.Remove(keysToRemove[i]);
            }

            //shot out of bounds(enemies)
            keysToRemove = new List<int>();
            foreach (KeyValuePair<int, IShot> entry in enemiesShots)
            {
                Point shotPoint = entry.Value.GetPoint();
                bool vertical = shotPoint.Y > dim || shotPoint.Y < 0;
                bool horizontal = shotPoint.X > dim || shotPoint.X < 0;
                if (vertical || horizontal)
                {
                    keysToRemove.Add(entry.Key);
                }
            }
            for (int i = 0; i < keysToRemove.Count; i++)
            {
                enemiesShots.Remove(keysToRemove[i]);
            }

            //check shots out of bounds (player)
            for (int shotIndex = playerShots.Count - 1; shotIndex >= 0; shotIndex--)
            {
                var shot = playerShots[shotIndex];
                Point shotPoint = shot.GetPoint();
                if (shotPoint.Y < 0)
                {
                    player.DestroyShot(shotIndex);
                }
            }

            //check if enemy doesn't colide with player
            if (boss == null) //no need to check boss for this
            {
                //player Rectangle already declared
                for (int i = 0; i < enemies.Count; i++)
                {
                    int lastIndex = enemies[i].Count - 1;
                    Rectangle lowestEnemyRect = enemies[i][lastIndex].GetRect();
                    Point leftLow = new Point(lowestEnemyRect.Left, lowestEnemyRect.Bottom);
                    Point rightLow = new Point(lowestEnemyRect.Right, lowestEnemyRect.Bottom);
                    bool vert = leftLow.Y > playerRect.Top; //both corners have same Y
                    bool horL = leftLow.X > playerRect.Left && leftLow.X < playerRect.Right;
                    bool horR = rightLow.X > playerRect.Left && rightLow.X < playerRect.Right;
                    if (vert && (horL || horR))
                    {
                        while (player.GetLives() > 0)
                        {
                            player.RemoveLife();
                        }
                    }
                }
            }

            /* ########## ENEMIES SHOOTING ########### */
            if (boss == null)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    int chance = 1 + (rand.Next() % 1000);
                    int lastIndex = enemies[i].Count - 1;
                    if (chance < lvl * (stage + 1) && !enemiesShots.ContainsKey(i))
                    {
                        enemiesShots.Add(i, enemies[i][lastIndex].Shoot()[0]); //fix the shots!!!
                    }
                }
            }
            else
            {
                if (boss.GetLifes() > 0)
                {
                    List<IShot> list = boss.Shoot(player);
                    int k = 0; //index in list
                    int i = 0; //index in dictionary
                    while (k < list.Count)
                    {
                        if (!enemiesShots.ContainsKey(i))
                        {
                            enemiesShots.Add(i, list[k]);
                            k++;
                        }
                        i++;
                    }
                }
            }

            /* ############# PRINTING OUT ############## */
            PrintOut();
        }
    }
}
