// Validators/CreateFindingInputValidator.cs
using FluentValidation;

namespace FindingsAPI.Gateway.Validators
{
    public class CreateFindingInputValidator : AbstractValidator<GraphQL.Mutations.CreateFindingInput>
    {
        public CreateFindingInputValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
            
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required");
            
            RuleFor(x => x.CompanyId)
                .GreaterThan(0).WithMessage("CompanyId must be greater than 0");
            
            RuleFor(x => x.Services)
                .Must(s => s == null || s.Count <= 10)
                .WithMessage("Cannot associate more than 10 services");
        }
    }
}