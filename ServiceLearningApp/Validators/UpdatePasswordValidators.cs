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
            RuleFor(e => e.PasswordRepeat).Equal(e => e.Password).WithMessage(PasswordNotEqual);
        }
    }
}
