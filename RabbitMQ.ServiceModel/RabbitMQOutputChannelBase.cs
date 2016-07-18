using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    internal abstract class RabbitMQOutputChannelBase : RabbitMQChannelBase, IOutputChannel
    {
        private readonly EndpointAddress _address;
        private readonly Action<Message, TimeSpan> _sendMethod;

        protected RabbitMQOutputChannelBase(BindingContext context, EndpointAddress address)
            : base(context)
        {
            _address = address;
            _sendMethod = Send;
        }

        public EndpointAddress RemoteAddress
        {
            get { return _address; }
        }

        public Uri Via
        {
            get { throw new NotImplementedException(); }
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _sendMethod.BeginInvoke(message, timeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return _sendMethod.BeginInvoke(message, Context.Binding.SendTimeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            _sendMethod.EndInvoke(result);
        }

        public abstract void Send(Message message, TimeSpan timeout);

        public virtual void Send(Message message)
        {
            Send(message, Context.Binding.SendTimeout);
        }
    }
}
