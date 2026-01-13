using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PeodeApp.Data;
using PeodeApp.Models;

namespace PeodeApp.Controllers
{
    public class KylalisedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KylalisedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kylalised
        public async Task<IActionResult> Index()
        {
            return View(await _context.Kylalined.ToListAsync());
        }

        // GET: Kylalised/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kylaline = await _context.Kylalined
                .FirstOrDefaultAsync(m => m.ID == id);
            if (kylaline == null)
            {
                return NotFound();
            }

            return View(kylaline);
        }

        // GET: Kylalised/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kylalised/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Nimi,Email,OnKutse,PyhaID")] Kylaline kylaline)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kylaline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kylaline);
        }

        // GET: Kylalised/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kylaline = await _context.Kylalined.FindAsync(id);
            if (kylaline == null)
            {
                return NotFound();
            }
            return View(kylaline);
        }

        // POST: Kylalised/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nimi,Email,OnKutse,PyhaID")] Kylaline kylaline)
        {
            if (id != kylaline.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kylaline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KylalineExists(kylaline.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(kylaline);
        }

        // GET: Kylalised/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kylaline = await _context.Kylalined
                .FirstOrDefaultAsync(m => m.ID == id);
            if (kylaline == null)
            {
                return NotFound();
            }

            return View(kylaline);
        }

        // POST: Kylalised/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kylaline = await _context.Kylalined.FindAsync(id);
            if (kylaline != null)
            {
                _context.Kylalined.Remove(kylaline);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KylalineExists(int id)
        {
            return _context.Kylalined.Any(e => e.ID == id);
        }
    }
}
