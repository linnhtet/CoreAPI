using CoreAPI.Data;
using CoreAPI.Domain;
using CoreAPI.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly TokenValidationParameters  _tokenValidationParameters;
        private readonly DataContext _dataContext;
        public IdentityServices(UserManager<IdentityUser> userManager,JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters,DataContext dataContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dataContext = dataContext;
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

            return await GenerateAuthencationResultForUserAsync(newUser);
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

            return await GenerateAuthencationResultForUserAsync(user);
        }
        private async Task<AuthenticationResult> GenerateAuthencationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim("id",user.Id)
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _dataContext.RefreshTokens.AddAsync(refreshToken);
            await _dataContext.SaveChangesAsync();


            return new AuthenticationResult { Success = true, Token = tokenHandler.WriteToken(token),RefreshToken=refreshToken.Token };
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshtoken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken==null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Token" }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(c=>c.Type==JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc>DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This token hasn't expired yet." }
                };
            }

            var jti = validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var dbRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshtoken);
            if (dbRefreshToken==null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token does not exist." }
                };
            }

            if (DateTime.UtcNow>dbRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has expired." }
                };
            }
            if (dbRefreshToken.Invalidated)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has been invalidated." }
                };
            }
            if (dbRefreshToken.Used)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has been used." }
                };
            }
            if (dbRefreshToken.JwtId!=jti)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token does not match JWT." }
                };
            }

            dbRefreshToken.Used = true;
            _dataContext.RefreshTokens.Update(dbRefreshToken);
            await _dataContext.SaveChangesAsync();


            var userId = validatedToken.Claims.Single(c => c.Type =="id").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return await GenerateAuthencationResultForUserAsync(user);

        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null; 
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtsecurityToken) && 
                jwtsecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256);
        }
    }
}
