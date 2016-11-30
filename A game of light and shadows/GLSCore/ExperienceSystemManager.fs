module GLSManager.ExperienceSystemManager

open System

open GLSCore.CharacterAction
open GLSCore.CharacterInformation
open GLSCore.GameElement
open GLSCore.PartyCharacter

open GLSManager.Protocol

open Akka
open Akka.FSharp
(*
 Determines the action factor which will be combine to
 the tactical advantage  and the target's tiers.
 The combination is multiplied by the unit's attributed experience points
*)
[<Literal>]
let UnitExperiencePoints = 10.0

let canCharacterLevelUp (pc: PartyCharacter) =
    pc.ExperiencePoints >= 125.00

let attributeLevelUpPoints (pc: PartyCharacter) =
    let lvlUpPoints =  Math.Floor (pc.ExperiencePoints % 125.00 ) |> int32
    pc.LevelUpPoints <- pc.LevelUpPoints + lvlUpPoints
    pc.ExperiencePoints <- 0.00


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



let system = System.create "system" <| Configuration.load ()

let processExperienceGain
    (mailbox: Actor<ExperienceSystemProtocol>) =
    let rec loop () = actor {
        let! message = mailbox.Receive ()
        match message with
        | ComputeGain (c, t, action) ->
            let experiencePoints = computeExperienceGains c t action
            if canCharacterLevelUp c then attributeLevelUpPoints c
            // return updated party member to the battle sequence manager

        return! loop ()
    }
    loop ()

let commandManagerRef = spawn system "Experience System" <| processExperienceGain
