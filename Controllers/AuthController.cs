using ChatingApp.Context;
using ChatingApp.Helpers;
using ChatingApp.Models;
using ChatingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ChatingContext _context;
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        
        private static readonly string MySecret = "asdv234234^&%&^%&^hjsdfb2%%%";

        public AuthController(ChatingContext context, IUserService userService, IOptions<AppSettings> appSettings)
        {
            this._context = context;
            this._userService = userService;
            _appSettings = appSettings.Value;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([Bind("username,password")] AuthenticateRequest account)
        {
            var response = _userService.Authenticate(account);

            if (response == null)
                return BadRequest(new { message = "Your username or password is incorrect" });

            var currentUser = _context.Accounts.Where(p => p.Username == account.Username).FirstOrDefault();
           
            if (currentUser == null)
            {
                return BadRequest();
            }

            currentUser.LastLogin = null;

            HttpClient httpClient = new HttpClient();
            using StringContent jsonContent = new(
                           JsonSerializer.Serialize(currentUser),
                           Encoding.UTF8,
                           "application/json");

            using HttpResponseMessage response2 = await new HttpClient().PutAsync($"{_appSettings.URI}/api/Accounts/{currentUser.Id}", jsonContent);

            response2.EnsureSuccessStatusCode();

            var jsonResponse = await response2.Content.ReadAsStringAsync();

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

        [HttpPut("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var currentUser = HttpContext.Items["User"] as Account;
            if (currentUser == null)
            {
                return BadRequest();
            }

            currentUser.LastLogin = DateTime.Now;
        
            using StringContent jsonContent = new(
                           JsonSerializer.Serialize(currentUser),
                           Encoding.UTF8,
                           "application/json");

            using HttpResponseMessage response = await new HttpClient().PutAsync($"{_appSettings.URI}/api/Accounts/{currentUser.Id}", jsonContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return Ok(currentUser);
        }

        [HttpPut("Reconnect")]
        [Authorize]
        public async Task<IActionResult> Reconnect()
        {
            var currentUser = HttpContext.Items["User"] as Account;
            if (currentUser == null)
            {
                return BadRequest();
            }

            currentUser.LastLogin = null;

            using StringContent jsonContent = new(
                           JsonSerializer.Serialize(currentUser),
                           Encoding.UTF8,
                           "application/json");

            using HttpResponseMessage response = await new HttpClient().PutAsync($"{_appSettings.URI}/api/Accounts/{currentUser.Id}", jsonContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return Ok(currentUser);
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
