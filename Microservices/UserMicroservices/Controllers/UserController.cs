using Banking.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserMicroservices.Data.ViewModel.Authentication;
using UserMicroservices.Data.ViewModel.UserVM;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    public class UserController : ControllerBase
    {
        public UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET api/user/{id}
        [HttpGet("user-by-{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // GET api/user/by-email?email=...
        [HttpGet("user-by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email is required");
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST api/user
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] UserMicroservices.Data.ViewModel.UserVM.CreateUserVM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (succeeded, errors, dto) = await _userService.CreateUserAsync(model);
            if (!succeeded) return BadRequest(new { errors });

            return CreatedAtAction(nameof(GetUserById), new { id = dto.Id }, dto);
        }

        // PUT api/user/{id}
        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserMicroservices.Data.ViewModel.UserVM.UpdateUserVM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (succeeded, errors) = await _userService.UpdateUserAsync(id, model);
            if (!succeeded) return BadRequest(new { errors });
            return NoContent();
        }

        // DELETE api/user/{id}
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (succeeded, errors) = await _userService.DeleteUserAsync(id);
            if (!succeeded) return BadRequest(new { errors });
            return NoContent();
        }

        // POST api/user/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserMicroservices.Data.ViewModel.UserVM.ChangePasswordVM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (succeeded, errors) = await _userService.ChangePasswordAsync(model);
            if (!succeeded) return BadRequest(new { errors });
            return Ok(new { message = "Password changed" });
        }

        // POST api/user/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserMicroservices.Data.ViewModel.UserVM.ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (succeeded, errors) = await _userService.ResetPasswordAsync(model);
            if (!succeeded) return BadRequest(new { errors });
            return Ok(new { message = "Password reset" });
        }

        // POST api/user/assign-role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserMicroservices.Data.ViewModel.UserVM.AssignRoleVM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (succeeded, errors) = await _userService.AssignRoleAsync(model);
            if (!succeeded) return BadRequest(new { errors });
            return Ok(new { message = "Role assigned" });
        }

        // POST api/user/remove-role
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] UserMicroservices.Data.ViewModel.UserVM.AssignRoleVM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (succeeded, errors) = await _userService.RemoveRoleAsync(model);
            if (!succeeded) return BadRequest(new { errors });
            return Ok(new { message = "Role removed" });
        }

    }

}
