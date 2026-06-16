namespace TitForTat.Shared
open System.Security.Claims
open System
open TitForTat.Shared.Commons

module ConverterUtils =
    let fromClaimsPrincipal (principal: ClaimsPrincipal) =
        if principal <> null && principal.Identity <> null && principal.Identity.IsAuthenticated then
            let userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
            let userIdValue = 
                match userIdClaim with
                | null -> 
                    // Try alternative claim types common in some providers
                    match principal.FindFirst("sub") with
                    | null -> 
                        match principal.FindFirst(ClaimTypes.Name) with
                        | null -> Guid.Empty.ToString()
                        | c -> c.Value
                    | c -> c.Value
                | c -> c.Value
            
            let guid = 
                match Guid.TryParse(userIdValue) with
                | (true, g) -> g
                | _ -> Guid.Empty
            
            let roles = 
                principal.Claims 
                |> Seq.filter (fun c -> c.Type = ClaimTypes.Role || c.Type = "role" || c.Type = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") 
                |> Seq.choose (fun c -> 
                    match c.Value.ToLowerInvariant() with
                    | "admin" -> Some Admin
                    | "manager" -> Some Manager
                    | "controller" -> Some Controller 
                    | _ -> None)
                |> Seq.toList
            UserContext.Authenticated(UserId(guid), roles)
        else
            UserContext.Anonymous