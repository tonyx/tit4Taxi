
namespace TitForTat.Shared.Services

open System.Threading
open System.Threading.Tasks
open System.Runtime.InteropServices
open TitForTat.Shared.Commons
open TitForTat.Domain.Coop
open TitForTat.Domain.Ledger

type ILedgerService =
    abstract member CreateLedger: context: UserContext * coop1: CoopId * coop2: CoopId * flow1: float * flow2: float * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member GetLedger: context: UserContext * ledgerId: LedgerId * ?ct: CancellationToken -> Task<Result<Ledger, string>>
    abstract member ArchiveLedger: context: UserContext * ledgerId: LedgerId * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member SpendToken: context: UserContext * ledgerId: LedgerId * coop: CoopId * user: UserId * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member GetActiveLedgers: context: UserContext * ?ct: CancellationToken -> Task<Result<List<Ledger>, string>>
    abstract member SetMarket1Value: context: UserContext * ledgerId: LedgerId * flow1: float * ?ct: CancellationToken -> Task<Result<unit, string>>
    abstract member SetMarket2Value: context: UserContext * ledgerId: LedgerId * flow2: float * ?ct: CancellationToken -> Task<Result<unit, string>>
    