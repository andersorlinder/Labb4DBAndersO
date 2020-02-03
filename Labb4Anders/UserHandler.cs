using System;
using System.Linq;

namespace Labb4Anders
{
    public class UserHandler
    {
        public AccountContext accounts;
        private DateTime loginDelay;
        public UserHandler(AccountContext acc)
        {
            accounts = acc;

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
        }

        public void AddUser()
        {
            string email;
            bool isUnique;
            string firstName;
            string lastName;
            string notApprovedPhoto;
            int identity = accounts.Users.Select(u => u.Id).Max();
            ConsoleKey addMore;

            do
            {
                isUnique = false;
                Console.Clear();

                Console.WriteLine("*** Add Account ***\n");
                Console.Write("Email: ");
                do
                {
                    email = Console.ReadLine().Trim();
                    if (email == "q")
                    {
                        break;
                    }
                    if (!accounts.Users.AsEnumerable().Any(u => u.Email == email))
                    {
                        isUnique = true;
                        continue;
                    }
                    Console.WriteLine("Email is already in use. Try another one! (Enter q to go back)\nEmail: ");

                } while (!isUnique);

                if (email == "q")
                {
                    break;
                }

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

                Console.Write("Press any key to add more accounts, press \"q\" to go back.");
                addMore = Console.ReadKey(true).Key;
            } while (addMore != ConsoleKey.Q);
        }

        public bool AdminLogin()
        {
            User user;
            string email;
            string password;
            bool isAdmin = false;
            int passwordRetry = 3;

            Console.Clear();

            Console.WriteLine("*** Admin Login ***\n");
            if (DateTime.Now < loginDelay)
            {
                Console.WriteLine($"Admin Login is locked till {loginDelay.ToString("HH: mm:ss")}.\nPlease return later. Press any key to go back.");
                Console.ReadKey(true);
                return false;
            }

            Console.WriteLine("Enter login details.");
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

            if (isAdmin == false)
            {
                loginDelay = DateTime.Now.AddMinutes(5);
                Console.WriteLine($"Wait till {loginDelay.ToString("HH:mm:ss")} to next try. Press any key to go back.");
                Console.ReadKey(true);
            }

            return isAdmin;
        }

        public void PrintUsers()
        {
            Console.Clear();

            Console.WriteLine("*** Account register ***\n" + "ID".PadRight(4) + "Name".PadRight(25) + "Email");
            foreach (var user in accounts.Users)
            {
                Console.WriteLine(user.Id.ToString().PadRight(4) + (user.FirstName + " " + user.LastName).PadRight(25) + user.Email);
            }
            Console.Write("\n Press any key to go back.");
            Console.ReadKey(true);
        }

        public void PhotoApproval()
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

                Console.Write("\nChoose one \"ID\" to approve photo, enter q to go back: ");
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

        public void AdminApproval()
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
                Console.Write("Admin with ID \"1\" can not be removed as admin.\n" +
                    "Choose one \"ID\" to add or remove as admin, enter q to go back): ");

                inputString = Console.ReadLine().Trim();
                inputInt = int.TryParse(inputString, out inputInt) ? inputInt : 0;

                if (inputString == "q")
                {
                    continue;
                }

                if (inputInt == 1)
                {
                    Console.WriteLine("Admin with ID \"1\" can not be removed as admin. Press any key and try again!");
                    Console.ReadKey(true);
                    continue;
                }

                if (!accounts.Users.AsEnumerable().Any(u => u.Id == inputInt))
                {
                    Console.WriteLine("Incorrect choice, press any key and try again!");
                    Console.ReadKey(true);
                    continue;
                }

                user = accounts.Users.Single(u => u.Id == inputInt);
                if (user.IsAdmin)
                {
                    Console.Write($"Confirm removal of no {user.Id} {user.FirstName} {user.LastName} as admin? (Y/N)");

                    if (Confirmation())
                    {
                        user.IsAdmin = false;
                        accounts.SaveChanges();
                        Console.WriteLine($"\nNo {user.Id} {user.FirstName} {user.LastName} is no longer an admin.\n" +
                            $"Press any key to continue!");
                    }
                    Console.ReadKey(true);
                    continue;
                }
                else if (!user.IsAdmin)
                {
                    Console.Write($"Confirm adding of no {user.Id} {user.FirstName} {user.LastName} as admin? (Y/N)");

                    if (Confirmation())
                    {
                        Console.Write("\nSet password: ");
                        user.Password = Console.ReadLine().Trim();
                        user.IsAdmin = true;
                        accounts.SaveChanges();
                        Console.WriteLine($"No {user.Id} {user.FirstName} {user.LastName} is now an admin.\n" +
                            $"Press any key to continue!");
                    }
                    Console.ReadKey(true);
                }
            } while (inputString != "q");
        }

        public bool Confirmation()
        {
            ConsoleKey inputConfirm;
            bool Confirmed = false;

            do
            {
                inputConfirm = Console.ReadKey(true).Key;
                if (!(inputConfirm == ConsoleKey.Y || inputConfirm == ConsoleKey.N))
                {
                    Console.Write("\nIncorrect choice, try again (Y/N)!");
                }
                else
                {
                    Confirmed = true;
                }
            } while (!Confirmed);

            if (inputConfirm == ConsoleKey.N)
            {
                Console.WriteLine("\nNo changes were made!");
                return false;
            }
            return true;
        }

    }
}