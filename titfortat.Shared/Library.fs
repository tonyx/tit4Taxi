namespace TitForTat.Shared

type SharedResources() = class end

module Localization =
    let SupportedCultures = [| "en"; "it" |]

module Say =
    let hello name =
        printfn "Hello %s" name
