using System;
using BattleShip.BLL.Responses;

namespace BattleShip.UI
{
    abstract class GameManagerBase
    {
        public const int PLAYER_ONE = 0;
        public const int PLAYER_TWO = 1;
        public const int MAX_PLAYERS = 2;
        public const bool DEBUG_SHOW_SHIPS = false;

        protected Player[] players = new Player[MAX_PLAYERS];

        public void Execute()
        {
            // Prompts the players for their names and set up the player array.
            SetupPlayers();
            Output.ClearScreen();

            // Prompt the players to set up their ships.
            SetupShipPlacements();
            Output.ClearScreen();

            // Choose a random player to get the first turn for the new game.
            int curPlayerNumber = new Random().Next(0, MAX_PLAYERS);
            Output.PrintPlayerChosen(players[curPlayerNumber].Name);
            Input.WaitForAnyKey();

            // The rest of the game falls into this logic where we swap the player until it's no longer a victory.
            FireShotResponse shotResponse;

            do
            {
                shotResponse = TakeTurn(curPlayerNumber);
                curPlayerNumber = Utils.GetEnemyOfPlayerNumber(curPlayerNumber);
            } while (shotResponse.ShotStatus != ShotStatus.Victory);

            // Swap the player back so that we can get their name, they won.
            curPlayerNumber = Utils.GetEnemyOfPlayerNumber(curPlayerNumber);
            Output.PrintEndOfGameSummary(players, curPlayerNumber);
            Input.WaitForAnyKey();
        }

        /// <summary>
        /// Checks if the shot did happen, this occurs when the shot status is not invalid and not a duplicate.
        /// </summary>
        /// <param name="shotStatus">The status of the shot to check for.</param>
        /// <returns>TRUE if the shot was not a duplicate and not invalid, FALSE otherwise.</returns>
        protected bool DidShotHappen(ShotStatus shotStatus)
        {
            return shotStatus != ShotStatus.Duplicate && shotStatus != ShotStatus.Invalid;
        }

        /// <summary>
        /// Gets the player names and initializes them in the players array.
        /// </summary>
        protected abstract void SetupPlayers();

        /// <summary>
        /// Gets the player ship positions and places them on the board.
        /// </summary>
        protected abstract void SetupShipPlacements();

        /// <summary>
        /// Takes a turn for the given player, getting their decision and updating/outputting the board.
        /// </summary>
        /// <param name="playerNumber">The player who is shooting.</param>
        /// <returns>A response for the shot the player fired on their turn.</returns>
        protected abstract FireShotResponse TakeTurn(int playerNumber);
    }
}
