using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PeodeApp.Data;
using PeodeApp.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

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
            if (kylaline == null)
                return NotFound();

            ViewBag.PyhaNimi = _context.Pyhad.Find(kylaline.PyhaID)?.Nimi;
            if (kylaline.OnKutse)
                ViewBag.Pilt = "kolmas.jpg";
            else
                ViewBag.Pilt = "close_box_red.png";

            SaadaEmail(kylaline, ViewBag.Pilt, ViewBag.PyhaNimi);

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
        [ValidateAntiForgeryToken]
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
        
        private void SaadaEmail(Kylaline kylaline, string pilt, string pyha)
        {
            string failiTee = Path.Combine(
                Directory.GetCurrentDirectory(), // For ASP.NET Core, adjust path
                "wwwroot",
                "images",
                pilt
            );

            try
            {
                string sisu;

                if (kylaline.OnKutse)
                {
                    sisu = $"Tere, {kylaline.Nimi}!<br/><br/>" +
                           $"Sinu registreerumine sündmusele <b>{pyha}</b> on salvestud.<br/>" +
                           "Lisasime kirjale ka sündmuse kutse. Ootame sind väga!<br/><br/>" +
                           "Kohtumiseni!";
                }
                else
                {
                    sisu = $"Tere, {kylaline.Nimi}!<br/><br/>" +
                           $"Sinu registreerumine sündmusele <b>{pyha}</b> on salvestud.<br/>" +
                           "Lisasime kirjale ka sündmuse kutse. Kahju, et sa ei saa tule peole!<br/><br/>" +
                           "Kõige head!";
                }

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress("nikitalitvinenko28@gmail.com", "Peofirma");
                    mail.To.Add(kylaline.Email);
                    mail.Subject = "Vastus: " + pyha;
                    mail.Body = sisu;
                    mail.IsBodyHtml = true;

                    if (System.IO.File.Exists(failiTee))
                        mail.Attachments.Add(new Attachment(failiTee));

                    using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("nikitalitvinenko28@gmail.com", "");
                        smtp.EnableSsl = true;

                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
