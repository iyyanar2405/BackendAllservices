// GraphQL/Types/FindingType.cs
using FindingsAPI.Gateway.GraphQL.DataLoaders;

namespace FindingsAPI.Gateway.GraphQL.Types
{
    public class FindingType : ObjectType<Finding>
    {
        protected override void Configure(IObjectTypeDescriptor<Finding> descriptor)
        {
            descriptor.Description("A finding represents an audit observation or issue");
            
            descriptor.Field(f => f.FindingsId)
                .Type<NonNullType<IntType>>()
                .Description("Unique identifier for the finding");
            
            descriptor.Field(f => f.FindingNumber)
                .Type<StringType>()
                .Description("Human-readable finding number");
            
            descriptor.Field(f => f.Title)
                .Type<NonNullType<StringType>>()
                .Description("Title of the finding");
            
            descriptor.Field(f => f.Status)
                .Type<EnumType<FindingStatus>>()
                .Description("Current status of the finding");
            
            descriptor.Field(f => f.Category)
                .Type<StringType>()
                .Description("Category/severity of the finding");
            
            descriptor.Field(f => f.Response)
                .Type<StringType>()
                .Description("Response/action plan for the finding");
            
            descriptor.Field(f => f.CompanyId)
                .Type<NonNullType<IntType>>()
                .Description("ID of the associated company");
            
            descriptor.Field(f => f.OpenDate)
                .Type<DateTimeType>()
                .Description("Date when the finding was opened");
            
            descriptor.Field(f => f.DueDate)
                .Type<DateTimeType>()
                .Description("Due date for resolution");
            
            descriptor.Field(f => f.AcceptedDate)
                .Type<DateTimeType>()
                .Description("Date when the finding was accepted");
            
            descriptor.Field(f => f.ClosedDate)
                .Type<DateTimeType>()
                .Description("Date when the finding was closed");
            
            descriptor.Field(f => f.SiteId)
                .Type<IntType>()
                .Description("ID of the associated site");
            
            descriptor.Field("services")
                .Type<ListType<IntType>>()
                .Resolve(context => 
                    context.Parent<Finding>().Services ?? new List<int>())
                .Description("List of service IDs associated with the finding");
            
            // Nested fields with data loaders
            descriptor.Field("company")
                .Type<CompanyType>()
                .Resolve(async context =>
                {
                    var loader = context.DataLoader<CompanyDataLoader>();
                    var finding = context.Parent<Finding>();
                    return await loader.LoadAsync(finding.CompanyId, context.RequestAborted);
                })
                .Description("Company associated with this finding");
            
            // descriptor.Field("site")
            //     .Type<SiteType>()
            //     .Resolve(async context =>
            //     {
            //         var finding = context.Parent<Finding>();
            //         if (!finding.SiteId.HasValue)
            //             return null;
            //         
            //         var loader = context.DataLoader<SiteDataLoader>();
            //         return await loader.LoadAsync(finding.SiteId.Value, context.RequestAborted);
            //     })
            //     .Description("Site associated with this finding");
            
            // Computed fields
            descriptor.Field("isOverdue")
                .Type<BooleanType>()
                .Resolve(context =>
                {
                    var finding = context.Parent<Finding>();
                    return finding.DueDate.HasValue && 
                           finding.DueDate.Value < DateTime.UtcNow && 
                           finding.Status != "Closed";
                })
                .Description("Indicates if the finding is past its due date");
            
            descriptor.Field("daysOpen")
                .Type<IntType>()
                .Resolve(context =>
                {
                    var finding = context.Parent<Finding>();
                    if (!finding.OpenDate.HasValue)
                        return null;
                    
                    return (DateTime.UtcNow - finding.OpenDate.Value).Days;
                })
                .Description("Number of days the finding has been open");
            
            // Authorization
            descriptor.Authorize("CanViewFindings");
        }
    }
    
    public enum FindingStatus
    {
        Open,
        Accepted,
        Responded,
        NoAction,
        Closed
    }
}