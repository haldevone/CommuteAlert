namespace CommuteAlert.Web.Models;

public class Alert
{
    public int Id { get; set; }
    public int WatchId { get; set; }
    public AlertType Type { get; set; }
    public string Key { get; set; } = ""; // spec nyckel som identifierar händelsen
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime SentAt { get; set; }
}
