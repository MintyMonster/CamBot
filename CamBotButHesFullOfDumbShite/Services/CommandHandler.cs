using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;

namespace CamBotButHesFullOfDumbShite.Services
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
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
            var argPos = 0; 
            char prefix = '$'; 

            if (!(rawMessage is SocketUserMessage message))
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

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result) // Add some customisation and logging here
        {
            
            if (!command.IsSpecified)
            {
                Console.WriteLine($"{context.User.Username} => Command failed to execute");
                return;
            }

            if (result.IsSuccess)
            {
                Console.WriteLine($"{context.User.Username} => Executed a command");
                return;
            }

            await context.Channel.SendFileAsync(@"/home/pi/CamBotButHesFullOfDumbShite/CamBot_Sad.png", "Error, something went wrong :(");
        }
    }
}
