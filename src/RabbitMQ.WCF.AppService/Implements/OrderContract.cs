﻿using RabbitMQ.WCF.IAppService.Interfaces;
using System;
#if NET461_OR_GREATER
using System.ServiceModel;
#endif
#if NETSTANDARD2_0_OR_GREATER
using CoreWCF;
#endif

namespace RabbitMQ.WCF.AppService.Implements
{
    /// <summary>
    /// 订单管理服务契约实现
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class OrderContract : IOrderContract
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns>订单编号</returns>
        public string CreateOrder(string orderNo)
        {
            Console.WriteLine(orderNo);

            return orderNo;
        }
    }
}
