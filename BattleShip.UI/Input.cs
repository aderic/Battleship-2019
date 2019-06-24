using System;
using BattleShip.BLL.Requests;

namespace BattleShip.UI
{
    static class Input
    {
        /// <summary>
        /// Traps the user until they press Y or N.
        /// </summary>
        /// <returns>Returns TRUE if they press Y or FALSE if they press N.</returns>
        public static bool GetYesAnswer()
        {
            ConsoleKey key = Console.ReadKey().Key;

            while (key != ConsoleKey.Y && key != ConsoleKey.N)
            {
                Output.PrintYesNoInvalid();
                key = Console.ReadKey().Key;
            }

            Output.WriteLine();
            return key == ConsoleKey.Y;
        }

        /// <summary>
        /// Traps the user until they give a non-null and non-whitespace name.
        /// </summary>
        /// <returns>The chosen name for the player.</returns>
        public static string GetPlayerName()
        {
            string name = Console.ReadLine();

            // First player is required to put in their name.
            while (string.IsNullOrWhiteSpace(name))
            {
                Output.PrintPlayerNameInvalid();
                name = Console.ReadLine();
            }

            return name;
        }

        /// <summary>
        /// Gets the coordinates from the user's input.
        /// </summary>
        /// <returns>A new coordinate object that represents the input the user gave.</returns>
        public static Coordinate GetCoordinate()
        {
            Coordinate coords = null;

            while ((coords = Utils.StrToCoordinate(Console.ReadLine())) == null)
            {
                Output.PrintCoordinatesInvalid();
            }

            return coords;
        }

        /// <summary>
        /// Gets the direction from the user's input.
        /// </summary>
        /// <returns>A ShipDirection enum representing the input the user gave.</returns>
        public static ShipDirection GetDirection()
        {
            ShipDirection? direction = null;

            while (direction == null)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.U:
                    case ConsoleKey.UpArrow:
                        direction = ShipDirection.Up;
                        break;
                    case ConsoleKey.R:
                    case ConsoleKey.RightArrow:
                        direction = ShipDirection.Right;
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.DownArrow:
                        direction = ShipDirection.Down;
                        break;
                    case ConsoleKey.L:
                    case ConsoleKey.LeftArrow:
                        direction = ShipDirection.Left;
                        break;
                    default:
                        Output.PrintDirectionInvalid();
                        break;
                }
            }

            return (ShipDirection)direction;
        }

        /// <summary>
        /// Stalls the console, waiting for the user to press a key.
        /// </summary>
        public static void WaitForAnyKey()
        {
            Console.ReadKey();
        }
    }
}
