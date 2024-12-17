using System.Collections.Generic;
using System.Drawing;

namespace Elon_The_Explorer
{
    public interface IBoss : IEnemy
    {
        int GetMaxLifes();
        void Move(int dim, int bounds, Point playerPoint);
        int GetLifes();
        void RemoveLife();
        List<IShot> Shoot(Player player);
    }
}
