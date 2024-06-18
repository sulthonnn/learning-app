namespace ServiceLearningApp.Model.Dto
{
    public class UpdatePasswordDto
    {
        public required string Id { get; set; }
        public required string PasswordOld { get; set; }
        public required string Password { get; set; }
        public required string PasswordRepeat { get; set; }

    }
}
