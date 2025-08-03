using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestWebSocket.Tests
{
    public class WebSocketTests
    {
        [Fact]
        public async Task MessageFromOneClientIsReceivedByAnother()
        {
            await using var factory = new WebApplicationFactory<TestWebSocket.API.Startup>();
            var client1 = factory.Server.CreateWebSocketClient();
            var client2 = factory.Server.CreateWebSocketClient();

            using var socket1 = await client1.ConnectAsync(new Uri("/ws", UriKind.Relative), CancellationToken.None);
            using var socket2 = await client2.ConnectAsync(new Uri("/ws", UriKind.Relative), CancellationToken.None);

            var message = "hello";
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket1.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new byte[1024];
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var result = await socket2.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            var received = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Assert.Equal(message, received);
        }
    }
}
