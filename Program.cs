using Discord;
using Discord.WebSocket;

namespace Linie;

public sealed class Program
{
    private DiscordSocketClient Client = null!;

    public static async Task Main(string[] _) => await new Program().MainAsync();

    public async Task MainAsync()
    {
        try
        {
            var configuration = Configuration.FromFile("Linie.cfg") ?? throw new Exception("Configurations file is missing");

            Client = new(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
            });
            Client.Log += (log) => Logger.WriteAsync(log.Message);
            Client.Ready += ReadyAsync;
            Client.InteractionCreated += HandleInteractionAsync;

            await Client.LoginAsync(TokenType.Bot, configuration.DiscordToken);
            await Client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception exception)
        {
            Logger.Write(exception.Message);
        }
    }

    private async Task ReadyAsync()
    {
        await Logger.WriteAsync($"Connected as {Client.CurrentUser}");
        await Commands.RegisterAsync(Client);
    }

    private Task HandleInteractionAsync(SocketInteraction interaction)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await (interaction.Type switch
                {
                    InteractionType.ApplicationCommand => Commands.HandleAsync(Client, (SocketSlashCommand)interaction),
                    InteractionType.MessageComponent => Components.HandleAsync(Client, (SocketMessageComponent)interaction),
                    InteractionType.ModalSubmit => Modals.HandleAsync(Client, (SocketModal)interaction),
                    _ => Task.CompletedTask
                });
            }
            catch (Exception exception)
            {
                Logger.Write($"Error handling interaction: {exception}");
                try
                {
                    var message = "An error occurred while processing your request.";
                    if (!interaction.HasResponded) await interaction.RespondAsync(message, ephemeral: true);
                    else await interaction.FollowupAsync(message, ephemeral: true);
                }
                catch { /* ignored */ }
            }
        });
        return Task.CompletedTask;
    }
}