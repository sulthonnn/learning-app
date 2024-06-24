using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Common.Dto;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;
using ServiceLearningApp.Security;
using ServiceLearningApp.Validators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ServiceLearningApp.Data
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        //private readonly IConfiguration configuration;
        private readonly TokenAuthOptions tokenOptions;
        private readonly IAuthorizationService authorizationService;


        public AccountRepository(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<TokenAuthOptions> tokenOptions,
            IAuthorizationService authorizationService
            //IConfiguration configuration
            )
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            //this.configuration = configuration;
            this.tokenOptions = tokenOptions.Value;
            this.authorizationService = authorizationService;
        }

        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            ApplicationUser? user = null;

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
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = "Username atau password salah"
                });
            }

            var checkPasswordResult = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (checkPasswordResult != PasswordVerificationResult.Success)
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = "Username atau password salah"
                });

            var requestAt = DateTime.Now;
            var token = await GetToken(user, requestAt);

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Berhasil login",
                requestAt,
                expiresIn = TokenAuthOptions.ExpiresSpan.TotalSeconds,
                tokenType = TokenAuthOptions.TokenType,
                accessToken = token
            });
        }

        public async Task<IActionResult> Register([FromBody] RegistrationDto model)
        {
            try
            {
                var validator = new RegistrationValidators();
                await validator.ValidateAndThrowAsync(model);

                var user = await CreateStudentUser(model);

                var responseData = new
                {
                    user.Id,
                    user.UserName,
                    user.NISN,
                    user.FullName
                };

                return new CreatedResult("", new 
                { 
                    Code = StatusCodes.Status201Created,
                    Status = "Created",
                    Message = "Berhasil melakukan registrasi", 
                    Data = responseData 
                });
            }
            catch (ValidationException ex)
            {
                var errorMessage = ex.Errors.FirstOrDefault()?.ErrorMessage;
                return new BadRequestObjectResult(new 
                { 
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = errorMessage 
                });
            }
            catch (BadHttpRequestException ex)
            {
                return new BadRequestObjectResult(new 
                { 
                    Code = ex.StatusCode,
                    Status = "Bad Request",
                    Message = ex.Message 
                });
            }
        }

        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            var user = await this.dbContext.Users
                .Include(e => e.Image)
                .SingleAsync(e => e.Id == id);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                NISN = user.NISN,
                Image = user.Image,
                FkImageId = user.FkImageId,
                DateOfBirth = user.DateOfBirth
            };

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data user berhasil didapatkan",
                Data = userDto
            });
        }


        public async Task<IActionResult> UpdateProfile(UserDto model)
        {

            using(var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var existingUser = await userManager.FindByIdAsync(model.Id);
                    if (existingUser == null)
                    {
                        return new NotFoundObjectResult(new 
                        { 
                            Code = StatusCodes.Status404NotFound, 
                            Status = "Not Found",
                            Message = "User tidak ditemukan"
                        });

                    }

                    existingUser.FullName = model.FullName;
                    existingUser.UserName = model.UserName;
                    existingUser.NISN = model.NISN;
                    existingUser.DateOfBirth = model.DateOfBirth;
                    existingUser.Email = model.Email;

                    dbContext.Update(existingUser);
                    await dbContext.SaveChangesAsync();


                    if (model.FkImageId != null)
                    {
                        existingUser.FkImageId = model.FkImageId;
                        await userManager.UpdateAsync(existingUser);
                    }

                    transaction.Commit();

                    return new OkObjectResult(new 
                    { 
                        Code = StatusCodes.Status200OK,
                        Status = "Ok",
                        Message = "Berhasil mengubah profil",
                        Data = model
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ObjectResult(new
                    {
                        Message = ex.Message,
                    });
                }
            }

        }

        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto model)
        {
            var validator = new UpdatePasswordValidators();
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(new 
                { 
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = validationResult.Errors.First().ErrorMessage 
                });
            }

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return new NotFoundObjectResult(new 
                { 
                    Code = StatusCodes.Status404NotFound,
                    Status = "Bad Request",
                    Message = "User tidak ditemukan" 
                });
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.PasswordOld, model.Password);
            if (!changePasswordResult.Succeeded)
            {
                return new BadRequestObjectResult(new 
                { 
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = "Gagal mengubah password"
                });
            }

            return new OkObjectResult(new 
            { 
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Sukses mengubah password"
            });
        }

        private async Task<ApplicationUser> CreateStudentUser(RegistrationDto model)
        {
            var existingUser = await userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                throw new BadHttpRequestException("Username sudah digunakan.", 400);
            }

            var existingUserWithNISN = await dbContext.Users.Where(u => u.NISN == model.NISN).FirstOrDefaultAsync();
            if (existingUserWithNISN != null)
            {
                throw new BadHttpRequestException("NISN sudah digunakan.", 400);
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
                Expires = expires,
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

            var id = new ClaimsIdentity(claims);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            id.AddClaim(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));
            id.AddClaim(new Claim("NISN", user.NISN ?? string.Empty));
            id.AddClaim(new Claim("fullName", user.FullName ?? string.Empty));

            var expiresIn = currentDate + TokenAuthOptions.ExpiresSpan;
            var token = GenerateToken(expiresIn, id);
            return token;
        }
    }
}
