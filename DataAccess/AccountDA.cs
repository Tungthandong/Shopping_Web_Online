using Shopping_Web.Models;

namespace Shopping_Web.DataAccess
{
    public class AccountDA : IAccountDA
    {
        YugiohCardShopContext context = new YugiohCardShopContext();

        public void AddAccount(Account a)
        {
            context.Add(a);
            context.SaveChanges();
        }

        public Account? GetAccountByEmail(string email, string status)
        {
            return context.Accounts.FirstOrDefault(x => x.Email == email && x.AccountStatus.Equals(status));
        }

        public Account? GetAccountByUsername(string username)
        {
            return context.Accounts.FirstOrDefault(x => x.Username == username && x.AccountStatus.Equals("active"));
        }

        public List<Account> GetAccounts()
        {
            return context.Accounts.ToList();
        }

        public Account? Login(string username, string password)
        {
            Account a = context.Accounts.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (a!=null&&a.AccountStatus.Equals("active"))
            {
                return a;
            }
            return null;
        }

        public string UpdateAccountStatus(string username, string status)
        {
            Account a = context.Accounts.FirstOrDefault(a => a.Username == username);
            if (a == null)
            {
                return "Account not exist";
            }
            a.AccountStatus = status;
            context.Accounts.Update(a);
            context.SaveChanges();
            return "Update successfully";
        }

        public void UpdateProfile(Account a)
        {
            context.Accounts.Update(a);
            context.SaveChanges();
        }
    }
}
