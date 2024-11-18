using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elon_The_Explorer
{
    public class ExplodingShot : IShot
    {
        readonly char type;
        PointF center, moving;
        byte steps;
        readonly double moveDist;
        public ExplodingShot(int x, int y, float moveX, float moveY)
        {
            type = 'x';
            steps = 10;
            center = new PointF(x, y);
            moving = new PointF(moveX, moveY);
            moveDist = Math.Sqrt(moveX * moveX + moveY * moveY);
        }

        public double GetAngle()
        {
            double x = moving.X;
            double y = moving.Y;
            double tanAngle = y / x;
            double possibleAngle = Math.Atan(tanAngle);
            if (x < 0)
            {
                possibleAngle += Math.PI;
            }
            return possibleAngle;
        }

        public Point GetPoint()
        {
            Point ret = new Point((int)Math.Round(center.X),
                            (int)Math.Round(center.Y));
            return ret;
        }

        public List<IShot> GetShots()
        {
            List<IShot> list = new List<IShot>();
            int shotAmount = 8;
            Random rand = new Random();
            double startingAngle = (rand.Next() % 24) * Math.PI/12;
            for (int i = 0; i < 2*shotAmount; i++)
            {
                list.Add(new BasicShot(center.X, center.Y, 
                    (int)Math.Round(moveDist * Math.Cos(startingAngle + i * Math.PI / shotAmount)),
                    -(int)Math.Round(moveDist * Math.Sin(startingAngle + i * Math.PI / shotAmount))));
            }
            return list;
        }

        public bool Multiply()
        {
            return (steps == 0);
        }

        public void Update()
        {
            center.X += moving.X;
            center.Y += moving.Y;
            steps -= 1;
        }
        public char GetType()
        {
            return type;
        }
    }
}
