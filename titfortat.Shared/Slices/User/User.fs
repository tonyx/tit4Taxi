module TitForTat.Domain.User
open TitForTat.Shared.Commons
open System.Text.Json

type User =
    {
        UserId: UserId
        AppUserInfo: AppUserInfo
    }
    
    static member New (userId: UserId) = 
        { UserId = userId; AppUserInfo = AppUserInfo.Empty } |> Ok

    static member New (userId: UserId, appUserInfo: AppUserInfo) = 
        match appUserInfo.IsEmpty() with 
        | true -> 
            Error "AppUserInfo cannot be empty"
        | false ->
            { UserId = userId; AppUserInfo = appUserInfo } |> Ok

    member this.SetAppUserInfo userInfo = 
        { this with AppUserInfo = userInfo } |> Ok

    member this.SetAppUserInfoIfEmpty userInfo =
        if this.AppUserInfo.IsEmpty() then
            { this with AppUserInfo = userInfo } |> Ok
        else
            Ok this

    member this.Id = this.UserId.Value
    static member StorageName = "_User"
    static member SnapshotsInterval = 100
    static member Version = "_01"
    member this.Serialize = (this, jsonOptions) |> JsonSerializer.Serialize

    static member Deserialize(data: string) =
        try
            (data, jsonOptions) |> JsonSerializer.Deserialize<User> |> Ok
        with ex ->
            Error ex.Message 