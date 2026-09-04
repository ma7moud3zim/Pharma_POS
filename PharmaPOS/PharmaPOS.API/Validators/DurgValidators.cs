using FluentValidation;
using PharmaPOS.API.DTOs.Drug;

namespace PharmaPOS.API.Validators;

public class CreateDrugValidator : AbstractValidator<CreateDrugDto>
{
    public CreateDrugValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Drug name is required.")
            .MaximumLength(200).WithMessage("Drug name cannot exceed 200 characters.");

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("Barcode is required.")
            .MaximumLength(50).WithMessage("Barcode cannot exceed 50 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.Form)
            .NotEmpty().WithMessage("Drug form is required.");

        RuleFor(x => x.CostPrice)
            .GreaterThan(0).WithMessage("Cost price must be greater than zero.");

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0).WithMessage("Selling price must be greater than zero.")
            .GreaterThan(x => x.CostPrice).WithMessage("Selling price must be greater than cost price.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative.");

        RuleFor(x => x.ReorderQuantity)
            .GreaterThan(0).WithMessage("Reorder quantity must be greater than zero.");
    }
}

public class UpdateDrugValidator : AbstractValidator<UpdateDrugDto>
{
    public UpdateDrugValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Drug name is required.")
            .MaximumLength(200).WithMessage("Drug name cannot exceed 200 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.Form)
            .NotEmpty().WithMessage("Drug form is required.");

        RuleFor(x => x.CostPrice)
            .GreaterThan(0).WithMessage("Cost price must be greater than zero.");

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0).WithMessage("Selling price must be greater than zero.")
            .GreaterThan(x => x.CostPrice).WithMessage("Selling price must be greater than cost price.");
    }
}