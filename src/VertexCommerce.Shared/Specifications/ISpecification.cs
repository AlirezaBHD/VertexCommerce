using System.Linq.Expressions;

namespace VertexCommerce.Shared.Specifications;

public interface ISpecification<T> where T : class
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int Skip { get; }
    int Take { get; }
    bool IsPagingEnabled { get; }
    bool AsNoTracking { get; }
}

public interface ISpecification<T, TResult> : ISpecification<T> where T : class
{
    Expression<Func<T, TResult>>? Selector { get; }
}
