namespace TitForTat.Shared.Services

open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices
open TitForTat.Shared.Commons
open TitForTat.Domain.User

type IUserService =
    abstract member SetAppUserInfoUnsafe: userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member SetAppUserInfoIfEmpty: context: UserContext * userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member CreateUser: context: UserContext * userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member CreateUserUnsafe: userId:UserId * appUserInfo:AppUserInfo * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member CreateUsersUnsafe: userIds:seq<UserId * AppUserInfo> * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<unit, string>>
    abstract member GetAllUsers: context: UserContext * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<List<User>, string>>
    abstract member GetUsers: context: UserContext * List<UserId> * [<Optional; DefaultParameterValue(null)>] ?ct:CancellationToken -> Task<Result<List<User>, string>>