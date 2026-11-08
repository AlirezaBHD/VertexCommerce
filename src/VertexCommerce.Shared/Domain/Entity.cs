namespace VertexCommerce.Shared.Domain;

/// <summary>
/// Base type for entities: objects with a stable identity that is preserved across state changes.
/// </summary>
/// <remarks>
/// Two entities are equal when they are of the same concrete type and share the same non-default id.
/// An entity whose id has not been assigned yet (transient) is only equal to itself.
/// </remarks>
public abstract class Entity<TId>
    where TId : notnull
{
    public TId Id { get; protected init; } = default!;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    private bool IsTransient => EqualityComparer<TId>.Default.Equals(Id, default);

    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void Restore()
    {
        IsDeleted = false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other || other.GetType() != GetType())
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return !IsTransient && !other.IsTransient && Id.Equals(other.Id);
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
