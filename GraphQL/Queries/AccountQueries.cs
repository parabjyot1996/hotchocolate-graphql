using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using HotChocolateGraphQL.Contracts;
using HotChocolateGraphQL.Entities;

namespace HotChocolateGraphQL.GraphQL.Queries
{
    [ExtendObjectType(Name = "Query")]
    public class AccountQueries
    {
        private readonly IAccountRepository _account;

        public AccountQueries(IAccountRepository account)
        {
            _account = account;
        }

        [UsePaging]
        [UseSelection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Account> GetAccounts() => _account.GetAllAccount();

        [UsePaging]
        [UseSelection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Account> GetAccountsByOwnerId(IEnumerable<int?> ownerIds) => _account.GetAccountByOwnerId(ownerIds);
    }
}