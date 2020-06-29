using System.Linq;

namespace HotChocolateGraphQL.Entities
{
    public class Response<T>
    {
        public string ErrorMessage { get; set; }

        public IQueryable<T> QueryableData { get; set; }

        public T ModelData { get; set; }
    }
}