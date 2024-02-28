using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatingApp.Models;
using New.Namespace;
using Microsoft.IdentityModel.Tokens;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ChatingContext _context;
        private readonly int DefaultQuatify = 10;

        public MessagesController(ChatingContext context)
        {
            _context = context;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(string? username, string? friend, int? limits, int? pageIndex)
        {
            limits = limits ?? DefaultQuatify;
            pageIndex = pageIndex ?? 1;

            var query = _context.Messages.Select(x => x);

            if (!username.IsNullOrEmpty() || !friend.IsNullOrEmpty())
            {
                query = query.Where(
                    x => (x.Username == username && x.Friendusername == friend)
                    || (x.Username == friend && x.Friendusername == username));
            }

            int skipQualify = Math.Max(query.Count(), limits.Value) - limits.Value;

            return await query
                .Skip(skipQualify)
                .Take(limits.Value)
                .OrderBy(p => p.CreateAt)
                .ToListAsync();
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
        public async Task<ActionResult<Message>> PostMessage([Bind("username,friendUsername,content")] Message message)
        {
            var id = $"{message.Username}_{message.Friendusername}_{_context.Messages.Count()}";
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

        private bool MessageExists(string id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
