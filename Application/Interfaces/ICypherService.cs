namespace Application.Interfaces
{
    public interface ICypherService
    {
        string Encrypt( string value );
        string Decrypt( string value );
    }
}