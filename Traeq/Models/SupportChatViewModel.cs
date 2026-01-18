using System;
using System.Collections.Generic;

namespace Traeq.Models
{
    public class SupportChatMessageViewModel
    {
        public string SenderLabel { get; set; } = "";
        public string SenderType { get; set; } = "";
        public string MessageText { get; set; } = "";
        public DateTime SentAt { get; set; }
    }

    public class SupportChatViewModel
    {
        public int ThreadId { get; set; }

        public string Title { get; set; } = "";
        public string SubTitle { get; set; } = "";
        public bool IsClosed { get; set; }

        public string BackLabel { get; set; } = "Contact Us";
        public string BackController { get; set; } = "Home";
        public string BackAction { get; set; } = "ContactUs";

        public string SendAction { get; set; } = "";
        public string CloseAction { get; set; } = "";

        public List<SupportChatMessageViewModel> Messages { get; set; } = new();
    }
}
