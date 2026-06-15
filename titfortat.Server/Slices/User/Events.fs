namespace titfortat.Domain
open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.User
open System.Text.Json

type UserEvent = 
    | AppUserInfoSet of AppUserInfo
    | AppUserInfoIfEmptSet of AppUserInfo

    interface Event<User> 
        with
            member this.Process user = 
                match this with
                | AppUserInfoSet appUserInfo -> user.SetAppUserInfo appUserInfo
                | AppUserInfoIfEmptSet appUserInfo -> user.SetAppUserInfoIfEmpty appUserInfo


    static member Deserialize (x: string): Result<UserEvent, string> =
        try
            JsonSerializer.Deserialize<UserEvent> (x, jsonOptions) |> Ok
        with
            | ex -> Error ex.Message
    
    member this.Serialize =
        JsonSerializer.Serialize (this, jsonOptions)        