using System;
using System.Threading;
using RabbitMQ.WCF.Server.Interfaces;

namespace RabbitMQ.WCF.Server.Implements
{
    /// <summary>
    /// 订单管理服务实现
    /// </summary>
    public class OrderService : IOrderService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns>订单编号</returns>
        public void CreateOrder(string orderNo)
        {
            Thread.Sleep(3000);
            Console.WriteLine(orderNo);
        }
    }
}
