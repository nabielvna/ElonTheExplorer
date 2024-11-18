using System;
using System.Collections.Generic;
using System.Drawing;

namespace Elon_The_Explorer
{
    public class BossEnemy2 : IBoss
    {
        Rectangle rect;
        byte shotCount, curShotCount;
        byte maxLives, lives, moveAmount;
        double angleMove, counter;
        MovementDirection movement;
        public BossEnemy2(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
            shotCount = 30;
            curShotCount = shotCount;
            maxLives = 5;
            lives = maxLives;
            counter = 0;
            angleMove = 0;
            moveAmount = 20;
            movement = MovementDirection.moveLeft;
        }
        public int GetLifes()
        {
            return lives;
        }

        public Rectangle GetRect()
        {
            return rect;
        }
        void Move()
        {
            switch (movement)
            {
                case MovementDirection.moveLeft:
                    rect.X -= moveAmount;
                    break;
                case MovementDirection.moveRight:
                    rect.X += moveAmount;
                    break;
            }
            rect.Y += (int)Math.Round(moveAmount * Math.Cos(counter));
            counter += Math.PI / 6;
        }
        public void Move(int dim, int bounds, Point playerPoint)
        {
            switch (movement)
            {
                case MovementDirection.moveLeft:
                    if (rect.Left < bounds)
                    {
                        movement = MovementDirection.moveRight;
                    }
                    break;
                case MovementDirection.moveRight:
                    if (rect.Right > dim - bounds)
                    {
                        movement = MovementDirection.moveLeft;
                    }
                    break;
            }
            Move();
        }

        public void Move(int x, int y = 0)
        {
            rect.X += x;
            rect.Y -= y;
        }

        public void RemoveLife()
        {
            lives -= 1;
        }

        public List<IShot> Shoot()
        {
            throw new NotImplementedException();
        }

        public List<IShot> Shoot(Player player)
        {
            if (curShotCount > 0)
            {
                curShotCount -= 1;
                return new List<IShot>();
            }
            else
            {
                Point playerPoint = player.GetPoint();
                Point shotStart = new Point();
                shotStart.Y = rect.Bottom;
                shotStart.X = rect.Left + rect.Width / 2;
                List<IShot> list = new List<IShot>();
                double angle = 0;
                int x = playerPoint.X - shotStart.X;
                int y = playerPoint.Y - shotStart.Y;
                if (x == 0)
                {
                    angle = 2 * Math.PI * (3 / 4);
                }
                else
                {
                    double angleTan = y / x;
                    angle = Math.Atan(angleTan);
                    if (angleTan < 0) { angle += Math.PI; }
                }

                for (int i = -2; i <= 2; i++)
                {
                    
                    list.Add(new BasicShot(shotStart.X, shotStart.Y,
                         Convert.ToSingle( 15 * Math.Cos(angle + i * Math.PI / 12)),
                        -Convert.ToSingle(15 * Math.Sin(angle + i * Math.PI / 12)) ));
                }

                curShotCount = shotCount;
                return list;
            }
        }

        public int GetMaxLifes()
        {
            return maxLives;
        }
    }
}
