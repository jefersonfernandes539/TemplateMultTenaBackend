namespace TemplateMultTenaBackend.Domain.RequestFeatures
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public MetaData MetaData { get; set; }

        public PagedResponse(IEnumerable<T> items, MetaData metaData)
        {
            Items = items;
            MetaData = metaData;
        }
    }
}
