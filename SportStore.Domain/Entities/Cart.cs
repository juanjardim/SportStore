using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportStore.Domain.Entities
{
    public class Cart
    {
        private readonly List<CartLine> _lineCollection = new List<CartLine>();

        public void AddItem(Product product, int quantity)
        {
            var line = _lineCollection
                .FirstOrDefault(p => p.Product.ProductID == product.ProductID);

            if (line == null)
            {
                _lineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product)
        {
            _lineCollection.RemoveAll(l => l.Product.ProductID == product.ProductID);
        }

        public decimal ComputeTotalValue()
        {
            return _lineCollection.Sum(p => p.Product.Price*p.Quantity);
        }

        public void Clear()
        {
            _lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines => _lineCollection;
    }

    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
