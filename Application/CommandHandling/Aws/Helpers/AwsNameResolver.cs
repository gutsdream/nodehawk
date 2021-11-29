using Domain.Entities;

namespace Application.CommandHandling.Aws.Helpers
{
    public static class AwsNameResolver
    {
        public static string BucketNameForNode( Node node )
        {
            return $"otnode{node.Id}";
        }
    }
}