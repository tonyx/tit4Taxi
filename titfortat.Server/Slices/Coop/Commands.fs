namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.Coop

type CoopCommand = 
    | AddManager of UserId
    | RemoveManager of UserId
    | AddMember of UserId
    | RemoveMember of UserId
    | SetCenterCoordinate of Coordinate
    | PromoteMember of UserId
    | DemoteMember of UserId

    interface AggregateCommand<Coop, CoopEvent> with
        member this.Execute coop = 
            match this with
            | AddManager userId ->
                coop.AddManager userId
                |> Result.map (fun c -> (c, [ManagerAdded(userId)]))
            | RemoveManager userId ->
                coop.RemoveManager userId
                |> Result.map (fun c -> (c, [ManagerRemoved(userId)]))
            | AddMember userId ->
                coop.AddMember userId
                |> Result.map (fun c -> (c, [MemberAdded(userId)]))
            | RemoveMember userId ->
                coop.RemoveMember userId
                |> Result.map (fun c -> (c, [MemberRemoved(userId)]))
            | SetCenterCoordinate coordinate ->
                coop.SetCenterCoordinate coordinate
                |> Result.map (fun c -> (c, [CenterCoordinateSet(coordinate)]))
            | PromoteMember userId ->
                coop.PromoteMember userId
                |> Result.map (fun c -> (c, [MemberPromoted(userId)]))
            | DemoteMember userId ->
                coop.DemoteMember userId
                |> Result.map (fun c -> (c, [MemberDemoted(userId)]))
                
        member this.Undoer = None
