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
NoteFaker faker = new();
int numberOfEntities = 100_000;
int chunkSize = 10_000;
int numberOfChunks = numberOfEntities / chunkSize;

if (context.Notes.Any())
{
    Console.WriteLine("Already generated");
    return;
}

using ProgressBar progressBar = new(numberOfChunks, "Generating and importing notes");
for (int i = 0; i < numberOfChunks; i++)
{
    List<Note> entities = faker.Generate(chunkSize);
    context.BulkInsert(entities);
    progressBar.Tick();
}