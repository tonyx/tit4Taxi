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
open TitForTat.Domain.Ledger
open TitForTat.Shared.Services

type LedgerService (eventStore: IEventStore<string>) =
    let messageSenders = MessageSenders.NoSender

    member this.CreateLedger (context: UserContext, coop1: CoopId, coop2: CoopId, flow1: float, flow2: float, ?ct: CancellationToken) =
        let ct = defaultArg ct CancellationToken.None
        taskResult {
            let ledger = Ledger.New (coop1, coop2, flow1, flow2)
            let! createLedger =
                runInitAsync<Ledger, LedgerEvent, string>
                    eventStore
                    messageSenders
                    ledger
                    (ct |> Some)
            return createLedger
        }

    member this.SpendToken (context: UserContext, ledgerId: LedgerId, coopId: CoopId, userId: UserId, ?ct: CancellationToken) =
        let ct = defaultArg ct CancellationToken.None
        taskResult {
            let! (_, ledger) =
                StateView.getAggregateFreshStateAsync<Ledger, LedgerEvent, string>
                    ledgerId.Value
                    eventStore
                    (ct |> Some)
            
            let spendTokenCmd = SpendToken (coopId, userId, DateTime.Now)
            let! result = 
                runAggregateCommandMdAsync<Ledger, LedgerEvent, string>
                    ledgerId.Value
                    eventStore
                    messageSenders
                    ""
                    spendTokenCmd
                    (ct |> Some)
            return result
        }

    member this.GetLedger (context: UserContext, ledgerId: LedgerId, ?ct: CancellationToken) =
        let ct = defaultArg ct CancellationToken.None
        taskResult {
            let! (_, ledger) =
                StateView.getAggregateFreshStateAsync<Ledger, LedgerEvent, string>
                    ledgerId.Value
                    eventStore
                    (ct |> Some)
            return ledger
        }

    member this.GetActiveLedgers (context: UserContext, ?ct: CancellationToken) =
        let ct = defaultArg ct CancellationToken.None
        taskResult {
            let! result =
                StateView.getAllFilteredAggregateStatesAsync<Ledger, LedgerEvent, string>
                    (fun (l: Ledger) -> l.LedgerStatus = LedgerStatus.Active)
                    eventStore
                    (ct |> Some)
                    |> TaskResult.map (fun list -> list |> List.map snd)
                    
            return result
        }

    member this.SetMarket1Value (context: UserContext, ledgerId: LedgerId, flow1: float, ?ct: CancellationToken) =
        let ct = defaultArg ct CancellationToken.None
        taskResult {
            let cmd = SetMarket1Value flow1
            let! result = 
                runAggregateCommandMdAsync<Ledger, LedgerEvent, string>
                    ledgerId.Value
                    eventStore
                    messageSenders
                    ""
                    cmd
                    (ct |> Some)
            return result
        }

    member this.SetMarket2Value (context: UserContext, ledgerId: LedgerId, flow2: float, ?ct: CancellationToken) =
        let ct = defaultArg ct CancellationToken.None
        taskResult {
            let cmd = SetMarket2Value flow2
            let! result = 
                runAggregateCommandMdAsync<Ledger, LedgerEvent, string>
                    ledgerId.Value
                    eventStore
                    messageSenders
                    ""
                    cmd
                    (ct |> Some)
            return result
        }
    member this.ArchiveLedger (context: UserContext, ledgerId: LedgerId, ?ct: CancellationToken) = 
        let ct = defaultArg ct CancellationToken.None
        taskResult {
            let cmd = Archive
            let! result = 
                runAggregateCommandMdAsync<Ledger, LedgerEvent, string>
                    ledgerId.Value
                    eventStore
                    messageSenders
                    ""
                    cmd
                    (ct |> Some)
            return result
        }

    interface ILedgerService with
        member this.CreateLedger (context, coop1, coop2, flow1, flow2, ?ct) =
            this.CreateLedger (context, coop1, coop2, flow1, flow2, ?ct = ct)
        member this.SpendToken (context, ledgerId, coopId, userId, ?ct) =
            this.SpendToken (context, ledgerId, coopId, userId, ?ct = ct)
        member this.GetLedger (context, ledgerId, ?ct) =
            this.GetLedger (context, ledgerId, ?ct = ct)
        member this.GetActiveLedgers (context, ?ct) =
            this.GetActiveLedgers (context, ?ct = ct)
        member this.SetMarket1Value (context, ledgerId, flow1, ?ct) =
            this.SetMarket1Value (context, ledgerId, flow1, ?ct = ct)
        member this.SetMarket2Value (context, ledgerId, flow2, ?ct) =
            this.SetMarket2Value (context, ledgerId, flow2, ?ct = ct)
        member this.ArchiveLedger (context, ledgerId, ?ct) =
            this.ArchiveLedger (context, ledgerId, ?ct = ct)
