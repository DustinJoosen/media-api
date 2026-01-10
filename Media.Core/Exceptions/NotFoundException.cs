namespace Media.Core.Exceptions
{
    public class NotFoundException(string message) : CustomException(message)
    {
	}
}
