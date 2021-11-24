namespace Application.QueryHandling
{
    public interface IQueryResult<TContent>
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