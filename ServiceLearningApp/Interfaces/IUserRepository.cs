using Microsoft.AspNetCore.Mvc;
using Model.Common.Dto;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Interfaces
{
    public interface IUserRepository
    {
        Task<ActionResult<UserDto>> GetById(string id);
        Task<IActionResult> Login(LoginDto loginDto);
        Task<IActionResult> Register(RegistrationDto loginDto);
        Task<IActionResult> UpdatePassword(UpdatePasswordDto model);
        Task<IActionResult> UpdateProfile(UserDto model);
    }
}
