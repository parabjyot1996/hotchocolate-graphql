using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolateGraphQL.Contracts;
using HotChocolateGraphQL.Entities;
using HotChocolateGraphQL.Entities.Context;
using Microsoft.EntityFrameworkCore;

namespace GraphQLServer.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Account> GetAccountByOwnerId(IEnumerable<int?> ownerIds)
        {
            return _context.Accounts.Where((acc) => ownerIds.Contains(acc.OwnerId));
        }

        public IQueryable<Account> GetAllAccount()
        {
            return _context.Accounts;
        }

        public Account GetAccountById(int id)
        {
            return _context.Accounts.SingleOrDefault(acc => acc.Id == id);
        }

        public async Task<Response<Account>> CreateAccount(Account account)
        {
            Response<Account> response = new Response<Account>();
            var owner = _context.Owners.SingleOrDefault(o => o.Id == account.OwnerId);
            if (owner == null)
            {
                response.ErrorMessage = $"Owner Id { account.OwnerId } does not exist";
                return response;
            }

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            response.ModelData = account;
            
            return response;
        }

        public async Task<Response<Account>> UpdateAccount(int accountId, Account account)
        {
            Response<Account> response = new Response<Account>();
            var accountDb = _context.Accounts.SingleOrDefault(acc => acc.Id == accountId);

            if (accountDb == null)
            {
                response.ErrorMessage = $"Account id { accountId } does not exist";
                return response;
            }

            var owner = _context.Owners.SingleOrDefault(o => o.Id == account.OwnerId);
            if (owner == null)
            {
                response.ErrorMessage = $"Owner Id { account.OwnerId } does not exist";
                return response;
            }

            accountDb.Type = account.Type;
            accountDb.Description = account.Description;
            accountDb.OwnerId = account.OwnerId;

            _context.Accounts.Update(accountDb);
            await _context.SaveChangesAsync();

            response.ModelData = accountDb;
            return response;
        }

        public async Task<bool> DeleteAccount(int accountId)
        {
            var account = _context.Accounts.SingleOrDefault(acc => acc.Id == accountId);

            if (account == null)
            {
                return false;
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}