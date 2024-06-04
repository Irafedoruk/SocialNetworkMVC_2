using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;
using SocialNetworkMVC.Data.Entities;
using System.Security.Claims;

namespace SocialNetworkMVC.Controllers
{
    public class LikeController : Controller
    {
        private SocialNetworkMVCDbContext context;

        public LikeController(SocialNetworkMVCDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikePost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };
            this.context.Likes.Add(like);
            await this.context.SaveChangesAsync();
            return RedirectToAction("Details", "Post", new { id = postId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var like = await this.context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
            if (like != null)
            {
                this.context.Likes.Remove(like);
                await this.context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Post", new { id = postId });
        }
    }
}
