using ProjectAPI.Primitives;
using FluentValidation;

namespace ProjectAPI.ModelValidation
{
    /// <summary>
    /// Provides product model validation.
    /// </summary>
    public class ProductModelValidator : AbstractValidator<ProductModel>
    {
        /// <summary>
        /// Constructs an instance of <see cref="ProductModelValidator"/> class.
        /// </summary>
        public ProductModelValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(2,50);

            RuleFor(x => x.Description).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(4, 1000);

            RuleFor(x => x.CategoryId).GreaterThan(1);
        }
    }
}