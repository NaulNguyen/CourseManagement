using CourseManager.API.DTOs;
using FluentValidation;

namespace CourseManager.API.Validations
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tên khóa học không được để trống")
                .MaximumLength(100).WithMessage("Tên không được vượt quá 100 ký tự");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá tiền phải từ 0 trở lên");
        }
    }
}