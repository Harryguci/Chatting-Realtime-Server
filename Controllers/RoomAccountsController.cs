using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatingApp.Context;
using ChatingApp.Models;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomAccountsController : ControllerBase
    {
        private readonly ChatingContext _context;

        public RoomAccountsController(ChatingContext context)
        {
            _context = context;
        }

        // GET: api/RoomAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomAccount>>> GetRoomAccounts()
        {
            return await _context.RoomAccounts.ToListAsync();
        }

        [HttpGet("Friend")]
        public ActionResult GetRoomFriend(string user1, string user2)
        {
            var query = _context.RoomAccounts.Where(p => p.Username == user1).Select(p => new RoomAccount() { Id = p.Id, RoomId = p.RoomId }).ToList();
            var query2 = _context.RoomAccounts.Where(p => p.Username == user2).Select(p => new RoomAccount() { Id = p.Id, RoomId = p.RoomId }).ToList();

            foreach (var item in query)
            {
                if (query2.Any(p => p.RoomId == item.RoomId))
                {
                    var result = _context.Rooms.Find(item.RoomId);

                    if (result != null && result.Type.ToLower() == "friend")
                        return Ok(new
                        {
                            id = result.Id,
                            type = result.Type,
                        });
                }
            }

            return BadRequest(new { error = "Can not found the room" });
        }

        // GET: api/RoomAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomAccount>> GetRoomAccount(int id)
        {
            var roomAccount = await _context.RoomAccounts.FindAsync(id);

            if (roomAccount == null)
            {
                return NotFound();
            }

            return roomAccount;
        }

        // PUT: api/RoomAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomAccount(int id, RoomAccount roomAccount)
        {
            if (id != roomAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomAccountExists(id))
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

        // POST: api/RoomAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomAccount>> PostRoomAccount(RoomAccount roomAccount)
        {

            //roomAccount.Room = await _context.Rooms.FindAsync(roomAccount.RoomId);
            //roomAccount.UsernameNavigation = await _context.Accounts
            //    .Where(p => p.Username == roomAccount.Username)
            //    .FirstOrDefaultAsync();

            _context.RoomAccounts.Add(roomAccount);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RoomAccountExists(roomAccount.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRoomAccount", new { id = roomAccount.Id }, roomAccount);
        }

        // DELETE: api/RoomAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomAccount(int id)
        {
            var roomAccount = await _context.RoomAccounts.FindAsync(id);
            if (roomAccount == null)
            {
                return NotFound();
            }

            _context.RoomAccounts.Remove(roomAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomAccountExists(int id)
        {
            return _context.RoomAccounts.Any(e => e.Id == id);
        }
    }
}
