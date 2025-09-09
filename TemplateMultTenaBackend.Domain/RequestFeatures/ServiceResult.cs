namespace TemplateMultTenaBackend.Domain.RequestFeatures
{
    public class ServiceResult<TResult>
    {
        private ServiceResult(bool isSuccess, TResult? data, int statusCode, IEnumerable<string>? errorMessages)
        {
            IsSuccess = isSuccess;
            Data = data;
            StatusCode = statusCode;
            ErrorMessages = errorMessages;
        }

        public bool IsSuccess { get; private set; }
        public TResult? Data { get; private set; }
        public int StatusCode { get; private set; }
        public IEnumerable<string>? ErrorMessages { get; private set; }

        public static ServiceResult<TResult> Failure(IEnumerable<string>? errorMessages = null, int statusCode = 500)
            => new(false, default, statusCode, errorMessages);

        public static ServiceResult<TResult> Success(TResult? data = default, int statusCode = 200)
           => new(true, data, statusCode, []);
    }
}
