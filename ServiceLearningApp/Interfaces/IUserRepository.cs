using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IActionResult> Login([FromBody] LoginDto loginDto);
        Task<IActionResult> Register([FromBody] RegistrationDto loginDto);

    }
}
