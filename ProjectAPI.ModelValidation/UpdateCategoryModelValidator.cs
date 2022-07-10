using ProjectAPI.Primitives;
using FluentValidation;

namespace ProjectAPI.ModelValidation
{
    /// <summary>
    /// Provides model validation when updating an existing category.
    /// </summary>
    public class UpdateCategoryModelValidator : AbstractValidator<UpdateCategoryModel>
    {
        /// <summary>
        /// Constructs an instance of <see cref="CategoryModelValidator"/> class.
        /// </summary>
        public UpdateCategoryModelValidator()
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

