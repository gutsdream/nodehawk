namespace Application.Core.Models.Results
{
    public class QueryResult<TContent> : IQueryResult<TContent> where TContent : class
    {
        public TContent Content { get; }
        public Status State { get; }

        protected QueryResult( TContent content, Status state )
        {
            Content = content;
            State = state;
        }

        public static QueryResult<TContent> Found( TContent content )
        {
            return new QueryResult<TContent>( content, Status.Found );
        }

        public static QueryResult<TContent> NotFound( )
        {
            return new QueryResult<TContent>( null, Status.NotFound );
        }
    }
}