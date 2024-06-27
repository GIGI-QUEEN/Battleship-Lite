using BattleshipLiteLibrary;
using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI
{
    internal class Program
    {
        static void Main(string[] args)
        {

            bool restart = true;

            while (restart)
            {
                RunGame();
                Console.WriteLine();
                string shouldRestart = PromptUser("Do you want to restart the game (yes/no): ");

                if (shouldRestart == "yes".ToLower())
                {
                    Clear();
                }
                else
                {
                    restart = false;
                }

            }



        }

        private static void RunGame()
        {
            WelcomeMessage();
            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);
                Console.WriteLine();

                RecordPlayerShot(activePlayer, opponent);
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);


                if (doesGameContinue == true)
                {
                    // swap positions
                    (activePlayer, opponent) = (opponent, activePlayer);

                }
                else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner);

           // Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to {winner.UsersName} for winning!");
            Console.WriteLine($"{winner.UsersName} took {GameLogic.GetShotCount(winner)} shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;
            do
            {
                string shot = PromptUser($"{activePlayer.UsersName}, please enter your shot selection: ");
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex)
                {
                    isValidShot = false;
                    //Console.WriteLine("Error: " + ex.Message);
                }

                if (isValidShot == false)
                {
                    Console.WriteLine("Invalid Shot Location. Please try again.");
                }
            } while (isValidShot == false);

            bool isAHit = GameLogic.IdentifyShotResults(opponent, row, column);
            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            DisplayShotResults(row, column, isAHit);

        }

        private static void DisplayShotResults(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($"{row}{column} is a Hit!");
            }
            else
            {
                Console.WriteLine($"{row}{column} is a miss.");
            }
            Console.WriteLine();
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            PrintFirstRow();

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;

                }
                if (gridSpot.SpotNumber == 1)
                {
                    Console.Write($" {gridSpot.SpotLetter} ");
                }


                switch (gridSpot.Status)
                {
                    case GridSpotStatus.Empty:
                        Console.Write(" o ");
                        break;
                    case GridSpotStatus.Hit:
                        Console.Write(" X ");
                        break;
                    case GridSpotStatus.Miss:
                        Console.Write(" M ");
                        break;
                    default:
                        Console.Write(" ? ");
                        break;

                }
            }
            Console.WriteLine();
            Console.WriteLine();

        }

        private static void PrintFirstRow()
        {
            for (int i = 0; i < 6; i++)
            {
                if (i == 0)
                {
                    Console.Write(" + ");

                }
                else
                {
                    Console.Write($" {i} ");
                }
            }

            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("Created by Nikita Masalov");
            Console.WriteLine();
            Console.WriteLine("Rules:");
            Console.WriteLine("1. The game is played on a 5x5 grid (A1 to E5).");
            Console.WriteLine("2. Each player has 5 ships, and each ship occupies exactly one cell.");
            Console.WriteLine("3. Players take turns calling out a cell to attack.");
            Console.WriteLine("4. The goal is to be the first player to hit all 5 of the opponent's ships.");
            Console.WriteLine("5. The grid markers are as follows:");
            Console.WriteLine("   - 'o' represents an empty spot.");
            Console.WriteLine("   - 'x' represents a hit.");
            Console.WriteLine("   - 'm' represents a miss.");
            Console.WriteLine("6. The game continues until one player has hit all 5 of the opponent's ships.");
            Console.WriteLine();
            Console.WriteLine("Let's start the game!");
            Console.WriteLine();

        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();
            Console.WriteLine($"Player information for {playerTitle}");

            output.UsersName = AskForUsersName();

            GameLogic.InitializeGrid(output);

            PlaceShips(output);


            Clear();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                // Console.Write("Where do you want to place your next ship: ");
                string shipLocation = PromptUser($"Where do you want to place ship number {model.ShipLocations.Count + 1}: ");
                bool isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, shipLocation);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (isValidLocation == false)
                {
                    Console.WriteLine("That was not a valid location. Please try again.");
                }
            } while (model.ShipLocations.Count < 5);
        }

        private static string AskForUsersName()
        {
            Console.Write("What is your name: ");
            return Console.ReadLine();
        }

        private static void Clear()
        {
            Console.Clear();
        }

        private static string PromptUser(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
    }
}
