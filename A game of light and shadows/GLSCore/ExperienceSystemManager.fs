module GLSManager.ExperienceSystemManager

open System

open GLSCore.CharacterAction
open GLSCore.CharacterInformation
open GLSCore.GameElement
open GLSCore.PartyCharacter

(*
 Determines the action factor which will be combine to 
 the tactical advantage  and the target's tiers.
 The combination is multiplied by the unit's attributed experience points
*)    
[<Literal>]
let UnitExperiencePoints = 10.0

let computeExperienceGains (caller: PartyCharacter) (target: PartyCharacter) (action: EngageAction) = 
    let actionFactor = 
        match action with 
        | AttackedTarget -> 1.10
        | EvadeAttacker -> 0.90
        | EliminatedTarget -> 1.25 
        | BlockedAttacker  -> 0.75
        | _ -> 0.00


    let tacticalAdvantageFactor = 
        match doesHaveTacticalAdvantage caller.Job target.Job  with 
        | true -> 0.05
        | false -> -0.05    

    let tiersFactor =
        target.Tiers
        |> Option.map(fun t ->
            match t with 
            | Low -> 1.00
            | MidLow -> 1.05
            | Mid -> 1.08
            | High -> 1.12
            | HeroClass -> 1.35
        )
    let tiersFactor = 
        match tiersFactor with 
        | None -> 1.00
        | Some v -> v

    UnitExperiencePoints * ((actionFactor + tacticalAdvantageFactor) * tiersFactor)
