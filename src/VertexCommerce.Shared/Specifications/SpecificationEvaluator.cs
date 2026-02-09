using Microsoft.EntityFrameworkCore;

namespace VertexCommerce.Shared.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> ApplySpecification<T>(
        IQueryable<T> query,
        ISpecification<T> spec) where T : class
    {
        if (spec.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (spec.Criteria is not null)
        {
            query = query.Where(spec.Criteria);
        }

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        if (spec.OrderBy is not null)
        {
            query = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending is not null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        if (spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip).Take(spec.Take);
        }

        return query;
    }

    public static IQueryable<TResult> ApplySpecification<T, TResult>(
        IQueryable<T> query,
        ISpecification<T, TResult> spec) where T : class
    {
        var baseQuery = ApplySpecification(query, (ISpecification<T>)spec);

        if (spec.Selector is not null)
        {
            return baseQuery.Select(spec.Selector);
        }

        throw new InvalidOperationException("Selector must be specified for projection.");
    }
}
