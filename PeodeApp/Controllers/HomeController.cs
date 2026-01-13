using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PeodeApp.Data;
using PeodeApp.Models;
using System.Diagnostics;

namespace PeodeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Tanan(Kylaline kylaline)
        {
            return View(kylaline);
        }

        [HttpGet]
        public IActionResult Ankeet()
        {
            var pyhad = _context.Pyhad.ToList();
            ViewBag.Pyhad = new SelectList(pyhad, "ID", "Nimi");
            return View();
        }

        [HttpPost]
        public IActionResult Ankeet(Kylaline kylaline)
        {
            if(ModelState.IsValid)
            {
                _context.Kylalined.Add(kylaline);
                _context.SaveChanges();
                return RedirectToAction("Tanan", kylaline);
            }

            return View(kylaline);
        }
    }
}
