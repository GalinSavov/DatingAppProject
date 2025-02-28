namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = [];
    public Task<bool> UserConnected(string username, string connectionID)
    {
        var isOnline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionID);
            }
            else
            {
                OnlineUsers.Add(username, new List<string> { connectionID });
                isOnline = true;
            }
        }
        return Task.FromResult(isOnline);
    }
    public Task<bool> UserDisconnected(string username, string connectionID)
    {
        var isOffline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Remove(connectionID);
            }
            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true; 
            }
        }
        return Task.FromResult(isOffline);
    }
    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers = [];
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(user => user.Key).Select(user => user.Key).ToArray();
        }
        return Task.FromResult(onlineUsers);
    }
    public static Task<List<string>> GetConnectionsForUser(string username)
    {
        List<string> connectionIDsForUser = [];
        if (OnlineUsers.ContainsKey(username))
        {
            lock (OnlineUsers)
            {
                foreach (var connectionID in OnlineUsers[username])
                {
                    connectionIDsForUser.Add(connectionID);
                }
            }
        }
        return Task.FromResult(connectionIDsForUser);
    }
}