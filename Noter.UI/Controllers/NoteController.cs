using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noter.Domain;
using Noter.UI.Models;

namespace Noter.UI.Controllers;

[Authorize]
public class NoteController(DataContext dataContext, UserManager<User> userManager) : Controller
{
    private const int PreviewMaxLength = 200;
    private const int PageSize = 100;
    private string UserId => userManager.GetUserId(User) ?? throw new UnauthorizedAccessException();
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
        
        int notesCount = await dataContext.Notes
            .Where(x => x.UserId == UserId)
            .CountAsync();
        int totalPages = notesCount == 0 ? 1 : (notesCount + PageSize - 1) / PageSize;
        if (pageNumber > totalPages)
        {
            return RedirectToAction("Page", new { pageNumber = totalPages });
        }

        List<NoteViewModels.Preview> notes = await dataContext.Notes
            .AsNoTracking()
            .Where(x => x.UserId == UserId)
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
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages,
        };
        
        return View(model);
    }
    
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        Note? note = await dataContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (note is null)
        {
            return NotFound();
        }

        NoteViewModels.Main model = new()
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
        };
        return View(model);
    }
    
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        Note? note = await dataContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (note is null)
        {
            return NotFound();
        }

        NoteViewModels.Edit model = new()
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
        };
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(NoteViewModels.Edit model)
    {
        if (model.Title is null || model.Title.Length == 0)
        {
            ModelState.AddModelError("Title", "Title is required");
            return View(model);
        }
        
        Note? note = await dataContext.Notes
            .FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == UserId);
        if (note is null)
        {
            return BadRequest();
        }

        Note? sameNameNote = await dataContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Title == model.Title && x.Id != model.Id);
        if (sameNameNote is not null)
        {
            ModelState.AddModelError("Title", "Note with this title already exists");
            return View(model);
        }

        note.Title = model.Title;
        note.Content = model.Content ?? "";
        await dataContext.SaveChangesAsync();
        return RedirectToAction("Details", new { id = note.Id });
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(NoteViewModels.Create model)
    {
        if (model.Title is null || model.Title.Length == 0)
        {
            ModelState.AddModelError("Title", "Title is required");
            return View(model);
        }

        Note? sameNameNote = await dataContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Title == model.Title);
        if (sameNameNote is not null)
        {
            ModelState.AddModelError("Title", "Note with this title already exists");
            return View(model);
        }

        Note note = new()
        {
            Title = model.Title,
            Content = model.Content ?? "",
            UserId = UserId,
        };
        dataContext.Notes.Add(note);
        await dataContext.SaveChangesAsync();
        return RedirectToAction("Details", new { id = note.Id });
    }
}