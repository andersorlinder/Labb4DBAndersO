using System;
using Microsoft.EntityFrameworkCore;

namespace Labb4Anders
{
    public class Program
    {
        public static UserHandler userHandler;
        static void Main(string[] args)
        {
            var accounts = new AccountContext();
            accounts.Database.EnsureCreated();
            userHandler = new UserHandler(accounts);

            MainMenu();

            Console.Clear();
            Console.WriteLine("*** Thank you for using Account Portal. Welcome back anytime! ***");
        }

        public static void MainMenu()
        {
            ConsoleKey inputMainMenu;

            do
            {
                Console.Clear();

                Console.WriteLine("*** Welcome to Account Portal ***\n\n" +
                    "Choose an option:\n" +
                    "1. Register account\n" +
                    "2. Admin login\n" +
                    "3. Close application\n");

                inputMainMenu = Console.ReadKey(true).Key;
                switch (inputMainMenu)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        userHandler.AddUser();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        if (userHandler.AdminLogin())
                        {
                            AdminMenu();
                        }
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        break;
                    default:
                        break;
                }
            } while ((inputMainMenu != ConsoleKey.D3));
        }

        public static void AdminMenu()
        {
            ConsoleKey inputAdminMenu;

            do
            {
                Console.Clear();

                Console.WriteLine("*** Admin Menu ***\n" +
                    "Choose an option:\n" +
                    "1. Show accounts\n" +
                    "2. Administrate photo approval\n" +
                    "3. Add or remove admins\n" +
                    "4. Main Menu\n\n");

                inputAdminMenu = Console.ReadKey(true).Key;
                switch (inputAdminMenu)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        userHandler.PrintUsers();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        userHandler.PhotoApproval();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        userHandler.AdminApproval();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        break;
                    default:
                        break;
                }

            } while ((inputAdminMenu != ConsoleKey.D4));
        }
    }
}
