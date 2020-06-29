using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using HotChocolateGraphQL.Contracts;
using HotChocolateGraphQL.Entities;

namespace HotChocolateGraphQL.GraphQL.Mutations
{
    [ExtendObjectType(Name = "Mutation")]
    public class OwnerMutations
    {
        private readonly IOwnerRepository _repository;

        public OwnerMutations(IOwnerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Owner> CreateOwner(Owner owner)
        {
            if (owner == null)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("Please provide valid data")
                        .SetCode("OWNER_NULL")
                        .Build());
            }

            var result = await _repository.CreateOwner(owner);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(result.ErrorMessage)
                        .SetCode("ERROR")
                        .Build());
            }

            return result.ModelData;
        }

        public async Task<Owner> UpdateOwner(int ownerId, Owner owner)
        {
            if (ownerId == 0)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("Please provide owner id to update")
                        .SetCode("OWNER_ID_NULL")
                        .Build());
            }

            var result = await _repository.UpdateOwner(ownerId, owner);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(result.ErrorMessage)
                        .SetCode("OWNER_NOT_FOUND")
                        .Build());
            }

            return result.ModelData;
        }

        public async Task<string> DeleteOwner(int ownerId)
        {
            if (ownerId == 0)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage($"Please provide owner id")
                        .SetCode("OWNER_ID_NULL")
                        .Build());
            }

            if (await _repository.DeleteOwner(ownerId))
            {
                return $"Owner with id { ownerId } deleted successfully";
            }
            else
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage($"Owner with id { ownerId } not found")
                        .SetCode("OWNER_NOT_FOUND")
                        .Build());
            }
        }
    }
}