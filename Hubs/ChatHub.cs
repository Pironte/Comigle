using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Comigle.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string connectionId, string message)
        {
            Console.WriteLine(message);
            await Clients.Others.SendAsync("Receive", connectionId, message);
        }
    }
}
