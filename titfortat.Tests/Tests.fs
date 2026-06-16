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
            Expect.equal token.EstimatedFlowFromCoop1TerritoryToCoop2Territory 1.0 "flow 1-2 mismatch"

        testCase "when the flow from coop1 to coop2 is 150 and the flow from coop2 to coop1 is 100 then the exchange rate applied to coop1 (to carry from territory of coop2) will be 1.5" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 1.5, 1.0)
            let exchangeRateForCoop1 = token.ExchangeRateForCoop1ToLoadOnCoop2Territory()
            Expect.equal exchangeRateForCoop1 1.5 "should be 1.5"

        testCase "when the flow from coop1 to coop2 is 150 and the flow from coop2 to coop1 is 100 then the exchange rate applied to coop2 (to carry from territory of coop1) will be 0.6" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 1.5, 1.0)
            let exchangeRateForCoop2 = token.ExchangeRateForCoop2ToLoadOnCoop1Territory()
            Expect.equal exchangeRateForCoop2 (1.0/1.5) "should be 0.6666666666666666"

        testCase "flow from coop1 to coop2 is 150 and the flow from coop2 to coop1 is 100. Coop1 spend a token and so the balance will change, simplified" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 1.5, 1.0)
            let tokenAfterFirstExpence = token.SpendToken(coop1, UserId.New())
            Expect.isOk tokenAfterFirstExpence "should be ok"
            Expect.equal (tokenAfterFirstExpence.OkValue.TokenSpentByCoop1) ((1.0 / 1.5)) "error"

        testCase "flow from coop1 to coop2 is 150 and the flow from coop2 to coop1 is 100. Coop2 spend a token and so the balance will change, simplified" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 1.5, 1.0)
            let tokenAfterFirstExpence = token.SpendToken(coop2, UserId.New())
            Expect.isOk tokenAfterFirstExpence "should be ok"
            Expect.equal (tokenAfterFirstExpence.OkValue.TokenSpentByCoop2) ((1.5 / 1.0)) "error"

        testCase "equivalent flow check using exchange rates" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 1.5, 1.0)
            let k = 0.7
            
            // Coop1 flow scaled to Coop2 territory
            let flow1Scaled = token.EstimatedFlowFromCoop1TerritoryToCoop2Territory * k * token.ExchangeRateForCoop2ToLoadOnCoop1Territory()
            let expectedFlow2 = token.EstimatedFlowFromCoop2TerritoryToCoop1Territory * k
            Expect.floatClose Accuracy.medium flow1Scaled expectedFlow2 "scaling Coop1 flow by Coop2 rate should equal Coop2 flow"

            // Coop2 flow scaled to Coop1 territory
            let flow2Scaled = token.EstimatedFlowFromCoop2TerritoryToCoop1Territory * k * token.ExchangeRateForCoop1ToLoadOnCoop2Territory()
            let expectedFlow1 = token.EstimatedFlowFromCoop1TerritoryToCoop2Territory * k
            Expect.floatClose Accuracy.medium flow2Scaled expectedFlow1 "scaling Coop2 flow by Coop1 rate should equal Coop1 flow"

        testCase "user token spent is tracked separately per coop" <| fun _ ->
            let coop1 = CoopId.New ()
            let coop2 = CoopId.New ()
            let token = Ledger.New (coop1, coop2, 1.5, 1.0)
            let userId = UserId.New()
            
            // Spend a token in Coop1 territory
            let tokenAfterCoop1Spend = token.SpendToken(coop1, userId)
            Expect.isOk tokenAfterCoop1Spend "should be ok"
            
            // Spend a token in Coop2 territory
            let tokenAfterBothSpends = tokenAfterCoop1Spend.OkValue.SpendToken(coop2, userId)
            Expect.isOk tokenAfterBothSpends "should be ok"
            
            let finalLedger = tokenAfterBothSpends.OkValue
            
            let spentCoop1 = finalLedger.GetTokenSpentByUserCoop1(userId)
            let spentCoop2 = finalLedger.GetTokenSpentByUserCoop2(userId)
            let spentTotal = finalLedger.GetTokenSpentByUser(userId)
            
            Expect.isOk spentCoop1 "should find balance for coop1"
            Expect.isOk spentCoop2 "should find balance for coop2"
            Expect.isOk spentTotal "should find total balance"
            
            Expect.equal spentCoop1.OkValue (1.0 / 1.5) "Coop1 spent amount mismatch"
            Expect.equal spentCoop2.OkValue (1.5 / 1.0) "Coop2 spent amount mismatch"
            Expect.equal spentTotal.OkValue ((1.0 / 1.5) + (1.5 / 1.0)) "Total spent amount mismatch"
    ]
