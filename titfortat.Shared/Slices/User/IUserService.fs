namespace titfortat.Shared.Services

open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices
open FsToolkit.ErrorHandling
open TitForTat.Domain.User
open TitForTat.Shared.Commons
open System

type IUserService =
    abstract member SetAppUserInfoUnsafe: userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member SetAppUserInfoIfEmpty: context: UserContext * userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member CreateUser: context: UserContext * userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member CreateUserUnsafe: userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member CreateUsersUnsafe: userIds:seq<UserId * AppUserInfo> * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>