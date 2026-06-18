namespace TitForTat.Shared.Services

open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices
open TitForTat.Shared.Commons
open TitForTat.Domain.Coop

type ICoopService = 
    abstract member CreateCoop: context: UserContext * name: string * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member GetAllCoops: context: UserContext * ?ct: CancellationToken -> Task<Result<List<Coop>, string>>
    abstract member GetCoopsThatHaveCoordinatesDefined: context: UserContext * ?ct: CancellationToken -> Task<Result<List<Coop>, string>>
    abstract member PromoteMember: context: UserContext * coopId: CoopId * userId: UserId * ?ct: CancellationToken -> Task<Result<unit, string>> 
    abstract member DemoteMember: context: UserContext * coopId: CoopId * userId: UserId * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member AddMember: context: UserContext * coopId: CoopId * userId: UserId * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member RemoveMember: context: UserContext * coopId: CoopId * userId: UserId * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member GetCoop: context: UserContext * coopId: CoopId * ?ct:CancellationToken -> Task<Result<Coop, string>>
    abstract member SetCenterCoordinate: context: UserContext * coopId: CoopId * coordinate: Coordinate * ?ct:CancellationToken -> Task<Result<unit, string>>
    
    