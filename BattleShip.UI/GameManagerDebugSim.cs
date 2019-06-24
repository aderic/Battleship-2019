using System;
using System.Diagnostics;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    /// <summary>
    /// A game object that represents a Bot vs Bot scenario, used for debugging purposes.
    /// </summary>
    class GameManagerDebugSim : GameManagerBase
    {
        protected BotBase[] bots = new BotBase[MAX_PLAYERS];

        /// <summary>
        /// Instantiates both bots, giving then fixed names.
        /// </summary>
        protected override void SetupPlayers()
        {
            Output.WriteLine();

            players[PLAYER_ONE] = new Player($"Bot 1 (Advanced)");
            players[PLAYER_TWO] = new Player($"Bot 2");

            bots[PLAYER_ONE] = new BotCheaty(players[PLAYER_TWO].Board);
            bots[PLAYER_TWO] = new BotRealistic(players[PLAYER_ONE].Board);
        }

        /// <summary>
        /// Tells the bots to randomly decide the positions and directions of their ships.
        /// </summary>
        protected override void SetupShipPlacements()
        {
            ShipType[] shipTypes = (ShipType[])Enum.GetValues(typeof(ShipType));

            // For each ship type..
            foreach (ShipType shipType in shipTypes)
            {
                // Have both bots randomly place ships.
                for (int i = 0; i < MAX_PLAYERS; i++)
                {
                    PlaceShipRequest placeRequest = new PlaceShipRequest();
                    placeRequest.ShipType = shipType;

                    ShipPlacement? placeResponse = null;

                    do
                    {
                        placeRequest.Coordinate = BotBase.GetRandomCoordinate();
                        placeRequest.Direction = BotBase.GetRandomDirection();
                        placeResponse = players[i].Board.PlaceShip(placeRequest);
                    } while (placeResponse != ShipPlacement.Ok);
                }
            }
        }

        /// <summary>
        /// Takes a turn for the given bot, getting their decision and updating/outputting the board.
        /// </summary>
        /// <param name="botNumber">The bot who is shooting.</param>
        /// <returns>A response for the shot the bot fired on their turn.</returns>
        protected override FireShotResponse TakeTurn(int botNumber)
        {
            int enemyIndex = Utils.GetEnemyOfPlayerNumber(botNumber);

            Player player = players[botNumber];
            BotBase botPlayer = bots[botNumber];

            Debug.Write($"{player.Name}: ");

            Coordinate chosenCoords;
            FireShotResponse shotResponse = botPlayer.TakeTurn(out chosenCoords);
            player.Stats.Update(shotResponse.ShotStatus);
            return shotResponse;
        }
    }
}
