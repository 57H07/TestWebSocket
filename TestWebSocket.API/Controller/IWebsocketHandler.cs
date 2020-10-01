using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace TestWebSocket.API.Controller
{
    public interface IWebsocketHandler
    {
        Task Handle(Guid id, WebSocket webSocket);
    }
}