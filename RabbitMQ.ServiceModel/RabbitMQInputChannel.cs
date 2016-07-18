using System;
using System.Collections.Concurrent;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Lee 修改过队列声明部分</remarks>
    internal sealed class RabbitMQInputChannel : RabbitMQInputChannelBase
    {
        private static readonly object _SyncLock = new object();

        private RabbitMQTransportBindingElement m_bindingElement;
        private MessageEncoder m_encoder;
        private IModel m_model { get; set; }
        private EventingBasicConsumer m_consumer;
        private BlockingCollection<BasicDeliverEventArgs> m_queue =
            new BlockingCollection<BasicDeliverEventArgs>(new ConcurrentQueue<BasicDeliverEventArgs>());

        public RabbitMQInputChannel(BindingContext context, IModel model, EndpointAddress address)
            : base(context, address)
        {
            m_bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            TextMessageEncodingBindingElement encoderElem = context.BindingParameters.Find<TextMessageEncodingBindingElement>();
            encoderElem.ReaderQuotas.MaxStringContentLength = (int)m_bindingElement.MaxReceivedMessageSize;
            if (encoderElem != null)
            {
                m_encoder = encoderElem.CreateMessageEncoderFactory().Encoder;
            }
            m_model = model;
            m_consumer = null;
        }


        public override Message Receive(TimeSpan timeout) //TODO: timeout isn't used
        {
            try
            {
                BasicDeliverEventArgs msg = m_queue.Take();
#if VERBOSE
                DebugHelper.Start();
#endif
                Message result = m_encoder.ReadMessage(new MemoryStream(msg.Body), (int)m_bindingElement.MaxReceivedMessageSize);
                result.Headers.To = base.LocalAddress.Uri;
                m_consumer.Model.BasicAck(msg.DeliveryTag, false);
#if VERBOSE
                DebugHelper.Stop(" #### Message.Receive {{\n\tAction={2}, \n\tBytes={1}, \n\tTime={0}ms}}.",
                        msg.Body.Length,
                        result.Headers.Action.Remove(0, result.Headers.Action.LastIndexOf('/')));
#endif
                return result;
            }
            catch (EndOfStreamException)
            {
                if (m_consumer == null || m_consumer.ShutdownReason != null && m_consumer.ShutdownReason.ReplyCode != Constants.ReplySuccess)
                {
                    OnFaulted();
                }
                Close();
                return null;
            }
        }

        public override bool TryReceive(TimeSpan timeout, out Message message)
        {
            message = Receive(timeout);
            return true;
        }

        public override bool WaitForMessage(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public override void Close(TimeSpan timeout)
        {

            if (base.State == CommunicationState.Closed
                || base.State == CommunicationState.Closing)
            {
                return; // Ignore the call, we're already closing.
            }

            OnClosing();
#if VERBOSE
            DebugHelper.Start();
#endif
            if (m_consumer != null)
            {
                m_model.BasicCancel(m_consumer.ConsumerTag);
                m_consumer = null;
            }
#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Close {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            OnClosed();
        }

        public override void Open(TimeSpan timeout)
        {
            if (State != CommunicationState.Created && State != CommunicationState.Closed)
                throw new InvalidOperationException(string.Format("Cannot open the channel from the {0} state.", base.State));

            OnOpening();
#if VERBOSE
            DebugHelper.Start();
#endif
            /********Lee修改部分********/
            lock (_SyncLock)
            {
                string queue = m_model.QueueDeclare(base.LocalAddress.Uri.PathAndQuery, false, false, false, null);
                m_consumer = new EventingBasicConsumer(m_model);
                m_consumer.Received += (sender, args) => m_queue.Add(args);
                m_model.BasicConsume(queue, false, m_consumer);
            }

#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Open {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            OnOpened();
        }
    }
}
