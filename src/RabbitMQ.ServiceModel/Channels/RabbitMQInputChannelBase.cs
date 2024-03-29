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
    internal abstract class RabbitMQInputChannelBase : RabbitMQChannelBase, IInputChannel
    {
        #region Fields and Constructors

        private readonly EndpointAddress _localAddress;
        private readonly Func<TimeSpan, Message> _receiveMethod;
        private readonly CommunicationOperation<bool, Message> _tryReceiveMethod;
        private readonly Func<TimeSpan, bool> _waitForMessage;

        protected RabbitMQInputChannelBase(BindingContext bindingContext, EndpointAddress localAddress)
            : base(bindingContext)
        {
            this._localAddress = localAddress;
            this._receiveMethod = this.Receive;
            this._tryReceiveMethod = this.TryReceive;
            this._waitForMessage = this.WaitForMessage;
        }

        #endregion

        #region Properties

        public EndpointAddress LocalAddress
        {
            get { return this._localAddress; }
        }

        #endregion

        #region Methods

        public virtual Message Receive()
        {
            return this.Receive(base.BindingContext.Binding.ReceiveTimeout);
        }

        public abstract Message Receive(TimeSpan timeout);

        public virtual IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this._receiveMethod.BeginInvoke(base.BindingContext.Binding.ReceiveTimeout, callback, state);
        }

        public virtual IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._receiveMethod.BeginInvoke(timeout, callback, state);
        }

        public abstract bool TryReceive(TimeSpan timeout, out Message message);

        public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Message message;
            return this._tryReceiveMethod.BeginInvoke(timeout, out message, callback, state);
        }

        public virtual Message EndReceive(IAsyncResult result)
        {
            return this._receiveMethod.EndInvoke(result);
        }

        public virtual bool EndTryReceive(IAsyncResult result, out Message message)
        {
            return this._tryReceiveMethod.EndInvoke(out message, result);
        }

        public abstract bool WaitForMessage(TimeSpan timeout);

        public virtual IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._waitForMessage.BeginInvoke(timeout, callback, state);
        }

        public virtual bool EndWaitForMessage(IAsyncResult result)
        {
            return this._waitForMessage.EndInvoke(result);
        }

        #endregion
    }
}
