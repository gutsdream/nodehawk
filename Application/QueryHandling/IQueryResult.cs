namespace Application.QueryHandling
{
    public interface IQueryResult<out TContent>
    {
        public TContent Content { get; }
        public Status State { get; }
    }

    public enum Status
    {
        Found,
        NotFound
    }
}