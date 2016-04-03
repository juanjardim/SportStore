using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;

namespace SportStore.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private readonly EFDbContext context = new EFDbContext();

        public IEnumerable<Product> Products => context.Products;
    }
}
