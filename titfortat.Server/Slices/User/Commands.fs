namespace TitForTat.Domain

open System
open Sharpino.Core
open TitForTat.Shared.Commons
open TitForTat.Domain.User

type UserCommand = 
    | SetAppUserInfo of AppUserInfo
    | SetAppUserInfoIfEmpty of AppUserInfo

    interface AggregateCommand<User, UserEvent> 
        with
            member this.Execute user = 
                match this with
                | SetAppUserInfo appUserInfo ->
                    user.SetAppUserInfo appUserInfo
                    |> Result.map (fun u -> (u, [AppUserInfoSet(appUserInfo)]))
                | SetAppUserInfoIfEmpty appUserInfo ->
                    user.SetAppUserInfoIfEmpty appUserInfo
                    |> Result.map (fun u -> (u, [AppUserInfoIfEmptSet(appUserInfo)]))
                    
            member this.Undoer = None