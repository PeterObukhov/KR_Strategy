namespace KR_Strategy
{
    class Player
    {
        public string name;
        //Цвет юнитов и баз игрока (красный или синий)
        public string color;
        public Player(string name, string color)
        {
            this.name = name;
            this.color = color;
        }
        //Количество газа у игрока
        public int gasAmount = 1000;
        //Количество минералов у игрока
        public int mineralsAmount = 1000;
        //Массив юнитов игрока
        public Unit[,] playerUnits = new Unit[4, 9];
        //Массив баз игрока
        public Base[,] playerBases = new Base[4, 9];
        //Проверка победы
        public bool WinCheck()
        {
            bool win = true;
            //Если у игрока нет баз, он проиграл
            foreach (Base playerBase in playerBases)
            {
                if (playerBase != null) win = false;
            }
            return win;
        }
    }
}
