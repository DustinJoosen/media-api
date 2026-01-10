namespace Media.Core.Exceptions
{
    public abstract class CustomException(string message) : Exception(message)
    {
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
