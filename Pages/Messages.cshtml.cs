using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using miniReddit.Models;
using System.Security.Claims;

namespace miniReddit.Pages
{
    public class MessagesModel(APIManager.UserManager userManager, APIManager.MessageManager messManager) : PageModel
    {
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.MessageManager _messManager = messManager;
        public List<Models.Message> Messages { get; set; }
        public List<MessageModel> MessageModels { get; set; }
        public List<Models.UserInfo> Senders { get; set; }
        public List<Models.UserInfo> Users { get; set; }
        [BindProperty]
        public string SelectedUserId { get; set; } = string.Empty;
        [BindProperty]
        public string MessageText { get; set; } = string.Empty;
        [BindProperty]
        public string ReplyId { get; set; } = string.Empty;
        public async Task OnGet(string? userid)
        {
            Console.WriteLine("Messeges körs");
            await MarkAsRead();
            Messages = await GetRecivedMessages();
            Senders = await GetSenders(Messages);
            MessageModels = await CreateModels(Messages, userid);
            Users = await GetUsers();
            Console.WriteLine("Users: " + Users.Count);
            foreach(var user in Users)
            {
                Console.WriteLine("User names:" + user.Username);
            }
        }
        
        public async Task<RedirectToPageResult> OnPostCreateMessageAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new Models.Message { SendId = userid, ResiveId = SelectedUserId, Text = MessageText };
            try
            {
                await _messManager.SendMessage(message);
                Console.WriteLine("Message sent to " + message.ResiveId + " wrote: " + message.Text);
                return RedirectToPage();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); ModelState.AddModelError("", "Failed to send message"); }
            return RedirectToPage();
        }

        public async Task<RedirectToPageResult> OnPostReplyAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new Models.Message { SendId = userid, ResiveId = ReplyId, Text = MessageText };
            try
            {
                await _messManager.SendMessage(message);
                Console.WriteLine("Message sent to " + message.ResiveId + " wrote: " + message.Text);
                return RedirectToPage();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); ModelState.AddModelError("", "Failed to send message"); }
            return RedirectToPage();

        }

        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> OnPostAnyNewMessages()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new JsonResult(false);

            Console.WriteLine($"UserId: {userId ?? "NULL"}");
            if (await _messManager.HasUnread(userId))
                return new JsonResult(true);
            else
                return new JsonResult(false);
                
        }

        public async Task MarkAsRead()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userid == null) return;

            await _messManager.MarkAsRead(userid);
        }

        public async Task<List<UserInfo>> GetUsers()
        {
            return await _userManager.GetAllUserinfo();
        }
        public async Task<List<Models.UserInfo>> GetSenders(List<Models.Message> messages)
        {
            var userInfos = new List<Models.UserInfo>();
            foreach (var message in messages)
            {
                var userInfo = await _userManager.GetUserinfo(message.SendId);
                userInfos.Add(userInfo);
            }
            return userInfos;
        }
        public async Task<List<Models.Message>> GetRecivedMessages()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var messages = await _messManager.GetMessages(userid);
            return messages;
        }
        public async Task<List<MessageModel>> CreateModels(List<Models.Message> Allmessages, string? userid)
        {
            var messages = Allmessages.ToList();
            if (userid != null)
            {
                messages = Allmessages.Where(m => m.SendId == userid).ToList();
            }

            var messModels = new List<MessageModel>();
            foreach (var message in messages)
            {
                var userInfo = await _userManager.GetUserinfo(message.SendId);
                var messModel = new MessageModel { Message = message, UserInfo = userInfo };
                messModels.Add(messModel);
            }
            return messModels;
        }
        public class MessageModel
        {
            public Models.UserInfo UserInfo { get; set; }
            public Models.Message Message { get; set; }
        }
    }
}
