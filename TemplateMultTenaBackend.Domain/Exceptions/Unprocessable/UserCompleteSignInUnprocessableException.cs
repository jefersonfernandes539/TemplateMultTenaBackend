namespace TemplateMultTenaBackend.Domain.Exceptions.Unprocessable
{
    public sealed class UserCompleteSignInUnprocessableException : UnprocessableException
    {
        public UserCompleteSignInUnprocessableException() : base("Usuário já completou o cadastro")
        {
        }

        public UserCompleteSignInUnprocessableException(string message) : base(message)
        {
        }
    }
}
