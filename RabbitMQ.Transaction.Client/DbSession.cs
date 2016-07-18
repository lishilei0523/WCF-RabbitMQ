using System.Data.Entity;

namespace RabbitMQ.Transaction.Client
{
    public class DbSession : DbContext
    {
        public DbSession()
            : base("name=RabbitMQ_Client")
        {
            this.Database.CreateIfNotExists();
        }

        public DbSet<Person> Persons { get; set; }
    }
}
