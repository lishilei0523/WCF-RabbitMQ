#region License

// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (c) 2007-2016 Pivotal Software, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License. 

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    internal abstract class RabbitMQChannelBase : IChannel
    {
        #region Fields and Constructors

        public event EventHandler Opening;
        public event EventHandler Opened;
        public event EventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler Faulted;

        private readonly Action<TimeSpan> _openMethod;
        private readonly Action<TimeSpan> _closeMethod;
        private readonly BindingContext _bindingContext;
        private CommunicationState _state;

        private RabbitMQChannelBase()
        {
            this._openMethod = this.Open;
            this._closeMethod = this.Close;
            this._state = CommunicationState.Created;
        }

        protected RabbitMQChannelBase(BindingContext bindingContext)
            : this()
        {
            this._bindingContext = bindingContext;
        }

        #endregion

        #region Properties

        public CommunicationState State
        {
            get { return this._state; }
        }

        protected BindingContext BindingContext
        {
            get { return this._bindingContext; }
        }

        protected string Exchange
        {
            get { return "amq.direct"; }
        }

        #endregion

        #region Methods

        public abstract void Close(TimeSpan timeout);

        public abstract void Open(TimeSpan timeout);

        public virtual void Open()
        {
            this.Open(this._bindingContext.Binding.OpenTimeout);
        }

        public virtual void Abort()
        {
            this.Close();
        }

        public virtual void Close()
        {
            this.Close(this._bindingContext.Binding.CloseTimeout);
        }

        public virtual T GetProperty<T>() where T : class
        {
            return default(T);
        }

        public virtual IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._openMethod.BeginInvoke(timeout, callback, state);
        }

        public virtual IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return this._openMethod.BeginInvoke(this._bindingContext.Binding.OpenTimeout, callback, state);
        }

        public virtual void EndOpen(IAsyncResult result)
        {
            this._openMethod.EndInvoke(result);
        }

        public virtual IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._closeMethod.BeginInvoke(timeout, callback, state);
        }

        public virtual IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return this._closeMethod.BeginInvoke(this._bindingContext.Binding.CloseTimeout, callback, state);
        }

        public virtual void EndClose(IAsyncResult result)
        {
            this._closeMethod.EndInvoke(result);
        }

        #endregion

        #region Event Raising Methods

        protected void OnOpening()
        {
            this._state = CommunicationState.Opening;
            this.Opening?.Invoke(this, null);
        }

        protected void OnOpened()
        {
            this._state = CommunicationState.Opened;
            this.Opened?.Invoke(this, null);
        }

        protected void OnClosing()
        {
            this._state = CommunicationState.Closing;
            this.Closing?.Invoke(this, null);
        }

        protected void OnClosed()
        {
            this._state = CommunicationState.Closed;
            this.Closed?.Invoke(this, null);
        }

        protected void OnFaulted()
        {
            this._state = CommunicationState.Faulted;
            this.Faulted?.Invoke(this, null);
        }

        #endregion
    }
}
