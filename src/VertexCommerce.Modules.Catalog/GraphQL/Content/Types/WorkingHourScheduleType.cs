using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class WorkingHourScheduleType : ObjectType<WorkingHourScheduleItem>
{
    protected override void Configure(IObjectTypeDescriptor<WorkingHourScheduleItem> descriptor)
    {
        descriptor.Name("WorkingHourSchedule");
        descriptor.Field(i => i.Day).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Time).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Status).Type<NonNullType<StringType>>();
    }
}
