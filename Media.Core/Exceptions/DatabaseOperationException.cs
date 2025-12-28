namespace Media.Core.Exceptions
{
    public class DatabaseOperationException : CustomException
    {
        public DatabaseOperationException(string message) : base(message)
        {
            
        }
    }
}
