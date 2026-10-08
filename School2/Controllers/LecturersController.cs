using Microsoft.AspNetCore.Mvc;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.Controllers;

public class LecturersController : Controller
{
    private readonly School2Context _context;
    private readonly ILecturersServices _lecturersServices;
    private readonly IFilesServices _filesServices;

    public LecturersController(
        School2Context context, 
        ILecturersServices lecturersServices,
        IFilesServices filesServices)
    {
        _context = context;
        _lecturersServices = lecturersServices;
        _filesServices = filesServices;
    }

    public IActionResult Index()
    {
        return View();
    }
}