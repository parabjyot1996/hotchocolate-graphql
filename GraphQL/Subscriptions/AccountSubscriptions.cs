using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using HotChocolateGraphQL.Entities;

namespace HotChocolateGraphQL.GraphQL.Subscriptions
{
    [ExtendObjectType(Name = "Subscription")]
    public class AccountSubscriptions
    {
        [SubscribeAndResolve]
        public async Task<IAsyncEnumerable<Account>> OnAccountCreation(
            [Service]ITopicEventReceiver eventReceiver,
            CancellationToken cancellationToken)
        {
            return await eventReceiver.SubscribeAsync<string, Account>(
                "Created", cancellationToken);
        }
    }
}