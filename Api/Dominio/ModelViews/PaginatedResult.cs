namespace MinimalApi.Dominio.ModelViews;

public class PaginatedResult<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public PaginationMetadata Metadata { get; set; } = new();
}

public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public int FirstItemOnPage => (CurrentPage - 1) * PageSize + 1;
    public int LastItemOnPage => Math.Min(CurrentPage * PageSize, TotalCount);
}
