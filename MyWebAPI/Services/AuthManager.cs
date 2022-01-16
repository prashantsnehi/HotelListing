using System.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyWebAPI.Data;
using MyWebAPI.Models;

namespace MyWebAPI.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _config;
        private ApiUser _user;

        public AuthManager(UserManager<ApiUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<string> CreateToken()
        {
            var signInCredentials = GetSignInCredentials();
            var claims = await GetClaims();
            var tokenOptions = GetTokenOptions(signInCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private JwtSecurityToken GetTokenOptions(SigningCredentials signInCredentials, List<Claim> claims)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var expiration = DateTime.Now.AddDays(Convert.ToDouble(jwtSettings.GetSection("LifeTime").Value));
            
            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("ValidIssuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signInCredentials
                );

            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSignInCredentials()
        {
            var key = "this is my secret key";
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.Sha256);
        }

        public async Task<bool> ValidateUser(LoginDTO model)
        {
            _user = await _userManager.FindByNameAsync(model.Email);
            return (_user != null && await _userManager.CheckPasswordAsync(_user, model.Password));
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, "MyWebAPI", "RefreshToken");
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user,"MyWebAPI", "RefreshToken");
            var result = await _userManager.SetAuthenticationTokenAsync(_user, "MyWebAPI", "RefreshToken", newRefreshToken);
            return newRefreshToken;
        }

        public async Task<TokenRequest> VerifyRefreshToken(TokenRequest request)
        {
            // JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == ClaimTypes.Name).Value;
            _user = await _userManager.FindByNameAsync(username);
            try
            {
                var isValid = await _userManager.VerifyUserTokenAsync(_user, "MyWebAPI", "RefreshToken", request.RefreshToken);
                if(isValid) return new TokenRequest { Token = await CreateToken(), RefreshToken = await CreateRefreshToken() };

                await _userManager.UpdateSecurityStampAsync(_user);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return null;
        }
    }
}
