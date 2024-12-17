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
    public class BasicShot : IShot
    {
        readonly char type;
        PointF center;
        PointF moving;
        double moveDist;
        double angle;

        public BasicShot(int x, int y, float moveX, float moveY)
        {
            type = 'b';
            center = new PointF(x,y);
            moving = new PointF(moveX, moveY);
            moveDist = Math.Sqrt(moveX*moveX + moveY*moveY);
        }
        public BasicShot(float x, float y, float moveX, float moveY)
        {
            type = 'b';
            center = new PointF(x, y);
            moving = new PointF(moveX, moveY);
            moveDist = Math.Sqrt(moveX * moveX + moveY * moveY);
        }
        public Point GetPoint()
        {
            return new Point((int)Math.Round(center.X), (int)Math.Round(center.Y));
        }
        public void Update()
        {
            center.X += moving.X;
            center.Y -= moving.Y;
        }
        public char GetType()
        {
            return type;
        }
        public bool Multiply()
        {
            throw new NotImplementedException();
        }

        public List<IShot> GetShots()
        {
            throw new NotImplementedException();
        }

        public double GetAngle()
        {
            double x = moving.X;
            double y = moving.Y;
            double tanAngle = y / x;
            double possibleAngle = Math.Atan(tanAngle);
            return possibleAngle;
        }
    }
}
