using System;
using SocketBootstrap.Models;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace SocketBootstrap
{
    class Program
    {
        static void Main(string[] args)
        {            
            Console.WriteLine("Press any key to start the server!");

            Console.ReadKey();
            Console.WriteLine();

            var bootstrap = BootstrapFactory.CreateBootstrap();

            if (!bootstrap.Initialize())
            {
                Console.WriteLine("Failed to initialize!");
                Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();

            Console.WriteLine("Start result: {0}!", result);

            if (result == StartResult.Failed)
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            foreach(var s in bootstrap.AppServers)
            {
                if (s is TelnetServer)
                {
                    TelnetServer x = s as TelnetServer;
                    x.NewSessionConnected += new SessionHandler<TelnetSession>(appServer_NewSessionConnected);
                    x.SessionClosed += new SessionHandler<TelnetSession, CloseReason>(appServer_SessionClosed);
                }
            }
            
            Console.WriteLine("Press key 'q' to stop it!");

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

        static void appServer_SessionClosed(TelnetSession session, CloseReason reason)
        {
            Console.WriteLine("{0}: Session {1} is closed.", session.AppServer.Name, session.SessionID);
        }

        static void appServer_NewSessionConnected(TelnetSession session)
        {
            Console.WriteLine("{0}: Session {1} is opened.", session.AppServer.Name, session.SessionID);
        }
    }
}
