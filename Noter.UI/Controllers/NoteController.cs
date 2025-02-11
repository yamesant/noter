using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noter.Domain;
using Noter.UI.Models;

namespace Noter.UI.Controllers;

public class NoteController(DataContext dataContext) : Controller
{
    private const int PreviewMaxLength = 200;
    private const int PageSize = 100;
    public IActionResult Index()
    {
        return RedirectToAction("Page", new { pageNumber = 1 });
    }

    public async Task<IActionResult> Page(int pageNumber)
    {
        if (pageNumber < 1)
        {
            return RedirectToAction("Page", new { pageNumber = 1 });
        }

        int notesCount = await dataContext.Notes.CountAsync();
        int totalPages = notesCount == 0 ? 1 : (notesCount + PageSize - 1) / PageSize;
        if (pageNumber > totalPages)
        {
            return RedirectToAction("Page", new { pageNumber = totalPages });
        }

        List<NoteViewModels.Preview> notes = await dataContext.Notes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(x => new NoteViewModels.Preview
            {
                Id = x.Id,
                Title = x.Title,
                ContentPreview = x.Content.Substring(0, PreviewMaxLength),
            }).ToListAsync();
        NoteViewModels.Paginated model = new()
        {
            NotePreviews = notes,
            PageNumber = pageNumber,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages,
        };
        
        return View(model);
    }
}