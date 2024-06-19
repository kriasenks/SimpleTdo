using Microsoft.AspNetCore.Mvc;
using SimpleTdo.Contracts;
using SimpleTdo.DataAccess;
using SimpleTdo.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;


namespace SimpleTdo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly NotesDbContext _dbContext;
        public NotesController(NotesDbContext context)
        {
            _dbContext = context;
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateNoteRequest request, CancellationToken ct) 
        {
            var note = new Note(request.Title, request.Description);

            await _dbContext.Notes.AddAsync(note, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Ok();
        }
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetNotesRequest request, CancellationToken ct)
        {
            var notesQuery = _dbContext.Notes.Where(n => string.IsNullOrWhiteSpace(request.search) || n.Title.ToLower().Contains(request.search.ToLower()));

            Expression<Func<Note, object>> selectorKey = request.sortItem switch
            {
                "date" => note => note.CreatedAt,
                "title" => note => note.Title,
                _ => note => note.Id,
            };


            if (request.sortOrder == "desc")
            {
                notesQuery = notesQuery.OrderByDescending(selectorKey);
            }
            else
            {
                notesQuery = notesQuery.OrderBy(selectorKey);
            }

            var noteDtos = await notesQuery.Select(n => new NotesDto(n.Id, n.Title, n.Description, n.CreatedAt)).ToListAsync(cancellationToken: ct);

            return Ok(new GetNotesResponse(noteDtos));
        }
    }
}
