namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.Ledger

type LedgerCommand = 
    | SpendTokenCommand of CoopId * UserId
    | SpendToken of CoopId * UserId * DateTime
    | SetMarket1Value of float
    | SetMarket2Value of float
    | Archive

    interface AggregateCommand<Ledger, LedgerEvent> with
        member this.Execute ledger = 
            match this with
            | SpendTokenCommand (coopId, userId) ->
                ledger.SpendToken(coopId, userId)
                |> Result.map (fun l -> (l, [TokenSpentEvent(coopId, userId)]))
            | SpendToken (coopId, userId, timeStamp) ->
                ledger.SpendToken(coopId, userId)
                |> Result.map (fun l -> (l, [TokenSpent(coopId, userId, timeStamp)]))
            | SetMarket1Value newFlow ->
                let updated = ledger.SetMarket1Value(newFlow)
                (updated, [Market1ValueSet(newFlow)]) |> Ok
            | SetMarket2Value newFlow ->
                let updated = ledger.SetMarket2Value(newFlow)
                (updated, [Market2ValueSet(newFlow)]) |> Ok
            | Archive -> 
                ledger.Archive()
                |> Result.map (fun l -> (l, [LedgerEvent.Archived]))
                
        member this.Undoer = None
