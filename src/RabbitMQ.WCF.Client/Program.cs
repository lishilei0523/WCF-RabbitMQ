using RabbitMQ.WCF.IAppService.Interfaces;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

namespace RabbitMQ.WCF.Client
{
    class Program
    {
        static async Task Main()
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

            //Parallel.For(0, 10, index =>
            //{
            //    IOrderService orderService = orderSvcFactory.CreateChannel();

            //    string orderNo = $"编号{index:D3}";
            //    string result = orderService.CreateOrder(orderNo);
            //    Console.WriteLine(result);
            //});

            for (int index = 1; index <= 100; index++)
            {
                IOrderService orderService = orderSvcFactory.CreateChannel();

                string orderNo = $"编号{index:D3}";
                string result = await Task.Run(() => orderService.CreateOrder(orderNo));
                Console.WriteLine(result);
            }

            watch.Stop();

            Console.WriteLine("==============================================");
            Console.WriteLine(watch.Elapsed);
            Console.ReadKey();
        }
    }
}
