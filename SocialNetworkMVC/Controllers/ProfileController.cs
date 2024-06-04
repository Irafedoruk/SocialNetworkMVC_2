using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;
using System.Security.Claims;

namespace SocialNetworkMVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private SocialNetworkMVCDbContext context;

        public ProfileController(SocialNetworkMVCDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = await this.context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .ToListAsync();

            return View(posts);
        }
    }
}
