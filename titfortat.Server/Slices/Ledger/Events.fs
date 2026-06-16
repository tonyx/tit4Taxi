namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.Ledger
open System.Text.Json

type LedgerEvent = 
    | TokenSpentEvent of CoopId * UserId
    | EtimatedFlowFromCoop1TerritoryToCoop2TerritoryAdjusted of float
    | EtimatedFlowFromCoop2TerritoryToCoop1TerritoryAdjusted of float

    interface Event<Ledger> with
        member this.Process ledger = 
            match this with
            | TokenSpentEvent (coopId, userId) -> ledger.SpendToken(coopId, userId)
            | EtimatedFlowFromCoop1TerritoryToCoop2TerritoryAdjusted newFlow ->
                ledger.AdjustEtimatedFlowFromCoop1TerritoryToCoop2Territory(newFlow) |> Ok
            | EtimatedFlowFromCoop2TerritoryToCoop1TerritoryAdjusted newFlow ->
                ledger.AdjustEtimatedFlowFromCoop2TerritoryToCoop1Territory(newFlow) |> Ok

    static member Deserialize (x: string): Result<LedgerEvent, string> =
        try
            JsonSerializer.Deserialize<LedgerEvent> (x, jsonOptions) |> Ok
        with
            | ex -> Error ex.Message
    
    member this.Serialize =
        JsonSerializer.Serialize (this, jsonOptions)
