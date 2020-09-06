using Topshelf;
using WCF.WindowsHost;

namespace RabbitMQ.WCF.Host
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(config =>
            {
                config.Service<ServiceLauncher>(host =>
                {
                    host.ConstructUsing(name => new ServiceLauncher());
                    host.WhenStarted(launcher => launcher.Start());
                    host.WhenStopped(launcher => launcher.Stop());
                });
                config.RunAsLocalSystem();

                config.SetServiceName("RabbitMQ WCF Service");
                config.SetDisplayName("RabbitMQ WCF Service");
                config.SetDescription("RabbitMQ WCF Service");
            });
        }
    }
}
