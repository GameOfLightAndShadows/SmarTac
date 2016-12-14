module GLSManager.ExperienceSystemManager

open System

open GLSCore.CharacterInformation
open GLSCore.GameElement
open GLSCore.HelperFunctions

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

let canCharacterLevelUp (gc: HumanCharacter) =
    gc.ExperiencePoints >= 125.00

let attributeLevelUpPoints (gc: HumanCharacter) =
    let lvlUpPoints =  Math.Floor (gc.ExperiencePoints % 125.00 ) |> int32
    { gc with LevelUpPoints = gc.LevelUpPoints + lvlUpPoints; ExperiencePoints = 0.00 }
   
let computeExperienceGains (caller: HumanCharacter) (target: BrainCharacter) (action: EngageAction) =
    let actionFactor =
        match action with
        | AttackedTarget -> 1.10
        | EvadeAttacker -> 0.90
        | EliminatedTarget -> 1.25
        | BlockedAttacker  -> 0.75
        | _ -> 0.00

    let tacticalAdvantageFactor =
        if caller.Job.IsNone || target.Job.IsNone then 0.00
        else
            match doesHaveTacticalAdvantage caller.Job.Value target.Job.Value  with
            | true -> 0.05
            | false -> -0.05

    let tiersFactor =
        match target.Rank.Value with
        | Low -> 1.00
        | MidLow -> 1.05
        | Mid -> 1.08
        | High -> 1.12
        | HeroClass -> 1.35

    UnitExperiencePoints * ((actionFactor + tacticalAdvantageFactor) * tiersFactor)

let system = System.create "system" <| Configuration.load ()


type ExperienceState = {
    UpgradedCharacter : HumanCharacter option 
}
with 
    static member Initial = { UpgradedCharacter = None }
let processExperienceGain
    (mailbox: Actor<ExperienceSystemProtocol>) =
    let rec loop (state: ExperienceState) = actor {
        let! message = mailbox.Receive ()
        match message with
        | ComputeGain (c, t, action) ->
            let experiencePoints = computeExperienceGains c t action
            if canCharacterLevelUp c then 
                let betterGameCharacter = attributeLevelUpPoints c
                return! loop { state with UpgradedCharacter = Some betterGameCharacter }
            else 
               return! loop state 

        | ProvideUpgradedCharacterWhenPossible -> 
            match state.UpgradedCharacter with 
            | Some char -> mailbox.Sender() <! ReceiveBetterCharacter char 
            | None -> () 
            return! loop { state with UpgradedCharacter = None }

        | Stop ->  return killActor
    }
    loop ExperienceState.Initial

let experienceSystem = spawn system "Experience System" <| processExperienceGain
