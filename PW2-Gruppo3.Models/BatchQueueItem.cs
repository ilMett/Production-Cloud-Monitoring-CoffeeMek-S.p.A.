namespace PW2_Gruppo3.Models;

public class BatchQueueItem
{
    public Guid Id { get; set; }
    public Guid BatchUuid { get; set; }
    public int Position { get; set; }
    
    public DateTime CreatedAt { get; set; }

}