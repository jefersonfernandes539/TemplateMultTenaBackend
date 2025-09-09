namespace TemplateMultTenaBackend.Domain.Exceptions.Unprocessable
{
    public sealed class UserUpdateUnprocessableException : UnprocessableException
    {
        public UserUpdateUnprocessableException() : base("Ocorreu alguma falha ao atualizar as informações do usuário")
        {
        }

        public UserUpdateUnprocessableException(string message) : base(message)
        {
        }
    }
}
