using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotChocolate;

namespace HotChocolateGraphQL.Entities
{
    public class Account
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        public TypeOfAccount Type { get; set; }

        public string Description { get; set; }

        [ForeignKey("OwnerId")]
        public int? OwnerId { get; set; }

        [GraphQLIgnore] //Attribute to ignore property in GraphQL Schema
        public Owner Owner { get; set; }
    }
}