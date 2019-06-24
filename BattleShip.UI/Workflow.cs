namespace BattleShip.UI
{
    static class Workflow
    {
        public static void Execute()
        {
            Output.ClearScreen();
            Output.PrintSplashScreen();

            GameManagerBase gameManager;

            do
            {
                // Depending on whether they have two players or one, we'll set up
                // a game that either has a bot or two players.
                Output.PrintHasSecondPlayerQuestion();
                
                // If they have a second player, create a PVP game.
                if (Input.GetYesAnswer())
                {
                    gameManager = new GameManagerPVP();
                }
                else
                { // If they are playing alone, create a BOT game.
                    gameManager = new GameManagerPVC(); // Call GameManagerDebugSim to have two bots play against each other.
                }
                // Start the game manager's workflow.
                gameManager.Execute();

                // After the game is over, update the UI and ask if they want to play again.
                Output.ClearScreen();
                Output.PrintSplashScreen();
                Output.PrintPlayAgainMessage();
            } while (Input.GetYesAnswer()); // If they want to play again, jump to top of loop.
        }
    }
}
