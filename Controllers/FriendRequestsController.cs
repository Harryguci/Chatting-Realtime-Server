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
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendRequestsController : ControllerBase
    {
        private readonly ChatingContext _context;
        private readonly AppSettings _appSettings;

        public FriendRequestsController(ChatingContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        // GET: api/FriendRequests
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetFriendRequests()
        {
            var currentUser = HttpContext.Items["User"] as Account;
            if (currentUser == null)
            {
                return BadRequest();
            }

            return await _context.FriendRequests
                .Where(p => ((p.Accepted == null || p.Accepted.Value == false) && p.User2 == currentUser.Username))
                .OrderByDescending(p => p.CreateAt)
                .ToListAsync();
        }

        [HttpGet("Size")]
        [Authorize]
        public ActionResult GetFriendRequestNumber()
        {
            var currentUser = HttpContext.Items["User"] as Account;
            if (currentUser == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                size = _context.FriendRequests
                .Where(p => ((p.Accepted == null || p.Accepted.Value == false) 
                        && p.User2 == currentUser.Username))
                .Count()
            });
        }

        // GET: api/FriendRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FriendRequest>> GetFriendRequest(string id)
        {
            var friendRequest = await _context.FriendRequests.FindAsync(id);

            if (friendRequest == null)
            {
                return NotFound();
            }

            return friendRequest;
        }

        // PUT: api/FriendRequests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriendRequest(string id, FriendRequest friendRequest)
        {
            if (id != friendRequest.Id)
            {
                return BadRequest();
            }

            _context.Entry(friendRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (friendRequest.Accepted == true)
            {

                using StringContent jsonContent = new(
                            JsonSerializer.Serialize(new
                            {
                                id = "",
                                user1 = friendRequest.User1,
                                user2 = friendRequest.User2,
                                type = "friend"
                            }),
                            Encoding.UTF8,
                            "application/json");

                using HttpResponseMessage response = await new HttpClient().PostAsync($"{_appSettings.URI}/api/Relationships", jsonContent);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Console.WriteLine($"{jsonResponse}\n");
            }

            return NoContent();
        }

        // POST: api/FriendRequests
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FriendRequest>> PostFriendRequest(FriendRequest friendRequest)
        {
            var currentUser = HttpContext.Items["User"] as Account;
            if (friendRequest.User1 != currentUser?.Username)
                return BadRequest(new { error = "Bạn không có quyền thực hiện" });

            var num = _context.FriendRequests.Count();
            var id = $"fr{num}";
            friendRequest.Id = id;

            _context.FriendRequests.Add(friendRequest);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FriendRequestExists(friendRequest.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFriendRequest", new { id = friendRequest.Id }, friendRequest);
        }

        // DELETE: api/FriendRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendRequest(string id)
        {
            var friendRequest = await _context.FriendRequests.FindAsync(id);
            if (friendRequest == null)
            {
                return NotFound();
            }

            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FriendRequestExists(string id)
        {
            return _context.FriendRequests.Any(e => e.Id == id);
        }
    }
}
