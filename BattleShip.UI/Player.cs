using System;
using BattleShip.BLL.GameLogic;

namespace BattleShip.UI
{
    class Player
    {
        public string Name { get; private set; }
        public Board Board { get; private set; }
        public Stats Stats { get; private set; }

        public Player(string name)
        {
            Name = name;
            Board = new Board();
            Stats = new Stats();
        }
    }
}
