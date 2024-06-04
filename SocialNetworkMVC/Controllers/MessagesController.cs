using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialNetworkMVC.Data;
using SocialNetworkMVC.Data.Entities;
using SocialNetworkMVC.Hubs;
using SocialNetworkMVC.Models;
using System.Security.Claims;

namespace SocialNetworkMVC.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private SocialNetworkMVCDbContext context;
        public MessagesController(IHubContext<ChatHub> hubContext, SocialNetworkMVCDbContext context)
        {
            _hubContext = hubContext;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var messages = await this.context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var model = messages.Select(m => new MessageViewModel
            {
                SenderId = m.SenderId,
                SenderName = m.Sender?.UserName,
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver?.UserName,
                Content = m.Content,
                Timestamp = m.Timestamp,
                IsSentByCurrentUser = m.SenderId == userId
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(errors);
            }

            try
            {
                var senderId = model.SenderId ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
                var sender = await context.Users.FindAsync(senderId);
                var receiver = await context.Users.FindAsync(model.ReceiverId);

                var message = new Message
                {
                    SenderId = senderId,
                    Sender = sender,
                    ReceiverId = model.ReceiverId,
                    Receiver = receiver,
                    Content = model.Content,
                    Timestamp = DateTime.Now
                };

                this.context.Messages.Add(message);
                await this.context.SaveChangesAsync();

                await _hubContext.Clients.User(model.ReceiverId).SendAsync("ReceiveMessage", sender.UserName, message.Content, message.Timestamp, false);

                return Json(new { redirectToUrl = Url.Action("Index", "Messages") });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }        

        public IActionResult ComposeMessage(string receiverId, string receiverName)
        {
            var model = new MessageViewModel
            {
                ReceiverId = receiverId,
                ReceiverName = receiverName
            };
            return View(model);
        }

        public async Task<string> GetUserNameByIdAsync(string userId)
        {
            var user = await this.context.Users.FindAsync(userId);
            return user?.UserName;
        }
    }
}
