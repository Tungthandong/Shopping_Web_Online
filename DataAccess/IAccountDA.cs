using Shopping_Web.Models;

namespace Shopping_Web.DataAccess
{
    public interface IAccountDA
    {
        Account? Login(string username, string password);
        Account? GetAccountByUsername(string username);
        void UpdateProfile(Account a);
        void AddAccount(Account a);
        List<Account> GetAccounts();
        string UpdateAccountStatus(string username, string status);
    }
}
