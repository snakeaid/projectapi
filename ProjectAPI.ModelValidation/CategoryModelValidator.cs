using ProjectAPI.Primitives;
using FluentValidation;

namespace ProjectAPI.ModelValidation
{
    /// <summary>
    /// Provides category model validation.
    /// </summary>
    public class CategoryModelValidator : AbstractValidator<CategoryModel>
    {
        /// <summary>
        /// Constructs an instance of <see cref="CategoryModelValidator"/> class.
        /// </summary>
        public CategoryModelValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(2,50);

            RuleFor(x => x.Description).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(4, 1000);
        }
    }
}

