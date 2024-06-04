using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;
using SocialNetworkMVC.Data.Entities;
using System.Security.Claims;

namespace SocialNetworkMVC.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private SocialNetworkMVCDbContext context;
        private IWebHostEnvironment webHostEnvironment;
        public GalleryController(SocialNetworkMVCDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photos = await this.context.Photos
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return View(photos);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var photo = new Photo
                {
                    UserId = userId,
                    DateAdded = DateTime.Now
                };

                // Отримати шлях до папки завантажень
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
                                
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Створити унікальне ім'я файлу
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                // Повний шлях до файлу
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Зберегти файл
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Зберегти інформацію про фото в базі даних
                photo.Url = "/uploads/" + uniqueFileName;
                context.Photos.Add(photo);
                await context.SaveChangesAsync();

                // Повернутися до галереї після завантаження фото
                return RedirectToAction("Index");
            }

            // Повернутися на сторінку завантаження фото, якщо файл не було обрано
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int photoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photo = await this.context.Photos.FirstOrDefaultAsync(p => p.Id == photoId && p.UserId == userId);

            if (photo != null)
            {
                // Видалення фото з сервера або з хмарового сховища
                var filePath = Path.Combine(webHostEnvironment.WebRootPath, photo.Url.Replace("/", "\\"));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Видалення фото з бази даних
                this.context.Photos.Remove(photo);
                await this.context.SaveChangesAsync();
            }

            // Повернення до галереї після видалення фото
            return RedirectToAction("Index");
        }

    }
}
