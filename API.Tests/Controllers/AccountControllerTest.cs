using API.Controllers;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System.Threading.Tasks;
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

            mockSignInManager.Setup(signInManager => signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false)).ReturnsAsync(SignInResult.Failed);
            
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

            mockSignInManager.Setup(signInManager => signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false)).ReturnsAsync(SignInResult.Success);

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
    }
}
