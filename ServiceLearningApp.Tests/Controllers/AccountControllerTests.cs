using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceEsgDataHub.Services;
using ServiceLearningApp.Controllers;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model.Dto;
using System.Text;

namespace ServiceLearningApp.Tests.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var accountRepository = A.Fake<IAccountRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userResolverService = A.Fake<UserResolverService>();

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("bdkajbd72i31kjn1ep;.d/sadmsaldnajndsKSnjnakdnas"));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenAuthOptions = new
            {
                Issuer = "LearningAppIssuer",
                Audience = "LearningAppAudience",
                SigningCredentials = signingCredentials,
                ExpiresSpan = TimeSpan.FromDays(1),
                TokenType = "Bearer"
            };

            var optionsMock = Options.Create(tokenAuthOptions);
            var loginDto = new LoginDto
            {
                UserName = "sulthon123",
                Password = "sulthon123"
            };

            var tokenResponse = new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Berhasil login",
                requestAt = DateTime.Now,
                expiresIn = tokenAuthOptions.ExpiresSpan.TotalSeconds,
                tokenType = tokenAuthOptions.TokenType,
                accessToken = "fake_jwt_token",
            };

            var validUsername = "sulthon123";
            var validPassword = "sulthon123";

            loginDto.UserName.Should().Match(validUsername);
            loginDto.Password.Should().Match(validPassword);

            A.CallTo(() => accountRepository.Login(loginDto)).Returns(Task.FromResult<IActionResult>(new OkObjectResult(tokenResponse)));

            var controller = new AccountController(accountRepository, authorizationService, userResolverService);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var actualTokenResponse = okResult.Value.Should().BeAssignableTo<object>().Subject;
            actualTokenResponse.Should().BeEquivalentTo(tokenResponse);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var accountRepository = A.Fake<IAccountRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userResolverService = A.Fake<UserResolverService>();

            var loginDto = new LoginDto
            {
                UserName = "sulthon123",
                Password = "sulthon" //password salah
            };

            var validPassword = "sulthon123";
            loginDto.Password.Should().NotMatch(validPassword);

            var errorResponse = new
            {
                Code = StatusCodes.Status400BadRequest,
                Status = "Bad Request",
                Message = "Username atau password salah"
            };

            A.CallTo(() => accountRepository.Login(loginDto)).Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(errorResponse)));

            var controller = new AccountController(accountRepository, authorizationService, userResolverService);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResponse = badRequestResult.Value.Should().BeAssignableTo<object>().Subject;
            actualResponse.Should().BeEquivalentTo(errorResponse);
      
        }

        [Fact]
        public async Task Register_ValidRegistration_ReturnsOkWithUserId()
        {
            // Arrange
            var accountRepository = A.Fake<IAccountRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userResolverService = A.Fake<UserResolverService>();

            var registrationDto = new RegistrationDto
            {
                UserName = "testuser",
                FullName = "Test User",
                NISN = "1234567890",
                Password = "password",
                PasswordRepeat = "password"
            };
            var existingUserName = "testuser2";
            var existingNISN = "1234567899";

            // Pemeriksaan jika ada properti RegistrationDto yang null
            registrationDto.Should().NotBeNull();
            registrationDto.UserName.Should().NotBeNullOrEmpty();
            registrationDto.FullName.Should().NotBeNullOrEmpty();
            registrationDto.NISN.Should().NotBeNullOrEmpty();
            registrationDto.Password.Should().NotBeNullOrEmpty();
            registrationDto.PasswordRepeat.Should().NotBeNullOrEmpty();

            registrationDto.UserName.Should().NotMatch(existingUserName);
            registrationDto.NISN.Should().NotMatch(existingNISN);
            registrationDto.PasswordRepeat.Should().Match(registrationDto.Password);

            var id = new Guid();

            var responseData = new
            {
                Id = id,
                registrationDto.UserName,
                registrationDto.NISN,
                registrationDto.FullName
            };

            var successResponse = new
            {
                Code = StatusCodes.Status201Created,
                Status = "Created",
                Message = "Berhasil melakukan registrasi",
                Data = registrationDto
            };

            A.CallTo(() => accountRepository.Register(registrationDto)).Returns(Task.FromResult<IActionResult>(new CreatedResult("", successResponse)));

            var controller = new AccountController(accountRepository, authorizationService, userResolverService);

            // Act
            var result = await controller.Register(registrationDto);

            // Assert
            var okResult = result.Should().BeOfType<CreatedResult>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            var actualResponse = okResult.Value.Should().BeAssignableTo<object>().Subject;
            actualResponse.Should().BeEquivalentTo(successResponse);
        }


        [Fact]
        public async Task Register_InvalidNull_ReturnsBadRequest()
        {
            // Arrange
            var accountRepository = A.Fake<IAccountRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userResolverService = A.Fake<UserResolverService>();

            var registrationDto = new RegistrationDto
            {
                UserName = "testuser",
                FullName = "", // Data FullName tidak valid (kosong)
                NISN = "1234567890",
                Password = "password",
                PasswordRepeat = "password"
            };

            registrationDto.FullName.Should().BeNullOrEmpty();

            var errorResponse = new
            {
                Code = StatusCodes.Status400BadRequest,
                Status = "Bad Request",
                Message = "Nama lengkap tidak boleh kosong" // Pesan kesalahan validasi
            };

            A.CallTo(() => accountRepository.Register(registrationDto)).Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(errorResponse)));

            var controller = new AccountController(accountRepository, authorizationService, userResolverService);

            // Act
            var result = await controller.Register(registrationDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResponse = badRequestResult.Value.Should().BeAssignableTo<object>().Subject;
            actualResponse.Should().BeEquivalentTo(errorResponse);
        }

        [Fact]
        public async Task Register_InvalidUserName_ReturnsBadRequest()
        {
            // Arrange
            var accountRepository = A.Fake<IAccountRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userResolverService = A.Fake<UserResolverService>();

            var registrationDto = new RegistrationDto
            {
                UserName = "existinguser", // Username yang sudah ada
                FullName = "Test User",
                NISN = "1234567890",
                Password = "password",
                PasswordRepeat = "password"
            };

            var existingUser = "existinguser";
            registrationDto.UserName.Should().Match(existingUser);

            var errorResponse = new
            {
                Code = StatusCodes.Status400BadRequest,
                Status = "Bad Request",
                Message = "Username sudah digunakan." // Pesan kesalahan validasi
            };

            A.CallTo(() => accountRepository.Register(registrationDto)).Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(errorResponse)));

            var controller = new AccountController(accountRepository, authorizationService, userResolverService);

            // Act
            var result = await controller.Register(registrationDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResponse = badRequestResult.Value.Should().BeAssignableTo<object>().Subject;
            actualResponse.Should().BeEquivalentTo(errorResponse);
        }

        [Fact]
        public async Task Register_InvalidNISN_ReturnsBadRequest()
        {
            // Arrange
            var accountRepository = A.Fake<IAccountRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userResolverService = A.Fake<UserResolverService>();


            var registrationDto = new RegistrationDto
            {
                UserName = "testuser",
                FullName = "Test User",
                NISN = "existingnisn", // NISN yang sudah ada
                Password = "password",
                PasswordRepeat = "password"
            };

            var existingNISN = "exisingnisn";
            registrationDto.NISN.Should().NotMatch(existingNISN);

            var errorResponse = new
            {
                Code = StatusCodes.Status400BadRequest,
                Status = "Bad Request",
                Message = "NISN sudah digunakan." // Pesan kesalahan validasi
            };

            A.CallTo(() => accountRepository.Register(registrationDto)).Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(errorResponse)));

            var controller = new AccountController(accountRepository, authorizationService, userResolverService);

            // Act
            var result = await controller.Register(registrationDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResponse = badRequestResult.Value.Should().BeAssignableTo<object>().Subject;
            actualResponse.Should().BeEquivalentTo(errorResponse);
        }


        [Fact]
        public async Task Register_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var accountRepository = A.Fake<IAccountRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userResolverService = A.Fake<UserResolverService>();


            var registrationDto = new RegistrationDto
            {
                UserName = "testuser",
                FullName = "Test User",
                NISN = "1234567890",
                Password = "password",
                PasswordRepeat = "wrongpassword" // Password tidak sesuai dengan PasswordRepeat
            };

            registrationDto.Password.Should().NotMatch(registrationDto.PasswordRepeat);

            var errorResponse = new
            {
                Code = StatusCodes.Status400BadRequest,
                Status = "Bad Request",
                Message = "Kata sandi dan ulangi kata sandi harus sama." // Pesan kesalahan validasi
            };

            A.CallTo(() => accountRepository.Register(registrationDto)).Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(errorResponse)));

            var controller = new AccountController(accountRepository, authorizationService, userResolverService);

            // Act
            var result = await controller.Register(registrationDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var actualResponse = badRequestResult.Value.Should().BeAssignableTo<object>().Subject;
            actualResponse.Should().BeEquivalentTo(errorResponse);
        }

    }
}
