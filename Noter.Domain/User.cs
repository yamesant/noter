using Microsoft.AspNetCore.Identity;

namespace Noter.Domain;

public class User : IdentityUser
{
    public List<Note> Notes { get; set; } = null!;
}