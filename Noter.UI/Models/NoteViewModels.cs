namespace Noter.UI.Models;

public static class NoteViewModels
{
    public class Preview
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string ContentPreview { get; set; }
    }
}