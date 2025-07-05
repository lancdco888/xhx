using BaseServer.Platform;
using BaseServer.Util;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using Wisher.Basic.Log;

namespace GatewayServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrap = BootstrapFactory.CreateBootstrap();

            try
            {
                if (!bootstrap.Initialize())
                {
                    Console.WriteLine("GatewayServer Failed to initialize!");
                    Console.ReadKey();
                    return;
                }

                var result = bootstrap.Start();



                Console.WriteLine("GatewayServer Start result: {0}!", result);

                if (result == StartResult.Failed)
                {
                    Console.WriteLine("GatewayServer Failed to start!");
                    Console.ReadKey();
                    return;
                }


                PlatformGlobalData.Instance.Init();

                Console.WriteLine("Press key 'q' to stop it!");

                while (Console.ReadKey().KeyChar != 'q')
                {
                    Console.WriteLine();
                    continue;
                }
            }
            catch (Exception ex)
            {
                MyLog.Error(typeof(Program), ex.ToString());
            }

            Console.WriteLine();

            //Stop the appServer
            bootstrap.Stop();

            Console.WriteLine("The GatewayServer was stopped!");
        }
    }
}
