namespace TemplateMultTenaBackend.Application.RequestFeatures
{
    public abstract class RequestParameters
    {
        private const int maxPageSize = 100;

        public int PageNumber { get; set; } = 1;
        private int _pageSize = 20;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        public string? OrderBy { get; set; }
        public string? SearchTerm { get; set; }
    }
}
