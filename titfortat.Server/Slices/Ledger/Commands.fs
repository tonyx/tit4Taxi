namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.Ledger

type LedgerCommand = 
    | SpendTokenCommand of CoopId * UserId
    | AdjustEtimatedFlowFromCoop1TerritoryToCoop2Territory of float
    | AdjustEtimatedFlowFromCoop2TerritoryToCoop1Territory of float

    interface AggregateCommand<Ledger, LedgerEvent> with
        member this.Execute ledger = 
            match this with
            | SpendTokenCommand (coopId, userId) ->
                ledger.SpendToken(coopId, userId)
                |> Result.map (fun l -> (l, [TokenSpentEvent(coopId, userId)]))
            | AdjustEtimatedFlowFromCoop1TerritoryToCoop2Territory newFlow ->
                let updated = ledger.AdjustEtimatedFlowFromCoop1TerritoryToCoop2Territory(newFlow)
                (updated, [EtimatedFlowFromCoop1TerritoryToCoop2TerritoryAdjusted(newFlow)]) |> Ok
            | AdjustEtimatedFlowFromCoop2TerritoryToCoop1Territory newFlow ->
                let updated = ledger.AdjustEtimatedFlowFromCoop2TerritoryToCoop1Territory(newFlow)
                (updated, [EtimatedFlowFromCoop2TerritoryToCoop1TerritoryAdjusted(newFlow)]) |> Ok
                
        member this.Undoer = None
