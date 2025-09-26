using Shopping_Web.DataAccess;
using Shopping_Web.Models;

namespace Shopping_Web.Services
{
    public class AccountService : IAccountService
    {
        IAccountDA _accountDA;
        public AccountService(IAccountDA accountDA) {
            _accountDA = accountDA;
        }

        public void AddAccount(Account a)
        {
            _accountDA.AddAccount(a);
        }

        public Account? GetAccountByEmail(string email, string status)
        {
            return _accountDA.GetAccountByEmail(email, status);
        }

        public Account? GetAccountByUsername(string username)
        {
            return _accountDA.GetAccountByUsername(username);
        }

        public List<Account> GetAccounts()
        {
            return _accountDA.GetAccounts();
        }

        public Account? Login(string username, string password)
        {
            return _accountDA.Login(username, password);
        }

        public string UpdateAccountStatus(string username, string status)
        {
            return _accountDA.UpdateAccountStatus(username, status);
        }

        public void UpdateProfile(Account a)
        {
            _accountDA.UpdateProfile(a);
        }
    }
}
