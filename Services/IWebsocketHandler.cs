using ChatingApp.Context;
using System.Net.WebSockets;

namespace ChatingApp.Services
{
    public interface IWebsocketHandler
    {
        public ChatingContext DbContext { get; set; }
        Task Handle(Guid id, string username, WebSocket websocket);
    }
}
