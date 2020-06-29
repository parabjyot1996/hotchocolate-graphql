using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using HotChocolateGraphQL.Contracts;
using HotChocolateGraphQL.Entities;

namespace HotChocolateGraphQL.GraphQL.Mutations
{
    [ExtendObjectType(Name = "Mutation")]
    public class AccountMutations
    {
        private readonly IAccountRepository _repository;

        public AccountMutations(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<Account> CreateAccount([Service]ITopicEventSender eventSender,
            CancellationToken cancellationToken, 
            Account account)
        {
            if (account == null)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("Please provide valid data")
                        .SetCode("ACCOUNT_NULL")
                        .Build());
            }

            var result = await _repository.CreateAccount(account);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(result.ErrorMessage)
                        .SetCode("OWNER_ID_INVALID")
                        .Build());
            }

            await eventSender.SendAsync("Created", result, cancellationToken);
            return result.ModelData;
        }

        public async Task<Account> UpdateAccount(int accountId, Account account)
        {
            if (accountId == 0)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("Please provide account id to update")
                        .SetCode("ACCOUNT_ID_NULL")
                        .Build());
            }

            var result = await _repository.UpdateAccount(accountId, account);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(result.ErrorMessage)
                        .SetCode("ACCOUNT_NOT_FOUND")
                        .Build());
            }

            return result.ModelData;
        }

        public async Task<string> DeleteAccount(int accountId)
        {
            if (accountId == 0)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage($"Please provide account id")
                        .SetCode("ACCOUNT_ID_NULL")
                        .Build());
            }

            if (await _repository.DeleteAccount(accountId))
            {
                return $"Account with account id { accountId } deleted successfully";
            }
            else
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage($"Account with account id { accountId } not found")
                        .SetCode("ACCOUNT_NOT_FOUND")
                        .Build());
            }
        }
    }
}