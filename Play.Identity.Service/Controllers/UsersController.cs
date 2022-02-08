using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Play.Identity.Service.Areas.Identity.Entites;
using Play.Identity.Service.DTOs;
using static IdentityServer4.IdentityServerConstants;

namespace Play.Identity.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy= LocalApi.PolicyName)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> Get()
        {
            var users = userManager.Users
                .ToList()
                .Select(u => u.AsDTO());

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetByIdAsync(Guid id)
        {
            if(id == Guid.Empty)
                return BadRequest();

            var user = await userManager.FindByIdAsync(id.ToString());

            if(user == null)
                return NotFound();

            return Ok(user.AsDTO());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateUserDTO updateData)
        {
            if(id == Guid.Empty)
                return BadRequest();

            var user = await userManager.FindByIdAsync(id.ToString());

            if(user == null)
                return NotFound();

            user.Email = updateData.Email;
            user.UserName = updateData.Email;
            user.NormalizedEmail = updateData.Email.ToUpperInvariant();
            user.Gil = updateData.Gil;

            await userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            if(id == Guid.Empty)
                return BadRequest();

            var user = await userManager.FindByIdAsync(id.ToString());

            if(user == null)
                return NotFound();

            await userManager.DeleteAsync(user);

            return NoContent();
        }
    }
}