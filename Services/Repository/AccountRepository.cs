using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Repository
{
    public interface IAccountRepository : IRepositoryBase<Account>
    {
        Account GetAccount(LoginModel loginModel);
        Account GetAccountById(int key);
        string ChekcUsernameAndEmail(string username, string email);
        Account CreateAccount(SingUpModel acc);
        void UpdateAccount(Account account);
        void DeleteAccount(Account account);
    }
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public Account GetAccount(LoginModel loginModel)
        {
            Account? result = new Account();
            result = _repositoryContext.Account.SingleOrDefault(p =>
             p.Username==loginModel.Username && p.Password==loginModel.Password);
            return result;
        }

        public Account GetAccountById(int key)
        {
            Account? result = new Account();
            result = _repositoryContext.Account.SingleOrDefault(p =>
             p.AccountID == key);
            return result;
        }

        public string ChekcUsernameAndEmail(string username, string email)
        {
            Account? a = new Account();
            a = _repositoryContext.Account.SingleOrDefault(p => p.Username == username);
            if (a != null)
                return "Username already exist!";
            a = _repositoryContext.Account.SingleOrDefault(p => p.Email == email);
            if (a != null)
                return "Email already exist!";
            return null;
        }
        public Account CreateAccount(SingUpModel acc)
        {
            var account = new Account();
            try
            {
                account.Username = acc.UserName;
                account.Password = acc.Password;
                account.FullName = acc.FullName;
                account.Email = acc.Email;
                Create(account);
                _repositoryContext.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return account;

        }
        public void UpdateAccount(Account account)
        {
            Update(account);
        }

        public void DeleteAccount(Account account)
        {
            Delete(account);
        }
    }
}
