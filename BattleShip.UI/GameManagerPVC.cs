using System;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    /// <summary>
    /// A game object that represents a Player vs Bot scenario.
    /// </summary>
    class GameManagerPVC : GameManagerBase
    {
        protected BotBase bot;

        /// <summary>
        /// Gets the player names and initializes them in the players array.
        /// </summary>
        protected override void SetupPlayers()
        {
            // Get player one's name (the human).
            Output.PrintPlayerNameQuestion(PLAYER_ONE);
            players[PLAYER_ONE] = new Player(Input.GetPlayerName());

            // Player TWO is the bot, so we'll use an appropriate name.
            players[PLAYER_TWO] = new Player("Your computer");
            bot = new BotAdvanced(players[PLAYER_ONE].Board);
        }

        /// <summary>
        /// Gets the player ship positions and places them on the board.
        /// </summary>
        protected override void SetupShipPlacements()
        {
            ShipType[] shipTypes = (ShipType[])Enum.GetValues(typeof(ShipType));

            // For each ship type..
            foreach (ShipType shipType in shipTypes)
            {
                for (int i = 0; i < MAX_PLAYERS; i++)
                {
                    // Ask the player for the ships they want to place.
                    if (i == PLAYER_ONE)
                    {
                        Output.ClearScreen();
                        Output.PrintBoard(players[PLAYER_ONE], true);

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

                            placeResponse = players[PLAYER_ONE].Board.PlaceShip(placeRequest);

                            Output.ClearScreen();
                            Output.PrintBoard(players[PLAYER_ONE], true);
                            if (placeResponse == ShipPlacement.Ok) {
                                Output.PrintPlacementResponse(shipName, placeResponse);
                            }
                        } while (placeResponse != ShipPlacement.Ok);
                        
                    } // The bot will randomly pick positions to place their ships.
                    else
                    {
                        PlaceShipRequest placeRequest = new PlaceShipRequest();
                        placeRequest.ShipType = shipType;
                        
                        do
                        {
                            placeRequest.Coordinate = BotRealistic.GetRandomCoordinate();
                            placeRequest.Direction = BotRealistic.GetRandomDirection();
                        } while (players[PLAYER_TWO].Board.PlaceShip(placeRequest) != ShipPlacement.Ok);
                    }
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
            FireShotResponse shotResponse;
            Player player = players[playerNumber];
            Player enemy = players[Utils.GetEnemyOfPlayerNumber(playerNumber)];

            // The human player gets asked what they want to do for this turn.
            if (playerNumber == PLAYER_ONE)
            {
                Output.PrintBoard(enemy, player.Stats, DEBUG_SHOW_SHIPS);
                Output.PrintFireQuestion(player.Name);

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
            }
            else // The bot has its own logic for where it wants to fire.
            {
                Coordinate chosenCoords;
                shotResponse = bot.TakeTurn(out chosenCoords);
                player.Stats.Update(shotResponse.ShotStatus);

                Output.PrintBoard(enemy, player.Stats, DEBUG_SHOW_SHIPS);
                Output.PrintFireShotBotResponse(shotResponse, chosenCoords, players[PLAYER_TWO].Name);
                Input.WaitForAnyKey();
            }
            
            return shotResponse;
        }
    }
}
