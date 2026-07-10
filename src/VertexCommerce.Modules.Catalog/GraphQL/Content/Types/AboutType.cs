using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutType : ObjectType<AboutDocument>
{
    protected override void Configure(IObjectTypeDescriptor<AboutDocument> descriptor)
    {
        descriptor.Name("AboutContent");

        descriptor.Field(a => a.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(a => a.Title).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Description).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Mission).Type<StringType>();
        descriptor.Field(a => a.Vision).Type<StringType>();
        descriptor.Field(a => a.Values).Type<NonNullType<ListType<NonNullType<AboutValueType>>>>();
        descriptor.Field(a => a.Stats).Type<NonNullType<ListType<NonNullType<AboutStatType>>>>();
        descriptor.Field(a => a.Team).Type<NonNullType<ListType<NonNullType<AboutTeamMemberType>>>>();
        descriptor.Field(a => a.UpdatedAt).Type<NonNullType<DateTimeType>>();
    }
}

public sealed class AboutValueType : ObjectType<AboutValueItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutValueItem> descriptor)
    {
        descriptor.Name("AboutValue");
        descriptor.Field(v => v.Icon).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.Title).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.Description).Type<NonNullType<StringType>>();
    }
}

public sealed class AboutStatType : ObjectType<AboutStatItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutStatItem> descriptor)
    {
        descriptor.Name("AboutStat");
        descriptor.Field(s => s.Label).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Value).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Suffix).Type<StringType>();
    }
}

public sealed class AboutTeamMemberType : ObjectType<AboutTeamMember>
{
    protected override void Configure(IObjectTypeDescriptor<AboutTeamMember> descriptor)
    {
        descriptor.Name("AboutTeamMember");
        descriptor.Field(t => t.Name).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Role).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Bio).Type<StringType>();
        descriptor.Field(t => t.ImagePath).Type<StringType>();
    }
}
