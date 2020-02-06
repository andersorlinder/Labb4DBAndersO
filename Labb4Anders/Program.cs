namespace Labb4Anders
{
    public class Program
    {
        static void Main(string[] args)
        {
            var accountContext = new AccountContext();
            var accountController = new AccountController(accountContext);
            accountController.Run();
        }
    }
}
