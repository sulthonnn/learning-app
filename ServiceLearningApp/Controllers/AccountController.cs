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
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
            var user = await dbContext.Set<ApplicationUser>()
                .Where(e => e.UserName == model.UserName || e.NISN == model.NISN)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Username atau password salah"
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
            id.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            id.AddClaim(new Claim("NISN", user.NISN ?? string.Empty));
            id.AddClaim(new Claim("fullName", user.FullName ?? string.Empty));

            var expiresIn = currentDate + TokenAuthOptions.ExpiresSpan;
            var token = GenerateToken(expiresIn, id);
            return token;
        }
    }
}
