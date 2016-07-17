using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.WCF.Server.Interfaces;

namespace RabbitMQ.WCF.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("商品部分");

            //ChannelFactory<IProductService> productSvcFactory = new ChannelFactory<IProductService>(typeof(IProductService).FullName);

            //for (int i = 0; i < 10; i++)
            //{
            //    IProductService productService = productSvcFactory.CreateChannel();

            //    string product = productService.GetProducts();
            //    Console.WriteLine(product);
            //    Guid newProductId = productService.CreateProduct("新商品");
            //    Console.WriteLine(newProductId);
            //    Console.WriteLine("=========================================================");
            //    Console.WriteLine();
            //}

            //Console.ReadKey();


            ChannelFactory<IOrderService> orderSvcFactory = new ChannelFactory<IOrderService>(typeof(IOrderService).FullName);

            for (int i = 0; i < 10; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                IOrderService orderService = orderSvcFactory.CreateChannel();

                orderService.CreateOrder("编号" + i.ToString("D2"));

                watch.Stop();
                Console.WriteLine("==============================================");
                Console.WriteLine(watch.Elapsed);
            }

            Console.ReadKey();
        }
    }
}
