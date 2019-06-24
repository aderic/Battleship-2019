using System;
using System.Linq;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    static class Output
    {
        // The format every cell is going to take, changing this changes the sizing of the board too.
        private const string BOARD_CELL_FMT = "  {0}  ";

        // The one unused cell on the top-left corner of the board.
        private const string BOARD_UNUSED_CELL_CONTENT = " WAR ";

        // The cell state indicators, used for when a ship is hit, missed or neither.
        private const char BOARD_INDICATORS_NONE = ' ';
        private const char BOARD_INDICATORS_MISS = 'M';
        private const char BOARD_INDICATORS_HIT = 'H';

        // The frame and styling for the inner part of the board: the "grid".
        private const char BOARD_GRID_VERTICAL = '│';
        private const char BOARD_GRID_HORIZONTAL = '─';
        private const char BOARD_GRID_CROSS = '┼';

        // The frame and styling for the outer part of the board. the "frame".
        private const char BOARD_FRAME_HORIZONTAL = '═';
        private const char BOARD_FRAME_VERTICAL = '║';
        private const char BOARD_FRAME_CORNER_TL = '╔';
        private const char BOARD_FRAME_CORNER_BL = '╚';
        private const char BOARD_FRAME_CORNER_TR = '╗';
        private const char BOARD_FRAME_CORNER_BR = '╝';
        private const char BOARD_FRAME_JOINER_TOP = '╤';
        private const char BOARD_FRAME_JOINER_BOTTOM = '╧';
        private const char BOARD_FRAME_JOINER_LEFT = '╟';
        private const char BOARD_FRAME_JOINER_RIGHT = '╢';
        private const ConsoleColor BOARD_FRAME_COLOR = ConsoleColor.DarkGreen;

        // The coloring of the letters and numbers that indicate the board's coordinate system.
        private const ConsoleColor BOARD_COORD_TEXT_COLOR = ConsoleColor.DarkGreen;

        // The board's outer spacing.
        private const int BOARD_MARGIN_TOP = 1;
        private const int BOARD_MARGIN_RIGHT = 2;
        private const int BOARD_MARGIN_BOTTOM = 1;
        private const int BOARD_MARGIN_LEFT = 1;

        // The styling of the infopanel's board title.
        private const int BOARD_INFOPANEL_TITLE_ROWNUM = 1;
        private const int BOARD_INFOPANEL_TITLE_LEFT_MARGIN = 4;
        private const ConsoleColor BOARD_INFOPANEL_TITLE_COLOR = ConsoleColor.White;

        // The row number used on the board to display ships left.
        private const int BOARD_INFOPANEL_SHIPS_LEFT_ROWNUM = 2;
        private const ConsoleColor BOARD_INFOPANEL_SHIPS_LEFT_COLOR = ConsoleColor.Gray;

        // The shots hit info styling.
        private const int BOARD_INFOPANEL_SHOTS_HIT_ROWNUM = 3;
        private const ConsoleColor BOARD_INFOPANEL_SHOTS_HIT_COLOR = ConsoleColor.Green;

        // The shots missed info styling.
        private const int BOARD_INFOPANEL_SHOTS_MISSED_ROWNUM = 4;
        private const ConsoleColor BOARD_INFOPANEL_SHOTS_MISSED_COLOR = ConsoleColor.Red;

        private const int BOARD_INFOPANEL_LEFT_MARGIN = 5;


        // The width of each cell, determined by BOARD_CELL_FMT.
        private static int cellWidth = string.Format(BOARD_CELL_FMT, ' ').Length;

        // The width of the entirety of the board, with seperators between each cell included.
        private static int boardWidth = (cellWidth * (Board.MAX_COORDINATE + 1)) + (cellWidth * (Board.MAX_COORDINATE + 1) / cellWidth);
        
        public static void PrintBoard(Player player, bool showPlacedShips = false)
        {
            PrintBoard(player, null, showPlacedShips);
        }

        /// <summary>
        /// Clears the screen and prints the entire board for the given player.
        /// </summary>
        /// <param name="enemy">The player to print the board of.</param>
        /// <param name="stats">The stats to display in the info panel, NULL to display no stats.</param>
        /// <param name="showPlacedShips">Determines whether ships should be shown on the board.</param>
        public static void PrintBoard(Player enemy, Stats stats, bool showPlacedShips = false)
        {
            ClearScreen();
            PrintLines(BOARD_MARGIN_TOP);

            // Set our console color for drawing the grid.
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = BOARD_FRAME_COLOR;

            // Write the top row of the frame.
            PrintTopFrame(boardWidth, cellWidth);

            // Write the first row, which contains the column numbering for the board.
            PrintBoardHeader(cellWidth, BOARD_COORD_TEXT_COLOR);

            for (int row = Board.MIN_COORDINATE; row <= Board.MAX_COORDINATE; row++)
            {
                // Writes a horizontal bar between each row.
                PrintRowDivider(boardWidth, cellWidth);

                PrintSpaces(BOARD_MARGIN_LEFT);
                // Write the left-most character that will begin our new series of columns.
                Write(BOARD_FRAME_VERTICAL);

                // Write our lettering for the current row, before we write the series of cells.
                PrintBoardCell((char)('A' + row - 1), BOARD_COORD_TEXT_COLOR);

                // Draw our cell's seperator.
                Write(BOARD_GRID_VERTICAL);

                for (int col = Board.MIN_COORDINATE; col <= Board.MAX_COORDINATE; col++)
                {
                    // Draw a single cell whose inner data depends on the board's state at those coords.
                    PrintBoardCell(enemy.Board, new Coordinate(col, row), showPlacedShips);

                    // If it's not the last column on the board, draw a thin seperator bar to start the next column.
                    if (col != Board.MAX_COORDINATE)
                    {
                        Write(BOARD_GRID_VERTICAL);
                    }
                }

                Write(BOARD_FRAME_VERTICAL);

                if (row == BOARD_INFOPANEL_TITLE_ROWNUM)
                {
                    PrintSpaces(BOARD_INFOPANEL_TITLE_LEFT_MARGIN);
                    Write($"{enemy.Name}'s board.", BOARD_INFOPANEL_TITLE_COLOR);
                }
                else if (stats != null)
                {
                    PrintSpaces(BOARD_INFOPANEL_LEFT_MARGIN);

                    switch (row)
                    {
                        case BOARD_INFOPANEL_SHIPS_LEFT_ROWNUM:
                            int shipsLeft = enemy.Board.Ships.Where(s => !s.IsSunk).Count();
                            Write($"Ships left on this board: {shipsLeft}.", BOARD_INFOPANEL_SHIPS_LEFT_COLOR);
                            break;
                        case BOARD_INFOPANEL_SHOTS_HIT_ROWNUM:
                            Write($"Shots hit: {stats.ShotsHit}", BOARD_INFOPANEL_SHOTS_HIT_COLOR);
                            break;
                        case BOARD_INFOPANEL_SHOTS_MISSED_ROWNUM:
                            Write($"Shots missed: {stats.ShotsMissed}", BOARD_INFOPANEL_SHOTS_MISSED_COLOR);
                            break;
                        case 5:
                            uint totalShots = stats.ShotsHit + stats.ShotsMissed;

                            if (totalShots > 0)
                            {
                                Write($"Accuracy: {decimal.Divide(stats.ShotsHit, totalShots):P0}");
                            }
                            break;
                    }
                }

                WriteLine();
            }

            PrintBottomFrame(boardWidth, cellWidth);
            PrintLines(BOARD_MARGIN_BOTTOM);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Calls WriteLine multiple times with the given amount.
        /// </summary>
        /// <param name="amount">The amount of new lines to print.</param>
        private static void PrintLines(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                WriteLine();
            }
        }

        /// <summary>
        /// Calls Write multiple times, writing a double space.
        /// </summary>
        /// <param name="amount">The amount of double spaces to print.</param>
        private static void PrintSpaces(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Write("  ");
            }
        }

        /// <summary>
        /// Draws the top frame using the given size.
        /// </summary>
        /// <param name="frameWidth">The total width of the frame.</param>
        /// <param name="cellWidth">The width of each cell for the frame.</param>
        private static void PrintTopFrame(int frameWidth, int cellWidth)
        {
            PrintStaticRow(frameWidth, cellWidth, BOARD_FRAME_CORNER_TL, BOARD_FRAME_HORIZONTAL, BOARD_FRAME_JOINER_TOP, BOARD_FRAME_CORNER_TR);
        }

        /// <summary>
        /// Draws the bottom frame using the given size.
        /// </summary>
        /// <param name="frameWidth">The total width of the frame.</param>
        /// <param name="cellWidth">The width of each cell for the frame.</param>
        private static void PrintBottomFrame(int frameWidth, int cellWidth)
        {
            PrintStaticRow(frameWidth, cellWidth, BOARD_FRAME_CORNER_BL, BOARD_FRAME_HORIZONTAL, BOARD_FRAME_JOINER_BOTTOM, BOARD_FRAME_CORNER_BR);
        }

        /// <summary>
        /// Draws a divider that goes between rows.
        /// </summary>
        /// <param name="frameWidth">The total width of the frame.</param>
        /// <param name="cellWidth">The width of each cell for the frame.</param>
        private static void PrintRowDivider(int frameWidth, int cellWidth)
        {
            PrintStaticRow(frameWidth, cellWidth, BOARD_FRAME_JOINER_LEFT, BOARD_GRID_HORIZONTAL, BOARD_GRID_CROSS, BOARD_FRAME_JOINER_RIGHT);
        }

        /// <summary>
        /// Creates a new row and calls WriteLine to draw it.
        /// </summary>
        /// <param name="boardWidth">The width of the board to draw.</param>
        /// <param name="cellWidth">Thewidth of each cell for the board.</param>
        /// <param name="leftChar">The left character to begin the row with.</param>
        /// <param name="fillChar">The character to fill the row with.</param>
        /// <param name="gridCrossChar">The character to use when there is an intersection between a cell and fillchar.</param>
        /// <param name="rightChar">The right-most character to end the row with.</param>
        private static void PrintStaticRow(int boardWidth, int cellWidth, char leftChar, char fillChar, char gridCrossChar, char rightChar)
        {
            PrintSpaces(BOARD_MARGIN_LEFT);
            Write(leftChar);
            for (int i = 1; i < boardWidth; i++)
            {
                // If the character we're about to print would create a cross in the grid, use a cross character.
                if (i % (cellWidth + 1) == 0)
                {
                    Write(gridCrossChar);
                }
                else
                {
                    Write(fillChar);
                }
            }
            WriteLine(rightChar);
        }

        /// <summary>
        /// Creates a new row, filling in the column numbers and calls WriteLine to print it.
        /// </summary>
        /// <param name="frameWidth">The total width of the frame.</param>
        /// <param name="color">The color of each cell for the frame.</param>
        private static void PrintBoardHeader(int cellWidth, ConsoleColor color)
        {
            PrintSpaces(BOARD_MARGIN_LEFT);
            Write(BOARD_FRAME_VERTICAL);

            for (int col = 0; col <= Board.MAX_COORDINATE; col++)
            {
                // The first column will be the top-left most cell in the grid, so we'll just use filler for that.
                if (col == 0)
                {
                    Write(BOARD_UNUSED_CELL_CONTENT.PadRight(cellWidth).Substring(0, cellWidth), BOARD_COORD_TEXT_COLOR);
                }
                else
                {
                    Write(string.Format(BOARD_CELL_FMT, col).Substring(0, cellWidth), color);
                }

                if (col != Board.MAX_COORDINATE)
                {
                    Write(BOARD_GRID_VERTICAL);
                }
            }

            WriteLine(BOARD_FRAME_VERTICAL);
        }

        /// <summary>
        /// Prints a cell containing only a character to the board.
        /// </summary>
        /// <param name="charToPrint">The character to print to the board.</param>
        /// <param name="color">The color used to print the character.</param>
        private static void PrintBoardCell(char charToPrint, ConsoleColor? color = null)
        {
            Write(string.Format(BOARD_CELL_FMT, charToPrint), color);
        }

        /// <summary>
        /// Prints a cell containing a string to the board.
        /// </summary>
        /// <param name="cellContents">The string to print to the board.</param>
        /// <param name="color">The color used to print the character.</param>
        private static void PrintBoardCell(string cellContents, ConsoleColor? color = null)
        {
            Write(cellContents.PadLeft(cellWidth), color);
        }

        /// <summary>
        /// Prints a single cell using the given board and coordinates.
        /// </summary>
        /// <param name="board">The board to print a cell on.</param>
        /// <param name="coord">The coordinate you want to print, this will determine the cell's contents.</param>
        /// <param name="showPlacedShips">Will show the ship's name as the cell's content.</param>
        private static void PrintBoardCell(Board board, Coordinate coord, bool showPlacedShips = false)
        {
            ShotHistory shotHist = board.CheckCoordinate(coord);

            if (shotHist == ShotHistory.Hit)
            {
                PrintBoardCell(BOARD_INDICATORS_HIT, ConsoleColor.Red);
            }
            else if (shotHist == ShotHistory.Miss)
            {
                PrintBoardCell(BOARD_INDICATORS_MISS, ConsoleColor.Yellow);
            }
            else if (showPlacedShips)
            {
                bool isShipFound = false;
                foreach (Ship ship in board.Ships)
                {
                    if (ship != null && ship.BoardPositions.Contains(coord))
                    {
                        PrintBoardCell(GetShipShortName(ship.ShipType), ConsoleColor.White);
                        isShipFound = true;
                        break;
                    }
                }

                if (!isShipFound)
                {
                    PrintBoardCell(BOARD_INDICATORS_NONE, ConsoleColor.White);
                }
            }
            else
            {
                PrintBoardCell(BOARD_INDICATORS_NONE, ConsoleColor.White);
            }
        }

        /// <summary>
        /// Prints an error message indicating that the input the user gave was not yes or no.
        /// </summary>
        public static void PrintYesNoInvalid()
        {
            Write("\nPlease press 'Y' or 'N': ", ConsoleColor.Red);
        }

        /// <summary>
        /// Prints a message asking the user if they have a player they wish to play with.
        /// </summary>
        public static void PrintHasSecondPlayerQuestion()
        {
            Write($"Do you have a second player that you want to play with? y/n: ", ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Converts the given ship type into a string, trimming it so it will fit in a cell.
        /// </summary>
        /// <param name="shipType">The ship type enum to convert to string</param>
        /// <returns>A trimmed string matching the width of the board's cell.</returns>
        private static string GetShipShortName(ShipType shipType)
        {
            string shipName = shipType.ToString().ToUpper();

            if (shipName.Length <= cellWidth)
            {
                return shipName.PadRight(cellWidth);
            }

            return shipName.Substring(0, cellWidth);
        }

        /// <summary>
        /// Prints the splash screen graphic and title.
        /// </summary>
        public static void PrintSplashScreen()
        {
            WriteLine(@"
                                                    |__
════════════════════════╗                           |\/
        Battleship 2019 ║                           ---
════════════════════════╝                           / | [
                                             !      | |||
                                           _/|     _/|-++'
                                       +  +--|    |--|--|_ |-
                                    { /|__|  |/\__|  |--- |||__/
                                   +---------------___[}-_===_.'____                 /\
                               ____`-' ||___-{]_| _[}-  |     |_[___\==--            \/   _
                __..._____--==/___]_|__|_____________________________[___\==--____,------' .7
               |                                                                           /
                \                                                                         |
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
░░░░░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒", ConsoleColor.DarkCyan);

            PrintLines(3);
        }

        /// <summary>
        /// Asks the user for their name.
        /// </summary>
        /// <param name="playerNumber">The player number of the user we're asking.</param>
        public static void PrintPlayerNameQuestion(int playerNumber)
        {
            Write($"Please enter your name, player {playerNumber + 1}: ", ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Notifies the user that their name was blank.
        /// </summary>
        public static void PrintPlayerNameInvalid()
        {
            Write($"The name you entered can't be blank, try again please: ", ConsoleColor.Red);
        }

        /// <summary>
        /// Notifies the user that the coordinates are invalid.
        /// </summary>
        public static void PrintCoordinatesInvalid()
        {
            Write($"Invalid coordinates given! Please try again: ", ConsoleColor.Red);
        }

        /// <summary>
        /// Asks the user wheich direction they want to place their ship.
        /// </summary>
        /// <param name="shipName">The ship name being asked about</param>
        public static void PrintDirectionQuestion(string shipName)
        {
            WriteLine($"Which direction do you want to place your {shipName.ToLower()}?", ConsoleColor.DarkCyan);
            Write("Press the corresponding keys for (u)p, (r)ight, (d)own or (l)eft or use arrow keys: ", ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Notifies the suer that the direction is invalid.
        /// </summary>
        public static void PrintDirectionInvalid()
        {
            Write($"\nInvalid direction given, try again: ", ConsoleColor.Red);
        }

        /// <summary>
        /// Asks the user which coordinates they want to place their ship.
        /// </summary>
        /// <param name="shipName">The ship name to ask about</param>
        public static void PrintPlacementQuestion(string shipName)
        {
            Write($"Where do you want to place your {shipName}? Enter your coordinates (e.g: A5, B2, ...): ", ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Notifies the user of the status of their placement request.
        /// </summary>
        /// <param name="shipName">The ship we're notifying them about.</param>
        /// <param name="placement">The result of the placement request.</param>
        public static void PrintPlacementResponse(string shipName, ShipPlacement placement)
        {
            if (placement == ShipPlacement.NotEnoughSpace)
            {
                WriteLine($"There was not enough space to place the {shipName}. Please try again.", ConsoleColor.Red);
            }
            else if (placement == ShipPlacement.Overlap)
            {
                WriteLine($"The coordinates you gave overlap with another ship. Please try again.", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Notifies the user that the ships have all been placed.
        /// </summary>
        /// <param name="playerName">The name of the player who is going next.</param>
        public static void PrintPlacementComplete(string playerName)
        {
            ClearScreen();
            PrintLines(3);
            WriteLine($"Your ships have all been placed, it's now {playerName}'s turn to set up their ships.", ConsoleColor.DarkCyan);
            Write($"Press any key when you're here, {playerName}.", ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Notifies the players of which player was chosen.
        /// </summary>
        /// <param name="playerName">The name of the player who was chosen to go first.</param>
        public static void PrintPlayerChosen(string playerName)
        {
            ClearScreen();
            PrintLines(3);
            WriteLine($"All ships have been placed. The player that gets the first turn is: {playerName}.", ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Asks the given player to enter coordinates to fire at.
        /// </summary>
        /// <param name="playerName">The player's name to ask.</param>
        public static void PrintFireQuestion(string playerName)
        {
            Write($"{playerName}, choose your firing coordinates: ", ConsoleColor.Yellow);
        }

        /// <summary>
        /// Converts the FireShotResponse shot status into a user-friendly message and prints it to the console.
        /// </summary>
        /// <param name="shotResponse">The FireShotResponse enum to convert and print.</param>
        public static void PrintFireShotResponse(FireShotResponse shotResponse, Coordinate coords)
        {
            switch (shotResponse.ShotStatus)
            {
                case ShotStatus.Duplicate:
                    Write($"The coordinates you entered have already been fired at, please choose different coordinates: ", ConsoleColor.Red);
                    break;
                case ShotStatus.Miss:
                    Write($"Your shot missed!", ConsoleColor.Yellow);
                    break;
                case ShotStatus.Hit:
                    Write($"Your shot hit a target!", ConsoleColor.Green); // We don't want to give away what the player hit.
                    break;
                case ShotStatus.HitAndSunk:
                case ShotStatus.Victory:
                    Write($"Your shot sunk a {shotResponse.ShipImpacted.ToLower()}!", ConsoleColor.Green);
                    break;
            }
        }

        /// <summary>
        /// Converts the FireShotResponse shot status into a user-friendly message and prints it to the console.
        /// This function is different from PrintFireShotResponse in that it refers to YOU and does not hide which ship was hit
        /// </summary>
        /// <param name="shotResponse">The FireShotResponse enum to convert and print.</param>
        /// <param name="coords"></param>
        /// <param name="botName"></param>
        public static void PrintFireShotBotResponse(FireShotResponse shotResponse, Coordinate coords, string botName)
        {
            switch (shotResponse.ShotStatus)
            {
                case ShotStatus.Miss:
                    Write($"{botName} fired at {Utils.CoordinateToStr(coords)} and missed!", ConsoleColor.Yellow);
                    break;
                case ShotStatus.Hit:
                    Write($"{botName} hit your {shotResponse.ShipImpacted.ToLower()} on {Utils.CoordinateToStr(coords)}!", ConsoleColor.Green); // It's okay to give away what they hit here, they're a bot.
                    break;
                case ShotStatus.HitAndSunk:
                case ShotStatus.Victory:
                    Write($"{botName} sunk your {shotResponse.ShipImpacted.ToLower()}!", ConsoleColor.Green);
                    break;
                case ShotStatus.Duplicate:
                    throw new NotSupportedException("The board reported a duplicate hit on the board, but the bot should not be able to do this.");
            }
        }

        /// <summary>
        /// Prints a victory message for the given player name and the statistics for the game.
        /// </summary>
        /// <param name="players">The players that were in the game.</param>
        /// <param name="winningPlayerIndex">The index of the winning player in the array</param>
        public static void PrintEndOfGameSummary(Player[] players, int winningPlayerIndex)
        {
            ClearScreen();

            PrintLines(3);
            WriteLine($"{players[winningPlayerIndex].Name} is victorious!", ConsoleColor.Green);

            uint totalRounds = players[winningPlayerIndex].Stats.ShotsHit + players[winningPlayerIndex].Stats.ShotsMissed;
            PrintLines(2);
            WriteLine($"The game lasted {totalRounds:N0} rounds.", ConsoleColor.Yellow);

            foreach (Player player in players)
            {
                WriteLine($"      {player.Name}", ConsoleColor.White);
                WriteLine($"            Hits: {player.Stats.ShotsHit:N0}", ConsoleColor.Green);
                WriteLine($"            Misses: {player.Stats.ShotsMissed:N0}", ConsoleColor.Red);
                WriteLine($"            Accuracy: {player.Stats.Accuracy:P}", ConsoleColor.Yellow);
                PrintLines(3);
            }
        }

        /// <summary>
        /// Prints a generic exit message.
        /// </summary>
        public static void PrintPlayAgainMessage()
        {
            WriteLine("Would you like to play again? y/n", ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Interally calls Console.Write, with an optional console color to write the text with.
        /// </summary>
        /// <param name="value">The value to write to console.</param>
        /// <param name="color">Optional: The color to print the value as, if left NULL it will use the current console color.</param>
        public static void Write(object value = null, ConsoleColor? color = null)
        {
            if (color == null)
            {
                Console.Write(value);
            }
            else
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = (ConsoleColor)color;
                Console.Write(value);
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Interally calls Console.WriteLine, with an optional console color to write the text with.
        /// </summary>
        /// <param name="value">The value to write to console.</param>
        /// <param name="color">Optional: The color to print the value as, if left NULL it will use the current console color.</param>
        public static void WriteLine(object value = null, ConsoleColor? color = null)
        {
            if (color == null)
            {
                Console.WriteLine(value);
            }
            else
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = (ConsoleColor)color;
                Console.WriteLine(value);
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Clears the screen.
        /// </summary>
        public static void ClearScreen()
        {
            Console.Clear();
        }
    }
}
