using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.BLL.GameLogic
{
    enum BotSearchMode
    {
        Random,                     // Will fire totally randomly.
        FindDirection,              // Hit something randomly, and is now doing a circle to find neighboring coords.
        FollowDirection,            // Hit something again (after FindDirection) and now knows direction.
        FollowDirectionReversed     // Got to the end of the ship, but never sunk the ship. Reverse direction to find tiles we didn't hit.
    }
}
