using ProjectAPI.Primitives;
using FluentValidation;

namespace ProjectAPI.ModelValidation
{
    /// <summary>
    /// Provides model validation when creating a new category.
    /// </summary>
    public class CreateCategoryModelValidator : AbstractValidator<CreateCategoryModel>
    {
        /// <summary>
        /// Constructs an instance of <see cref="CategoryModelValidator"/> class.
        /// </summary>
        public CreateCategoryModelValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(2, 50);

            RuleFor(x => x.Description).Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .Length(4, 1000);
        }
    }
}

