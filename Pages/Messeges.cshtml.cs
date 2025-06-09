using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace miniReddit.Pages
{
    public class MessegesModel(APIManager.UserManager userManager, APIManager.MessageManager messManager) : PageModel
    {
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.MessageManager _messManager = messManager;
        public List<Models.Message> Messages { get; set; }
        public List<MessageModel> MessageModels { get; set; }
        public List<Models.UserInfo> Senders { get; set; }
        public async Task OnGet()
        {
            Messages = await GetRecivedMessages();
            Senders = await GetSenders(Messages);
            MessageModels = await CreateModels(Messages);
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
            return await _messManager.GetMessages(userid);
        }
        public async Task<List<MessageModel>> CreateModels(List<Models.Message> messages)
        {
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
