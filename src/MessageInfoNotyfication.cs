namespace TelegrammBott
{
    public class MessageInfoNotyfication: MessageInfo
    {
        public static MessageInfoNotyfication New (MessageInfo messageInfo)
        {
            return new MessageInfoNotyfication
            {
                Id = messageInfo.Id,
                UserName = messageInfo.UserName,
                Text = messageInfo.Text,
                Path = messageInfo.Path,
                DateTime = messageInfo.DateTime
            };
        }
    }
}
