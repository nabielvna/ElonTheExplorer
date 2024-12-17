using System;
using System.Collections.Generic;
using System.Drawing;

namespace Elon_The_Explorer
{
    public class BossEnemy1 : IBoss
    {
        Rectangle rect;
        byte shotCount, curShotCount = 0;
        int maxLives, lives, moveAmount;
        public BossEnemy1(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
            shotCount = 30;
            curShotCount = shotCount;
            maxLives = 5;
            lives = maxLives;
            moveAmount = 20;
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

        public void Move(int x, int y = 0)
        {
            rect.X += x;
            rect.Y += y;
        }

        public void Move(int dim, int bounds, Point playerPoint)
        {
            int playerX = playerPoint.X;
            int centerX = rect.Left + rect.Width / 2;
            if (Math.Abs(playerX - centerX) <= moveAmount)
            {
                if (playerX == centerX) { return; }
                rect.X += playerX - centerX;
            }
            else
            {
                rect.X += Math.Sign(playerX - centerX) * moveAmount;
            }
        }

        public void RemoveLife()
        {
            lives -= 1;
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
                List<IShot> list = new List<IShot>();
                int x = rect.Left + rect.Width / 2;
                int y = rect.Bottom;
                list.Add(new ExplodingShot(x, y, 0, 10));
                curShotCount = shotCount;
                return list;
            }
        }

        public List<IShot> Shoot()
        {
            return Shoot(null);
        }
    }
}
