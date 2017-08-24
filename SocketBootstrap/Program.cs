using System;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {            
            Console.WriteLine("Press any key to start the server!");

            Console.ReadKey();

            var bootstrap = BootstrapFactory.CreateBootstrap();

            if (!bootstrap.Initialize())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to initialize!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();

            Console.Write("Start result: ");
            switch(result)
            {
                case StartResult.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case StartResult.PartialSuccess:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case StartResult.Failed:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine("{0}!", result);
            Console.ResetColor();

            if (result == StartResult.Failed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to start!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("-----------------------------");
            Console.WriteLine("- Press key 'q' to stop it! -");
            Console.WriteLine("-----------------------------");
            Console.WriteLine();

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            Console.WriteLine();

            //Stop the appServer
            bootstrap.Stop();

            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
        }
    }
}
