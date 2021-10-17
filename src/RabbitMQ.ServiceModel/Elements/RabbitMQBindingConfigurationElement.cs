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
using System.Configuration;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// Represents the configuration for a RabbitMQBinding.
    /// </summary>
    /// <remarks>
    /// This configuration element should be imported into the client
    /// and server configuration files to provide declarative configuration
    /// of a AMQP bound service.
    /// </remarks>
    public sealed class RabbitMQBindingConfigurationElement : StandardBindingElement
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the RabbitMQBindingConfigurationElement Class.
        /// </summary>
        public RabbitMQBindingConfigurationElement()
            : this(null)
        {

        }

        /// <summary>
        /// Creates a new instance of the RabbitMQBindingConfigurationElement
        /// Class initialized with values from the specified configuration.
        /// </summary>
        /// <param name="configurationName"></param>
        public RabbitMQBindingConfigurationElement(string configurationName)
            : base(configurationName)
        {

        }

        #endregion

        #region Methods

        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            RabbitMQBinding rabbind = binding as RabbitMQBinding;
            if (rabbind != null)
            {
                this.HostName = rabbind.HostName;
                this.Port = rabbind.Port;
                this.MaxMessageSize = rabbind.MaxMessageSize;
                this.OneWayOnly = rabbind.OneWayOnly;
                this.TransactionFlowEnabled = rabbind.TransactionFlow;
                this.VirtualHost = rabbind.Transport.ConnectionFactory.VirtualHost;
                this.Username = rabbind.Transport.ConnectionFactory.UserName;
                this.Password = rabbind.Transport.ConnectionFactory.Password;
            }
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }
            if (!(binding is RabbitMQBinding rabbitMqBinding))
            {
                throw new ArgumentException($"Invalid type for binding. Expected {typeof(RabbitMQBinding).AssemblyQualifiedName}, Passed: {binding.GetType().AssemblyQualifiedName}");
            }

            rabbitMqBinding.HostName = this.HostName;
            rabbitMqBinding.Port = this.Port;
            rabbitMqBinding.OneWayOnly = this.OneWayOnly;
            rabbitMqBinding.TransactionFlow = this.TransactionFlowEnabled;
            rabbitMqBinding.Transport.Password = this.Password;
            rabbitMqBinding.Transport.Username = this.Username;
            rabbitMqBinding.Transport.VirtualHost = this.VirtualHost;
            rabbitMqBinding.Transport.MaxReceivedMessageSize = this.MaxMessageSize;
        }

        #endregion

        #region Properties

        protected override Type BindingElementType
        {
            get { return typeof(RabbitMQBinding); }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection configurationProperties = base.Properties;
                Type currentType = this.GetType();
                PropertyInfo[] publicProperties = currentType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo propertyInfo in publicProperties)
                {
                    object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ConfigurationPropertyAttribute), false);
                    foreach (ConfigurationPropertyAttribute attribute in customAttributes)
                    {
                        configurationProperties.Add(new ConfigurationProperty(attribute.Name, propertyInfo.PropertyType, attribute.DefaultValue));
                    }
                }

                return configurationProperties;
            }
        }

        /// <summary>
        /// Specifies the hostname of the broker that the binding should connect to.
        /// </summary>
        [ConfigurationProperty("hostname", IsRequired = true)]
        public string HostName
        {
            get { return ((string)base["hostname"]); }
            set { base["hostname"] = value; }
        }

        /// <summary>
        /// Specifies the port of the broker that the binding should connect to.
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = AmqpTcpEndpoint.UseDefaultPort)]
        public int Port
        {
            get { return ((int)base["port"]); }
            set { base["port"] = value; }
        }

        /// <summary>
        /// Specifies whether or not the CompositeDuplex and ReliableSession
        /// binding elements are added to the channel stack.
        /// </summary>
        [ConfigurationProperty("oneWay", DefaultValue = false)]
        public bool OneWayOnly
        {
            get { return ((bool)base["oneWay"]); }
            set { base["oneWay"] = value; }
        }

        /// <summary>
        /// Password to use when authenticating with the broker
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = ConnectionFactory.DefaultPass)]
        public string Password
        {
            get { return ((string)base["password"]); }
            set { base["password"] = value; }
        }

        /// <summary>
        /// Specifies whether or not WS-AtomicTransactions are supported by the binding
        /// </summary>
        [ConfigurationProperty("transactionFlow", DefaultValue = false)]
        public bool TransactionFlowEnabled
        {
            get { return ((bool)base["transactionFlow"]); }
            set { base["transactionFlow"] = value; }
        }

        /// <summary>
        /// The username  to use when authenticating with the broker
        /// </summary>
        [ConfigurationProperty("username", DefaultValue = ConnectionFactory.DefaultUser)]
        public string Username
        {
            get { return ((string)base["username"]); }
            set { base["username"] = value; }
        }

        /// <summary>
        /// Specifies the maximum encoded message size
        /// </summary>
        [ConfigurationProperty("maxmessagesize", DefaultValue = 8192L)]
        public long MaxMessageSize
        {
            get { return (long)base["maxmessagesize"]; }
            set { base["maxmessagesize"] = value; }
        }

        /// <summary>
        /// The virtual host to access.
        /// </summary>
        [ConfigurationProperty("virtualHost", DefaultValue = ConnectionFactory.DefaultVHost)]
        public string VirtualHost
        {
            get { return ((string)base["virtualHost"]); }
            set { base["virtualHost"] = value; }
        }

        #endregion
    }
}
