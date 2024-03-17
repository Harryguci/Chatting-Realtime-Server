using ChatingApp.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ChatingApp.Context;
using ChatingApp.Services;
using ChatingApp.Helpers;
using System.Diagnostics;

namespace ChatingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        //private readonly ILogger<WebSocketController> _logger;
        private readonly ChatingContext _context;
        public IWebsocketHandler WebsocketHandler { get; }

        public WebSocketController(IWebsocketHandler websocketHandler, ChatingContext context)
        {
            WebsocketHandler = websocketHandler;
            WebsocketHandler.DbContext = context;

            _context = context;
        }

        [HttpGet("/ws")]
        public async Task Get(string? token)
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;
            if (token == null)
            {
                context.Response.StatusCode = 400;
                return;
            }
            string? username = null;

            try
            {
                username = AuthController.GetClaim(token, "username");
                // Debug.WriteLine(claims);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (isSocketRequest)
            {
                WebSocket websocket = await context.WebSockets.AcceptWebSocketAsync();

                await WebsocketHandler.Handle(Guid.NewGuid(), username ?? "", websocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        //[NonAction]
        //private async Task Echo(WebSocket webSocket)
        //{
        //    var buffer = new byte[1024 * 4];
        //    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    _logger.Log(LogLevel.Information, "Message received from Client");


        //    while (!result.CloseStatus.HasValue)
        //    {
        //        // Send to the client
        //        // Get Raw data and deserialize to object'
        //        Message? message = null;
        //        var rawData = Encoding.UTF8.GetString(buffer);
        //        try
        //        {
        //            message = DeserializeRawData(rawData);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //        }

        //        var serverMsg = Encoding.UTF8.GetBytes($"Server: Hello. You said: {(message != null ? message.Content : rawData)}");

        //        await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length),
        //            result.MessageType, result.EndOfMessage, CancellationToken.None);

        //        _logger.Log(LogLevel.Information, "Message sent to Client");


        //        // receive the message
        //        buffer = new byte[1024 * 4];
        //        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //        _logger.Log(LogLevel.Information, "Message received from Client");

        //    }
        //    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        //    _logger.Log(LogLevel.Information, "WebSocket connection closed");
        //}
    }
}
