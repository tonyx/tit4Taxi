module Tests

open Expecto
open TitForTat.Shared.Commons
open TitForTat.Domain.Ledger

[<Tests>]
let tests =
    testList "samples" [
        testCase "universe exists (╭ರᴥ•́)" <| fun _ ->
            let subject = true
            Expect.isTrue subject "I compute, therefore I am."
        
        testCase "can initialize a token" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 1.0, 1.5)
            Expect.equal token.TokenSpentByCoop1 0.0 "balance should be 0"
            Expect.equal token.TokenSpentByCoop2 0.0 "balance should be 0"
            Expect.equal token.Coop1MarketValue 1.0 "flow 1-2 mismatch"

        // using new forumula
        testCase "when the flow from coop1 to coop2 is 2 and the flow from coop2 to coop1 is 1 then the exchange rate applied to coop2 (to carry from territory of coop1) will be 2" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 2.0, 1.0)
            let exchangeRateForCoop2 = token.ExchangeRateAppliedToCoop2()
            Expect.equal exchangeRateForCoop2 2.0 "should be 2"

        testCase "when the flow from coop1 to coop2 is 2 and the flow from coop2 to coop1 is 1 then the exchange rate applied to coop1 (to carry from territory of coop2) will be 1" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 2.0, 1.0)
            let exchangeRateForCoop2 = token.ExchangeRateAppliedToCoop1()
            Expect.equal exchangeRateForCoop2 1.0 "should be 2"

    ]
