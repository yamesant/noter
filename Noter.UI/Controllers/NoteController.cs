using Microsoft.AspNetCore.Mvc;

namespace Noter.UI.Controllers;

public class NoteController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}