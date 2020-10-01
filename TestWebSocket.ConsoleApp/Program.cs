using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebSocket.ConsoleApp
{
    class Program
    {
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static byte[] _bufferSize = new byte[1024 * 4];

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Connect To server ? (y/n)");
            var res = Console.ReadKey();
            if (res.Key != ConsoleKey.Y)
                return;

            ClientWebSocket clientWebSocket = new ClientWebSocket();
            await clientWebSocket.ConnectAsync(new Uri("ws://localhost:60616/ws"), _cts.Token);

            // Listening new messages
            _ = Task.Factory.StartNew(
                async () =>
                {
                    var rcvBuffer = new ArraySegment<byte>(_bufferSize);
                    while (true)
                    {
                        WebSocketReceiveResult rcvResult = await clientWebSocket.ReceiveAsync(rcvBuffer, _cts.Token);
                        byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                        string rcvMsg = Encoding.UTF8.GetString(msgBytes);
                        Console.WriteLine("\nMessage received: {0}", rcvMsg);
                    }
                }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            Console.Write("\nType in console to send messages :");

            while (true)
            {
                var msg = Console.ReadLine();
                if (msg == "Bye")
                {
                    _cts.Cancel();
                    return;
                }
                
                byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
                await clientWebSocket.SendAsync(new ArraySegment<byte>(msgBytes), WebSocketMessageType.Text, true, _cts.Token);
            }
        }
    }
}
