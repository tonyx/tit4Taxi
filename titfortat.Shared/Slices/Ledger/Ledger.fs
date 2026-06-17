module TitForTat.Domain.Ledger

open TitForTat.Shared.Commons
open System.Text.Json
open Sharpino

type PerUserBalance =
    { 
        UserId: UserId
        TokenSpentCoop1: float
        TokenSpentCoop2: float
    }

type Ledger =
    {
        LedgerId: LedgerId
        Coops: CoopId * CoopId
        Coop1MarketValue: float
        Coop2MarketValue: float
        TokenSpentByCoop1: float
        TokenSpentByCoop2: float
        PerUserBalance: List<PerUserBalance>
    }

    static member New (coop1: CoopId, coop2: CoopId, flow1: float, flow2: float) =
        {
            LedgerId = LedgerId.New()
            Coops = (coop1, coop2)
            Coop1MarketValue = flow1
            Coop2MarketValue = flow2
            TokenSpentByCoop1 = 0.0
            TokenSpentByCoop2 = 0.0
            PerUserBalance = []
        }

        member private this.SpendTokenAmountPerUserCoop1 (userId: UserId) (tokenAmount: float) =
            result
                {
                    let existingBalance = 
                        this.PerUserBalance |> List.tryFind (fun perUserBalance -> perUserBalance.UserId = userId)
                    let newBalance = 
                        match existingBalance with
                            | Some perUserBalance -> { perUserBalance with TokenSpentCoop1 = perUserBalance.TokenSpentCoop1 + tokenAmount }
                            | None -> { UserId = userId; TokenSpentCoop1 = tokenAmount; TokenSpentCoop2 = 0.0 }
                    let otherBalances = this.PerUserBalance |> List.filter (fun perUserBalance -> perUserBalance.UserId <> userId)
                    return 
                        { this with PerUserBalance = newBalance :: otherBalances }
                }

        member private this.SpendTokenAmountPerUserCoop2 (userId: UserId) (tokenAmount: float) =
            result
                {
                    let existingBalance = 
                        this.PerUserBalance |> List.tryFind (fun perUserBalance -> perUserBalance.UserId = userId)
                    let newBalance = 
                        match existingBalance with
                            | Some perUserBalance -> { perUserBalance with TokenSpentCoop2 = perUserBalance.TokenSpentCoop2 + tokenAmount }
                            | None -> { UserId = userId; TokenSpentCoop1 = 0.0; TokenSpentCoop2 = tokenAmount }
                    let otherBalances = this.PerUserBalance |> List.filter (fun perUserBalance -> perUserBalance.UserId <> userId)
                    return 
                        { this with PerUserBalance = newBalance :: otherBalances }
                }

        member this.GetTokenSpentByCoop (coop: CoopId) =
            match coop with
                | coop1 when coop1 = (this.Coops |> fst) -> this.TokenSpentByCoop1 |> Ok
                | coop2 when coop2 = (this.Coops |> snd) -> this.TokenSpentByCoop2 |> Ok
                | _ -> Error "Invalid coop"

        member this.GetTokenSpentByUser (user: UserId) =
            let token = this.PerUserBalance |> List.tryFind (fun perUserBalance -> perUserBalance.UserId = user)
            match token with
                | Some token -> (token.TokenSpentCoop1 + token.TokenSpentCoop2) |> Ok
                | None -> Error "Balance not found"

        member this.GetTokenSpentByUserCoop1 (user: UserId) =
            let token = this.PerUserBalance |> List.tryFind (fun perUserBalance -> perUserBalance.UserId = user)
            match token with
                | Some token -> token.TokenSpentCoop1 |> Ok
                | None -> Error "Balance not found"

        member this.GetTokenSpentByUserCoop2 (user: UserId) =
            let token = this.PerUserBalance |> List.tryFind (fun perUserBalance -> perUserBalance.UserId = user)
            match token with
                | Some token -> token.TokenSpentCoop2 |> Ok
                | None -> Error "Balance not found"

        member this.SetMarket1Value (newFlow: float) =
            { this with Coop1MarketValue = newFlow }

        member this.SetMarket2Value (newFlow: float) =
            { this with Coop2MarketValue = newFlow }
                

        member this.ExchangeRateAppliedToCoop1 () = 
            this.Coop2MarketValue

        member this.ExchangeRateAppliedToCoop2 () = 
            this.Coop1MarketValue

        member this.SpendToken (coop: CoopId, user: UserId) =
            result
                {
                    let! result =
                        match coop with
                            | coop1 when coop1 = (this.Coops |> fst) ->
                                let balance = this.TokenSpentByCoop1 + this.ExchangeRateAppliedToCoop1()
                                { this with TokenSpentByCoop1 = balance } |> Ok

                            | coop2 when coop2 = (this.Coops |> snd) ->
                                let balance = this.TokenSpentByCoop2 + this.ExchangeRateAppliedToCoop2()
                                { this with TokenSpentByCoop2 = balance } |> Ok
                            | _ -> Error "Invalid coop"
                    
                    let! resultAdjustingUserLedger =
                        match coop with
                            | coop1 when coop1 = (this.Coops |> fst) ->
                                let tokenAmount = this.ExchangeRateAppliedToCoop1()
                                result.SpendTokenAmountPerUserCoop1 user tokenAmount
                            | coop2 when coop2 = (this.Coops |> snd) ->
                                let tokenAmount = this.ExchangeRateAppliedToCoop2()
                                result.SpendTokenAmountPerUserCoop2 user tokenAmount
                            | _ -> Error "Invalid coop"
                    return resultAdjustingUserLedger
                }

        member this.Id = this.LedgerId.Value

        static member StorageName = "_Ledger"
        static member SnaphotsInterval = 100
        static member Version = "_01"
        member this.Serialize = (this, jsonOptions) |> JsonSerializer.Serialize

        static member Deserialize (data: string) = 
            try
                (data, jsonOptions) |> JsonSerializer.Deserialize<Ledger> |> Ok
            with | ex -> 
                Error ex.Message