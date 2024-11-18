using System.Collections.Generic;
using System.Drawing;
namespace Elon_The_Explorer
{
    public interface IEnemy
    {
        Rectangle GetRect();
        void Move(int x, int y = 0);
        List<IShot> Shoot();
    }
}
