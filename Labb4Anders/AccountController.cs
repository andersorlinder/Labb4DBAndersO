using System;
using System.Linq;

namespace Labb4Anders
{
    public class AccountController
    {
        public AccountContext accountContext;
        public AccountView accountView;
        private DateTime loginDelay;
        public AccountController(AccountContext accountContext)
        {
            this.accountContext = accountContext;
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.WriteLine("*** Welcome to Account Portal ***\n\nLoading...");
            accountContext.Database.EnsureCreated();
            Initialize();

            accountView.MainMenu();

            Console.Clear();
            Console.WriteLine("*** Thank you for using Account Portal. Welcome back anytime! ***");
        }

        public void Initialize()
        {
            accountView = new AccountView
            {
                AddAccount = AddAccount,
                AdminLogin = AdminLogin,
                PrintAllAccounts = PrintAllAccounts,
                PhotoApproval = PhotoApproval,
                AdminApproval = AdminApproval
            };

            if (!accountContext.Accounts.AsEnumerable().Any())
            {
                accountContext.Accounts.Add(new Account
                {
                    Id = 1,
                    Email = "admin@mail.com",
                    FirstName = "Admin",
                    LastName = "(locked)",
                    NotApprovedPhoto = null,
                    ApprovedPhoto = "http://facebookfplus.com/upload/images/600_97d118b7a6f8f87d18f7b1385ea7665e.png",
                    IsAdmin = true,
                    Password = "1234"
                });
                accountContext.SaveChanges();
            }
        }

        public void AddAccount()
        {
            string email;
            string firstName;
            string lastName;
            string notApprovedPhoto;
            int identity = accountContext.Accounts.Select(u => u.Id).Max();
            bool addAnotherAccount = true;

            while (addAnotherAccount)
            {
                Console.Clear();
                Console.WriteLine("*** Add Account ***\n");

                if (CheckEmail(out email))
                {
                    Console.Write("First Name: ");
                    firstName = Console.ReadLine().Trim();
                    Console.Write("Last Name: ");
                    lastName = Console.ReadLine().Trim();
                    Console.Write("Photo url: ");
                    notApprovedPhoto = Console.ReadLine().Trim();
                    identity++;

                    accountContext.Accounts.Add(new Account
                    {
                        Id = identity,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        NotApprovedPhoto = notApprovedPhoto
                    });
                    accountContext.SaveChanges();
                }
                Console.CursorVisible = false;
                if (email == "q")  //exit in CheckEmail()
                {
                    addAnotherAccount = false;
                }
                else
                {
                    Console.Write("Press any key to add more accounts, press \"q\" to go back.");
                    if (Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        addAnotherAccount = false;
                    }
                }
            }
        }

        private bool CheckEmail(out string email)
        {
            string compareEmail;

            do
            {
                Console.Write("Email (enter \"q\" to go back): ");
                Console.CursorVisible = true;
                email = Console.ReadLine().Trim();
                compareEmail = email;
                if (email == "q")
                {
                    return false;
                }
                if (email == "")
                {
                    Console.WriteLine(" - Incorrect email!");
                }
                else if (accountContext.Accounts.AsEnumerable().Any(u => u.Email == compareEmail))
                {
                    Console.WriteLine(" - Email is already in use!");
                }
                else break;
            } while (email != "q");
            return true;
        }

        public bool AdminLogin()
        {
            Account Account;
            string email;
            string password;
            bool isAdmin = false;
            int passwordRetry = 3;

            Console.Clear();

            Console.WriteLine("*** Admin Login ***\n");
            if (DateTime.Now < loginDelay)
            {
                Console.WriteLine($"Admin Login is locked till {loginDelay.ToString("HH:mm:ss")}.\nPlease return later. Press any key to go back.");
                Console.ReadKey(true);
                return false;
            }

            Console.WriteLine("Enter login details.");
            Console.CursorVisible = true;
            do
            {
                Console.Write("\nEmail: ");
                email = Console.ReadLine().Trim();
                Console.Write("Password: ");
                password = Console.ReadLine().Trim();
                passwordRetry--;
                try
                {
                    Account = accountContext.Accounts.Single(u => u.Email == email);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Account could not be found. {passwordRetry} attemps left.");
                    continue;
                }

                if (Account.IsAdmin && Account.Password == password)
                {
                    isAdmin = true;
                }
                else
                {
                    Console.WriteLine($"Account is not an admin. {passwordRetry} attemps left.");
                }
            } while (passwordRetry > 0 && !isAdmin);
            Console.CursorVisible = false;

            if (isAdmin == false)
            {
                loginDelay = DateTime.Now.AddMinutes(5);
                Console.WriteLine($"Wait till {loginDelay.ToString("HH:mm:ss")} until next try. Press any key to go back.");
                Console.ReadKey(true);
            }

            return isAdmin;
        }

        public void PrintAllAccounts()
        {
            Console.Clear();

            Console.WriteLine("*** Account register ***\n" + "ID".PadRight(4) + "Name".PadRight(25) + "Email");
            foreach (var account in accountContext.Accounts)
            {
                Console.WriteLine(account.Id.ToString().PadRight(4) + (account.FirstName + " " + account.LastName).PadRight(25) + account.Email);
            }
            Console.Write("\n Press any key to go back.");
            Console.ReadKey(true);
        }

        public void PhotoApproval()
        {
            string inputString;
            int inputInt;
            var notApprovedUserPhotos = accountContext.Accounts.Where(u => u.NotApprovedPhoto != null).AsEnumerable();

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
                Console.CursorVisible = true;
                inputString = Console.ReadLine().Trim();
                Console.CursorVisible = false;
                inputInt = int.TryParse(inputString, out inputInt) ? inputInt : 0;

                if (inputString == "q")
                {
                    continue;
                }

                if (notApprovedUserPhotos.Any(u => u.Id == inputInt))
                {
                    var toApprove = notApprovedUserPhotos.Single(u => u.Id == inputInt);
                    var photo = toApprove.NotApprovedPhoto;
                    toApprove.ApprovedPhoto = photo;
                    toApprove.NotApprovedPhoto = null;
                    accountContext.SaveChanges();
                    Console.WriteLine($"Photo with ID: {inputInt} has been approved! Press any key.");
                    notApprovedUserPhotos = accountContext.Accounts.Where(u => u.NotApprovedPhoto != null).AsEnumerable();
                }
                else
                {
                    Console.WriteLine("Incorrect choice, press any key and try again!");
                }
                Console.ReadKey(true);
            } while (inputString != "q");
        }

        public void AdminApproval()
        {
            string inputString;
            int inputInt;
            Account account;

            do
            {
                Console.Clear();

                Console.WriteLine("*** Add or remove admins ***\n" +
                    "ID".PadRight(4) + "Admin".PadRight(7) + "Name".PadRight(25) + "Email");
                foreach (var u in accountContext.Accounts)
                {
                    Console.WriteLine(u.Id.ToString().PadRight(4) + u.IsAdmin.ToString().PadRight(7) + (u.FirstName + " " + u.LastName).PadRight(25) + u.Email);
                }
                Console.WriteLine();
                Console.Write("Admin with ID \"1\" can not be removed as admin.\n" +
                    "Choose one \"ID\" to add or remove as admin, enter q to go back: ");

                Console.CursorVisible = true;
                inputString = Console.ReadLine().Trim();
                Console.CursorVisible = false;
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

                if (!accountContext.Accounts.AsEnumerable().Any(u => u.Id == inputInt))
                {
                    Console.WriteLine("Incorrect choice, press any key and try again!");
                    Console.ReadKey(true);
                    continue;
                }

                account = accountContext.Accounts.Single(u => u.Id == inputInt);
                if (account.IsAdmin)
                {
                    Console.WriteLine($"Confirm removal of no {account.Id} {account.FirstName} {account.LastName} as admin? (Y/N)");

                    if (Confirmation())
                    {
                        account.IsAdmin = false;
                        accountContext.SaveChanges();
                        Console.WriteLine($"\nNo {account.Id} {account.FirstName} {account.LastName} is no longer an admin.\n" +
                            $"Press any key to continue!");
                    }
                }
                else if (!account.IsAdmin)
                {
                    Console.Write($"Confirm adding of no {account.Id} {account.FirstName} {account.LastName} as admin? (Y/N)");

                    if (Confirmation())
                    {
                        Console.Write("\nSet password: ");
                        Console.CursorVisible = true;
                        account.Password = Console.ReadLine().Trim();
                        Console.CursorVisible = false;
                        account.IsAdmin = true;
                        accountContext.SaveChanges();
                        Console.WriteLine($"\nNo {account.Id} {account.FirstName} {account.LastName} is now an admin.\n" +
                            $"Press any key to continue!");
                    }
                }
                Console.ReadKey(true);
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