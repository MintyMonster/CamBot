using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;
using CamBotButHesFullOfDumbShite.Database; 
using System.Collections.Generic;
using System.Linq;

namespace CamBotButHesFullOfDumbShite.Services
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly ServerConfigEntities _db;

        public CommandHandler(IServiceProvider services)
        {
            _db = services.GetRequiredService<ServerConfigEntities>();
            _commands = services.GetRequiredService<CommandService>(); // grab services for those pre-set
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync; // Handler for when a command is executed

            _client.MessageReceived += MessageReceivedAsync; // handler for when a message is received
        }

        public async Task InitialiseAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services); // Initialise the client
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage.Channel is SocketDMChannel) return;

            var message = rawMessage as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            ulong guildId = context.Guild.Id;
            var argPos = 0;
            char prefix = '$';

            var getGuildId = _db.serverConfigModel.AsEnumerable().Where(a => Convert.ToUInt64(a.guildid) == guildId).FirstOrDefault();

            if (getGuildId != null)
            {
                prefix = char.Parse(getGuildId.prefix);
            }
            else
            {
                await _db.AddAsync(new ServerConfigModel
                {
                    guildid = guildId.ToString(),
                    prefix = "$",
                    color = "purple"
                });

                await _db.SaveChangesAsync();
                prefix = '$';
            }

            if (!(rawMessage is SocketUserMessage))
            {
                return;
            }

            

            if (message.Source != MessageSource.User)
            {
                return;
            }

            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }

            
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result) // Add some customisation and logging here
        {
            
            if (!command.IsSpecified)
            {
                Console.WriteLine($"{context.User.Username} => Command failed to execute");
                var cross = new Emoji("\u274C");
                await context.Message.AddReactionAsync(cross);
                return;
            }

            if (result.IsSuccess)
            {
                Console.WriteLine($"{context.User.Username} => Executed a command");
                var checkMark = new Emoji("\u2705");
                await context.Message.AddReactionAsync(checkMark);
                return;
            }

            await context.Channel.SendFileAsync(@"/home/pi/CamBot/CamBot_Sad.png", ":x: Something went wrong :(");
        }
    }
}
