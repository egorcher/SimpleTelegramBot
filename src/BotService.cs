using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegrammBott
{
    public class BotService : IHostedService
    {
        private readonly IEnumerable<ICommand> commands;
        private readonly TelegramBotClient telegramBotClient;

        public BotService(IEnumerable<ICommand> commands, TelegramBotClient telegramBotClient)
        {
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            telegramBotClient.OnMessage += BotClient_OnMessage;
            if (!telegramBotClient.IsReceiving)
            {
                telegramBotClient.StartReceiving();
            }
            
            return Task.CompletedTask;
        }

        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            telegramBotClient.StopReceiving();
            return Task.CompletedTask;
        }

        private void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var command = commands.FirstOrDefault(c => c.Contains(e.Message));

            if (command == null)
                return;

            command.Execute(e.Message);
        }

    }
}
