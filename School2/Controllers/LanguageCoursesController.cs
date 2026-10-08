using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.Models.LanguageCourses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace School2.Controllers
{
    public class LanguageCoursesController : Controller
    {
        private readonly School2Context _context;
        private readonly ILanguageCoursesServices _languageCoursesServices;

        public LanguageCoursesController(School2Context context, ILanguageCoursesServices languageCoursesServices)
        {
            _context = context;
            _languageCoursesServices = languageCoursesServices;
        }
        public IActionResult Index()
        {
            var result = _context.LanguageCourses
                .Select(x => new LanguageCourseViewModel
                {
                    Id = x.Id,
                    Nimetus = x.Nimetus,
                    Keel = x.Keel
                }).Take(20);
            
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LanguageCourseCreateUpdateViewModel vm = new();
            return View("CreateUpdate", vm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(LanguageCourseViewModel vm) 
        {
            //kontrollime et vm ei oleks null
            if (vm == null)
            {
                return RedirectToAction("Error", "Home");
            }
            //kontrollime et vmi modelstate on õige
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            //teeme uue DTO-objekti
            //asetame dtosse vmi andmed
            var dto = new LanguageCourseDTO() 
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus
            };
            //teostatakse päring teenusele
            var result = await _languageCoursesServices.Create(dto);
            //teenus peab objekti tagastama
            //kontrollime kas tagastatud objekt on null
            if (result == null)
            {
                //  kui on, suuname vealehele
                return RedirectToAction("Error", "Home");
            }
            else
            {
                //  kui ei, suuname tagasi indeksisse
                return RedirectToAction(nameof(Index));
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var result = await _languageCoursesServices.DetailsAsync(id);
            if (result is null)
                return NotFound();

            var vm = new LanguageCourseViewModel
            {
                Id = result.Id,
                Kirjeldus = result.Kirjeldus,
                Keel = result.Keel,
                Tase = result.Tase
            };
            
            ViewData["ViewType"] = "details";

            return View("DetailsDelete", vm);
        }        
        
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var result = await _languageCoursesServices.DetailsAsync(id);
            if (result is null)
                return NotFound();

            var vm = new LanguageCourseViewModel
            {
                Id = result.Id,
                Kirjeldus = result.Kirjeldus,
                Keel = result.Keel,
                Tase = result.Tase
            };
            
            ViewData["ViewType"] = "delete";

            return View("DetailsDelete", vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var courseToDelete = await _languageCoursesServices.Delete(id);
            if (courseToDelete is null)
                return NotFound();
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();
            
            var courseToUpdate = await _languageCoursesServices.DetailsAsync(id);
            if (courseToUpdate is null)
                return NotFound();
            
            var vm = new LanguageCourseCreateUpdateViewModel();
            vm.Id = courseToUpdate.Id;
            vm.Kirjeldus = courseToUpdate.Kirjeldus;
            vm.Nimetus = courseToUpdate.Nimetus;
            vm.Keel = courseToUpdate.Keel;
            vm.Tase = courseToUpdate.Tase;
            vm.CreatedAt = courseToUpdate.CreatedAt;
            vm.ModifiedAt = courseToUpdate.ModifiedAt;
            
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(LanguageCourseCreateUpdateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("CreateUpdate", vm);

            var dto = new LanguageCourseDTO()
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Kirjeldus = vm.Kirjeldus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                CreatedAt = vm.CreatedAt,
                ModifiedAt = vm.ModifiedAt
            };
            
            var result = await _languageCoursesServices.Update(dto);
            var resultId = result.Id;
            if (result is null)
                return RedirectToAction(nameof(Index));
            
            return RedirectToAction(nameof(Update), new { id = resultId });
        }
    }
}
