using AutoMapper.Execution;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace Model.Common.Dto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
        public string PasswordOld { get; set; }
        public List<ClaimDto> Claims { get; set; }
        public Upload? Image { get; set; }
        public long? FkImageId { get; set; }
    }
}
