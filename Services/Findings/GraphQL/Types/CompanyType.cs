// GraphQL/Types/CompanyType.cs
namespace FindingsAPI.Gateway.GraphQL.Types
{
    public class CompanyType : ObjectType<Company>
    {
        protected override void Configure(IObjectTypeDescriptor<Company> descriptor)
        {
            descriptor.Description("A company entity");
            
            descriptor.Field(c => c.CompanyId)
                .Type<NonNullType<IntType>>()
                .Description("Unique identifier for the company");
            
            descriptor.Field(c => c.CompanyName)
                .Type<NonNullType<StringType>>()
                .Description("Name of the company");
            
            descriptor.Field(c => c.Industry)
                .Type<StringType>()
                .Description("Industry sector");
            
            descriptor.Field(c => c.Status)
                .Type<StringType>()
                .Description("Company status");
            
            descriptor.Authorize();
        }
    }
}