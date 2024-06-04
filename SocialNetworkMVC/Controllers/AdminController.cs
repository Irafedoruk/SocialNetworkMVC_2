using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkMVC.Data.Entities;
using System.Data;

namespace SocialNetworkMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public UserManager<User> userManager;
        public AdminController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Users()
        {
            var users = this.userManager.Users;
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            var result = await this.userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Users");
            }
            return BadRequest(result.Errors);
        }
    }
}
