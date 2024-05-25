using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceLearningApp.Data;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;
using ServiceLearningApp.Security;
using ServiceLearningApp.Validators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ServiceLearningApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class AccountController : Controller
    {
        private readonly IUserRepository userRepository;

        public AccountController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            return await this.userRepository.Login(model);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationDto model)
        {
            return await this.userRepository.Register(model);
        }
    }
}
