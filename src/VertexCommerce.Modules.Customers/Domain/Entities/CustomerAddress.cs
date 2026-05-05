using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Customers.Domain.Entities;

    public sealed class CustomerAddress : Entity<Guid>
    {
        public Guid CustomerId { get; private set; }
        public string Province { get; private set; } = default!;
        public string City { get; private set; } = default!;
        public string PostalAddress { get; private set; } = default!;
        public string PostalCode { get; private set; } = default!;
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public string? Label { get; private set; }

    private CustomerAddress() { }

    public static CustomerAddress Create(
        Guid customerId,
        string province,
        string city,
        string postalAddress,
        string postalCode,
        decimal latitude,
        decimal longitude,
        string? label = null)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude));

        return new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Province = province,
            City = city,
            PostalAddress = postalAddress,
            PostalCode = postalCode,
            Latitude = latitude,
            Longitude = longitude,
            Label = label,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string province,
        string city,
        string postalAddress,
        string postalCode,
        decimal latitude,
        decimal longitude,
        string? label)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude));

        Province = province;
        City = city;
        PostalAddress = postalAddress;
        PostalCode = postalCode;
        Latitude = latitude;
        Longitude = longitude;
        Label = label;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullAddress => $"{Province}، {City}، {PostalAddress} — {PostalCode}";
}
