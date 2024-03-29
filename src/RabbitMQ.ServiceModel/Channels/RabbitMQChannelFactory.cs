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

using RabbitMQ.Client;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    internal sealed class RabbitMQChannelFactory : ChannelFactoryBase<IOutputChannel>
    {
        #region Fields and Constructors

        private readonly Action<TimeSpan> _openMethod;
        private readonly BindingContext _context;
        private readonly RabbitMQTransportBindingElement _bindingElement;
        private IModel _model;

        public RabbitMQChannelFactory(BindingContext bindingContext)
        {
            this._openMethod = this.Open;
            this._context = bindingContext;
            this._bindingElement = bindingContext.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            this._model = null;
        }

        #endregion

        #region Methods

        protected override IOutputChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            return new RabbitMQOutputChannel(this._context, this._model, address);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
#if DEBUG
            DebugHelper.Start();
#endif
            this._model = this._bindingElement.Open(timeout);
#if DEBUG
            DebugHelper.Stop(" ## Out.Open {{Time={0}ms}}.");
#endif
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._openMethod.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this._openMethod.EndInvoke(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
#if DEBUG
            DebugHelper.Start();
#endif

            if (this._model != null)
            {
                this._bindingElement.Close(this._model, timeout);
                this._model = null;
            }

#if DEBUG
            DebugHelper.Stop(" ## Out.Close {{Time={0}ms}}.");
#endif
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            this.OnClose(this._context.Binding.CloseTimeout);
        }

        #endregion
    }
}
