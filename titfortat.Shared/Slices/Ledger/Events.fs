namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.Ledger
open System.Text.Json

type LedgerEvent = 
    | TokenSpentEvent of CoopId * UserId
    | TokenSpent of CoopId * UserId * DateTime
    | Market1ValueSet of float
    | Market2ValueSet of float
    | Archived 

    interface Event<Ledger> with
        member this.Process ledger = 
            match this with
            | TokenSpentEvent (coopId, userId) -> ledger.SpendToken(coopId, userId)
            | TokenSpent (coopId, userId, timeStamp) -> ledger.SpendToken(coopId, userId)
            | Market1ValueSet newFlow ->
                ledger.SetMarket1Value(newFlow) |> Ok
            | Market2ValueSet newFlow ->
                ledger.SetMarket2Value(newFlow) |> Ok
            | Archived -> 
                ledger.Archive()

    static member Deserialize (x: string): Result<LedgerEvent, string> =
        try
            JsonSerializer.Deserialize<LedgerEvent> (x, jsonOptions) |> Ok
        with
            | ex -> Error ex.Message
    
    member this.Serialize =
        JsonSerializer.Serialize (this, jsonOptions)
