using System;

using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Logging;

namespace NDA
{
    class Services
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly String _logLevel;

        public Services(CommandService commands = null, DiscordSocketClient client = null, String logLevel = null)
        {
            _commands = commands ?? new CommandService();
            _client = client ?? new DiscordSocketClient();
            _logLevel = logLevel;
        }

        public IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection()
                        .AddSingleton(_client)
                        .AddSingleton(_commands)
                         // You can pass in an instance of the desired type

                         //.AddSingleton(new NotificationService())

                         // ...or by using the generic method.
                         //
                         // The benefit of using the generic method is that 
                         // ASP.NET DI will attempt to inject the required
                         // dependencies that are specified under the constructor 
                         // for us.

                         //.AddSingleton<DatabaseService>()
                         .AddSingleton<CommandHandler>()
                        .AddSingleton<LoggingService>()
                        .AddLogging(configure => configure.AddSerilog());

            if (!string.IsNullOrEmpty(_logLevel))
            {
                switch (_logLevel.ToLower())
                {
                    case "info":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                            break;
                        }
                    case "error":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                            break;
                        }
                    case "debug":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                            break;
                        }
                    default:
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                            break;
                        }
                }
            }
            else
            {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
            }

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
