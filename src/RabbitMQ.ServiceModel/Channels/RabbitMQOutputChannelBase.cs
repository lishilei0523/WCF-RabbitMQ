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
    internal abstract class RabbitMQOutputChannelBase : RabbitMQChannelBase, IOutputChannel
    {
        #region Fields and Constructors

        private readonly EndpointAddress _endpointAddress;
        private readonly Action<Message, TimeSpan> _sendMethod;

        protected RabbitMQOutputChannelBase(BindingContext bindingContext, EndpointAddress endpointAddress)
            : base(bindingContext)
        {
            this._endpointAddress = endpointAddress;
            this._sendMethod = this.Send;
        }

        #endregion

        #region Properties

        public EndpointAddress RemoteAddress
        {
            get { return this._endpointAddress; }
        }

        public Uri Via
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Methods

        public virtual void Send(Message message)
        {
            this.Send(message, base.BindingContext.Binding.SendTimeout);
        }

        public abstract void Send(Message message, TimeSpan timeout);

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return this._sendMethod.BeginInvoke(message, base.BindingContext.Binding.SendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._sendMethod.BeginInvoke(message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            this._sendMethod.EndInvoke(result);
        }

        #endregion
    }
}
