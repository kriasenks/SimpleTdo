using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTdo.Contracts;
using SimpleTdo.DataAccess;
using SimpleTdo.Models;
using System.Linq.Expressions;
using System.Security.Claims;


namespace SimpleTdo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly NotesDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotesController(NotesDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim.Value);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            var note = new Note(request.Title, request.Description, userId);

            await _dbContext.Notes.AddAsync(note, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetNotesRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            var notesQuery = _dbContext.Notes
                .Where(n => n.UserId == userId &&
                            (string.IsNullOrWhiteSpace(request.search) ||
                             n.Title.ToLower().Contains(request.search.ToLower())));

            Expression<Func<Note, object>> selectorKey = request.sortItem?.ToLower() switch
            {
                "date" => note => note.CreatedAt,
                "title" => note => note.Title,
                _ => note => note.Id
            };

            notesQuery = request.sortOrder == "desc"
                ? notesQuery.OrderByDescending(selectorKey)
                : notesQuery.OrderBy(selectorKey);

            var noteDtos = await notesQuery
                .Select(n => new NoteDto(n.Id, n.Title, n.Description, n.CreatedAt))
                .ToListAsync(cancellationToken: ct);

            return Ok(new GetNotesResponse(noteDtos));
        }
    }
}