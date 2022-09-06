using Newtonsoft.Json;
using Notifications.Wpf;
using System;
using System.Configuration;
using System.Threading;
using Utility;
using WebSocketSharp;

namespace Harmos
{
    public class Client
    {
        #region Property
        public string ClientID { get; set; }
        public NotificationManager NotificationManager { get; set; }
        #endregion

        #region Memberfunction
        public Client()
        {
            try
            {
                var server = ConfigurationSettings.AppSettings["Server"];
                NotificationManager = new NotificationManager();
                ClientID = Guid.NewGuid().ToString();
                var ws = new WebSocket($"ws://{server}:5566/Connect");

                SetupPing(ws);
                ws.OnMessage += Ws_OnMessage;
                ws.OnOpen += Ws_OnOpen;
                ws.Connect();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化發生例外{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("ReceiveMessage");
            var ws = (sender as WebSocket);
            if (ws is null)
            {
                return;
            }

            string data = e.Data;
            Notify($"收到伺服器指令 {data}", NotificationType.Information);
            switch (data)
            {
                case "STATUS":
                    DeviceStatus status = new DeviceStatus()
                    {
                        CPU = Hardward.CPU,
                        RAM = Hardward.RAM,
                        DISK = Hardward.DISK
                    };
                    string output = JsonConvert.SerializeObject(status);

                    ws.Send(output);
                    Notify($"回傳伺服器設備目前狀態{output}", NotificationType.Success);
                    break;
                case "CASE":
                    Notify($"收到伺服器指令 {data}", NotificationType.Information);
                    break;
                case "CLOSE":
                    ws.Close();
                    for(int i = 10; i > 0; i--) 
                    {
                        Notify($"{i}秒後關閉設備Client端連線", NotificationType.Error);
                        Thread.Sleep(1000);
                    }
                    Environment.Exit(0);
                    break;
                case "RESTART":
                    System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName);

                    for (int i = 5; i > 0; i--)
                    {
                        Notify($"{i}秒後重新啟動設備Client端連線", NotificationType.Error);
                        Thread.Sleep(1000);
                    }
                    Environment.Exit(0);
                    break;
            }
        }

        void SetupPing(WebSocket ws)
        {
            var interval = 5000;
            Timer timer = null;

            timer = new Timer(state =>
            {
                if (ws.IsAlive)
                {
                    timer.Change(interval, Timeout.Infinite);
                }
                else
                {
                    ws.Close();
                    Notify($"與伺服器連線中斷", NotificationType.Error);
                    for (int i = 5; i > 0; i--)
                    {
                        Notify($"{i}秒後關閉設備Client端連線", NotificationType.Error);
                        Thread.Sleep(1000);
                    }
                    Environment.Exit(0);
                }

            },
            null, 5000, Timeout.Infinite);
        }

        private void Notify(string message, NotificationType type)
        {
            Thread th = new Thread(() => NotifyTask(message, type));
            th.ApartmentState = ApartmentState.STA;
            th.Start();
        }

        [STAThread]
        private void NotifyTask(string message, NotificationType type)
        {
            var notificationManager = new NotificationManager();
            notificationManager.Show(
                new NotificationContent
                {
                    Title = "通知",
                    Message = message,
                    Type = type,
                },
                "",
                TimeSpan.FromSeconds(5)
            );
        }
        #endregion
    }
}
