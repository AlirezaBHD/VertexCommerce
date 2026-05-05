namespace VertexCommerce.Shared.Contracts.Pagination;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    bool HasNextPage,
    bool HasPreviousPage,
    int TotalCount
);
