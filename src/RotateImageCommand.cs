using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegrammBott
{
    public class RotateImageCommand : ICommand
    {
        private readonly TelegramBotClient telegramBotClient;
        private readonly MessageInfoHub messageInfoHub;
        private readonly Dictionary<string, RotateFlipType> _rotates = new Dictionary<string, RotateFlipType>
        {
            { @"/rotate_90", RotateFlipType.Rotate90FlipNone },
            { @"/rotate_180" ,RotateFlipType.Rotate180FlipNone },
            { @"/rotate_270" ,RotateFlipType.Rotate270FlipNone }
        };

        public RotateImageCommand(TelegramBotClient telegramBotClient, MessageInfoHub messageInfoHub)
        {
            this.telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
            this.messageInfoHub = messageInfoHub ?? throw new ArgumentNullException(nameof(messageInfoHub));
        }
        public bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            var name = message.Text.Split(" ").First();

            return _rotates.Keys.Contains(name);
        }

        public async Task Execute(Message message)
        {
            var chatId = message.Chat.Id;
            var commandParts = message.Text.Split(" ");

            await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    Id = message.MessageId,
                    UserName = message.From.FirstName,
                    DateTime = message.Date,
                    Text = new[] { message.Text }
                });

            if (commandParts.Length != 2)
            {
                var text = $"Некоректная команда: {message.Text}";
                await telegramBotClient.SendTextMessageAsync(chatId, text) ;

                await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    UserName = "Bot",
                    DateTime = DateTime.Now,
                    Text = new[] { text }
                });

                return;
            }

            var command = _rotates[commandParts[0]];
            var fileName = commandParts[1];
            var path = Path.Combine(Environment.CurrentDirectory, "Photo", fileName);

            if (!System.IO.File.Exists(path))
            {
                var text = $"Файл не найден: {fileName}";

                await telegramBotClient.SendTextMessageAsync(chatId, text);

                await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    UserName = "Bot",
                    DateTime = DateTime.Now,
                    Text = new[] { text }
                });

                return;
            }

            var bitmap = Image.FromFile(path);
            bitmap.RotateFlip(command);
            
            using (var writeStream = new MemoryStream())
            {
                bitmap.Save(writeStream, ImageFormat.Png);
                using(var readStream = new MemoryStream(writeStream.GetBuffer()))
                {
                    var file = new InputOnlineFile(readStream, fileName);
                    await telegramBotClient.SendDocumentAsync(chatId, file);
                }
            }

            await messageInfoHub.SendMessage(
                new MessageInfo
                {
                    UserName = "Bot",
                    DateTime = DateTime.Now,
                    Text = new[] { $"Bot выполнил операцию {commandParts[0]} с файлом: {fileName}" }
                });
        }

        public string GetHelp()
        {
            return "/rotate_90; /rotate_180;/rotate_270 imgName.png - повернуть изображение";
        }
    }
}
