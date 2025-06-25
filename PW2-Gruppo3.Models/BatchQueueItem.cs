namespace PW2_Gruppo3.Models;

public class BatchQueueItem
{
    public Guid Id { get; set; }
    public Guid BatchUuid { get; set; }
    public int Position { get; set; }
    
    // TODO: potremmo inserire un attributo che dice se è iniziato o meno e un attributo che dice se è completato o meno?
    public DateTime CreatedAt { get; set; }

}