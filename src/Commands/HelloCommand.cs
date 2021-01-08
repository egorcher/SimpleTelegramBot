using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegrammBott.Commands
{
    public class HelloCommand : ICommand
    {
        private readonly TelegramBotClient telegramBotClient;
        private readonly MessageInfoHub messageInfoHub;
        private const string Name = @"/hello";

        public HelloCommand(TelegramBotClient telegramBotClient, MessageInfoHub messageInfoHub)
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
            var resiveMessage = "Hello I'm ASP.NET Core Bot";
            await telegramBotClient.SendTextMessageAsync(chatId, resiveMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

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
                    Text = new[] { resiveMessage }
                });
        }

        public string GetHelp()
        {
            return "/hello - Бот ответит приветствием";
        }
    }
}
