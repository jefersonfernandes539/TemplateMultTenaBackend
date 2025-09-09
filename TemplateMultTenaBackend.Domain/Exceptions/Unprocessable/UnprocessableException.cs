namespace TemplateMultTenaBackend.Domain.Exceptions.Unprocessable
{
    public abstract class UnprocessableException : Exception
    {
        protected UnprocessableException(string message) : base(message)
        {
        }
    }
}
