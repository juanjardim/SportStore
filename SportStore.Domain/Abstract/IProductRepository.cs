using System.Collections.Generic;
using SportStore.Domain.Entities;

namespace SportStore.Domain.Abstract
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }
    }
}