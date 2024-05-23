using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceLearningApp.Data;
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
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly TokenAuthOptions tokenOptions;

        public AccountController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<TokenAuthOptions> tokenOptions,
            IConfiguration configuration
            )
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.tokenOptions = tokenOptions.Value;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            ApplicationUser? user = null;

            // Attempt to find user by username
            if (!string.IsNullOrEmpty(model.UserName))
            {
                user = await userManager.FindByNameAsync(model.UserName);
            }

            if (user == null && !string.IsNullOrEmpty(model.NISN))
            {
                user = await dbContext.Set<ApplicationUser>()
                    .Where(e => e.NISN == model.NISN)
                    .FirstOrDefaultAsync();
            }

            if (user == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Username atau NISN tidak ditemukan"
                });
            }

            var checkPasswordResult = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (checkPasswordResult != PasswordVerificationResult.Success)
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Username atau password salah"
                });

            var requestAt = DateTime.Now;
            var token = await GetToken(user, requestAt);

            return Ok(new
            {
                requestAt,
                expiresIn = TokenAuthOptions.ExpiresSpan.TotalSeconds,
                tokenType = TokenAuthOptions.TokenType,
                accessToken = token
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationDto model)
        {
            try
            {
                var validator = new RegistrationValidators();
                await validator.ValidateAndThrowAsync(model);

                var user = await CreateStudentUser(model);

                return Ok(new { StatusCode = StatusCodes.Status200OK, Message = "Success", Data = user.Id });
            }
            catch (ValidationException ex)
            {
                var errorMessage = ex.Errors.FirstOrDefault()?.ErrorMessage;
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = errorMessage });
            }
            catch (BadHttpRequestException ex)
            {
                var statusCode = ex.StatusCode;
                var errorMessage = ex.Message;
                return BadRequest(new { StatusCode = statusCode, Message = errorMessage });
            }
        }

        private async Task<ApplicationUser> CreateStudentUser(RegistrationDto model)
        {
            var existingUserWithNISN = await userManager.FindByNameAsync(model.UserName);
            if (existingUserWithNISN != null)
            {
                throw new BadHttpRequestException("Username sudah digunakan.", 400);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FullName = model.FullName,
                NISN = model.NISN
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new BadHttpRequestException(result.Errors.First().Description);
            }

            var claims = new List<Claim>() { new Claim(ClaimTypes.Role, Role.Student) };
            await userManager.AddClaimsAsync(user, claims);

            return user;
        }


        private string GenerateToken(DateTime expires, ClaimsIdentity claims)
        {
            var handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenOptions.Issuer,
                Audience = tokenOptions.Audience,
                SigningCredentials = tokenOptions.SigningCredentials,
                Subject = claims,
                NotBefore = DateTime.Now,
                Expires = expires
            });

            return handler.WriteToken(securityToken);
        }

        private async Task<string> GetToken(ApplicationUser user, DateTime currentDate)
        {
            var claims = await userManager.GetClaimsAsync(user);
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);


            foreach (var roleName in roles)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(role);
                    claims = new List<Claim>(claims.Concat(roleClaims));
                }
            }

            var isActivated = user.EmailConfirmed || user.PhoneNumberConfirmed;

            var id = new ClaimsIdentity(claims);
            id.AddClaim(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            id.AddClaim(new Claim("NISN", user.NISN ?? string.Empty));
            id.AddClaim(new Claim("fullName", user.FullName ?? string.Empty));

            var expiresIn = currentDate + TokenAuthOptions.ExpiresSpan;
            var token = GenerateToken(expiresIn, id);
            return token;
        }
    }
}
