module TitForTat.Shared.Commons

open System
open System.Threading
open System.Threading.Tasks
open FsToolkit.ErrorHandling

open FSharp.SystemTextJson
open System.Text.Json.Serialization

let jsonOptions = JsonFSharpOptions.Default().WithAllowNullFields(true).ToJsonSerializerOptions()

type AggregateViewerAsync2<'A> = Option<CancellationToken> -> Guid -> Task<Result<int * 'A, string>>

type CoopId =
    | CoopId of Id: Guid
    with 
        static member New() = CoopId(Guid.NewGuid())
        member this.Value =
            match this with
            | CoopId id -> id

type LedgerId =
    | LedgerId of Id: Guid
    with 
        static member New() = LedgerId(Guid.NewGuid())
        member this.Value =
            match this with
            | LedgerId id -> id

type LedgerStatus =
    | Active
    | Archived

type UserId =
    | UserId of Id: Guid
    with 
        static member New() = UserId(Guid.NewGuid())
        member this.Value =
            match this with
            | UserId id -> id

type MailQueueItemId =
    | MailQueueItemId of Id: Guid
    with 
        static member New() = MailQueueItemId(Guid.NewGuid())
        member this.Value =
            match this with
            | MailQueueItemId id -> id

type MailQueueId =
    | MailQueueId of Id: Guid
    with 
        static member New() = MailQueueId(Guid.NewGuid())
        member this.Value =
            match this with
            | MailQueueId id -> id


type AppUserInfo =
    {
        UserName: string
        Email: string
        IsEmailConfirmed: bool
        PhoneNumber: string
        IsPhoneNumberConfirmed: bool
        TwoFactorEnabled: bool
    } 
        with 
            static member Empty = 
                { 
                    UserName = ""
                    Email = ""
                    IsEmailConfirmed = false
                    PhoneNumber = ""
                    IsPhoneNumberConfirmed = false
                    TwoFactorEnabled = false
                }
            static member New (userName: string, email: string, phoneNumber: string) =
                { 
                    UserName = userName
                    Email = email
                    IsEmailConfirmed = false
                    PhoneNumber = phoneNumber
                    IsPhoneNumberConfirmed = false
                    TwoFactorEnabled = false
                }
            member this.IsEmpty () = 
                this.Equals(AppUserInfo.Empty)

type Coordinate =
    {
        Latitude: double
        Longitude: double
    }

type Role =
    | Admin
    | Manager
    | Controller

type UserContext =
    | Authenticated of UserId: UserId * Roles: List<Role>
    | Anonymous