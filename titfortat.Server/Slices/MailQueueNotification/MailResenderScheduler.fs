namespace TitForTat.Server.Slices.MailQueueNotification

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open TitForTat.Server.MailQueueNotification

type MailResenderScheduler(scopeFactory: IServiceScopeFactory, configuration: IConfiguration) =
    inherit BackgroundService()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        let resendEmailTimeBoxMinMinutes = configuration.GetValue<int>("MailQueueNotification:ResendEmailTimeBoxMinutes", 10)
        task {
            use timer = new PeriodicTimer(TimeSpan.FromMinutes(resendEmailTimeBoxMinMinutes |> int64))
            while! (timer.WaitForNextTickAsync stoppingToken)
                do
                    use scope = scopeFactory.CreateScope()
                    let mailResenderService = scope.ServiceProvider.GetRequiredService<IMailResenderService>()
                    let! _ = mailResenderService.ReSendPendingItemsAsync stoppingToken
                    ()
        }