using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noter.Domain;
using Noter.UI.Models;

namespace Noter.UI.Controllers;

public class NoteController(DataContext dataContext) : Controller
{
    public async Task<IActionResult>Index()
    {
        List<NoteViewModels.Preview> notes = await dataContext.Notes
            .AsNoTracking()
            .Take(20)
            .Select(x => new NoteViewModels.Preview
            {
                Id = x.Id,
                Title = x.Title,
                ContentPreview = x.Content.Substring(0, 200),
            }).ToListAsync();
        return View(notes);
    }
}