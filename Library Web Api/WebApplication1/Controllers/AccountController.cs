using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models.DTO.AccountDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUIDTO register, string Role)
        {
            IdentityUser newUser = new IdentityUser();
            newUser.UserName = register.UserName;
            newUser.Email = register.Email;
            var result = await _userManager.CreateAsync(newUser, register.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, Role);
                return StatusCode(StatusCodes.Status200OK, newUser.UserName);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateUIDTO role)
        {
            var r = await _roleManager.FindByNameAsync(role.RoleName);
            if(r == null)
            {
                IdentityRole newRole = new IdentityRole();
                newRole.Name = role.RoleName;
                await _roleManager.CreateAsync(newRole);
                return Ok(newRole.Name);
            }
            else
            {
                return BadRequest("This role has already been created");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRoleToUser(AddRoleToUserUIDTO user)
        {
            var u = await _userManager.FindByEmailAsync(user.Email);
            if(u != null)
            {
                await _userManager.AddToRoleAsync(u, user.Role);
                return StatusCode(StatusCodes.Status200OK,u.UserName);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUIDTO user)
        {
            var u = await _userManager.FindByEmailAsync(user.Email);
            if(u != null)
            {
                var Result = await _signInManager.PasswordSignInAsync(u, user.Password, true, false);
                if (Result.Succeeded)
                {
                    return Ok(u.UserName);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return NotFound(user.Email);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPut]
        public async Task<IActionResult> ChangeInfromation(UpdateInforamtionUIDTO user)
        {
            var u = await _userManager.FindByEmailAsync(user.Email);
            if(u != null)
            {
                var result = await _userManager.CheckPasswordAsync(u, user.OldPassword);
                if (result)
                {
                    u.UserName = user.NewUserName;
                    await _userManager.ChangePasswordAsync(u,user.OldPassword,user.NewPassword);
                    return Ok(u.Email);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(DeleteUserUIDTO user)
        {
            var u = await _userManager.FindByEmailAsync(user.Email);
            if(u != null)
            {
                await _userManager.DeleteAsync(u);
                return StatusCode(StatusCodes.Status200OK, u.Email);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }
    }
}
