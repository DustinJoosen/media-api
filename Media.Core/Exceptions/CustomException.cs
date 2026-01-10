namespace Media.Core.Exceptions
{
    public abstract class CustomException : Exception
    {
        protected CustomException(string message) : base(message)
        {
            
        }

        /// <summary>
        /// Allow custom exceptions to set some extra headers. These headers automatically
        /// get added to any response the ExceptionMiddleware writes.
        /// </summary>
        public virtual Dictionary<string, string> GetHeaders()
        {
            return [];
        }
    }
}
