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
using RabbitMQ.Client.Exceptions;
using System;
using System.Configuration;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// Represents the binding element used to specify AMQP transport for transmitting messages.
    /// </summary>
    public sealed class RabbitMQTransportBindingElement : TransportBindingElement
    {
        #region Fields and Constructors

        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private string _vhost;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;

        /// <summary>
        /// Creates a new instance of the RabbitMQTransportBindingElement Class using the default protocol.
        /// </summary>
        public RabbitMQTransportBindingElement()
        {
            this.MaxReceivedMessageSize = RabbitMQBinding.DefaultMaxMessageSize;
        }

        private RabbitMQTransportBindingElement(RabbitMQTransportBindingElement transportBindingElement)
        {
            this.HostName = transportBindingElement.HostName;
            this.Port = transportBindingElement.Port;
            this.Username = transportBindingElement.Username;
            this.Password = transportBindingElement.Password;
            this.VirtualHost = transportBindingElement.VirtualHost;
            this.MaxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the scheme used by the binding, soap.amqp
        /// </summary>
        public override string Scheme
        {
            get { return CurrentVersion.Scheme; }
        }

        /// <summary>
        /// The largest receivable encoded message
        /// </summary>
        public override long MaxReceivedMessageSize { get; set; }

        /// <summary>
        /// Specifies the hostname of the RabbitMQ Server
        /// </summary>
        [ConfigurationProperty("hostname")]
        public string HostName
        {
            get { return this._host; }
            set
            {
                this._host = value;
                this._connectionFactory = null;
            }
        }

        /// <summary>
        /// Specifies the RabbitMQ Server port
        /// </summary>
        [ConfigurationProperty("port")]
        public int Port
        {
            get { return this._port; }
            set
            {
                this._port = value;
                this._connectionFactory = null;
            }
        }

        /// <summary>
        /// The username to use when authenticating with the broker
        /// </summary>
        internal string Username
        {
            get { return this._username; }
            set
            {
                this._username = value;
                this._connectionFactory = null;
            }
        }

        /// <summary>
        /// Password to use when authenticating with the broker
        /// </summary>
        internal string Password
        {
            get { return this._password; }
            set
            {
                this._password = value;
                this._connectionFactory = null;
            }
        }

        /// <summary>
        /// Specifies the broker virtual host
        /// </summary>
        internal string VirtualHost
        {
            get { return this._vhost; }
            set
            {
                this._vhost = value;
                this._connectionFactory = null;
            }
        }

        /// <summary>
        /// Get the broker ConnectionFactory
        /// </summary>
        internal ConnectionFactory ConnectionFactory
        {
            get
            {
                if (this._connectionFactory != null)
                {
                    return this._connectionFactory;
                }

                ConnectionFactory connectionFactory = new ConnectionFactory();
                if (this.HostName != null)
                {
                    connectionFactory.HostName = this.HostName;
                }
                if (this.Port != AmqpTcpEndpoint.UseDefaultPort)
                {
                    connectionFactory.Port = this.Port;
                }
                if (this.Username != null)
                {
                    connectionFactory.UserName = this.Username;
                }
                if (this.Password != null)
                {
                    connectionFactory.Password = this.Password;
                }
                if (this.VirtualHost != null)
                {
                    connectionFactory.VirtualHost = this.VirtualHost;
                }
                this._connectionFactory = connectionFactory;

                return this._connectionFactory;
            }
        }

        #endregion

        #region Methods

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (this.HostName == null)
            {
                throw new InvalidOperationException("No broker was specified.");
            }

            return (IChannelFactory<TChannel>)(object)new RabbitMQChannelFactory(context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (this.HostName == null)
            {
                throw new InvalidOperationException("No broker was specified.");
            }

            return (IChannelListener<TChannel>)((object)new RabbitMQChannelListener<TChannel>(context));
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IOutputChannel);
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IInputChannel);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return context.GetInnerProperty<T>();
        }

        public override BindingElement Clone()
        {
            return new RabbitMQTransportBindingElement(this);
        }

        internal void EnsureConnectionAvailable()
        {
            if (this._connection == null)
            {
                this._connection = this.ConnectionFactory.CreateConnection();
            }
        }

        internal IModel Open(TimeSpan timeout)
        {
            // TODO: Honour timeout.
            try
            {
                return this.InternalOpen();
            }
            catch (AlreadyClosedException)
            {
                // fall through
            }
            catch (ChannelAllocationException)
            {
                // fall through
            }

            this._connection = null;
            return this.InternalOpen();
        }

        internal IModel InternalOpen()
        {
            this.EnsureConnectionAvailable();
            IModel model = this._connection.CreateModel();

            //TODO RabbitMQ.Cient 6.2.2 doesn't exist AutoClose property
            //this._connection.AutoClose = true;

            return model;
        }

        internal void Close(IModel model, TimeSpan timeout)
        {
            // TODO: Honour timeout.
            model?.Close(Constants.ReplySuccess, "Goodbye");
        }

        #endregion
    }
}
