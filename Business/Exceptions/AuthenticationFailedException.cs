namespace Business.Exceptions
{
    // I dont know who you are exception class
    
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(string? message) : base(message)
        {
        }
    }
}
