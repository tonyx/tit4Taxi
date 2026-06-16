namespace TitForTat.Services
open System.Threading
open System
open Sharpino
open Sharpino.Cache
open FSharpPlus.Operators
open Sharpino.CommandHandler
open Sharpino.EventBroker
open Microsoft.Extensions.Configuration
open Sharpino.Definitions
open Sharpino.Core
open Sharpino.Storage
open TitForTat.Shared.Commons
open FsToolkit.ErrorHandling
open TitForTat.Domain
open TitForTat.Domain.User
open TitForTat.Shared.Services

type UserService
    (eventStore: IEventStore<string>)  =
        let messageSender = MessageSenders.NoSender
        let userStateViewer = CommandHandler.getAggregateStorageFreshStateViewerAsync<User, UserEvent, string> eventStore
 
        member this.SetAppUserUnsafe (userId: UserId, appUserInfo: AppUserInfo, ?ct: CancellationToken) =
            taskResult {
                let ct = defaultArg ct CancellationToken.None
                let command = UserCommand.SetAppUserInfo appUserInfo
                let! result =
                    runAggregateCommandMdAsync<User, UserEvent, string>
                        userId.Value
                        eventStore
                        messageSender
                        ""
                        command
                        (ct |> Some)
                return result
            }

        member this.SetAppUserInfoIfEmpty (context: UserContext, userId: UserId, appUserInfo: AppUserInfo, ?ct: CancellationToken) =
            taskResult {
                let ct = defaultArg ct CancellationToken.None
                let command = UserCommand.SetAppUserInfoIfEmpty appUserInfo
                let! result =
                    runAggregateCommandMdAsync<User, UserEvent, string>
                        userId.Value
                        eventStore
                        messageSender
                        ""
                        command
                        (ct |> Some)
                return result
            }

        member this.CreateUser (context: UserContext, userId: UserId, appUserInfo: AppUserInfo, ?ct: CancellationToken) =
            taskResult {
                let ct = defaultArg ct CancellationToken.None
                let! user = User.New (userId, appUserInfo) 
                let! result =
                    runInitAsync<User, UserEvent, string>
                        eventStore
                        messageSender
                        user
                        (ct |> Some)
                return result
            }
        member this.CreateUserUnsafe (userId: UserId, appUserInfo: AppUserInfo, ?ct: CancellationToken) =
            taskResult {
                let ct = defaultArg ct CancellationToken.None
                let! user = User.New (userId, appUserInfo) 
                let! result =
                    runInitAsync<User, UserEvent, string>
                        eventStore
                        messageSender
                        user
                        (ct |> Some)
                return result
            }

        member this.CreateUsersUnsafe (users:seq<User>, ?ct: CancellationToken) =
            taskResult {
                let ct = defaultArg ct CancellationToken.None
                let! result =
                    runMultipleInitAsync<User, UserEvent, string>
                        eventStore
                        messageSender
                        (users |> Seq.toArray)
                        (ct |> Some)
                return result
            }
        member this.GetAllUsers (context: UserContext, ?ct: CancellationToken) =
            taskResult {
                let ct = defaultArg ct CancellationToken.None
                let! users = 
                    StateView.getAllAggregateStatesAsync<User, UserEvent, string>
                        eventStore
                        (ct |> Some)
                return users |>> snd
            }

        member this.GetUsers (context: UserContext, userIds:list<UserId>, ?ct: CancellationToken) =
            taskResult {
                let ct = defaultArg ct CancellationToken.None
                let! users = 
                    userIds
                    |> List.traverseTaskResultM
                            (fun userId -> 
                                userStateViewer (ct |> Some) userId.Value)
                return users |>> snd
            }

        interface IUserService with    
            member this.SetAppUserInfoUnsafe (userId: UserId, appUserInfo: AppUserInfo, ct: CancellationToken option): Tasks.Task<Result<unit,string>> = 
                this.SetAppUserUnsafe (userId, appUserInfo, ?ct = ct)
            member this.SetAppUserInfoIfEmpty (context: UserContext, userId: UserId, appUserInfo: AppUserInfo, ?ct: CancellationToken) =
                this.SetAppUserInfoIfEmpty (context, userId, appUserInfo, ?ct = ct)
            member this.CreateUser (context: UserContext, userId: UserId, appUserInfo: AppUserInfo, ?ct: CancellationToken) =
                this.CreateUser (context, userId, appUserInfo, ?ct = ct)  
                
            member this.CreateUsersUnsafe (usersIdAndAppUserInfo:seq<UserId * AppUserInfo>, ?ct: CancellationToken) =
                taskResult
                    {
                        let! users =
                            usersIdAndAppUserInfo 
                            |> Seq.toList
                            |> List.traverseResultM 
                                (fun (userId, appUserInfo) -> User.New(userId, appUserInfo))
                        return! 
                            this.CreateUsersUnsafe (users, ?ct = ct)
                    }
            member this.CreateUserUnsafe (userId: UserId, appUserInfo: AppUserInfo, ?ct: CancellationToken) = 
                this.CreateUserUnsafe (userId, appUserInfo, ?ct = ct) 

            member this.GetAllUsers (context: UserContext, ?ct: CancellationToken) = 
                let ct = defaultArg ct CancellationToken.None
                this.GetAllUsers(context, ct)

            member this.GetUsers (context: UserContext, userIds: List<UserId>, ?ct: CancellationToken) = 
                let ct = defaultArg ct CancellationToken.None
                this.GetUsers(context, userIds, ct)
                
    
            




