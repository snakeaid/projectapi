using ProjectAPI.Primitives;
using FluentValidation;

namespace ProjectAPI.ModelValidation
{
    /// <summary>
    /// Provides model validation when creating a new product.
    /// </summary>
    public class CreateProductModelValidator : AbstractValidator<CreateProductModel>
    {
        /// <summary>
        /// Constructs an instance of <see cref="CreateProductModelValidator"/> class.
        /// </summary>
        public CreateProductModelValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(2, 50);

            RuleFor(x => x.Description).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(4, 1000);

            RuleFor(x => x.CategoryId).GreaterThan(0);
        }
    }
}