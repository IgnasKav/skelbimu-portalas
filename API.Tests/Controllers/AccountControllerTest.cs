using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace API.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly Mock<UserManager<User>> mockUserManager;
        private readonly Mock<SignInManager<User>> mockSignInManager;
        private readonly Mock<TokenService> mockTokenService;

        private readonly AccountController controller;

        // Setup
        public AccountControllerTest()
        {
            var mockUserStore = new Mock<IUserStore<User>>();
            mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            mockSignInManager = new Mock<SignInManager<User>>(mockUserManager.Object, mockContextAccessor.Object, mockClaimsFactory.Object, null, null, null, null);

            var mockConfiguration = new Mock<IConfiguration>();
            mockTokenService = new Mock<TokenService>(mockConfiguration.Object);

            controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockTokenService.Object);
        }

        [Fact]
        public async Task TestLoginWhenUserDoesNotExist()
        {
            LoginDto loginDto = new() { Email = "Foo", Password = "Bar" };
            mockUserManager.Setup(userManager => userManager.FindByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);
            
            var result = await controller.Login(loginDto);

            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestLoginWhenPasswordDoesNotMatch()
        {            
            var user = new User
            {
                DisplayName = "Foo Bar",
                Email = "Foo",
                UserName = "Password"
            };
            LoginDto loginDto = new() { Email = "Foo", Password = "Bar" };


            mockUserManager.Setup(userManager => userManager.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            mockSignInManager.Setup(signInManager => signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(SignInResult.Failed);
            
            var result = await controller.Login(loginDto);

            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task TestSuccessfulLogin()
        {
            var user = new User
            {
                DisplayName = "Foo Bar",
                Email = "Foo",
                UserName = "Password"
            };
            LoginDto loginDto = new() { Email = "Foo", Password = "Password" };


            mockUserManager.Setup(userManager => userManager.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            mockSignInManager.Setup(signInManager => signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(SignInResult.Success);

            mockTokenService.Setup(tokenService => tokenService.CreateToken(user)).Returns("token123");

            var result = await controller.Login(loginDto);

            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            Assert.IsType<UserDto>(actionResult.Value);

            var expectedResult = new UserDto
            {
                DisplayName = user.DisplayName,
                Token = "token123",
                Username = user.UserName
            };

            var actualResult = actionResult.Value;

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(actualResult));
        }

        [Theory]
        [InlineData("Email", "Username", "Password", "Email taken")]
        [InlineData("UnusedEmail", "Username", "Password", "Username taken")]
        [InlineData("UnusedEmail", "UnusedUsername", "Password", "Problem registering user")]
        public async Task TestRegistrationFailsDueToBadData(string email, string username, string password, string expectedErrorMessage)
        {
            var users = new List<User>()
            {
              new User{Email = "Email", UserName = "Username", DisplayName = "Name"},  
            };

            var mockUsers = users.AsQueryable().BuildMock();

            mockUserManager
                .Setup(mockUserManager => mockUserManager.Users)
                .Returns(mockUsers);

            mockUserManager
                .Setup(mockUserManager => mockUserManager.CreateAsync(It.IsAny<User>(), "Password"))
                .ReturnsAsync(IdentityResult.Failed());                

            RegisterDto registerDto = new() { DisplayName = "Name", Email = email, Username = username, Password = password };
            var result = await controller.Register(registerDto);

            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);

            Assert.Equal(expectedErrorMessage, ((BadRequestObjectResult)actionResult.Result).Value);
        }        

        [Fact]
        public async Task TestSuccessfulRegistration()
        {
            var users = new List<User>()
            {
              new User{Email = "Email", UserName = "Username", DisplayName = "Name"},
            };

            var mockUsers = users.AsQueryable().BuildMock();

            mockUserManager
                .Setup(mockUserManager => mockUserManager.Users)
                .Returns(mockUsers);

            mockUserManager
                .Setup(mockUserManager => mockUserManager.CreateAsync(It.IsAny<User>(), "Password"))
                .ReturnsAsync(IdentityResult.Success);

            mockTokenService.Setup(tokenService => tokenService.CreateToken(It.IsAny<User>())).Returns("token123");

            RegisterDto registerDto = new() { DisplayName = "Name", Email = "Foo", Username = "Bar", Password = "Password" };
            var result = await controller.Register(registerDto);

            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            Assert.IsType<UserDto>(actionResult.Value);

            var expectedResult = new UserDto
            {
                DisplayName = "Name",
                Token = "token123",
                Username = "Bar"
            };

            var actualResult = actionResult.Value;

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(actualResult));
        }
    }
}
