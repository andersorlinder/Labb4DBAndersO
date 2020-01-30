using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Labb4Anders
{
    public class Program
    {
        static void Main(string[] args)
        {
            var accounts = new AccountContext();
            accounts.Database.EnsureCreated();
            ConsoleKey inputMainMenu;

            if (!accounts.Users.AsEnumerable().Any())
            {
                accounts.Users.Add(new User
                {
                    Id = 1,
                    Email = "admin@mail.com",
                    FirstName = "Admin",
                    LastName = "(locked)",
                    NotApprovedPhoto = null,
                    ApprovedPhoto = "http://facebookfplus.com/upload/images/600_97d118b7a6f8f87d18f7b1385ea7665e.png",
                    IsAdmin = true,
                    Password = "1234"
                }); ;
                accounts.SaveChanges();
            }

            do
            {
                Console.Clear();

                Console.WriteLine("*** Welcome to Account Portal ***\n" +
                    "Choose an option:\n" +
                    "1. Register account\n" +
                    "2. Admin login\n" +
                    "3. Close application\n");

                inputMainMenu = Console.ReadKey(true).Key;
                switch (inputMainMenu)
                {
                    case ConsoleKey.D1:
                        addUser(accounts);
                        break;
                    case ConsoleKey.D2:
                        if (AdminLogin(accounts))
                        {
                            AdminMenu(accounts);
                        }
                        break;
                    case ConsoleKey.D3:
                        break;
                    default:
                        break;
                }
            } while ((inputMainMenu != ConsoleKey.D3));

            Console.Clear();
            Console.WriteLine("*** Thank you for using Account Portal. Welcome back anytime! ***");
        }

        private static void addUser(AccountContext accounts)
        {
            string email;
            bool isUnique = false;
            string firstName;
            string lastName;
            string notApprovedPhoto;
            int identity = accounts.Users.Select(u => u.Id).Max();
            ConsoleKey addMore;

            do
            {
                Console.Clear();

                Console.WriteLine("*** Add Account ***\n");
                Console.Write("Email: ");
                do
                {
                    email = Console.ReadLine().Trim();
                    if (!accounts.Users.AsEnumerable().Any(u => u.Email == email))
                    {
                        isUnique = true;
                        continue;
                    }
                    Console.WriteLine("Email is already in use. Try another one!");

                } while (!isUnique);
                Console.Write("First Name: ");
                firstName = Console.ReadLine().Trim();
                Console.Write("Last Name: ");
                lastName = Console.ReadLine().Trim();
                Console.Write("Photo url: ");
                notApprovedPhoto = Console.ReadLine().Trim();
                identity++;

                accounts.Users.Add(new User
                {
                    Id = identity,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    NotApprovedPhoto = notApprovedPhoto
                });
                accounts.SaveChanges();

                Console.Write("Press any key to add more accounts, press \"q\" to go back");
                addMore = Console.ReadKey(true).Key;

            } while (addMore != ConsoleKey.Q);
        }

        private static bool AdminLogin(AccountContext accounts)
        {
            Console.Clear();

            Console.WriteLine("*** Admin Login ***\n\n" +
                "Enter login details.");

            bool isAdmin = false;
            int passwordRetry = 3;
            User user;
            string email;
            string password;

            do
            {
                Console.Write("\nEmail: ");
                email = Console.ReadLine().Trim();
                Console.Write("Password: ");
                password = Console.ReadLine().Trim();
                passwordRetry--;
                try
                {
                    user = accounts.Users.Single(u => u.Email == email);
                }

                catch (Exception)
                {
                    Console.WriteLine($"User could not be found. {passwordRetry} attemps left.\n");
                    continue;
                }

                if (user.IsAdmin && user.Password == password)
                {
                    isAdmin = true;
                }

                else
                {
                    Console.WriteLine($"User is not an admin. {passwordRetry} attemps left.\n");
                }

            } while (passwordRetry > 0 && !isAdmin);

            return isAdmin;
        }

        private static void AdminMenu(AccountContext accounts)
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
                        PrintUsers(accounts);
                        break;
                    case ConsoleKey.D2:
                        PhotoApproval(accounts);
                        break;
                    case ConsoleKey.D3:
                        AdminApproval(accounts);
                        break;
                    case ConsoleKey.D4:
                        break;
                    default:
                        break;
                }

            } while ((inputAdminMenu != ConsoleKey.D4));
        }

        private static void PrintUsers(AccountContext accounts)
        {
            Console.Clear();

            Console.WriteLine("*** Account register ***\n" + "ID".PadRight(4) + "Name".PadRight(25) + "Email");
            foreach (var user in accounts.Users)
            {
                Console.WriteLine(user.Id.ToString().PadRight(4) + (user.FirstName + " " + user.LastName).PadRight(25) + user.Email);
            }
            Console.Write("\n Press any key to go back");
            Console.ReadKey(true);
        }

        private static void PhotoApproval(AccountContext accounts)
        {
            string inputString;
            int inputInt;
            var notApprovedUserPhotos = accounts.Users.Where(u => u.NotApprovedPhoto != null).AsEnumerable();

            do
            {
                Console.Clear();

                Console.WriteLine("*** Photo Approval ***\n" +
                    "ID".PadRight(4) + "Name".PadRight(25) + "Not approved photo");
                foreach (var u in notApprovedUserPhotos)
                {
                    Console.WriteLine(u.Id.ToString().PadRight(4) + (u.FirstName + " " + u.LastName).PadRight(25) + u.NotApprovedPhoto);
                }

                Console.Write("\nChoose one \"ID\" to approve photo, press q to go back: ");
                inputString = Console.ReadLine().Trim();
                inputInt = int.TryParse(inputString, out inputInt) ? inputInt : 0;

                if (notApprovedUserPhotos.Any(u => u.Id == inputInt))
                {
                    var toApprove = notApprovedUserPhotos.Single(u => u.Id == inputInt);
                    var photo = toApprove.NotApprovedPhoto;
                    toApprove.ApprovedPhoto = photo;
                    toApprove.NotApprovedPhoto = null;
                    accounts.SaveChanges();
                    Console.WriteLine($"Photo with ID: {inputInt} has been approved!");
                    notApprovedUserPhotos = accounts.Users.Where(u => u.NotApprovedPhoto != null).AsEnumerable();
                    Console.ReadKey(true);
                }

                else if (inputString != "q")
                {
                    Console.WriteLine("Incorrect choice, press any key and try again!");
                    Console.ReadKey(true);
                }

            } while (inputString != "q");
        }
        
        private static void AdminApproval(AccountContext accounts)
        {
            string inputString;
            int inputInt;
            User user;

            do
            {
                Console.Clear();

                Console.WriteLine("*** Add or remove admins ***\n" +
                    "ID".PadRight(4) + "Admin".PadRight(7) + "Name".PadRight(25) + "Email");
                foreach (var u in accounts.Users)
                {
                    Console.WriteLine(u.Id.ToString().PadRight(4) + u.IsAdmin.ToString().PadRight(7) + (u.FirstName + " " + u.LastName).PadRight(25) + u.Email);
                }
                Console.WriteLine();
                Console.Write("Admin with ID: 1 can not be removed as admin.\n" +
                    "Choose one \"ID\" to add or remove as admin, press q to go back): ");

                inputString = Console.ReadLine().Trim();
                inputInt = int.TryParse(inputString, out inputInt) ? inputInt : 0;

                if (inputInt == 1)
                {
                    Console.WriteLine("Admin with ID: 1 can not be removed as admin. Press any key and try again!");
                    Console.ReadKey(true);
                    continue;
                }

                if (accounts.Users.AsEnumerable().Any(u => u.Id == inputInt))
                {
                    user = accounts.Users.Single(u => u.Id == inputInt);
                    if (user.IsAdmin)
                    {
                        Console.WriteLine($"Confirm removal of no {user.Id} {user.FirstName} {user.LastName} as admin? (Y/N)");

                        if (Confirmation())
                        {
                            user.IsAdmin = false;
                            accounts.SaveChanges();
                            Console.WriteLine($"No {user.Id} {user.FirstName} {user.LastName} is no longer an admin.\n" +
                                $"Press any key to go back!");
                        }
                        
                    }
                    else if (!user.IsAdmin)
                    {
                        Console.WriteLine($"Confirm adding of no {user.Id} {user.FirstName} {user.LastName} as admin? (Y/N)");

                        if (Confirmation())
                        {
                            Console.Write("Set password: ");
                            user.Password = Console.ReadLine().Trim();
                            user.IsAdmin = true;
                            accounts.SaveChanges();
                            Console.WriteLine($"No {user.Id} {user.FirstName} {user.LastName} is now an admin.\n" +
                                $"Press any key to go back!");
                        }
                    }
                    Console.ReadKey(true);
                }

                else if (inputString != "q")
                {
                    Console.WriteLine("Incorrect choice, press any key and try again!");
                    Console.ReadKey(true);
                }

            } while (inputString != "q");
        }

        private static bool Confirmation()
        {
            ConsoleKey inputConfirm;
            bool Confirmed = false;

            do
            {
                inputConfirm = Console.ReadKey(true).Key;
                if (!(inputConfirm == ConsoleKey.Y || inputConfirm == ConsoleKey.N))
                {
                    Console.WriteLine("Incorrect choice, try again!");
                }
                else
                {
                    Confirmed = true;
                }

            } while (!Confirmed);
            
            if (inputConfirm == ConsoleKey.N)
            {
                Console.WriteLine("No changes were made!");
                return false;
            }
            return true;
        }
    }
}
