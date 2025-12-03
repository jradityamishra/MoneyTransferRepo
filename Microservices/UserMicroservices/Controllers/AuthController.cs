using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserMicroservices.Data;
using UserMicroservices.Data.Model;
using UserMicroservices.Data.Model.Entity;
using UserMicroservices.Data.ViewModel.Authentication;

namespace UserMicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            DatabaseContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult TestEndpoint()
        {
            return Ok("Authentication Microservice is working!");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO payload)
        {
            var userExists = await _userManager.FindByNameAsync(payload.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });
            
            User user = new User()
            {
                Email = payload.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = payload.Username,
            };
            
            var result = await _userManager.CreateAsync(user, payload.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            // Automatically assign "User" role to all new registrations
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            return Ok(new { Status = "Success", Message = "User registered successfully, please Login!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                var token = await GenerateJwtToken(user);
                return Ok(new { token, user });
            }
            
            return Unauthorized(new { Status = "Error", Message = "Invalid email or password" });
        }

        [HttpGet("check-user/{id}")]
        public async Task<IActionResult> CheckUserExists(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            bool exists = user != null;
            return Ok(new {user });
        }

        private async Task<AuthResultDTO> GenerateJwtToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
              
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigninKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(1440),
                claims: authClaims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigninKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            var response = new AuthResultDTO()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = token.ValidTo
            };
            return response;
        }
    }
}
