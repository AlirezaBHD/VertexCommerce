using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactWorkingHoursType : ObjectType<ContactWorkingHoursSection>
{
    protected override void Configure(IObjectTypeDescriptor<ContactWorkingHoursSection> descriptor)
    {
        descriptor.Name("ContactWorkingHours");
        descriptor.Field(w => w.IsOpenNow).Type<NonNullType<BooleanType>>();
        descriptor.Field(w => w.Items).Type<NonNullType<ListType<NonNullType<WorkingHourScheduleType>>>>();
    }
}
