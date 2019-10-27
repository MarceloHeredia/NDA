using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NDA.CustomPreconditions;

namespace NDA
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    // Create a module with no prefix
    public class EchoMessage : ModuleBase<SocketCommandContext>
    {
        // ~speak hello world -> hello world
        [Command("speak")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        // ReplyAsync is a method on ModuleBase 
    }

    public class EchoMessage2 : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echoes a message and delete the user's message.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task SayAndDeleteAsync([Remainder][Summary("The text to echo")] string echo)
        {
            await Context.Channel.SendMessageAsync(echo);
            await Context.Message.DeleteAsync();
        }
    }

    [Group("help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("Janela principal de ajuda")]
        public async Task DefaultHelp()
        {
            await ReplyAsync("");
        }

        [Command("commands")]
        [Summary("Lista comandos disponiveis")]
        public async Task ListCommands()
        {
            StringBuilder answer = new StringBuilder();
            answer.AppendLine("&speak <mensagem>  -> bot manda uma mensagem.");
            answer.AppendLine("&say <mensagem> ->  bot manda uma mensagem e apaga a sua.");

            answer.AppendLine("&normie carro pica x celta -> não necessita explicação.");

            answer.AppendLine("&math square -> square 20 = 400");
            answer.AppendLine("&math cube -> cube 3 = 27");

            answer.AppendLine("&mine ip -> ip do servidor");
            answer.AppendLine("&mine start -> inicia o servidor");
            answer.AppendLine("&mine stop -> força interrupção do servidor");


            await ReplyAsync(answer.ToString());
        }
    }

    [Group("normie")]
    public class NormieModule : ModuleBase<SocketCommandContext>
    {
        [Command("carro pica x celta")]
        [Summary("Responde o obvio")]
        public async Task CarroPicaVsCelta()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync("Um carro pica x celta 2012, os dois a 80km, fica lado a lado?");
            await Context.Channel.SendMessageAsync("\\🤔");
            await Task.Delay(2000);
            await Context.Channel.SendMessageAsync("Não fica lado a lado!");
        }
    }

    // Create a module with the 'Math' prefix
    [Group("math")]
    public class MathModule : ModuleBase<SocketCommandContext>
    {
        // ~sample square 20 -> 400
        [Command("square")]
        [Summary("Squares a number.")]
        public async Task SquareAsync(
                    [Summary("The number to square.")]
                    int num)
        {
            double sq = await Calc.Pow(num, 2);
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"{num}^2 = {sq}");
        }

        [Command("cube")]
        [Summary("Cubes a number.")]
        public async Task CubeAsync(
                        [Summary("The number to cube.")] 
                        int num)
        {
            double cub = await Calc.Pow(num, 3);
            await ReplyAsync($"{num}^3 = {cub}");
        }

        [Command("fibonacci")]
        [Summary("Gives the fibonacci number")]
        public async Task FiboAsync(int num)
        {
            double fibo = await Calc.Fibo(num);
            await ReplyAsync($"fibo {num} = {fibo}");
        }
    }

    [Group("info")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        // ~info userinfo --> foxbot#0282
        // ~info userinfo Khionu#8708 --> Khionu#8708
        // ~info userinfo Khionu --> Khionu#8708
        // ~info userinfo @Khionu --> Khionu#8708
        // ~info userinfo 96642168176807936 --> Khionu#8708
        // ~info whois 96642168176807936 --> Khionu#8708
        [Command("userinfo")]
        [Summary
        ("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync(
            [Summary("The (optional) user to get info from")]
            SocketUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }
    }
    [Group("mine")]
    [RequireRoleAttribute("Miner")]
    public class MinerModule : ModuleBase<SocketCommandContext>
    {
        [Command("ip")]
        public async Task ServerIp()
        {
            try
            {
                String externalIp = new WebClient().DownloadString("https://ipinfo.io/ip");
                await ReplyAsync($"O ip do servidor é: {externalIp}");
            }
            catch (Exception ex)
            {
                await ReplyAsync("IP não encontrado." + ex.Message);
            }
        }
        [Command("start")]
        public async Task Start()
        {
            var t = MineServer.StartServer();
            if (!t.Result.IsFaulted)
            {
                await ReplyAsync("O servidor está iniciando.");
            }
            else
            {
                await ReplyAsync(
                    String.Concat("O servidor está em execução, em caso de problema  ",
                                                    "usar o comando 'stop' e depois 'start' novamente. "));
                await ReplyAsync("Não recomendado.");
            }
        }

        [Command("stop")]
        public async Task Stop()
        {
            var t = MineServer.Stop();
            if (!t.Result.IsFaulted)
            {
                await ReplyAsync("O servidor foi encerrado com sucesso.");
            }
            else
            {
                await ReplyAsync("Falha ao encerrar o servidor, talvez ele não estivesse aberto?");
            }
        }
    }

    [Group("admin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    [RequireBotPermission(GuildPermission.Administrator)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Group("clean")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public class CleanModule : ModuleBase<SocketCommandContext>
        {
            // ~admin clean
            [Command("all")]
            public async Task CleanAllAsync()
            {
                var messages = await this.Context.Channel.GetMessagesAsync(500).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            }

            // ~admin clean messages 15
            [Command("messages")]
            public async Task CleanAsync(int count)
            {
                var messages = await this.Context.Channel.GetMessagesAsync(count + 1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            }
        }
        // ~admin ban foxbot#0282
        [Command("ban")]
        public async Task BanAsync(IGuildUser user)
        {
            await Context.Guild.AddBanAsync(user);
            await ReplyAsync($"Adeus, {user.Username}.");
        }
    }
}
