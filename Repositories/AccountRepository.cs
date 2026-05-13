using Shopping_Web.Data;
using Shopping_Web.Models;

namespace Shopping_Web.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly YugiohCardShopContext _context;

        public AccountRepository(YugiohCardShopContext context)
        {
            _context = context;
        }

        public void AddAccount(Account a)
        {
            _context.Accounts.Add(a);
            _context.SaveChanges();
        }

        public Account? GetAccountByEmail(string email, string status)
        {
            return _context.Accounts
                .FirstOrDefault(x => x.Email == email && x.AccountStatus == status);
        }

        public Account? GetAccountByUsername(string username)
        {
            return _context.Accounts
                .FirstOrDefault(x => x.Username == username && x.AccountStatus == "active");
        }

        public List<Account> GetAccounts()
        {
            return _context.Accounts.ToList();
        }

        public Account? Login(string username, string password)
        {
            var account = _context.Accounts
                .FirstOrDefault(a => a.Username == username && a.Password == password);

            if (account != null && account.AccountStatus == "active")
                return account;

            return null;
        }

        public string UpdateAccountStatus(string username, string status)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Username == username);
            if (account == null)
                return "Account not exist";

            account.AccountStatus = status;
            _context.SaveChanges();
            return "Update successfully";
        }

        public void UpdateProfile(Account a)
        {
            _context.Accounts.Update(a);
            _context.SaveChanges();
        }
    }
}
