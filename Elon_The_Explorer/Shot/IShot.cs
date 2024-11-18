using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Elon_The_Explorer
{
    public interface IShot
    {
        char GetType();
        double GetAngle();
        Point GetPoint();
        bool Multiply();
        List<IShot> GetShots();
        void Update();
    }
}
