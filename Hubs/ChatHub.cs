using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Comigle.Hubs
{
    public class ChatHub : Hub
    {
        public static ConcurrentDictionary<string, string> peersDic;

        public static ConcurrentDictionary<string, string> peersDicConnect;

        public async Task Send(string connectionId, string message)
        {
            Console.WriteLine(message);
            await Clients.Others.SendAsync("Receive", connectionId, message);
        }

        public async Task SendToPeer(string connectionId, string message)
        {
            Console.Write(message);
            var client = Clients.Client(connectionId);
            await client.SendAsync("Receive", connectionId, message);
        }

        public void SavePeersDictionary(string connectionId, string state)
        {
            if (peersDic == null)
                peersDic = new ConcurrentDictionary<string, string>();

            if (string.IsNullOrEmpty(state))
                state = "new";

            peersDic[connectionId] = state;
        }

        public void SavePeersConnect(string localConnectionId, string remoteConnectionId)
        {
            if (peersDicConnect == null)
                peersDicConnect = new ConcurrentDictionary<string, string>();

            peersDicConnect[localConnectionId] = remoteConnectionId;
        }

        public void DeletePeersConnect(string connectionId, string state)
        {
            var remoteConnectionid = peersDicConnect[connectionId];
            peersDicConnect.TryRemove(connectionId, out var peers);
            peersDicConnect.TryRemove(remoteConnectionid, out peers);
        }

        public string GetPeersConnect(string connectionId)
        {
            if (peersDicConnect == null)
                peersDicConnect = new ConcurrentDictionary<string, string>();

            if (peersDicConnect.ContainsKey(connectionId))
                return peersDicConnect[connectionId];

            return string.Empty;
        }

        public void DeletePeersDictionary(string connectionId, string state)
        {
            peersDic.TryRemove(connectionId, out var peers);
        }

        public string GetSateOfPeer(string connectionId)
        {
            return peersDic[connectionId];
        }
    }
}
