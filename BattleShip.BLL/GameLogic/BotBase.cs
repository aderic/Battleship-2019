using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.BLL.GameLogic
{
    public abstract class BotBase
    {
        // Random number static that the bot will use.
        protected static Random random = new Random();

        // Represents all four directions
        protected const int TOTAL_DIRECTIONS = 4;

        // The board reference that this bot will use to fire at.
        protected Board board;

        // A collection of shuffled coordinates that can be fired at.
        protected List<Coordinate> shootableCoords;

        public BotBase(Board board)
        {
            this.board = board;

            shootableCoords = new List<Coordinate>(Board.MIN_COORDINATE * Board.MAX_COORDINATE);
            for (int row = Board.MIN_COORDINATE; row <= Board.MAX_COORDINATE; row++)
            {
                for (int col = Board.MIN_COORDINATE; col <= Board.MAX_COORDINATE; col++)
                {
                    shootableCoords.Add(new Coordinate(row, col));
                }
            }

            // Shuffle our fireable coordinate list.
            for (int i = 0; i < shootableCoords.Count; i++)
            {
                Coordinate tempVal = shootableCoords[i];
                int randIndex = random.Next(i, shootableCoords.Count);

                // Perform swap.
                shootableCoords[i] = shootableCoords[randIndex];
                shootableCoords[randIndex] = tempVal;
            }
        }

        /// <summary>
        /// Chooses and fires at a random position on the board.
        /// </summary>
        /// <param name="chosenCoordinates">Will contain the coordinates that the bot chose.</param>
        /// <returns>A FireShotResponse object containing details of the shot's outcome.</returns>
        public abstract FireShotResponse TakeTurn(out Coordinate chosenCoordinates);

        /// <summary>
        /// Adjusts the given coordinate one space toward the given direction.
        /// </summary>
        /// <param name="coords">The coordinates to adjust</param>
        /// <param name="fireDirection">The direction to adjust them</param>
        /// <returns>A new coordinate objerct moved one place toward the given direction</returns>
        protected Coordinate GetNextCoordinateTowards(Coordinate coords, ShipDirection fireDirection)
        {
            Coordinate newCoords = new Coordinate(coords.XCoordinate, coords.YCoordinate);

            switch (fireDirection)
            {
                case ShipDirection.Down:
                    newCoords.YCoordinate += 1;
                    break;

                case ShipDirection.Up:
                    newCoords.YCoordinate -= 1;
                    break;

                case ShipDirection.Left:
                    newCoords.XCoordinate -= 1;
                    break;

                default:
                    newCoords.XCoordinate += 1;
                    break;
            }

            return newCoords;
        }

        /// <summary>
        /// Checks if the given coordinates exist on the board.
        /// </summary>
        /// <param name="coords">The coords to test.</param>
        /// <returns>TRUE if they exist on the board, false otherwise.</returns>
        protected static bool IsCoordinateInRange(Coordinate coords)
        {
            return !(coords.XCoordinate < Board.MIN_COORDINATE || coords.XCoordinate > Board.MAX_COORDINATE || coords.YCoordinate < Board.MIN_COORDINATE || coords.YCoordinate > Board.MAX_COORDINATE);
        }

        /// <summary>
        /// Returns a new direction which is the input direction rotated clockwise.
        /// </summary>
        /// <param name="currentDirection">The direction to rotate.</param>
        /// <returns>A clock-wise rotated direction</returns>
        protected ShipDirection RotateDirection(ShipDirection currentDirection)
        {
            return (ShipDirection)(((int)currentDirection + 1) % TOTAL_DIRECTIONS);
        }

        /// <summary>
        /// Reverses the given direction.
        /// </summary>
        /// <param name="currentDirection">The direction you want to reverse.</param>
        /// <returns>DOWN if given UP, RIGHT if given LEFT and so on.</returns>
        protected ShipDirection GetReverseDirection(ShipDirection currentDirection)
        {
            switch (currentDirection)
            {
                case ShipDirection.Up:
                    return ShipDirection.Down;
                case ShipDirection.Down:
                    return ShipDirection.Up;
                case ShipDirection.Left:
                    return ShipDirection.Right;
                case ShipDirection.Right:
                    return ShipDirection.Left;
                default:
                    throw new Exception("Direction given is invalid.");
            }
        }

        /// <summary>
        /// Checks if the coordinate has not been hit or missed.
        /// </summary>
        /// <param name="coords">The coord to check for.</param>
        /// <returns>TRUE if the coord has been hit or missed, FALSE otherwise.</returns>
        protected bool CanFireOnCoordinate(Coordinate coords)
        {
            return shootableCoords.Contains(coords);
        }

        /// <summary>
        /// Checks if the coordinate has been hit.
        /// </summary>
        /// <param name="coords">The coord to check for.</param>
        /// <returns>TRUE if the given coordinate has been hit, or FALSE otherwise.</returns>
        protected bool HasCoordBeenHit(Coordinate coords)
        {
            return board.CheckCoordinate(coords) == ShotHistory.Hit;
        }

        /// <summary>
        /// Returns a random coordinate that does exist on the board.
        /// </summary>
        /// <returns>A coordinate object with a random X and Y.</returns>
        public static Coordinate GetRandomCoordinate()
        {

            int col = random.Next(Board.MIN_COORDINATE, Board.MAX_COORDINATE + 1);
            int row = random.Next(Board.MIN_COORDINATE, Board.MAX_COORDINATE + 1);
            return new Coordinate(col, row);
        }

        /// <summary>
        /// Returns a random direction.
        /// </summary>
        /// <returns>A ShipDirection enum representing the randomized direction.</returns>
        public static ShipDirection GetRandomDirection()
        {
            return (ShipDirection)random.Next(0, TOTAL_DIRECTIONS);
        }

#if DEBUG
        /// <summary>
        /// Converts our coordinate to a string.
        /// This function also exists in BattleShip.UI's Utils file, but since we're not allowed
        /// to touch the BLL (short of this file and the enum), I elected to include it here,
        /// it's only used for debugging, so it only exists in the binary if it is built in DEBUG mode.
        /// </summary>
        /// <param name="coord">The coordinate to convert.</param>
        /// <returns>A string representation of the coordinate.</returns>
        public static string CoordinateToStr(Coordinate coord)
        {
            return ((char)(coord.YCoordinate - 1 + 'A')).ToString() + coord.XCoordinate.ToString();
        }
#endif
    }
}
