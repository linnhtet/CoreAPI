using CoreAPI.Domain;
using CoreAPI.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public class IdentityServices : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        public IdentityServices(UserManager<IdentityUser> userManager,JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }



        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with same email address already exists." }
                };
            }

            var newUser = new IdentityUser() { Email = email, UserName = email };
            var createdUser = await _userManager.CreateAsync(newUser, password);
            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return GenerateAuthencationResultForUser(newUser);
        }
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exist." }
                };
            }

            var userWithValidPassowrd = await _userManager.CheckPasswordAsync(user, password);
            if (!userWithValidPassowrd)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User and Password combination is wrong." }
                };
            }

            return GenerateAuthencationResultForUser(user);
        }
        private AuthenticationResult GenerateAuthencationResultForUser(IdentityUser newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Sub,newUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,newUser.Email),
                    new Claim("id",newUser.Id)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticationResult { Success = true, Token = tokenHandler.WriteToken(token) };
        }

        
    }
}
