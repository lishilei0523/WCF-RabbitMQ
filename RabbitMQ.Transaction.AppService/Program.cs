using System;
using System.Linq;
using System.ServiceModel;
using RabbitMQ.Transaction.AppService.Entities;
using RabbitMQ.Transaction.AppService.Implements;

namespace RabbitMQ.Transaction.AppService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost productSvcHost = new ServiceHost(typeof(ProductService));

            productSvcHost.Open();

            Console.WriteLine("服务已启动...");

            InitData();

            Console.ReadLine();
        }


        static void InitData()
        {
            DbSession dbSession = new DbSession();
            if (!dbSession.Set<Product>().Any())
            {
                for (int i = 0; i < 1000; i++)
                {
                    Product product = new Product(i.ToString(), i);

                    dbSession.Set<Product>().Add(product);
                }

                dbSession.SaveChanges();
            }
            dbSession.Dispose();
        }
    }
}
