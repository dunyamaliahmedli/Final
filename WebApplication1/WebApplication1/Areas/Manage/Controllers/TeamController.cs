using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TeamController : Controller
    {
        private AppDbContext _context { get; }
        private IWebHostEnvironment _env { get; }
        public TeamController(AppDbContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var page = _context.Teams.ToList();
            return View(page);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Team team)
        {
            if (!ModelState.IsValid) return NotFound();
            if (team.Photo != null)
            {
                if (team.Photo.ContentType != "image/jpg" && team.Photo.ContentType != "image/png")
                {
                    ModelState.AddModelError("", "Fayll png ve ya jpg olmalidir");
                    return View(team);
                }
              
                string filename = team.Photo.FileName;
                if (filename.Length > 64)
                {
                    filename.Substring(filename.Length - 64, 64);

                }
                string newFileName = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath,"assets","img",newFileName);
                using (FileStream fs = new FileStream(path,FileMode.Create))
                {
                    team.Photo.CopyTo(fs);
                }
                team.Image = newFileName;
                _context.Teams.Add(team);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Delete(int Id) {
            var deleted = _context.Teams.Find(Id);
            if (deleted == null) return NotFound();
            _context.Teams.Remove(deleted);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
                }

    }
}
