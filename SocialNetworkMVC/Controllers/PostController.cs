using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;
using SocialNetworkMVC.Data.Entities;
using System.Security.Claims;

namespace SocialNetworkMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private SocialNetworkMVCDbContext context;

        public PostController(SocialNetworkMVCDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = await context.Posts.Include(p => p.User)
                                           .Include(p => p.Comments).ThenInclude(c => c.User)
                                           .Include(p => p.Likes)
                                           .Where(p => p.UserId == userId)
                                           .ToListAsync();

            return View(posts);
        }              
                
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string text, string userId)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Post text cannot be empty.");
            }

            if (string.IsNullOrEmpty(userId) || userId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var user = await this.context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var post = new Post
            {
                Text = text,
                UserId = userId, // Задати автора поста
                UserProfileId = userId, // Задати користувача, на чиїй сторінці створено пост
                DatePublish= DateTime.Now
            };

            this.context.Posts.Add(post);
            await this.context.SaveChangesAsync();

            return RedirectToAction("UserProfile", "User", new { userId });
        }

        

        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                var dbPost = await context.Posts.FindAsync(post.Id);
                if (dbPost == null || dbPost.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Unauthorized("Unauthorized to edit this post.");
                }

                dbPost.Text = post.Text;
                context.Entry(dbPost).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return RedirectToAction("Index", "Profile");
            }
            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null || post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized("Unauthorized to delete this post.");
            }

            context.Posts.Remove(post);
            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Profile");
        }
    }
}
