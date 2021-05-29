using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController: BaseApiController
    {
        private readonly UserManager<AppUser> _userManager ;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRole)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select( u => new {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRole.Select(r => r.Role.Name).ToList()
                }).ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRole = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);
            
            if(user == null) return NotFound("Could not found user");
            
            var userRole =  await _userManager.GetRolesAsync(user);
            
            var result = await _userManager.AddToRolesAsync (user, selectedRole.Except(userRole));

            if(!result.Succeeded) return BadRequest("Failed to add a role");

            result = await _userManager.RemoveFromRolesAsync(user,userRole.Except(selectedRole));

            if(!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));

        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photo-to-moderate")]
        public ActionResult GetPhotoForModeration()
        {
            return Ok("Admin and Moderators can see this");
        }
        
    }
}