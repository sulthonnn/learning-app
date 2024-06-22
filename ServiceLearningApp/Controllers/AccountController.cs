using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Common.Dto;
using ServiceEsgDataHub.Services;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;
using ServiceLearningApp.Security;
using System.Security.Claims;

namespace ServiceLearningApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class AccountController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IAuthorizationService authorizationService;
        private readonly UserResolverService userResolverService;

        public AccountController(IUserRepository userRepository, IAuthorizationService authorizationService, UserResolverService userResolverService)
        {
            this.userRepository = userRepository;
            this.authorizationService = authorizationService;
            this.userResolverService = userResolverService;
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

        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(string id)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, new ApplicationUser { Id = id }, new EditUserRequirement());
            if (!authorizationResult.Succeeded)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status403Forbidden,
                    Status = "Forbidden",
                    Message = "Anda tidak memiliki izin untuk mengakses sumber daya ini."
                });
            }

            return await this.userRepository.GetById(id);
        }

        [HttpPut("user/profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserDto model)
        {
            var userId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value;

            var authorizationResult = await this.authorizationService.AuthorizeAsync(User, new ApplicationUser { Id = userId }, new EditUserRequirement());
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            return await this.userRepository.UpdateProfile(model);
        }

        [HttpPut("user/profile/password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto model)
        {
            var userId = this.userResolverService.GetNameIdentifier();
            if (userId != null)
                model.Id = userId;

            var authorizationResult = await this.authorizationService.AuthorizeAsync(User, new ApplicationUser { Id = model.Id }, new EditUserRequirement());
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }

            return await this.userRepository.UpdatePassword(model);
        }
    }
}
