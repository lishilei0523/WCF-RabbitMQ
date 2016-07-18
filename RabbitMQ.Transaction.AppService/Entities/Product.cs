using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Transaction.AppService.Entities
{

    public class Product
    {
        public Product() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Product(string name, decimal price)
        {
            this.Name = name;
            this.Price = price;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
