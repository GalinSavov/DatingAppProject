namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = [];
    public Task UserConnected(string username, string connectionID)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionID);
            }
            else
            {
                OnlineUsers.Add(username, new List<string> { connectionID });
            }
        }
        return Task.CompletedTask;
    }
    public Task UserDisconnected(string username, string connectionID)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Remove(connectionID);
            }
            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
            }
        }
        return Task.CompletedTask;
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
}