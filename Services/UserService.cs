using ChatingApp.Context;
using ChatingApp.Helpers;
using ChatingApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatingApp.Services
{
    public interface IUserService
    {
        AuthenticateResponse? Authenticate(AuthenticateRequest model);
        IEnumerable<Account> GetAll();
        Account? GetById(string id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private ChatingContext _context;

        //private List<Account> _users = new List<Account> {
        //        new Account { Id = "AC$00000", Username = "User", Email = "test", Password = "test", Roles = "user" }};

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings, ChatingContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public AuthenticateResponse? Authenticate(AuthenticateRequest model)
        {
            var user = _context.Accounts.Where(x => x.Username == model.Username).FirstOrDefault();


            // return null if user not found
            if (user == null) return null;

            var isMatch = SecurePasswordHasher.Verify(model.Password, user.Password);

            if (!isMatch) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public Account? GetById(string id)
        {
            return _context.Accounts.Where(x => x.Id == id).FirstOrDefault();
        }

        // helper methods

        private string generateJwtToken(Account user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username.ToString()),
                    new Claim("email", user.Email ?? ""),
                    new Claim("roles", user.Roles)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
