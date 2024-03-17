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
    public class RelationshipsController : ControllerBase
    {
        private readonly ChatingContext _context;

        public RelationshipsController(ChatingContext context)
        {
            _context = context;
        }

        // GET: api/Relationships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Relationship>>> GetRelationships()
        {
            return await _context.Relationships.ToListAsync();
        }

        // GET: api/Relationships/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Relationship>> GetRelationship(string id)
        {
            var relationship = await _context.Relationships.FindAsync(id);

            if (relationship == null)
            {
                return NotFound();
            }

            return relationship;
        }

        // PUT: api/Relationships/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRelationship(string id, Relationship relationship)
        {
            if (id != relationship.Id)
            {
                return BadRequest();
            }

            _context.Entry(relationship).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelationshipExists(id))
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

        // POST: api/Relationships
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Relationship>> PostRelationship(Relationship relationship)
        {
            _context.Relationships.Add(relationship);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RelationshipExists(relationship.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            Room roomChat = new Room() { Id = $"room{_context.Rooms.Count() + 1}", RoomAccounts = [], Type = "friend" };
            var num = _context.RoomAccounts.Count();

            RoomAccount roomUser1 = new RoomAccount()
            {
                Id = ++num,
                Username = relationship.User1,
                RoomId = roomChat.Id,
                Room = roomChat,
            };

            RoomAccount roomUser2 = new RoomAccount()
            {
                Id = ++num,
                Username = relationship.User2,
                RoomId = roomChat.Id,
                Room = roomChat,
            };

            roomChat.RoomAccounts.Add(roomUser1);
            roomChat.RoomAccounts.Add(roomUser2);

            _context.Rooms.Add(roomChat);
            _context.RoomAccounts.Add(roomUser1);
            _context.RoomAccounts.Add(roomUser2);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (RelationshipExists(relationship.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRelationship", new { id = relationship.Id }, relationship);
        }

        // DELETE: api/Relationships/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRelationship(string id)
        {
            var relationship = await _context.Relationships.FindAsync(id);
            if (relationship == null)
            {
                return NotFound();
            }

            _context.Relationships.Remove(relationship);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RelationshipExists(string id)
        {
            return _context.Relationships.Any(e => e.Id == id);
        }
    }
}
