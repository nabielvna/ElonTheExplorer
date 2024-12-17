using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elon_The_Explorer
{
    //for player and bosses
    public enum MovementDirection { moveLeft, moveRight, dontMove };
    //for Game class and easier use of KeyUp/KeyDown
    public enum Screen { start, game, gameover, leaderboard, credits };
    //for final boss
    public enum ShotCycle { basic, shotgun, exploding, doubleExploding};
    public enum Difficulty { EASY, MEDIUM, HARD };
}
