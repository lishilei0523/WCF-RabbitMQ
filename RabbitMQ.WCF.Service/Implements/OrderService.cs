using RabbitMQ.WCF.Service.Interfaces;
using System;
using System.ServiceModel;

namespace RabbitMQ.WCF.Service.Implements
{
    /// <summary>
    /// 订单管理服务实现
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class OrderService : IOrderService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns>订单编号</returns>
        public void CreateOrder(string orderNo)
        {
            Console.WriteLine(orderNo);
        }
    }
}
