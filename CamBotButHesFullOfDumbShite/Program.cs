using Discord;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.IO;
using CamBotButHesFullOfDumbShite.Modules;
using CamBotButHesFullOfDumbShite.Services;

namespace CamBotButHesFullOfDumbShite
{
    class Program
    {
        public static char prefix = '$';
        private DiscordSocketClient _client;
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var token = "Nzk3ODM5NjExMTMzMDM0NTAx.X_sUCg.sNOeOqoETjpHhO0SGtuXTEkrGj8";

            using(var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                _client.Log += Log;
                _client.Ready += ReadAsync;
                services.GetRequiredService<CommandService>().Log += Log;

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                await services.GetRequiredService<CommandHandler>().InitialiseAsync();
                await client.SetGameAsync("$help");

                await Task.Delay(-1);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());

            return Task.CompletedTask;
        }

        private Task ReadAsync()
        {
            Console.WriteLine($"{DateTime.Now} => CamBot and his shite are now online");

            return Task.CompletedTask;
        }
    }
}
