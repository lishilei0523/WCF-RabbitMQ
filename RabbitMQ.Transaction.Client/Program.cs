using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RabbitMQ.Transaction.AppService.Entities;
using RabbitMQ.Transaction.AppService.Interfaces;
using SD.IOC.Core.Mediator;

namespace RabbitMQ.Transaction.Client
{
    class Program
    {
        static void Main()
        {
            #region # 测试事务

            //Person person = new Person { Name = "人员1" };

            //DbSession dbSession = new DbSession();

            //dbSession.Persons.Add(person);

            //IProductService productService = ResolveMediator.Resolve<IProductService>();

            //using (TransactionScope scope = new TransactionScope())
            //{
            //    dbSession.SaveChanges();

            //    productService.CreateProduct("商品1", 15);

            //    scope.Complete();
            //}

            //Console.WriteLine("OK");
            //Console.ReadKey(); 

            #endregion

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Parallel.For(0, 200, index =>
            {
                IProductService productService = ResolveMediator.Resolve<IProductService>();

                IEnumerable<Product> products = productService.GetProducts();

                foreach (Product product in products)
                {
                    Console.WriteLine(product.Name);
                }
            });
            watch.Stop();

            ResolveMediator.Dispose();

            Console.WriteLine("===========================================");
            Console.WriteLine(watch.Elapsed);
            Console.ReadKey();
        }
    }
}
