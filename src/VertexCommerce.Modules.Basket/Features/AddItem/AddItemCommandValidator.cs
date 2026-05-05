// using FluentValidation;
//
// namespace VertexCommerce.Modules.Basket.Features.AddItem;
//
// public sealed class AddItemCommandValidator : AbstractValidator<AddItemCommand>
// {
//     public AddItemCommandValidator()
//     {
//         RuleFor(x => x.CustomerId)
//             .NotEmpty();
//
//         RuleFor(x => x.ProductId)
//             .NotEmpty();
//
//         RuleFor(x => x.Quantity)
//             .GreaterThan(0)
//             .LessThanOrEqualTo(100)
//             .WithMessage("Quantity must be between 1 and 100");
//     }
// }
