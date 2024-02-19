using Microsoft.AspNetCore.SignalR;

namespace Comigle.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            Console.WriteLine(message);
            await Clients.Others.SendAsync("Receive", message);
        }
    }
}
