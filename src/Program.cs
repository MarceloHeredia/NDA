using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Serilog;
using Microsoft.Extensions.Logging;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;

namespace NDA
{
    class Program
    {
        #region Variables
        private readonly DiscordSocketClient _client;
        // Keep the CommandService and DI container around for use with commands.
        // These two types require you install the Discord.Net.Commands package.
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private static String _logLevel;
        #endregion

        #region Public Methods
        /// <summary>Call the constructor then calls the MainAsync method and waits till it ends (shouldn't end)</summary>
        /// <param name="args">Arguments</param>
        public static void Main(String[] args)
        {
            if (args.Count() != 0)
            {
                _logLevel = args[0];
            }
            Log.Logger = new LoggerConfiguration()
                 .WriteTo.File("logs/nda.log", rollingInterval: RollingInterval.Day)
                 .WriteTo.Console()
                 .CreateLogger();

            new Program().MainAsync().GetAwaiter().GetResult();
        }
        public async Task MainAsync()
        {

            // Centralize the logic for commands into a separate method.
            await InitCommands();

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot,
                                                   Environment.GetEnvironmentVariable("NEG2_TOKEN"));
            await _client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }

        #endregion
        #region Private 
        #region Command Handler
        //creates an instance of CommandHandler and start
        private async Task InitCommands()
        {
            CommandHandler cmdHandler = new CommandHandler(_services);
            await cmdHandler.InstallCommandsAsync();
        }
        #endregion
        #region Logs
        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Conected as -> [] :)");
            return Task.CompletedTask;
        }
        #endregion
        #endregion
        #region Constructor Methods
        /// <summary>Initializes the program</summary>
        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                //what types of log I wanna see.
                LogLevel = LogSeverity.Info,

                // If you or another service needs to do anything with messages
                // (eg. checking Reactions, checking the content of edited/deleted messages),
                // you must set the MessageCacheSize. You may adjust the number as needed.
                //MessageCacheSize = 50,

                // If your platform doesn't have native WebSockets,
                // add Discord.Net.Providers.WS4Net from NuGet,
                // add the `using` at the top, and uncomment this line:
                //WebSocketProvider = WS4NetProvider.Instance
            });
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,

                //there's a few more properties you can set,
                //for example: case-insensitive commands.
                CaseSensitiveCommands = false,
            });

            //Setup the DI container.
            _services = new Services(_commands, _client, _logLevel).BuildServiceProvider();
        }

        #endregion
    }
}
