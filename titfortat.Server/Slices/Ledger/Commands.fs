namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.Ledger

type LedgerCommand = 
    | SpendTokenCommand of CoopId * UserId
    | SetMarket1Value of float
    | SetMarket2Value of float

    interface AggregateCommand<Ledger, LedgerEvent> with
        member this.Execute ledger = 
            match this with
            | SpendTokenCommand (coopId, userId) ->
                ledger.SpendToken(coopId, userId)
                |> Result.map (fun l -> (l, [TokenSpentEvent(coopId, userId)]))
            | SetMarket1Value newFlow ->
                let updated = ledger.SetMarket1Value(newFlow)
                (updated, [Market1ValueSet(newFlow)]) |> Ok
            | SetMarket2Value newFlow ->
                let updated = ledger.SetMarket2Value(newFlow)
                (updated, [Market2ValueSet(newFlow)]) |> Ok
                
        member this.Undoer = None
