namespace Application.CommandHandling.Nodes.Interfaces
{
    public interface IMutateNode
    {
        string Title { get; set; }
        string ExternalId { get; set; }

        string Host { get; set; }
        string Username { get; set; }
        string Key { get; set; }
    }
}