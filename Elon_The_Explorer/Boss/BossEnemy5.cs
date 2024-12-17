using System;
using System.Collections.Generic;
using System.Drawing;

namespace Elon_The_Explorer
{
    public class BossEnemy5 : IBoss
    {
        Rectangle rect;
        byte shotCount, curShotCount;
        byte maxLives, lives, moveAmount;
        ShotCycle shotType;
        double angleMove, counter;
        MovementDirection movement;
        public BossEnemy5(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
            maxLives = 5;
            lives = maxLives;
            angleMove = 0;
            moveAmount = 10;
            movement = MovementDirection.moveRight;
            shotType = ShotCycle.basic;
            SetShotCount();
            curShotCount = shotCount;
        }
        void SetShotCount()
        {
            switch (shotType)
            {
                case ShotCycle.basic:
                    shotCount = 20;
                    break;
                case ShotCycle.shotgun:
                    shotCount = 30;
                    break;
                case ShotCycle.exploding:
                    shotCount = 30;
                    break;
                case ShotCycle.doubleExploding:
                    shotCount = 30;
                    break;
            }
        }
        public int GetLifes()
        {
            return lives;
        }

        public int GetMaxLifes()
        {
            return maxLives;
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
            rect.Y += (int)Math.Round(10 * Math.Cos(counter));
            counter += Math.PI / 12;
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
            throw new NotImplementedException();
        }

        public void RemoveLife()
        {
            lives -= 1;
            if (shotType != ShotCycle.doubleExploding)
            {
                shotType += 1;
                SetShotCount();
            }

        }

        public List<IShot> Shoot(Player player)
        {
            if (curShotCount != 0)
            {
                curShotCount -= 1;
                return new List<IShot>();
            }
            else
            {
                List<IShot> list = new List<IShot>();
                Point playerPoint = playerPoint = player.GetPoint();
                Point shotStart = new Point();
                int x = 0;
                int y = 0;
                double angle = 0;
                switch (shotType)
                {
                    case ShotCycle.basic:
                        shotStart = new Point();
                        shotStart.Y = rect.Bottom;
                        shotStart.X = rect.Left + rect.Width / 2;
                        angle = 0;
                        x = playerPoint.X - shotStart.X;
                        y = playerPoint.Y - shotStart.Y;

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

                        list.Add(new BasicShot(shotStart.X, shotStart.Y,
                                Convert.ToSingle(15 * Math.Cos(angle)),
                            -Convert.ToSingle(15 * Math.Sin(angle))));
                        break;
                    case ShotCycle.shotgun:
                        shotStart = new Point();
                        shotStart.Y = rect.Bottom;
                        shotStart.X = rect.Left + rect.Width / 2;
                        angle = 0;
                        x = playerPoint.X - shotStart.X;
                        y = playerPoint.Y - shotStart.Y;
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
                                 Convert.ToSingle(15 * Math.Cos(angle + i * Math.PI / 12)),
                                -Convert.ToSingle(15 * Math.Sin(angle + i * Math.PI / 12))));
                        }
                        break;
                    case ShotCycle.exploding: // SHOOTS TO THE RIGHT SIZE
                        x = rect.Left + rect.Width / 2;
                        y = rect.Bottom;
                        list.Add(new ExplodingShot(x, y, 0, 10));
                        break;
                    case ShotCycle.doubleExploding:
                        shotStart.Y = rect.Bottom;
                        shotStart.X = rect.Left + (rect.Width / 2);
                        x = playerPoint.X - shotStart.X;
                        y = playerPoint.Y - shotStart.Y;
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

                        int divisor = 10;
                        list.Add(new ExplodingShot(shotStart.X, shotStart.Y,
                                Convert.ToSingle(15 * Math.Cos(angle - 1 * Math.PI / divisor)),
                            Convert.ToSingle(15 * Math.Sin(angle - 1 * Math.PI / divisor))));
                        list.Add(new ExplodingShot(shotStart.X, shotStart.Y,
                                Convert.ToSingle(15 * Math.Cos(angle + 1 * Math.PI / divisor)),
                            Convert.ToSingle(15 * Math.Sin(angle + 1 * Math.PI / divisor))));

                        curShotCount = shotCount;
                        return list;
                }
                curShotCount = shotCount;
                return list;
            }
        }

        public List<IShot> Shoot()
        {
            throw new NotImplementedException();
        }
    }
}
