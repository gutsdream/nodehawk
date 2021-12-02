namespace Application.Core.Models.Results
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