namespace ServiceLearningApp.Model.Dto
{
    public class RegistrationDto
    {
        public required string FullName { get; set; }
        public required string UserName { get; set; }
        public required string NISN { get; set; }
        public required string Password { get; set; }
        public required string PasswordRepeat { get; set; }
    }
}
