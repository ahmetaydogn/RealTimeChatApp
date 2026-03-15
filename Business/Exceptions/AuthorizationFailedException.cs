namespace Business.Exceptions
{
    // I know who you are but you're not allowed to do this / You can not do this.

    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(string? message) : base(message)
        {
        }
    }
}
