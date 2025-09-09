namespace TemplateMultTenaBackend.Domain.Exceptions.Unprocessable
{
    public class PasswordUnprocessableException : UnprocessableException
    {
        public PasswordUnprocessableException() : base("A senha deve conter ao menos 10 caracteres, tendo ao menos 1 letra maíuscula, 1 letra minúscula, 1 número e 1 símbolo.")
        {
        }
    }
}
