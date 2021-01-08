using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegrammBott.Commands
{
    public class DownloadImageCommand : ICommand
    {
        private readonly TelegramBotClient telegramBotClient;
        private readonly MessageInfoHub messageInfoHub;

        public DownloadImageCommand(TelegramBotClient telegramBotClient, MessageInfoHub messageInfoHub)
        {
            this.telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
            this.messageInfoHub = messageInfoHub ?? throw new ArgumentNullException(nameof(messageInfoHub));
        }

        public bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Document)
                return false;
                
            var extantions =  Path.GetExtension(message.Document.FileName);
            return extantions == ".png";
        }
        
        public async Task Execute(Message message)
        {
            var document = message.Document;
            var action = $"Home/DownloadFile?fileName={document.FileName}";
            var id = document.FileId;
            var path = Path.Combine(Environment.CurrentDirectory, "Photo", document.FileName);

            await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    Id = message.MessageId,
                    UserName = message.From.FirstName,
                    DateTime = message.Date,
                    Text = new[] { $"Bot сохранил файл {document.FileName}." },
                    Path = action
                });

            if (System.IO.File.Exists(path))
            {
                await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Файл с именем {document.FileName} уже существует");

                await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    UserName = "Bot",
                    DateTime = DateTime.Now,
                    Text = new[] { $"Файл с именем {document.FileName} уже существует" },
                });

                return;
            }

            using (var writeStream = new FileStream(path, FileMode.Create))
            {
                var info = await telegramBotClient.GetInfoAndDownloadFileAsync(id, writeStream);
            }

            await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Файл с именем {document.FileName} загружет на диск");

            await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    UserName = "Bot",
                    DateTime = DateTime.Now,
                    Text = new[] { $"Файл с именем {document.FileName} загружет на диск" }
                });
        }

        public string GetHelp()
        {
            return "Send file - сохранить изображение (только PNG)";
        }
    }
}
