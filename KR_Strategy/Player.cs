using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_Strategy
{
    class Player
    {
        public string name;
        public string color;
        public Player(string name, string color)
        {
            this.name = name;
            this.color = color;
        }
        public int gasAmount = 800;
        public int mineralsAmount = 600;
        public Unit[,] playerUnits = new Unit[4, 9];
        public Base[,] playerBases = new Base[4, 9];
    }
}
