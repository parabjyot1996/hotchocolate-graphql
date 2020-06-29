using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;

namespace HotChocolateGraphQL.Entities
{
    [Authorize]
    public class Owner
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [UseFiltering] //Attribute for EF projections
        public ICollection<Account> Accounts { get; set; }
    }
}