using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elon_The_Explorer
{
    public class Leaderboard
    {
        public readonly string name;
        public readonly TimeSpan time;
        public readonly int score;
        public Leaderboard(string nameLine, string timespanLine, string scoreLine)
        {
            name = nameLine;
            string[] timespanStr = timespanLine.Split(',');
            int hours = int.Parse(timespanStr[0]);
            int minutes = int.Parse(timespanStr[1]);
            int seconds = int.Parse(timespanStr[2]);
            int miliseconds = int.Parse(timespanStr[3]);
            time = new TimeSpan(0, hours, minutes, seconds, miliseconds);
            score = int.Parse(scoreLine);
        }
        public Leaderboard(string nameLine, TimeSpan span, string scoreLine)
        {
            name = nameLine;
            time = span;
            score = int.Parse(scoreLine);
        }
    }
}
