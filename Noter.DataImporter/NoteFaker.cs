using Bogus;
using Noter.Domain;

namespace Noter.DataImporter;

public sealed class NoteFaker : Faker<Note>
{
    public NoteFaker()
    {
        StrictMode(true);
        
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.Title, f => f.UniqueIndex + f.Lorem.Sentence());
        RuleFor(x => x.Content, f => f.Lorem.Paragraphs(f.Random.Number(0, 20)));
        
        // Navigation properties
        RuleFor(x => x.UserId, _ => "");
        RuleFor(x => x.User, _ => null!);
    }
}