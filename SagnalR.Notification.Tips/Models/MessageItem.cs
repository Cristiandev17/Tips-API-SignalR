namespace SagnalR.Notification.Tips.Models;
public class MessageItem
{
    public TipEntity Message { get; set; }

    public int SourceId { get; set; }

    public int TargetId { get; set; }
}
