using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateContact;

public sealed record UpdateContactRequest(
    ContactHeaderSection Header,
    List<ContactPhoneItem>? Phones,
    List<ContactEmailItem>? Emails,
    ContactWorkingHoursSection? WorkingHours,
    ContactLocationSection Location,
    List<ContactStorePhotoItem>? Photos,
    List<ContactSocialItem>? Socials,
    List<ContactFaqItem>? Faqs);