using System;
using System.Threading;
using Utility;
using WebSocketSharp.Server;
using WebSocketSharp;

namespace Server
{
    public class Server
    {
        #region Property
        public WebSocketServer SocketServer { get; set; }
        #endregion

        #region Memberfunction
        public Server(int port)
        {
            SocketServer = new WebSocketServer(port);
            //SocketServer.Log.Level = LogLevel.Trace;
            SocketServer.Log.Level = LogLevel.Info;

            SocketServer.AddWebSocketService<Connect>("/Connect");

            SocketServer.Start();

            
            while (true)
            {
                Console.WriteLine("0.GetClientList\r\n1.Get Current Status\r\n997.Exit\r\n998.Restrat all client\r\n999.Close all client");
                var command = Console.ReadLine();
                switch (command)
                {
                    case "0":
                        getClientList();
                        break;
                    case "1":
                        getDeviceStatus();
                        break;
                    case "997":
                        SocketServer.Stop();
                        break;
                    case "998":
                        restartDevice();
                        break;
                    case "999":
                        closeAll();
                        break;
                }
            }
        }

        private void restartDevice()
        {
            if (SocketServer != null) 
            {
                SocketServer.WebSocketServices["/Connect"].Sessions.Broadcast("RESTART");

            }
            else
            {
                Console.WriteLine("SocketServer is null");
            }
        }

        private void closeAll()
        {
            if (SocketServer != null)
                SocketServer.WebSocketServices["/Connect"].Sessions.Broadcast("CLOSE");
            else
            {
                Console.WriteLine("SocketServer is null");
            }
        }

        private void getDeviceStatus()
        {
            Console.WriteLine($"Current CPU {Hardward.CPU}");
            Console.WriteLine($"Current RAM {Hardward.RAM}");
            foreach (var item in Hardward.DISK)
            {
                Console.WriteLine($"{item.DiskName} : {item.Capacity}");
            }
        }
        public void getClientList()
        {

            if (SocketServer != null) 
            {
                SocketServer.WebSocketServices["/Connect"].Sessions.Broadcast("STATUS");
            }
            else
            {
                Console.WriteLine("SocketServer is null");
            }
        }
        #endregion
    }
}
