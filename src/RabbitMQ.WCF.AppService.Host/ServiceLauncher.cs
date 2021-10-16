using RabbitMQ.WCF.AppService.Implements;
using System;
using System.ServiceModel;

namespace RabbitMQ.WCF.AppService.Host
{
    /// <summary>
    /// 服务启动器
    /// </summary>
    public class ServiceLauncher
    {
        private readonly ServiceHost _productSvcHost;
        private readonly ServiceHost _orderSvcHost;

        /// <summary>
        /// 构造器
        /// </summary>
        public ServiceLauncher()
        {
            this._productSvcHost = new ServiceHost(typeof(ProductService));
            this._orderSvcHost = new ServiceHost(typeof(OrderService));
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            this._productSvcHost.Open();
            this._orderSvcHost.Open();

            Console.WriteLine("服务已启动...");
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this._productSvcHost.Close();
            this._orderSvcHost.Close();

            Console.WriteLine("服务已关闭...");
        }
    }
}
