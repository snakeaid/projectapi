using ProjectAPI.Primitives;
using FluentValidation;

namespace ProjectAPI.ModelValidation
{
    /// <summary>
    /// Provides model validation when updating an existing product.
    /// </summary>
    public class UpdateProductModelValidator : AbstractValidator<UpdateProductModel>
    {
        /// <summary>
        /// Constructs an instance of <see cref="ProductModelValidator"/> class.
        /// </summary>
        public UpdateProductModelValidator()
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