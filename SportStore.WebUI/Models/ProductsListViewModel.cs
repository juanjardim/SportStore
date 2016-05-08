using System.Collections.Generic;
using SportStore.Domain.Entities;

namespace SportStore.WebUI.Models
{
    public class ProductsListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PageInfo PageInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}
