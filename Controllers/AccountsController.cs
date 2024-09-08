using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatingApp.Context;
using ChatingApp.Models;
using ChatingApp.Helpers;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ChatingContext _context;

        public AccountsController(ChatingContext context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            var user = HttpContext.Features.Get<IUserFeature>();

            if (user != null)
                Debug.WriteLine("[Context] " + user.Username);
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(string id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        [HttpGet("Find/{username}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Account>>> FindByUsername(string username, bool onlyFriend = true)
        {
            var currentUser = HttpContext.Items["User"] as Account;
            if (currentUser == null)
            {
                return BadRequest();
            }
            var query = _context.Relationships.Where(p => (p.User1 == currentUser.Username)).Select(p => p.User2);
            var query2 = _context.Relationships.Where(p => (p.User2 == currentUser.Username)).Select(p => p.User1);
            var queryFriends = _context.Accounts.Where(p => (query.Any(t => t == p.Username) || (query2.Any(t => t == p.Username))));
            queryFriends = queryFriends.Where(p => p.Username.ToLower().StartsWith(username));
            var friends = await queryFriends.ToListAsync();
            if (friends == null)
                return NotFound();
            else
                return Ok(new { result = friends, listFriend = friends });
        }

        [HttpGet("FindUser")]
        [Authorize]
        public ActionResult<IEnumerable<Account>> FindByQuery(string? username, string? email)
        {
            // var result = _context.Accounts.Where(p => p.Username.ToLower().StartsWith(username)).ToList();
            var queryResult = _context.Accounts.Select(p => new Account()
            {
                Id = p.Id,
                Username = p.Username,
                Email = p.Email
            });

            if (username != null) queryResult = queryResult.Where(p => p.Username.ToLower().StartsWith(username));
            if (email != null) queryResult = queryResult.Where(p => p.Email != null && p.Email.ToLower().StartsWith(email));
            var result = queryResult.ToList();

            var currentUser = HttpContext.Items["User"] as Account;
            var queryListFriend = _context.Relationships.Select(p => new Relationship()
            {
                Id = p.Id,
                User1 = p.User1,
                User2 = p.User2,
                Type = p.Type,
            });

            List<Relationship>? listFriend = null;

            if (currentUser != null)
            {
                if (username != null)
                {
                    queryListFriend = queryListFriend.Where(p => (p.User1.ToLower().StartsWith(username) && p.User2 == currentUser.Username)
                    || (p.User1 == currentUser.Username && p.User2.ToLower().StartsWith(username)));

                    listFriend = queryListFriend.ToList();
                }

            }

            if (result == null)
                return NotFound();
            else
                return Ok(new { result, listFriend = (listFriend != null ? listFriend : []) });
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(Guid id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount([Bind("id,username,password,email,roles")] Account account)
        {
            var t = _context.Accounts.Count();
            var num = t.ToString();

            for (var i = 0; i <= 5 - t.ToString().Length; i++)
                num = '0' + num;

            account.Id = Guid.NewGuid();
            account.Password = SecurePasswordHasher.Hash(account.Password);

            _context.Accounts.Add(account);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AccountExists(account.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(Guid id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
