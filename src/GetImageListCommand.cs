using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegrammBott
{
    public class GetImageListCommand : ICommand
    {
        private const string Name = @"/image_list";
        private readonly TelegramBotClient telegramBotClient;
        private readonly MessageInfoHub messageInfoHub;

        public GetImageListCommand(TelegramBotClient telegramBotClient, MessageInfoHub messageInfoHub)
        {
            this.telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
            this.messageInfoHub = messageInfoHub ?? throw new ArgumentNullException(nameof(messageInfoHub));
        }

        public bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        public async Task Execute(Message message)
        {
            var chatId = message.Chat.Id;
            var path = Path.Combine(Environment.CurrentDirectory, "Photo");
            var files = Directory.GetFiles(path).Select(x=>Path.GetFileName(x)).ToArray();

            await telegramBotClient.SendTextMessageAsync(chatId, string.Join("\n",files));

            await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    Id = message.MessageId,
                    UserName = message.From.FirstName,
                    DateTime = message.Date,
                    Text = new[] { message.Text }
                });

            await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    UserName = "Bot",
                    DateTime = DateTime.Now,
                    Text = files
                });
        }

        public string GetHelp()
        {
            return "/image_list - получить список изображений.";
        }
    }
}
