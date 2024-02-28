using ChatingApp.Helpers;
using ChatingApp.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using New.Namespace;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ChatingContext _context;
        private readonly string MySecret = "asdv234234^&%&^%&^hjsdfb2%%%";

        public AuthController(ChatingContext context)
        {
            this._context = context;
        }

        [HttpPost("Login")]
        public IActionResult Login([Bind("username,password")] Account account)
        {
            var query = from t in _context.Accounts
                        where t.Username == account.Username
                        select t;
            var user = query.FirstOrDefault();

            if (user == null)
            {
                return NotFound(new
                {
                    error = $"User with username {account.Username} is not Found"
                });
            }

            // Hash
            // var hash = SecurePasswordHasher.Hash(account.Password);
            var isMatch = SecurePasswordHasher.Verify(account.Password, user.Password);
            if (!isMatch)
            {
                return ValidationProblem(new ValidationProblemDetails() { Detail = "The password is incorrect. Please try again" });
            }

            var token = GenerateToken(user);

            return Ok(new
            {
                accessToken = token
            });
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([Bind("username,password,roles")] Account account)
        {
            if (_context.Accounts.Any(p => p.Username == account.Username))
            {
                return BadRequest(new
                {
                    error = "username is already exist",
                });
            }

            var hashPass = SecurePasswordHasher.Hash(account.Password);

            account.Password = hashPass;

            _context.Accounts.Add(account);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }

            return Ok(account);
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securityToken == null) return "";

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }

        public string GenerateToken(Account account)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(MySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new Claim(ClaimTypes.UserData, account.Username.ToString()),
                    new Claim(ClaimTypes.Email, account.Emaill??"".ToString()),
                    new Claim("Role",value: account.Roles != null ? account.Roles.ToString() : "")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateCurrentToken(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(MySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
