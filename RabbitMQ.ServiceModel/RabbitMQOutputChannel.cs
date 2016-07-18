using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RabbitMQ.Client;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Lee 修改过队列声明部分</remarks>
    internal sealed class RabbitMQOutputChannel : RabbitMQOutputChannelBase
    {
        private static readonly object _SyncLock = new object();


        private RabbitMQTransportBindingElement m_bindingElement;
        private MessageEncoder m_encoder;
        private IModel m_model { get; set; }

        public RabbitMQOutputChannel(BindingContext context, IModel model, EndpointAddress address)
            : base(context, address)
        {
            m_bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            MessageEncodingBindingElement encoderElement = context.Binding.Elements.Find<MessageEncodingBindingElement>();
            if (encoderElement != null)
            {
                m_encoder = encoderElement.CreateMessageEncoderFactory().Encoder;
            }
            m_model = model;
        }

        public override void Send(Message message, TimeSpan timeout)
        {
            if (message.State != MessageState.Closed)
            {
                byte[] body = null;
#if VERBOSE
                DebugHelper.Start();
#endif
                using (MemoryStream str = new MemoryStream())
                {
                    m_encoder.WriteMessage(message, str);
                    body = str.ToArray();
                }
#if VERBOSE
                DebugHelper.Stop(" #### Message.Send {{\n\tAction={2}, \n\tBytes={1}, \n\tTime={0}ms}}.",
                    body.Length,
                    message.Headers.Action.Remove(0, message.Headers.Action.LastIndexOf('/')));
#endif
                /********Lee修改部分********/
                lock (_SyncLock)
                {
                    m_model.QueueDeclare(base.RemoteAddress.Uri.PathAndQuery, false, false, false, null);
                    m_model.BasicPublish(string.Empty, base.RemoteAddress.Uri.PathAndQuery, null, body);
                }
            }
        }

        public override void Close(TimeSpan timeout)
        {
            if (base.State == CommunicationState.Closed || base.State == CommunicationState.Closing)
                return; // Ignore the call, we're already closing.

            OnClosing();
            OnClosed();
        }

        public override void Open(TimeSpan timeout)
        {
            if (base.State != CommunicationState.Created && base.State != CommunicationState.Closed)
                throw new InvalidOperationException(string.Format("Cannot open the channel from the {0} state.", base.State));

            OnOpening();
            OnOpened();
        }
    }
}
