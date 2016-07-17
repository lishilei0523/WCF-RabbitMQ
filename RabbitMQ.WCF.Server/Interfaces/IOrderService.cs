using System.ServiceModel;

namespace RabbitMQ.WCF.Server.Interfaces
{
    /// <summary>
    /// 订单管理服务接口
    /// </summary>
    [ServiceContract]
    public interface IOrderService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns>订单编号</returns>
        [OperationContract(IsOneWay = true)]
        void CreateOrder(string orderNo);
    }
}
