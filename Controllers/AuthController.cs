using ChatingApp.Context;
using ChatingApp.Helpers;
using ChatingApp.Models;
using ChatingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        private IUserService _userService;
        
        private static readonly string MySecret = "asdv234234^&%&^%&^hjsdfb2%%%";

        public AuthController(ChatingContext context, IUserService userService)
        {
            this._context = context;
            this._userService = userService;
        }

        [HttpPost("Login")]
        public IActionResult Login([Bind("username,password")] AuthenticateRequest account)
        {
            var response = _userService.Authenticate(account);

            if (response == null)
                return BadRequest(new { message = "Your username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([Bind("id,username,password,roles")] Account account)
        {
            // AC$000000
            var number = _context.Accounts.Count();
            var id = $"AC${number}";

            while(isAccountExist(id))
            {
                number++;
                id = $"AC${number}";
            }
            account.Id = id;

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

        [NonAction]
        public bool isAccountExist(string id)
        {
            return _context.Accounts.Any(p => p.Id == id);
        }

        [NonAction]
        public static string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securityToken == null) return "";

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }

        [NonAction]
        public static List<Claim?> GetAllClaim(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securityToken == null) return new List<Claim?>();

            var stringClaimValue = securityToken.Claims.ToList();
            return stringClaimValue;
        }

        [NonAction]
        public static string GenerateToken(Account account)
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
                    new Claim("Username", account.Username.ToString()),
                    new Claim(ClaimTypes.Email, account.Email??"".ToString()),
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

        [NonAction]
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
