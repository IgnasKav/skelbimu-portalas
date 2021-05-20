using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly TokenService tokenService;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            System.Console.WriteLine(loginDto.Email);
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized();
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }
            return Unauthorized();
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto register)
        {
            if (await userManager.Users.AnyAsync(x => x.Email == register.Email))
            {
                return BadRequest("Email Taken");
            }
            if (await userManager.Users.AnyAsync(x => x.UserName == register.Username))
            {
                return BadRequest("Username Taken");
            }
            var user = new AppUser
            {
                DisplayName = register.DisplayName,
                Email = register.Email,
                UserName = register.Username
            };
            var result = await userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                return CreateUserObject(user);

            }
            return BadRequest("problem registering");
        }
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            return CreateUserObject(user);
        }
        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }

}

