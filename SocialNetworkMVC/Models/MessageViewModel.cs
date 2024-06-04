using NuGet.Protocol.Plugins;
using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkMVC.Models
{
    public class MessageViewModel
    {
        public string? SenderId { get; set; }
        public string? SenderName { get; set; }

        [Required(ErrorMessage = "ReceiverId is required.")]
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
        public bool IsSentByCurrentUser { get; set; }
    }
}

