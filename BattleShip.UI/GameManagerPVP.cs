using System;
using BattleShip.BLL.Ships;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Requests;

namespace BattleShip.UI
{
    /// <summary>
    /// A game object that represents a Player vs Player scenario.
    /// </summary>
    class GameManagerPVP : GameManagerBase
    {
        /// <summary>
        /// Gets the player names and initializes them in the players array.
        /// </summary>
        protected override void SetupPlayers()
        {
            // Ask each player for their name.
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                Output.PrintPlayerNameQuestion(i);
                players[i] = new Player(Input.GetPlayerName());
            }
        }

        /// <summary>
        /// Gets the player ship positions and places them on the board.
        /// </summary>
        protected override void SetupShipPlacements()
        {
            ShipType[] shipTypes = (ShipType[])Enum.GetValues(typeof(ShipType));

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                Player player = players[i];

                // We just moved to player two, prompt player one to go get player two so that he can place his buildings next.
                if (i == PLAYER_TWO)
                {
                    Output.ClearScreen();
                    Output.PrintPlacementComplete(player.Name);
                    Input.WaitForAnyKey();
                }

                foreach (ShipType shipType in shipTypes)
                {
                    Output.ClearScreen();
                    Output.PrintBoard(player, true);

                    string shipName = shipType.ToString().ToLower();

                    PlaceShipRequest placeRequest = new PlaceShipRequest();
                    placeRequest.ShipType = shipType;
                    
                    ShipPlacement placeResponse;
                    do
                    {
                        Output.PrintPlacementQuestion(shipName);
                        placeRequest.Coordinate = Input.GetCoordinate();

                        Output.PrintDirectionQuestion(shipName);
                        placeRequest.Direction = Input.GetDirection();

                        placeResponse = player.Board.PlaceShip(placeRequest);

                        Output.ClearScreen();
                        Output.PrintBoard(player, true);
                        Output.PrintPlacementResponse(shipName, placeResponse);
                    } while (placeResponse != ShipPlacement.Ok);
                }
            }
        }

        /// <summary>
        /// Takes a turn for the given player, getting their decision and updating/outputting the board.
        /// </summary>
        /// <param name="playerNumber">The player who is shooting.</param>
        /// <returns>A response for the shot the player fired on their turn.</returns>
        protected override FireShotResponse TakeTurn(int playerNumber)
        {
            Player player = players[playerNumber];
            Player enemy = players[Utils.GetEnemyOfPlayerNumber(playerNumber)];

            Output.PrintBoard(enemy, player.Stats, DEBUG_SHOW_SHIPS);
            Output.PrintFireQuestion(player.Name);

            FireShotResponse shotResponse;

            do
            {
                Coordinate chosenCoordinates = Input.GetCoordinate();
                shotResponse = enemy.Board.FireShot(chosenCoordinates);

                if (DidShotHappen(shotResponse.ShotStatus))
                {
                    player.Stats.Update(shotResponse.ShotStatus);
                    Output.PrintBoard(enemy, player.Stats, DEBUG_SHOW_SHIPS);
                }

                Output.PrintFireShotResponse(shotResponse, chosenCoordinates);

                if (shotResponse.ShotStatus != ShotStatus.Duplicate)
                {
                    Input.WaitForAnyKey();
                }
            } while (!DidShotHappen(shotResponse.ShotStatus));

            return shotResponse;
        }
    }
}
