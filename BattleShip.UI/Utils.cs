using System;
using BattleShip.BLL.Responses;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;

namespace BattleShip.UI
{
    public static class Utils
    {
        /// <summary>
        /// Converts a string containing a coordinate into a coordinate object.
        /// </summary>
        /// <param name="strCoord">The string coordinate, e.g: A1, B5, etc.</param>
        /// <returns>A new coordinate object representing the coordinate in the string.</returns>
        public static Coordinate StrToCoordinate(string strCoord)
        {
            // No coordinates are bad coordinates.
            if (string.IsNullOrWhiteSpace(strCoord))
            {
                return null;
            }
            // Take any whitespace out.
            strCoord = strCoord.Trim();

            ushort col;
            // Checks if given coordinate is between 2-3 characters long.
            // The first char must be a letter, while the next 1-2 characters are numeric and not greater than the board's max coordinate.
            if (strCoord.Length < 2 || strCoord.Length > 3 || !char.IsLetter(strCoord[0]) || !ushort.TryParse(strCoord.Substring(1), out col))
            {
                return null;
            }

            // Convert the letter of the row to a number by getting the numeric offset from the character 'A', add 1 to it since valid rows start at 1.
            int row = char.ToUpper(strCoord[0]) - 'A' + 1;
            Coordinate coord = new Coordinate(col, row);

            // If the coordinate was not in range, throw it all away.
            if (!IsValidCoordinate(coord))
            {
                return null;
            }

            return coord;
        }

        /// <summary>
        /// Checks if the given coordinates exist on the board.
        /// </summary>
        /// <param name="coords">The coords to test.</param>
        /// <returns>TRUE if they exist on the board, false otherwise.</returns>
        public static bool IsValidCoordinate(Coordinate coordinate)
        {
            return coordinate.XCoordinate >= Board.MIN_COORDINATE && coordinate.XCoordinate <= Board.MAX_COORDINATE
                && coordinate.YCoordinate >= Board.MIN_COORDINATE && coordinate.YCoordinate <= Board.MAX_COORDINATE;
        }

        /// <summary>
        /// Converts a coordinate into a string.
        /// </summary>
        /// <param name="coord">The coordinate object to retrieve a string from.</param>
        /// <returns>A string representing the coordinate object.</returns>
        public static string CoordinateToStr(Coordinate coord)
        {
            return ((char)(coord.YCoordinate - 1 + 'A')).ToString() + coord.XCoordinate.ToString();
        }

        /// <summary>
        /// Gets the enemy of the player index given.
        /// </summary>
        /// <param name="playerIndex">The 0-based player number to get the enemy of.</param>
        /// <returns>Returns the index of the other player.</returns>
        public static int GetEnemyOfPlayerNumber(int playerIndex)
        {
            return (playerIndex == GameManagerBase.PLAYER_ONE ? GameManagerBase.PLAYER_TWO : GameManagerBase.PLAYER_ONE);
        }
    }
}
