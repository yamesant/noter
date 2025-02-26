using Bogus;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Noter.DataImporter;
using Noter.Domain;
using ShellProgressBar;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
string connectionString = config.GetConnectionString("DataContextConnection")!;
DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
    .UseSqlServer(connectionString)
    .Options;
DataContext context = new(options);

Randomizer.Seed = new Random(-1);
UserFaker userFaker = new();
NoteFaker noteFaker = new();
int numberOfUsers = 50;
int averageNotesPerUser = 10;
int chunkSize = 10;
int numberOfChunks = numberOfUsers / chunkSize;

if (context.Users.Any() || context.Notes.Any())
{
    Console.WriteLine("Already generated");
    return;
}

using ProgressBar progressBar = new(numberOfChunks, "Generating and importing users and notes");
for (int i = 0; i < numberOfChunks; i++)
{
    List<User> users = userFaker.Generate(chunkSize);
    foreach (User user in users)
    {
        int numberOfNotes = Randomizer.Seed.Next(0, 2*averageNotesPerUser);
        List<Note> notes = noteFaker.Generate(numberOfNotes);
        user.Notes = notes;
    }

    context.BulkInsert(users, new BulkConfig { IncludeGraph = true });
    progressBar.Tick();
}