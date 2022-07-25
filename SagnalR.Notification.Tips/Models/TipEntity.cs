namespace SagnalR.Notification.Tips.Models
{
    public class TipEntity
    {
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime UpdateDate { get; set; }

        public int AuthorId { get; set; }
    }
}
