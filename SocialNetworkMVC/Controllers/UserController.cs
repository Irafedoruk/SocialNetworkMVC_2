using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;

namespace SocialNetworkMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private SocialNetworkMVCDbContext context;
        public UserController(SocialNetworkMVCDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = await this.context.Users
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
