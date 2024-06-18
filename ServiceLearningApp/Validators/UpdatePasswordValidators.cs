using FluentValidation;
using Model.Common.Dto;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Validators
{
    public class UpdatePasswordValidators : AbstractValidator<UpdatePasswordDto>
    {
        private readonly string PasswordNotEqual = "Kata sandi dan ulangi kata sandi harus sama.";

        public UpdatePasswordValidators() 
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.PasswordOld).NotEmpty().WithMessage("Old password is required.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
            RuleFor(e => e.PasswordRepeat).Equal(e => e.Password).WithMessage(PasswordNotEqual);

        }
    }
}
