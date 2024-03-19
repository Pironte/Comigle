using Microsoft.AspNetCore.SignalR;

namespace Comigle.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, bool>? _users { get; set; } // true para ocupado, false para disponível

        private object _lockUsers = new object();

        public async Task Send(string connectionId, string remoteConnectionId, string message)
        {
            if (string.IsNullOrEmpty(remoteConnectionId))
                return;

            var client = Clients.Client(remoteConnectionId);
            await client.SendAsync("Receive", connectionId, message);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            RemoveUser(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public string? TryFindAvailableUser(string connectionId)
        {
            lock (_lockUsers)
            {
                if (_users == null) _users = new Dictionary<string, bool>();

                var availableUser = _users.FirstOrDefault(u => !u.Value && u.Key != connectionId).Key;
                return availableUser;
            }
        }

        public void AddUser(string connectionId)
        {
            lock (_lockUsers)
            {
                if (_users == null)
                    _users = new Dictionary<string, bool>();

                if (!_users.ContainsKey(connectionId))
                {
                    _users.Add(connectionId, false);
                }
                else
                {
                    // Atualiza o estado do usuário caso ele já exista
                    _users[connectionId] = false;
                }
            }
        }

        public void MarkUserAsBusy(string connectionId)
        {
            lock (_lockUsers)
            {
                if (_users == null) _users = new Dictionary<string, bool>();

                if (_users.ContainsKey(connectionId))
                {
                    _users[connectionId] = true;
                }
            }
        }

        // Método para marcar usuário como disponível
        public void MarkUserAsAvailable(string connectionId)
        {
            lock (_lockUsers)
            {
                if (_users == null) _users = new Dictionary<string, bool>();

                if (_users.ContainsKey(connectionId))
                {
                    _users[connectionId] = false;
                }
            }
        }

        public void RemoveUser(string connectionId)
        {
            lock (_lockUsers)
            {
                if (_users == null) _users = new Dictionary<string, bool>();

                _users.Remove(connectionId);
            }
        }
    }
}
