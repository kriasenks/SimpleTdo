namespace SimpleTdo.Contracts
{
    public record CreateNoteRequest(string Title, string Description);
    public record GetNotesRequest(string? search, string? sortItem, string? sortOrder);
    public record GetNotesResponse(List<NoteDto> notes);
    public record LoginRequest(string Username, string Password);
    public record NoteDto(Guid Id, string Title, string description, DateTime CreatedAt);
    public record RegisterRequest(string Username, string Password);
}
