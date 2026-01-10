namespace Media.Core.Exceptions
{
    public class BadRequestException(string message) : CustomException(message)
    {
	}
}
