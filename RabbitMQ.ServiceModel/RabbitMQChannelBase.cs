using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    internal abstract class RabbitMQChannelBase : IChannel
    {
        private readonly Action<TimeSpan> _closeMethod;
        private readonly BindingContext _context;
        private readonly Action<TimeSpan> _openMethod;
        private CommunicationState _state;

        private RabbitMQChannelBase()
        {
            _state = CommunicationState.Created;
            _closeMethod = Close;
            _openMethod = Open;
        }

        protected RabbitMQChannelBase(BindingContext context)
            : this()
        {
            _context = context;
        }

        protected BindingContext Context
        {
            get { return _context; }
        }

        protected String Exchange
        {
            get { return "amq.direct"; }
        }

        public abstract void Close(TimeSpan timeout);

        public abstract void Open(TimeSpan timeout);

        public virtual void Abort()
        {
            Close();
        }

        public virtual void Close()
        {
            Close(_context.Binding.CloseTimeout);
        }

        public virtual T GetProperty<T>() where T : class
        {
            return default(T);
        }

        public virtual void Open()
        {
            Open(_context.Binding.OpenTimeout);
        }

        public CommunicationState State
        {
            get { return _state; }
        }

        public event EventHandler Closed;

        public event EventHandler Closing;

        public event EventHandler Faulted;

        public event EventHandler Opened;

        public event EventHandler Opening;

        #region Async Methods

        public virtual IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _closeMethod.BeginInvoke(timeout, callback, state);
        }

        public virtual IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return _closeMethod.BeginInvoke(_context.Binding.CloseTimeout, callback, state);
        }

        public virtual IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _openMethod.BeginInvoke(timeout, callback, state);
        }

        public virtual IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return _openMethod.BeginInvoke(_context.Binding.OpenTimeout, callback, state);
        }

        public virtual void EndClose(IAsyncResult result)
        {
            _closeMethod.EndInvoke(result);
        }

        public virtual void EndOpen(IAsyncResult result)
        {
            _openMethod.EndInvoke(result);
        }

        #endregion

        #region Event Raising Methods

        protected void OnOpening()
        {
            _state = CommunicationState.Opening;
            if (Opening != null)
                Opening(this, null);
        }

        protected void OnOpened()
        {
            _state = CommunicationState.Opened;
            if (Opened != null)
                Opened(this, null);
        }

        protected void OnClosing()
        {
            _state = CommunicationState.Closing;
            if (Closing != null)
                Closing(this, null);
        }

        protected void OnClosed()
        {
            _state = CommunicationState.Closed;
            if (Closed != null)
                Closed(this, null);
        }

        protected void OnFaulted()
        {
            _state = CommunicationState.Faulted;
            if (Faulted != null)
                Faulted(this, null);
        }

        #endregion
    }
}
