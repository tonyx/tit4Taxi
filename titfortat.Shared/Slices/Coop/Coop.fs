
module TitForTat.Domain.Coop
open TitForTat.Shared.Commons
open System.Text.Json
open Sharpino

type Coop =
    {
        CoopId: CoopId
        Name: string
        Managers: List<UserId>
        Members: List<UserId>
        CenterCoordinate: Option<Coordinate>
    }
    static member New (name: string) = { Name = name; CoopId = CoopId.New(); Managers = []; Members = []; CenterCoordinate = None }

    member this.AddManager (userId: UserId) = 
        { this with Managers = userId :: this.Managers  |> List.distinct } |> Ok
    
    member this.RemoveManager (userId: UserId) = 
        { this with Managers = this.Managers |> List.filter (fun id -> id <> userId) } |> Ok

    member this.AddMember (userId: UserId) = 
        { this with Members = userId :: this.Members |> List.distinct } |> Ok
    
    member this.RemoveMember (userId: UserId) = 
        { this 
            with 
                Members = this.Members |> List.filter (fun id -> id <> userId) 
                Managers = this.Managers |> List.filter (fun id -> id <> userId) 
                } |> Ok

    member this.PromoteMember (userId: UserId) = 
        result
            {
                do!
                    this.Members |> List.exists (fun id -> id = userId)
                    |> Result.ofBool "user doesn't exist"
                do!
                    this.Managers |> List.exists (fun id -> id = userId)
                    |> not
                    |> Result.ofBool "user is already a manager"
                return 
                    { this with Managers = userId :: this.Managers  |> List.distinct }
            }
    member this.DemoteMember (userId: UserId) = 
        result
            {
                do!
                    this.Managers |> List.exists (fun id -> id = userId)
                    |> Result.ofBool "user doesn't exist"
                return 
                    { this with Managers = this.Managers |> List.filter (fun id -> id <> userId) }
            }
    

    member this.SetCenterCoordinate (coordinate: Coordinate) = 
        { this with CenterCoordinate = Some coordinate } |> Ok

    member this.IsManager (userId: UserId) =
        this.Managers |> List.exists (fun id -> id = userId)
    member this.IsMember (userId: UserId) =
        this.Members |> List.exists (fun id -> id = userId) || this.Managers |> List.exists (fun id -> id = userId)

    member this.Id = this.CoopId.Value 
    static member StorageName = "_Coop"
    static member SnaphotsInterval = 100
    static member Version = "_01"
    member this.Serialize = (this, jsonOptions) |> JsonSerializer.Serialize


    static member Deserialize (data: string) = 
        try
            (data, jsonOptions) |> JsonSerializer.Deserialize<Coop> |> Ok
        with | ex -> 
            Error ex.Message

        