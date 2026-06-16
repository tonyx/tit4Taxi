
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
open TitForTat.Domain.Coop
open TitForTat.Shared.Services
open TitForTat.Domain.User


type CoopService
    (eventStore: IEventStore<string>) =
        let messageSenders = MessageSenders.NoSender

        member this.CreateCoop (context: UserContext, name: string, ?ct: CancellationToken) =
            let ct = defaultArg ct CancellationToken.None
            taskResult {
                let coop = Coop.New name
                let! createCoop =
                    runInitAsync<Coop,CoopEvent, string>
                        eventStore
                        messageSenders
                        coop
                        (ct |> Some)

                return createCoop     
            }
        member this.GetAllCoops (context: UserContext, ?ct: CancellationToken) =
            let ct = defaultArg ct CancellationToken.None
            taskResult
                {
                    let! result =
                        StateView.getAllAggregateStatesAsync<Coop,CoopEvent, string>
                            eventStore
                            (ct |> Some)
                    return result |>> snd     
                }
        member this.PromoteMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) = 
            let ct = defaultArg ct CancellationToken.None
            taskResult
                {
                    let! (_, coop) = 
                        StateView.getAggregateFreshStateAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            (ct |> Some)
                    let! (_, user) =
                        StateView.getAggregateFreshStateAsync<User,UserEvent, string>
                            userId.Value
                            eventStore
                            (ct |> Some)
                    let promoteMember = CoopCommand.PromoteMember userId

                    let! result =   
                        runAggregateCommandMdAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            messageSenders
                            ""
                            promoteMember
                            (ct |> Some)
                    return result        
                }

        member this.DemoteMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) = 
            let ct = defaultArg ct CancellationToken.None
            taskResult
                {
                    let! (_, coop) = 
                        StateView.getAggregateFreshStateAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            (ct |> Some)
                    let! (_, user) =
                        StateView.getAggregateFreshStateAsync<User,UserEvent, string>
                            userId.Value
                            eventStore
                            (ct |> Some)
                    let demoteMember = CoopCommand.DemoteMember userId

                    let! result =   
                        runAggregateCommandMdAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            messageSenders
                            ""
                            demoteMember
                            (ct |> Some)
                    return result        
                }

        member this.AddMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) = 
            let ct = defaultArg ct CancellationToken.None
            taskResult
                {
                    let! (_, coop) = 
                        StateView.getAggregateFreshStateAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            (ct |> Some)
                    let! (_, user) =
                        StateView.getAggregateFreshStateAsync<User,UserEvent, string>
                            userId.Value
                            eventStore
                            (ct |> Some)
                    let addMember = CoopCommand.AddMember userId

                    let! result =   
                        runAggregateCommandMdAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            messageSenders
                            ""
                            addMember
                            (ct |> Some)
                    return result        
                }

        member this.RemoveMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) = 
            let ct = defaultArg ct CancellationToken.None
            taskResult
                {
                    let! (_, coop) = 
                        StateView.getAggregateFreshStateAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            (ct |> Some)
                    let! (_, user) =
                        StateView.getAggregateFreshStateAsync<User,UserEvent, string>
                            userId.Value
                            eventStore
                            (ct |> Some)
                    let removeMember = CoopCommand.RemoveMember userId

                    let! result =   
                        runAggregateCommandMdAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            messageSenders
                            ""
                            removeMember
                            (ct |> Some)
                    return result        
                }

        member this.GetCoop (context: UserContext, coopId: CoopId, ?ct: CancellationToken) = 
            let ct = defaultArg ct CancellationToken.None
            taskResult
                {
                    let! (_, coop) = 
                        StateView.getAggregateFreshStateAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            (ct |> Some)
                    return coop        
                }
        member this.SetCenterCoordinate (context: UserContext, coopId: CoopId, coordinate: Coordinate, ?ct: CancellationToken) = 
            let ct = defaultArg ct CancellationToken.None
            taskResult
                {
                    let! (_, coop) = 
                        StateView.getAggregateFreshStateAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            (ct |> Some)
                    let setCenterCoordinate = CoopCommand.SetCenterCoordinate coordinate

                    let! result =   
                        runAggregateCommandMdAsync<Coop,CoopEvent, string>
                            coopId.Value
                            eventStore
                            messageSenders
                            ""
                            setCenterCoordinate
                            (ct |> Some)
                    return result        
                }

        interface ICoopService with
            member this.CreateCoop (context: UserContext, name: string, ?ct: CancellationToken) =
                this.CreateCoop (context, name, ?ct = ct)
            member this.GetAllCoops (context: UserContext, ?ct: CancellationToken) =
                this.GetAllCoops (context, ?ct = ct)
            member this.PromoteMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) =
                this.PromoteMember (context, coopId, userId, ?ct = ct)
            member this.DemoteMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) =
                this.DemoteMember (context, coopId, userId, ?ct = ct)
            member this.AddMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) =
                this.AddMember (context, coopId, userId, ?ct = ct)
            member this.RemoveMember (context: UserContext, coopId: CoopId, userId: UserId, ?ct: CancellationToken) =
                this.RemoveMember (context, coopId, userId, ?ct = ct)
            member this.GetCoop (context: UserContext, coopId: CoopId, ?ct: CancellationToken) =
                this.GetCoop (context, coopId, ?ct = ct)
            member this.SetCenterCoordinate (context: UserContext, coopId: CoopId, coordinate: Coordinate, ?ct: CancellationToken) =
                this.SetCenterCoordinate (context, coopId, coordinate, ?ct = ct)
            

