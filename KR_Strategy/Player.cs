using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_Strategy
{
    class Player
    {
        public Unit[,] playerUnits;
        public Base[,] playerBases;
        public Player()
        {
            playerUnits = Field.unitTiles;
            playerBases = Field.baseTiles;
        }
    }
}
