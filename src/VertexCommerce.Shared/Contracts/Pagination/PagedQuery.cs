namespace VertexCommerce.Shared.Contracts.Pagination;

public abstract record PagedQuery
{
    private const int MaxPageSize = 100;

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get;
        init => field = value > MaxPageSize ? MaxPageSize : value;
    } = 20;

    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;
}
