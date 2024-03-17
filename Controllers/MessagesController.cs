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

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ChatingContext _context;

        public MessagesController(ChatingContext context)
        {
            _context = context;
        }

        // GET: api/Messages
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            var query = _context.Messages.OrderBy(p => p.CreateAt);

            return await query.ToListAsync();
        }

        [HttpGet("RoomId/{roomId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Message>>> GetFromRoomId(string roomId, int? limit)
        {
            if (limit == null) limit = 10;
            var query = _context.Messages
                .Where(p => p.RoomId == roomId);

            var size = query.Count();

            return await query
                .OrderBy(p => p.CreateAt)
                .Skip(Math.Max(0, size - limit.Value))
                .ToListAsync();
        }

        [HttpGet("Count")]
        public ActionResult Count([Bind("roomId")] string roomId)
        {
            int count = _context.Messages.Where(x => x.RoomId == roomId).Count();

            return Ok(new { count = count });
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(string id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(string id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage([Bind("id,username,content,roomId")] Message message)
        {
            int num = _context.Messages.Where(p => p.RoomId == message.RoomId).Count();
            string id = $"{message.RoomId}_${num}";

            while (MessageExists(id))
            {
                num++;
                id = $"{message.RoomId}_${num}";
            }

            message.Id = id;
            message.CreateAt = DateTime.Now;

            _context.Messages.Add(message);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MessageExists(message.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("All")]
        public async Task<IActionResult> DeleteAll()
        {
            var query = await _context.Messages.Where(p => p.CreateAt == null).ToListAsync();
            if (query == null)
            {
                return NotFound();
            }
            _context.Messages.RemoveRange(query);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool MessageExists(string id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
