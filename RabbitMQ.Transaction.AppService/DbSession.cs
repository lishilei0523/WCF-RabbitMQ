using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Transaction.AppService.Entities;

namespace RabbitMQ.Transaction.AppService
{
    public class DbSession : DbContext
    {
        public DbSession()
            : base("name=RabbitMQ")
        {
            this.Database.CreateIfNotExists();
        }

        public DbSet<Product> Products { get; set; }
    }
}
