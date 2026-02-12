// GraphQL/Types/SiteType.cs
namespace FindingsAPI.Gateway.GraphQL.Types
{
    public class SiteType : ObjectType<Site>
    {
        protected override void Configure(IObjectTypeDescriptor<Site> descriptor)
        {
            descriptor.Description("A site/location entity");
            
            descriptor.Field(s => s.SiteId)
                .Type<NonNullType<IntType>>()
                .Description("Unique identifier for the site");
            
            descriptor.Field(s => s.SiteName)
                .Type<NonNullType<StringType>>()
                .Description("Name of the site");
            
            descriptor.Field(s => s.CompanyId)
                .Type<NonNullType<IntType>>()
                .Description("ID of the associated company");
            
            descriptor.Field(s => s.Location)
                .Type<StringType>()
                .Description("Location details");
            
            descriptor.Field(s => s.Status)
                .Type<StringType>()
                .Description("Site status");
            
            descriptor.Authorize();
        }
    }
}