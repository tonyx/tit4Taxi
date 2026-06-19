namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.Coop
open System.Text.Json

type CoopEvent = 
    | ManagerAdded of UserId
    | ManagerRemoved of UserId
    | MemberAdded of UserId
    | MemberRemoved of UserId
    | CenterCoordinateSet of Coordinate
    | MemberPromoted of UserId
    | MemberDemoted of UserId

    interface Event<Coop> with
        member this.Process coop = 
            match this with
            | ManagerAdded userId -> coop.AddManager userId
            | ManagerRemoved userId -> coop.RemoveManager userId
            | MemberAdded userId -> coop.AddMember userId
            | MemberRemoved userId -> coop.RemoveMember userId
            | CenterCoordinateSet coordinate -> coop.SetCenterCoordinate coordinate
            | MemberPromoted userId -> coop.PromoteMember userId
            | MemberDemoted userId -> coop.DemoteMember userId

    static member Deserialize (x: string): Result<CoopEvent, string> =
        try
            JsonSerializer.Deserialize<CoopEvent> (x, jsonOptions) |> Ok
        with
            | ex -> Error ex.Message
    
    member this.Serialize =
        JsonSerializer.Serialize (this, jsonOptions)
