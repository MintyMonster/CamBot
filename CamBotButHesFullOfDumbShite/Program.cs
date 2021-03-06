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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.AspNetCore.Hosting;
using CamBotButHesFullOfDumbShite.Database;
using CamBotButHesFullOfDumbShite.PlayerLevelsDatabase;

namespace CamBotButHesFullOfDumbShite
{
    class Program
    {
        private DiscordSocketClient _client;
        private IConfiguration _config;
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        

        public async Task MainAsync()
        {

            using(var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                _config = services.GetRequiredService<IConfiguration>();
                _client = client;

                _client.Log += Log;
                _client.Ready += ReadAsync;
                services.GetRequiredService<CommandService>().Log += Log;

                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();
                await services.GetRequiredService<CommandHandler>().InitialiseAsync();
                await client.SetGameAsync("$help");

                await Task.Delay(-1);
            }
        }

        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            _config = _builder.Build();
        }

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddDbContext<ServerConfigEntities>()
                .AddDbContext<PlayerLevelsEntities>();

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
