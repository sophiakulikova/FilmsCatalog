using FilmsCatalog.Data;
using FilmsCatalog.Models;
using FilmsCatalog.Models.EFModels;
using FilmsCatalog.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using FilmsCatalog.Helpers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace FilmsCatalog.Controllers
{
    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly CustomConfig _customConfig;

        public FilmsController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment appEnvironment, CustomConfig customConfig)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
            _customConfig = customConfig;
        }

        public IActionResult Index(int? page)
        {
            var films = _context.Films.ToList();
            int pageSize = _customConfig.FilmsOnPage;
            int pageNumber = page ?? 1;

            var filmsVM = new List<FilmViewModel>();
            foreach (var film in films)
            {
                var filmVM = new FilmViewModel(film);
                filmsVM.Add(filmVM);
            }

            return View(filmsVM.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddFilm()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFilm(FilmViewModel filmVM)
        {
            var PosterExtensions = _customConfig.FileExtensions.ToList();
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Возникла ошибка! Пожалуйста, проверьте введенные данные";
                return View(filmVM);
            }

            var fileExtension = Path.GetExtension(filmVM.Poster.FileName);
            if (!PosterExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError(nameof(filmVM.Poster), "Недопустимый формат файла");
                return View(filmVM);
            }

            if (user == null)
            {
                TempData["Alert"] = "Пожалуйста, войдите в систему, чтобы добавить новый фильм";
                return RedirectToAction("Index");
            }

            string path = _customConfig.DirectoryPath + filmVM.Poster.FileName;


            var film = new Film()
            {
                Name = filmVM.Name,
                Description = filmVM.Description,
                ReleaseYear = filmVM.ReleaseYear,
                Producer = filmVM.Producer,
                Path = path,
                UserId = user.Id
            };

            _context.Films.Add(film);
            await _context.SaveChangesAsync();
            SavePoster(path, filmVM.Poster);

            TempData["Message"] = "Фильм был успешно добавлен в список";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditFilm(Guid? id)
        {
            var film = await _context.Films.FirstOrDefaultAsync(x => x.FilmId == id);
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null || !PermissionHelper.CheckFilmCreator(user, film))
            {
                TempData["Alert"] = "У вас нет прав редактировать этот фильм";
                return RedirectToAction("Index");
            }

            var model = new FilmViewModel(film);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditFilm(Guid? id, FilmViewModel filmVM)
        {
            var PosterExtensions = _customConfig.FileExtensions.ToList();
            if (filmVM.Poster != null)
            {
                var fileExtension = Path.GetExtension(filmVM.Poster.FileName);
                if (!PosterExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError(nameof(filmVM.Poster), "Недопустимый формат файла");
                    return View(filmVM);
                }
            }
            var film = _context.Films.FirstOrDefault(x => x.FilmId == id);

            if (filmVM.Poster != null)
            {
                string path = _customConfig.DirectoryPath + filmVM.Poster.FileName;
                film.Path = path;
                SavePoster(path, filmVM.Poster);
            }
                

            film.Name = filmVM.Name;
            film.Description = filmVM.Description;
            film.Producer = filmVM.Producer;
            film.ReleaseYear = filmVM.ReleaseYear;
            _context.Entry(film).State = EntityState.Modified;
            await _context.SaveChangesAsync();
                

            TempData["Message"] = "Фильм был успешно отредактирован";
            return RedirectToAction("Details", new { id  = film.FilmId});
                   
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {

            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var film = _context.Films.FirstOrDefault(x => x.FilmId == id);
            var model = new FilmViewModel(film);

            var userCanEdit = PermissionHelper.CheckFilmCreator(user, film);
            if (userCanEdit)
                ViewBag.UserCanEdit = true;
            else ViewBag.UserCanEdit = false;

            return View(model);
        }

        public void SavePoster(string path, IFormFile file)
        {
            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
        }
    }
}
