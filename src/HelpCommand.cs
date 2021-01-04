using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.DependencyInjection;

namespace TelegrammBott
{
    public class HelpCommand : ICommand
    {
        private const string Name = @"/help";
        private readonly TelegramBotClient telegramBotClient;
        private readonly MessageInfoHub messageInfoHub;
        private readonly IServiceProvider serviceProvider;

        public HelpCommand(TelegramBotClient telegramBotClient, MessageInfoHub messageInfoHub, IServiceProvider serviceProvider)
        {
            this.telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
            this.messageInfoHub = messageInfoHub ?? throw new ArgumentNullException(nameof(messageInfoHub));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        public async Task Execute(Message message)
        {
            var commands = serviceProvider.GetServices<ICommand>();
            var chatId = message.Chat.Id;
            var resiveMessage = commands.Select(c=>c.GetHelp()).ToArray();
            await telegramBotClient.SendTextMessageAsync(chatId, string.Join("\n",resiveMessage), parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);

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
                    Text = resiveMessage
                });
        }

        public string GetHelp()
        {
            return Name + " - список доступных комманд";
        }
    }
}
