using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolateGraphQL.Entities;

namespace HotChocolateGraphQL.Contracts
{
    public interface IOwnerRepository
    {
        IQueryable<Owner> GetAll();

        IQueryable<Owner> GetOwnerById(int id);

        Task<Response<Owner>> CreateOwner(Owner owner);

        Task<Response<Owner>> UpdateOwner(int ownerId, Owner owner);

        Task<bool> DeleteOwner(int ownerId);

         Task<ILookup<string, Owner>> GetOwnersByAddress(IReadOnlyList<string> addresses, 
                                                            CancellationToken cancellationToken);
    }
}