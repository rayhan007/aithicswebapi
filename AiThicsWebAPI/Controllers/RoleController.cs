using aithics.data;
using aithics.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aithics.api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AithicsDbContext _context;

        public RoleController(AithicsDbContext context)
        {
            _context = context;
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            var user = await _context.Users.FindAsync(model.UserId);
            var role = await _context.Roles.FindAsync(model.RoleId);

            if (user == null || role == null)
                return BadRequest("Invalid User or Role");

            _context.UserRoles.Add(new UserRole { UserId = model.UserId, RoleId = model.RoleId });
            await _context.SaveChangesAsync();

            return Ok("Role assigned successfully");
        }
    }

    public class AssignRoleModel
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
}
