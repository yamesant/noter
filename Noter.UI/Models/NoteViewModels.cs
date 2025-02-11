namespace Noter.UI.Models;

public static class NoteViewModels
{
    public class Preview
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string ContentPreview { get; set; }
    }
    
    public class Paginated
    {
        public required List<Preview> NotePreviews { get; set; }
        public required int PageNumber { get; set; }
        public required bool HasPreviousPage { get; set; }
        public required bool HasNextPage { get; set; }
    }
    
    public class Main
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
    
    public class Edit
    {
        public required Guid Id { get; set; }
        public required string? Title { get; set; }
        public required string? Content { get; set; }
    }
}