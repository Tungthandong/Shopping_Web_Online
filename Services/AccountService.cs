using Shopping_Web.Models;
using Shopping_Web.Repositories;

namespace Shopping_Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void AddAccount(Account a)
        {
            _accountRepository.AddAccount(a);
        }

        public Account? GetAccountByEmail(string email, string status)
        {
            return _accountRepository.GetAccountByEmail(email, status);
        }

        public Account? GetAccountByUsername(string username)
        {
            return _accountRepository.GetAccountByUsername(username);
        }

        public List<Account> GetAccounts()
        {
            return _accountRepository.GetAccounts();
        }

        public Account? Login(string username, string password)
        {
            return _accountRepository.Login(username, password);
        }

        public string UpdateAccountStatus(string username, string status)
        {
            return _accountRepository.UpdateAccountStatus(username, status);
        }

        public void UpdateProfile(Account a)
        {
            _accountRepository.UpdateProfile(a);
        }
    }
}
