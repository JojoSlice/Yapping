using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace miniReddit.Pages
{
    public class ChatModel(APIManager.UserManager userManager, APIManager.ChatManager chatManager) : PageModel
    {
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.ChatManager _chat = chatManager;
        public List<Models.GroupChat> GroupChats { get; set; } = new();
        public GroupChatModel? SelectedChat { get; set; }
        public Models.UserInfo UserInfo { get; set; } = new();
        public List<Models.UserInfo> AllUsers { get; set; } = new();

        [BindProperty]
        public string ChatName { get; set; }
        [BindProperty]
        public string AddUserId { get; set; }
        [BindProperty]
        public string RemoveUserId { get; set; }
        [BindProperty]
        public string NewChatMessage { get; set; }

        public async Task OnGet(string? chatId)
        {
            UserInfo = await GetUser();
            if (UserInfo.UserId == string.Empty)
                RedirectToPage("/Index");

            AllUsers = await GetAllUsers();
            GroupChats = await GetChats();

            if(chatId != null)
                SelectedChat = await GetChat(chatId);

        }
        public async Task<RedirectToPageResult> OnPostNewChatMessageAsync(string chatId)
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new Models.ChatMessage
            {
                UserId = userid,
                ChatId =chatId,
                Text = NewChatMessage,
            };
            await _chat.CreateChatMessageAsync(message);

            return RedirectToPage("/Chat", new { chatId });
        }
        public async Task<RedirectToPageResult> OnPostRemoveUserAsync(string chatId)
        {
            await _chat.RemoveUserFromChatAsync(chatId, RemoveUserId);
            return RedirectToPage("/Chat", new { chatId });
        }


        public async Task<RedirectToPageResult> OnPostAddUserAsync(string chatId)
        {
            Console.WriteLine("AddUser  userId: " + AddUserId);

            await _chat.AddUserToChatAsync(chatId, AddUserId);
            return RedirectToPage("/Chat", new { chatId });
        }

        public async Task<RedirectToPageResult> OnPostNewChat()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var newChat = new Models.GroupChat
            {
                Name = ChatName,
                UserIds = {userid,}
            };
            await _chat.CreateGroupChatAsync(newChat);

            return RedirectToPage("/Chat");
        }

        public async Task<List<Models.GroupChat>> GetChats()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var chats = await _chat.GetUserChatsAsync(userid);

            return chats ?? new();
        }

        public async Task<List<Models.UserInfo>> GetAllUsers()
        {
            return await _userManager.GetAllUserinfo();
        }

        public async Task<Models.UserInfo> GetUser()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userid == null)
                return new();

            var user = await _userManager.GetUserinfo(userid);
            if (user != null)
                return user;

            return new();
        }
        public async Task<GroupChatModel> GetChat(string chatId)
        {
            var chat = GroupChats.Where(c => c.Id == chatId).FirstOrDefault();

            if (chat == null)
                return new();

            var users = new List<Models.UserInfo>();

            foreach(var userid in chat.UserIds)
            {
                var user = AllUsers.Where(u => u.UserId == userid).FirstOrDefault();
                if(user !=null)
                    users.Add(user);
            }
            var messages = await _chat.GetChatMessagesAsync(chatId);

            if (messages == null)
                return new();

            var chatMessages = new List<ChatMessageModel>();
            foreach (var message in messages)
            {
                var chatUser = users.Where(u => u.UserId == message.UserId).FirstOrDefault();
                if (chatUser != null)
                {

                    var chatMessageModel = new ChatMessageModel
                    {
                        User = chatUser,
                        ChatMessage = message
                    };
                    chatMessages.Add(chatMessageModel);
                }
            }

            var chatModel = new GroupChatModel
            {
                Chat = chat,
                ChatMessages = chatMessages,
                Users = users,
            };
         
            return chatModel;
        }
        public class GroupChatModel
        {
            public Models.GroupChat Chat { get; set; }
            public List<ChatMessageModel> ChatMessages { get; set; }
            public List<Models.UserInfo> Users { get; set; }
        }
        public class ChatMessageModel
        {
            public Models.UserInfo User { get; set; }
            public Models.ChatMessage ChatMessage { get; set; }
        }
    }
}
