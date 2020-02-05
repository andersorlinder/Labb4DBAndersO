using System;

namespace Labb4Anders
{
    public class AccountView
    {
        public Action AddAccount, PrintAllAccounts, PhotoApproval, AdminApproval;
        public Func<bool> AdminLogin;

        public void MainMenu()
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
                        AddAccount();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        if (AdminLogin())
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

        public void AdminMenu()
        {
            ConsoleKey inputAdminMenu;

            do
            {
                Console.Clear();

                Console.WriteLine("*** Admin Menu ***\n" +
                    "Choose an option:\n" +
                    "1. Show all accounts\n" +
                    "2. Administrate photo approval\n" +
                    "3. Add or remove admins\n" +
                    "4. Main Menu\n\n");

                inputAdminMenu = Console.ReadKey(true).Key;
                switch (inputAdminMenu)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        PrintAllAccounts();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        PhotoApproval();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        AdminApproval();
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