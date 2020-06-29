using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolateGraphQL.Entities;

namespace HotChocolateGraphQL.Contracts
{
    public interface IAccountRepository
    {
        IQueryable<Account> GetAccountByOwnerId(IEnumerable<int?> ownerId);

        IQueryable<Account> GetAllAccount();

        Account GetAccountById(int id);

        Task<Response<Account>> CreateAccount(Account account);

        Task<Response<Account>> UpdateAccount(int accountId, Account account);

        Task<bool> DeleteAccount(int accountId);
    }
}