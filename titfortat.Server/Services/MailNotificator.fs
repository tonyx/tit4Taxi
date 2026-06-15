namespace TitForTat.Infrastructure.Services

open Mailjet.Client;
open Mailjet.Client.TransactionalEmails;
open Mailjet.Client.TransactionalEmails.Response;
open titfortat.MessagesScheduler;
open TitForTat.Server.MailQueueNotification;
open TitForTat.Shared.Infrastructure.Services;
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

type MailNotificator(
    configuration: IConfiguration,
    mailjetClient: MailjetClient,
    logger: ILogger<MailNotificator>,
    mailResenderService: IMailResenderService
) =

    new (configuration: IConfiguration, mailResenderService: IMailResenderService, logger: ILogger<MailNotificator>) =

        let mailjetApiKey = configuration.GetSection("Mailjet").GetValue<string>("ApiKey")
        let mailjetSecretKey = configuration.GetSection("Mailjet").GetValue<string>("SecretKey")
        let mailjetClient = MailjetClient(mailjetApiKey, mailjetSecretKey)
        MailNotificator(configuration, mailjetClient, logger, mailResenderService)

    member this.SendEmailAsync(emailFrom: string, nameFrom: string, emaiRecipient: string, subject: string, content: string) =
        let isEmailSendEnabled =
            configuration.GetSection("EmailSettings").GetValue<bool>("EmailNotificationEnabled", true)
        if isEmailSendEnabled then
            task  
                {
                    let email = 
                        TransactionalEmailBuilder()
                            .WithFrom(SendContact(emailFrom, nameFrom))
                            .WithTo(SendContact(emaiRecipient))
                            .WithSubject(subject)
                            .WithHtmlPart(content)
                            .Build()
                    try 
                        let! response = mailjetClient.SendTransactionalEmailAsync(email)

                        if (response = null) then
                            logger.LogWarning("Failed to send email to {EmailRecipient}", emaiRecipient)
                            let! emailQueueItemAddedResult =
                                mailResenderService.AddMailQueueItemAsync
                                    (
                                        MailQueueItem.New(email)
                                    )
                            match emailQueueItemAddedResult with 
                                | Ok _ -> 
                                    logger.LogInformation("Email added to queue")
                                | Error ex ->
                                    logger.LogError(ex, "Failed to add email to queue")
                                    ()
                        with
                            ex -> 
                                logger.LogError(ex, "Error sending email")
                                let! emailQueueItemAddedResult =
                                    mailResenderService.AddMailQueueItemAsync
                                        (
                                            MailQueueItem.New(email)
                                        )
                                match emailQueueItemAddedResult with 
                                | Ok _ -> 
                                    logger.LogInformation("Email added to queue")
                                | Error ex ->
                                    logger.LogError(ex, "Failed to add email to queue")
                                    ()
                }
        else
            task {
                return ()
            }

    interface IMailNotificator with
        member this.SendEmailAsync(emailFrom: string, nameFrom: string, emaiRecipient: string, subject: string, content: string) = 
            this.SendEmailAsync(emailFrom, nameFrom, emaiRecipient, subject, content)