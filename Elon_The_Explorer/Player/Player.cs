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

namespace Elon_The_Explorer
{
    public class Player
    {
        byte lives;
        bool alive, show;
        bool leftKeyDown = false, rightKeyDown = false;
        bool isShootKeyDown = false; // Track shoot key state
        int shotSpeed;
        public int score { get; private set; }
        public int counter { get; private set; }
        Rectangle rect;
        Stopwatch stopwatch;

        private List<IShot> shots;
        private Stopwatch shotCooldown;
        private const int COOLDOWN_MS = 250;

        MovementDirection movement;
        Keys shootKeyCode1 = Keys.J, shootKeyCode2 = Keys.Space;
        Keys leftKeyCode1 = Keys.A, leftKeyCode2 = Keys.Left;
        Keys rightKeyCode1 = Keys.D, rightKeyCode2 = Keys.Right;

        public Player(int x, int y, int w, int h, int shotSpeed)
        {
            rect = new Rectangle(x, y, w, h);
            movement = MovementDirection.dontMove;
            shots = new List<IShot>();
            alive = true;
            show = true;
            this.shotSpeed = shotSpeed;
            score = 0;
            counter = 0;
            stopwatch = new Stopwatch();
            stopwatch.Reset();
            shotCooldown = new Stopwatch();
        }

        public Rectangle GetRectangle()
        {
            return rect;
        }

        public void SetLives(int number)
        {
            lives = (byte)number;
        }

        public bool Show()
        {
            if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds < 2000)
            {
                counter += 1;
                if (counter == 5)
                {
                    show = !show;
                    counter = 0;
                }
            }
            else
            {
                stopwatch.Reset();
                show = true;
                counter = 0;
            }
            return show;
        }

        public Point GetPoint()
        {
            Point point = new Point();
            point.X = rect.Left + (rect.Width / 2);
            point.Y = rect.Top + (rect.Height / 2);
            return point;
        }

        public List<IShot> GetShots()
        {
            return shots;
        }

        public void DestroyShot(int index)
        {
            if (index >= 0 && index < shots.Count)
            {
                shots.RemoveAt(index);
            }
        }

        public void RaiseScore(int i)
        {
            score += i;
        }

        public bool IsAlive()
        {
            return alive;
        }

        public void Move(int x, int y = 0)
        {
            rect.X += x;
            rect.Y += y;
        }

        public MovementDirection GetMovement()
        {
            return movement;
        }

        public void resolveKey(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.D:
                case Keys.Right:
                    movement = MovementDirection.moveRight;
                    rightKeyDown = true;
                    break;
                case Keys.A:
                case Keys.Left:
                    movement = MovementDirection.moveLeft;
                    leftKeyDown = true;
                    break;
                case Keys.J:
                case Keys.Space:
                    // Only shoot if the key wasn't already down and cooldown has elapsed
                    if (!isShootKeyDown && (!shotCooldown.IsRunning || shotCooldown.ElapsedMilliseconds > COOLDOWN_MS))
                    {
                        shots.Add(new BasicShot(rect.Left + rect.Width / 2, rect.Top, 0, 15));
                        shotCooldown.Restart();
                        isShootKeyDown = true;
                    }
                    break;
            }
        }

        public void stopMoving(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.A:
                case Keys.Left:
                    leftKeyDown = false;
                    if (rightKeyDown)
                    {
                        movement = MovementDirection.moveRight;
                    }
                    else
                    {
                        movement = MovementDirection.dontMove;
                    }
                    break;
                case Keys.D:
                case Keys.Right:
                    rightKeyDown = false;
                    if (leftKeyDown)
                    {
                        movement = MovementDirection.moveLeft;
                    }
                    else
                    {
                        movement = MovementDirection.dontMove;
                    }
                    break;
                case Keys.J:
                case Keys.Space:
                    isShootKeyDown = false; // Reset shoot key state when released
                    break;
            }
        }

        public void RemoveLife()
        {
            if (!stopwatch.IsRunning)
            {
                lives -= 1;
                stopwatch.Start();
                if (lives == 0)
                {
                    alive = false;
                }
            }
        }

        public int GetLives()
        {
            return lives;
        }
    }
}