﻿using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegrammBott
{
    public interface ICommand
    {
        bool Contains(Message message);
        Task Execute(Message message);
        string GetHelp();
    }
}