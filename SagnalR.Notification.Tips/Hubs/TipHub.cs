using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using SagnalR.Notification.Tips.Helpers;
using SagnalR.Notification.Tips.Models;
using System.Diagnostics;

namespace SagnalR.Notification.Tips.Hubs;

[HubName("TipHub")]
public class TipHub : Hub
{
    private static Dictionary<int, string> deviceConnections;
    private static Dictionary<string, int> connectionDevices;

    public TipHub()
    {
        deviceConnections = deviceConnections ?? new Dictionary<int, string>();
        connectionDevices = connectionDevices ?? new Dictionary<string, int>();
    }

    public override Task OnConnectedAsync()
    {
        Debug.WriteLine("Signal server connected");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        int? deviceId = connectionDevices.ContainsKey(Context.ConnectionId)
            ? connectionDevices[Context.ConnectionId]
            : null;

        if (deviceId.HasValue)
        {
            deviceConnections.Remove(deviceId.Value);
            connectionDevices.Remove(Context.ConnectionId);
        }

        Debug.WriteLine($"SirnalR server disconnected Device: {deviceId}");
        await base.OnDisconnectedAsync(exception);
    }

    [Microsoft.AspNetCore.SignalR.HubMethodName("Init")]
    public Task Init(DeviceInfo info)
    {
        deviceConnections.AddOrUpdate(info.Id, Context.ConnectionId);
        connectionDevices.AddOrUpdate(Context.ConnectionId, info.Id);
        return Task.CompletedTask;
    }

    [Microsoft.AspNetCore.SignalR.HubMethodName("NewMessageToAll")]
    public async Task NewMessageToAll(MessageItem item)
    {
        await Clients.All.SendAsync("NewTip", item.Message);
    }

    [Microsoft.AspNetCore.SignalR.HubMethodName("UpdateMessageToAll")]
    public async Task UpdateMessageToAll(MessageItem item)
    {
        await Clients.All.SendAsync("UpdateTip", item.Message);
    }

    [Microsoft.AspNetCore.SignalR.HubMethodName("DeleteMessageToAll")]
    public async Task DeleteMessageToAll(int item)
    {
        await Clients.All.SendAsync("NewTip", item);
    }

    [Microsoft.AspNetCore.SignalR.HubMethodName("SendMessageToDevice")]
    public async Task SendMessageToDevice(MessageItem item)
    {
        Debug.WriteLine($"SirnalR server send message from {item.SourceId} to {item.TargetId}");
        if (deviceConnections.ContainsKey(item.TargetId))
            await Clients.Client(deviceConnections[item.TargetId]).SendAsync("NewMessage", item.Message);
    }
}