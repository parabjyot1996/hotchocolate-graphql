using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using HotChocolateGraphQL.Contracts;
using HotChocolateGraphQL.Entities;

namespace HotChocolateGraphQL.GraphQL.Queries
{
    [ExtendObjectType(Name = "Query")]
    public class OwnerQueries
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnerQueries(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        [UsePaging]
        [UseSelection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Owner> GetOwners() =>  _ownerRepository.GetAll();

        [UseFirstOrDefault]
        [UseSelection]
        public IQueryable<Owner> GetOwnerById(int ownerId) => _ownerRepository.GetOwnerById(ownerId); 

        public async Task<IEnumerable<Owner>> GetOwnersByAddress(string address, IResolverContext context)
        {
            IDataLoader<string, Owner[]> ownerDataLoader =
                        context.GroupDataLoader<string, Owner>(
                            "ownersByAddress",
                            _ownerRepository.GetOwnersByAddress);

            return await ownerDataLoader.LoadAsync(context.Argument<string>("address"));
        }
    }
}