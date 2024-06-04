using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;
using SocialNetworkMVC.Data.Entities;
using SocialNetworkMVC.Models;
using System.Security.Claims;

namespace SocialNetworkMVC.Controllers
{
    public class CommentController : Controller
    {
        private SocialNetworkMVCDbContext context;

        public CommentController(SocialNetworkMVCDbContext context)
        {
            this.context = context;
        }
        private bool CommentExists(int id)
        {
            return this.context.Comments.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Index()
        {
            var comments = await this.context.Comments.Include(c => c.User).ToListAsync();
            return View(comments);
        }
        public async Task<IActionResult> Details(int id)
        {
            var comment = await this.context.Comments.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound(new { error = "Comment not found" });
            }
            return View(comment);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Text = model.Text,
                    DatePublish = DateTime.Now,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    PostId = model.PostId
                };
                this.context.Comments.Add(comment);
                await this.context.SaveChangesAsync();
                return RedirectToAction("Details", "Post", new { id = model.PostId });
            }
            return RedirectToAction("Details", "Post", new { id = model.PostId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CommentViewModel model)
        {
            var comment = await this.context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new { error = "Comment not found" });
            }

            if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized(new { error = "Unauthorized to edit this comment" });
            }

            comment.Text = model.Text;
            this.context.Entry(comment).State = EntityState.Modified;

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound(new { error = "Comment not found" });
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await this.context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new { error = "Comment not found" });
            }

            if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized(new { error = "Unauthorized to delete this comment" });
            }

            this.context.Comments.Remove(comment);
            await this.context.SaveChangesAsync();

            return RedirectToAction("Details", "Post", new { id = comment.PostId });
        }
    }
}
