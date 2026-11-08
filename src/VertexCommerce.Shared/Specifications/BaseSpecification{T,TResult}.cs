using System.Linq.Expressions;

namespace VertexCommerce.Shared.Specifications;

public abstract class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult> 
    where T : class
{
    public Expression<Func<T, TResult>>? Selector { get; private set; }

    protected BaseSpecification() { }

    protected BaseSpecification(Expression<Func<T, bool>> criteria) : base(criteria) { }

    protected void Select(Expression<Func<T, TResult>> selector)
    {
        Selector = selector;
    }
}
