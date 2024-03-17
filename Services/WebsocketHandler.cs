using ChatingApp.Context;
using ChatingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace ChatingApp.Services
{
    public class WebsocketHandler : IWebsocketHandler
    {
        public List<SocketConnection> websocketConnections = new List<SocketConnection>();
        public ChatingContext DbContext { get; set; } = null!;
        public WebsocketHandler()
        {
            SetupCleanUpTask();
        }

        public async Task Handle(Guid id, string username, WebSocket webSocket)
        {
            lock (websocketConnections)
            {
                websocketConnections.Add(new SocketConnection
                {
                    Id = id,
                    Username = username,
                    WebSocket = webSocket
                });
            }

            await Boardcast(JsonConvert.SerializeObject(new
            {
                type = "notification",
                content = $"User {id} has joined the chat"
            }));

            while (webSocket.State == WebSocketState.Open)
            {
                var messageStr = await ReceiveMessage(id, webSocket);
                if (messageStr != null)
                {
                    var message = DeserializeRawData(messageStr);

                    if (message != null && message.RoomId != null)
                    {
                        await SendToRoom(JsonConvert.SerializeObject(
                            new
                            {
                                id = message.Id,
                                username = message.Username,
                                content = message.Content,
                                roomId = message.RoomId,
                                filenmae = message.Filename,
                                createAt = message.CreateAt,
                                deleteAt = message.DeleteAt,
                                type = "message"
                            }), message.RoomId);
                    }
                    else if (message != null)
                        await Boardcast(JsonConvert.SerializeObject(
                            new
                            {
                                id = message.Id,
                                username = message.Username,
                                content = message.Content,
                                roomId = message.RoomId,
                                filenmae = message.Filename,
                                createAt = message.CreateAt,
                                deleteAt = message.DeleteAt,
                                type = "message"
                            }));
                }
            }
        }

        private async Task<string?> ReceiveMessage(Guid id, WebSocket webSocket)
        {
            var arraySegment = new ArraySegment<byte>(new byte[4096]);
            var receivedMessage = await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
            if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
                if (!string.IsNullOrWhiteSpace(message))
                    return message;
            }
            return null;
        }

        private async Task Boardcast(string message)
        {
            IEnumerable<SocketConnection> toSentTo;

            lock (websocketConnections)
            {
                toSentTo = websocketConnections.ToList();
            }

            var tasks = toSentTo.Select(async websocketConnection =>
            {
                // Send to all users that are connected
                if (websocketConnection.WebSocket.State == WebSocketState.Open)
                {
                    var bytes = Encoding.Default.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes);
                    await websocketConnection.WebSocket.SendAsync(arraySegment, WebSocketMessageType.Text,
                        true, CancellationToken.None);
                }
            });
            await Task.WhenAll(tasks);
        }

        private async Task SendToUser(string message, string username)
        {
            IEnumerable<SocketConnection> toSentTo;

            lock (websocketConnections)
            {
                toSentTo = websocketConnections.ToList();
            }

            var tasks = toSentTo.Select(async websocketConnection =>
            {
                // Send to all users that are connected
                if (websocketConnection.WebSocket.State == WebSocketState.Open
                && websocketConnection.Username == username)
                {
                    var bytes = Encoding.Default.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes);
                    await websocketConnection.WebSocket.SendAsync(arraySegment,
                        WebSocketMessageType.Text,
                        true, CancellationToken.None);
                }
            });
            await Task.WhenAll(tasks);
        }

        private async Task SendToRoom(string message, string roomId)
        {
            if (!DbContext.Rooms.Any(room => room.Id == roomId))
            {
                return;
            }

            IEnumerable<SocketConnection> toSentTo;

            var roomAccounts = await DbContext.RoomAccounts
                .Where(p => p.RoomId == roomId).ToListAsync();

            lock (websocketConnections)
            {
                List<SocketConnection> list = new List<SocketConnection>();

                using (var socket = websocketConnections.GetEnumerator())
                {
                    while (socket.MoveNext())
                    {
                        var currentItem = socket.Current;
                        // Your code logic here
                        if (roomAccounts.Any(p => p.Username == currentItem.Username))
                        {
                            list.Add(currentItem);
                        }
                    }
                }

                toSentTo = list;
            }

            var tasks = toSentTo.Select(async websocketConnection =>
            {
                // Send to all users that are connected
                if (websocketConnection.WebSocket.State == WebSocketState.Open)
                {
                    var bytes = Encoding.Default.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes);
                    await websocketConnection.WebSocket.SendAsync(arraySegment,
                        WebSocketMessageType.Text,
                        true, CancellationToken.None);
                }
            });
            await Task.WhenAll(tasks);
        }

        private void SetupCleanUpTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    IEnumerable<SocketConnection> openSockets;
                    IEnumerable<SocketConnection> closedSockets;

                    lock (websocketConnections)
                    {
                        openSockets = websocketConnections.Where(x => x.WebSocket.State == WebSocketState.Open || x.WebSocket.State == WebSocketState.Connecting);
                        closedSockets = websocketConnections.Where(x => x.WebSocket.State != WebSocketState.Open && x.WebSocket.State != WebSocketState.Connecting);

                        websocketConnections = openSockets.ToList();
                    }

                    foreach (var closedWebsocketConnection in closedSockets)
                    {
                        await Boardcast($"User with id <b>{closedWebsocketConnection.Id}</b> has left the chat");
                    }

                    await Task.Delay(5000);
                }

            });
        }

        [NonAction]
        public static Message? DeserializeRawData(string jsonString)
        {
            // Parse the escaped JSON string
            JToken token = JToken.Parse(jsonString);

            // Convert to a JObject (if needed)
            JObject json = JObject.Parse(token.ToString());

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string content = json["content"].ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (content.ToLower().Contains("deep learning"))
            {
                content = content.Replace("deep learning", @"<b>Deep Learning</b>");
            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return new Message()
            {
                Id = json["id"].ToString(),
                Username = json["username"].ToString(),
                RoomId = json["roomId"].ToString(),
                Content = content,
                CreateAt = DateTime.Now
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }

    public class SocketConnection
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public WebSocket WebSocket { get; set; } = null!;
    }
}
