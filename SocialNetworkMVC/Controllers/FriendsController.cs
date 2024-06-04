using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;
using System.Security.Claims;

namespace SocialNetworkMVC.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private SocialNetworkMVCDbContext context;

        public FriendsController(SocialNetworkMVCDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this.context.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);
            var friend = await this.context.Users.FindAsync(friendId);

            if (user == null || friend == null)
                return NotFound("User or friend not found.");

            if (user.Friends.Contains(friend))
                return BadRequest("Already friends.");

            user.Friends.Add(friend);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this.context.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);
            var friend = await this.context.Users.FindAsync(friendId);

            if (user == null || friend == null)
                return NotFound("User or friend not found.");

            if (!user.Friends.Contains(friend))
                return BadRequest("Not friends.");

            user.Friends.Remove(friend);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this.context.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            ViewBag.Friends = user.Friends;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
                return RedirectToAction(nameof(Index));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this.context.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            var users = await this.context.Users
                .Where(u => u.UserName.Contains(query) && u.Id != userId)
                .ToListAsync();

            ViewBag.SearchQuery = query;
            ViewBag.IsFriend = user.Friends.ToDictionary(f => f.Id, f => true);

            return View("Index", users);
        }
    }
}
