using System.ServiceModel;

namespace RabbitMQ.WCF.IAppService.Interfaces
{
    /// <summary>
    /// 订单管理服务契约接口
    /// </summary>
    [ServiceContract]
    public interface IOrderContract
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns>订单编号</returns>
        [OperationContract]
        string CreateOrder(string orderNo);
    }
}
