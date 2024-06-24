using FluentValidation;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Validators
{
    public class RegistrationValidators : AbstractValidator<RegistrationDto>
    {
        private readonly string EmptyUserName = "Username tidak boleh kosong.";
        private readonly string EmptyFullName = "Nama lengkap tidak boleh kosong.";
        private readonly string EmptyNISN = "NISN tidak boleh kosong.";
        private readonly string InvalidNISNLength = "NISN harus terdiri dari 10 digit.";
        private readonly string EmptyPassword = "Kata sandi tidak boleh kosong.";
        private readonly string EmptyPasswordRepeat = "Ulangi kata sandi tidak boleh kosong.";
        private readonly string PasswordNotEqual = "Kata sandi dan ulangi kata sandi harus sama.";

        public RegistrationValidators()
        {
            RuleFor(e => e.UserName).NotEmpty().WithMessage(EmptyUserName);
            RuleFor(e => e.FullName).NotEmpty().WithMessage(EmptyFullName);
            RuleFor(e => e.NISN).NotEmpty().WithMessage(EmptyNISN);
            RuleFor(e => e.NISN).Length(10).WithMessage(InvalidNISNLength);
            RuleFor(e => e.NISN).Must(BeValidNISN).WithMessage(InvalidNISNLength); // Additional check for numeric digits
            RuleFor(e => e.Password).NotEmpty().WithMessage(EmptyPassword);
            RuleFor(e => e.PasswordRepeat).NotEmpty().WithMessage(EmptyPasswordRepeat);
            RuleFor(e => e.Password).Equal(e => e.PasswordRepeat).WithMessage(PasswordNotEqual);
        }

        private bool BeValidNISN(string nisn)
        {
            // Check if NISN consists of exactly 10 numeric digits
            return !string.IsNullOrEmpty(nisn) && nisn.Length == 10 && nisn.All(char.IsDigit);
        }
    }
}
