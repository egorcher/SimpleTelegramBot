using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TelegrammBott
{
   
    public class MessageInfoHub: Hub
    {
        public async Task SendMessage(MessageInfo messageInfo)
        {
            var notification = MessageInfoNotyfication.New(messageInfo);
            await Clients?.All?.SendAsync("ReceiveMessage", notification);
        }
    }
}
