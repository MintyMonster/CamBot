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
using CamBotButHesFullOfDumbShite.PlayerLevelsDatabase;

namespace CamBotButHesFullOfDumbShite.Services
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly ServerConfigEntities _db;
        private readonly PlayerLevelsEntities _pldb;

        public CommandHandler(IServiceProvider services)
        {
            _db = services.GetRequiredService<ServerConfigEntities>();
            _pldb = services.GetRequiredService<PlayerLevelsEntities>();
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

            ulong playersId = context.User.Id;
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

            var getPoints = _pldb.playerLevelsModel.AsEnumerable().Where(a => Convert.ToUInt64(a.playerId) == playersId).FirstOrDefault();
            var points = 1;

            if(getPoints != null)
            {
                var currentPoints = Convert.ToInt32(getPoints.points);
                var newPoints = currentPoints + 1;
                getPoints.points = newPoints.ToString();
            }
            else
            {
                await _pldb.AddAsync(new PlayerLevelsModel
                {
                    playerId = playersId.ToString(),
                    playerUsername = context.User.Username,
                    points = points.ToString()
                });
            }

            await _pldb.SaveChangesAsync();
            
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result) // Add some customisation and logging here
        {
            
            if (!command.IsSpecified)
            {
                Console.WriteLine($"{context.User.Username} => Command failed to execute");
                await _client.GetGuild(797839905539096637).GetTextChannel(819924744228044800).SendMessageAsync($"**[FAILED]{DateTime.Now.ToString("dd/MM/yy | HH/mm")}** {context.User.Username} -> {command.Value.ToString()}");
                var cross = new Emoji("\u274C");
                await context.Message.AddReactionAsync(cross);
                return;
            }

            if (result.IsSuccess)
            {
                Console.WriteLine($"{context.User.Username} => Executed a command");
                await _client.GetGuild(797839905539096637).GetTextChannel(819924744228044800).SendMessageAsync($"**[SUCCESS] {DateTime.Now.ToString("dd/MM/yy | HH/mm")}** {context.User.Username} -> {command.Value.ToString()}");
                var checkMark = new Emoji("\u2705");
                await context.Message.AddReactionAsync(checkMark);
                return;
            }

            await context.Channel.SendFileAsync(@"/home/pi/CamBot/CamBot_Sad.png", ":x: Something went wrong :(");
        }
    }
}
