namespace SimpleTdo.Contracts
{
    public record GetNotesRequest(string? search, string? sortItem, string? sortOrder);
}
