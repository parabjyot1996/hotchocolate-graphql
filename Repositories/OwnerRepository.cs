using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolateGraphQL.Contracts;
using HotChocolateGraphQL.Entities;
using HotChocolateGraphQL.Entities.Context;
using Microsoft.EntityFrameworkCore;

namespace GraphQLServer.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Owner>> CreateOwner(Owner owner)
        {
            Response<Owner> response = new Response<Owner>();
            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();
            
            response.ModelData = owner;
            return response;
        }

        public async Task<bool> DeleteOwner(int ownerId)
        {
            var dbOwner = _context.Owners.SingleOrDefault(o => o.Id == ownerId);

            if (dbOwner == null)
            {
                return false;
            }

            _context.Owners.Remove(dbOwner);
            await _context.SaveChangesAsync();

            return true;
        }

        public IQueryable<Owner> GetAll()
        {
            return _context.Owners;
        }

        public IQueryable<Owner> GetOwnerById(int id)
        {
            return _context.Owners.Where(o => o.Id == id);
        }

        public async Task<Response<Owner>> UpdateOwner(int ownerId, Owner owner)
        {
            Response<Owner> response = new Response<Owner>();
            var dbOwner = _context.Owners.Include(o => o.Accounts).SingleOrDefault(o => o.Id == ownerId);

            if (dbOwner == null)
            {
                response.ErrorMessage = $"Owner with id { ownerId } not found";
                return response;
            }

            dbOwner.Name = owner.Name;
            dbOwner.Address = owner.Address;

            if (owner.Accounts != null)
            {
                foreach (var account in owner.Accounts)
                {
                    dbOwner.Accounts.Add(account);
                }
            }

            _context.Owners.Update(dbOwner);
            await _context.SaveChangesAsync();

            response.ModelData = dbOwner;
            return response;
        }

        public async Task<ILookup<string, Owner>> GetOwnersByAddress(IReadOnlyList<string> addresses, CancellationToken cancellationToken)
        {
            var accounts = await _context.Owners.Where(a => addresses.Contains(a.Address)).ToListAsync();
            return accounts.ToLookup(x => x.Address);
        }
    }
}