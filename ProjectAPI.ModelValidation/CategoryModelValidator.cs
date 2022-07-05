using ProjectAPI.Primitives;
using FluentValidation;

namespace ProjectAPI.ModelValidation
{
    public class CategoryModelValidator : AbstractValidator<CategoryModel>
    {
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

