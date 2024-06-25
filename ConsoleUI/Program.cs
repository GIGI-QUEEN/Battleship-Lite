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
            WelcomeMessage();
           PlayerInfoModel activePlayer = CreatePlayer("Player 1");
           PlayerInfoModel opponent = CreatePlayer("Player 2");
           PlayerInfoModel winner = null;
           // DisplayShotGrid(activePlayer);
            do
            {
               // Clear();
                DisplayShotGrid(activePlayer);
                Console.WriteLine();
             
                RecordPlayerShot(activePlayer, opponent);
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);
               

                if (doesGameContinue == true)
                {
                   // swap positions
                    (activePlayer, opponent) = (opponent, activePlayer);

                } else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
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
                } catch (Exception ex)
                {
                    isValidShot = false;
                    //Console.WriteLine("Error: " + ex.Message);
                }

                if (isValidShot == false)
                {
                    Console.WriteLine("Invalid Shot Location. Please try again.");
                }
            } while( isValidShot == false );

            bool isAHit = GameLogic.IdentifyShotResults(opponent, row, column);
            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            DisplayShotResults(row, column, isAHit);

        }

        private static void DisplayShotResults(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($"{row}{column} is a Hit!");
            } else
            {
                Console.WriteLine($"{row}{column} is a miss.");
            }
            Console.WriteLine();
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;
           foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;

                }

                // consider to use switch instead of if-else
                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
                } else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                } else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                } else
                {
                    Console.Write(" ?  ");
                }
           }
            Console.WriteLine(); 
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("created by Nikita Masalov");
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
                } catch (Exception ex)
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
