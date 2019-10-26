using System;
using System.Net;
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
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
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
        public async Task ServerIP()
        {
            String externalIP = new WebClient().DownloadString("https://ipinfo.io/ip");
            if (externalIP != null)
            {
                await ReplyAsync($"O ip do servidor é: {externalIP}");
            }
            else
            {
                await ReplyAsync("IP não encontrado.");
            }
        }
        [Command("start")]
        public async Task Start()
        {
            await MineServer.StartServer();
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
                var messages = await this.Context.Channel.GetMessagesAsync(100).FlattenAsync();
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
