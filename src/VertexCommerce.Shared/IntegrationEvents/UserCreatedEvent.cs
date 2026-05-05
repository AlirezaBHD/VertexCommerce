using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Shared.IntegrationEvents;

public class UserCreatedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public Guid UserId { get; }
    public string PhoneNumber { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime OccurredOn { get; }
    
    
    public UserCreatedEvent(Guid userId, string phoneNumber, string firstName, string lastName)
    {
        EventId = Guid.NewGuid();
        UserId = userId;
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        OccurredOn = DateTime.UtcNow;
    }


}
