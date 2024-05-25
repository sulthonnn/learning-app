using FluentValidation;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Validators
{
    public class RegistrationValidators : AbstractValidator<RegistrationDto>
    {
        private readonly string EmptyUserName = "Username tidak boleh kosong.";
        private readonly string EmptyFullName = "Nama lengkap tidak boleh kosong.";
        private readonly string EmptyNISN = "NISN tidak boleh kosong.";
        private readonly string EmptyPassword = "Kata sandi tidak boleh kosong.";
        private readonly string EmptyPasswordRepeat = "Ulangi kata sandi tidak boleh kosong.";
        private readonly string PasswordNotEqual = "Kata sandi dan ulangi kata sandi harus sama.";
        //private readonly string PasswordMustHaveUppercase = "Kata sandi harus ada huruf besar.";
        //private readonly string PasswordMustHaveDigit = "Kata sandi harus ada angka.";
        //private readonly string MinimumPasswordLength = "Kata sandi minimal 8 karakter";

        public RegistrationValidators()
        {
            RuleFor(e => e.UserName).NotEmpty().WithMessage(EmptyUserName);
            RuleFor(e => e.FullName).NotEmpty().WithMessage(EmptyFullName);
            RuleFor(e => e.NISN).NotEmpty().WithMessage(EmptyNISN);
            RuleFor(e => e.Password).NotEmpty().WithMessage(EmptyPassword);
            RuleFor(e => e.PasswordRepeat).NotEmpty().WithMessage(EmptyPasswordRepeat);
            RuleFor(e => e.Password).Equal(e => e.PasswordRepeat).WithMessage(PasswordNotEqual);
            //RuleFor(e => e.Password).MinimumLength(8).WithMessage(MinimumPasswordLength);
            //RuleFor(e => e.Password).Must(e => e.Any(char.IsUpper)).WithMessage(PasswordMustHaveUppercase);
            //RuleFor(e => e.Password).Must(e => e.Any(char.IsDigit)).WithMessage(PasswordMustHaveDigit);
        }
    }
}
