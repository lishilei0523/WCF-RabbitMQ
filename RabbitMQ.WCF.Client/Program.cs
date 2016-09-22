using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using RabbitMQ.WCF.Server.Interfaces;

namespace RabbitMQ.WCF.Client
{
    class Program
    {
        static void Main()
        {
            //Console.WriteLine("商品部分");

            //ChannelFactory<IProductService> productSvcFactory = new ChannelFactory<IProductService>(typeof(IProductService).FullName);

            //Parallel.For(0, 100, index =>
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();

            //    IProductService productService = productSvcFactory.CreateChannel();

            //    string product = productService.GetProducts();
            //    Console.WriteLine(product);
            //    Guid newProductId = productService.CreateProduct("新商品");

            //    watch.Stop();

            //    Console.WriteLine(newProductId);
            //    Console.WriteLine("=========================================================");
            //    Console.WriteLine(watch.Elapsed);
            //});

            //Console.ReadKey();


            ChannelFactory<IOrderService> orderSvcFactory = new ChannelFactory<IOrderService>(typeof(IOrderService).FullName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Parallel.For(0, 100, index =>
            {
                IOrderService orderService = orderSvcFactory.CreateChannel();

                orderService.CreateOrder("编号" + index.ToString("D2"));
            });

            watch.Stop();

            Console.WriteLine("==============================================");
            Console.WriteLine(watch.Elapsed);
            Console.ReadKey();
        }
    }
}
