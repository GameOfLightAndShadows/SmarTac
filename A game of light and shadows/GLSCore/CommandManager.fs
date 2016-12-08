module GLSManager.CommandManager

open Akka 
open Akka.Actor
open Akka.FSharp

open GLSCore.HelperFunctions
open GLSCore.GameElement
open GLSCore.GameItemsModel
open GLSCore.CharacterInformation

open GLSManager.Protocol
open GLSManager.ExperienceSystemManager

type CommandManagerState = {
    ActiveMember : HumanCharacter option
    RemainingActionPoint : int option
    AvailableActionInTurn : Act array
    BattleSystem : IActorRef option
}
with 
    static member Initial = 
    {
        ActiveMember = None 
        RemainingActionPoint = None 
        AvailableActionInTurn = [||]
        BattleSystem = None
    }

let reduceWeaponLimit 
    (weapon: Weaponry) = 

    match weapon with 
    | Fist -> weapon 
    | Dagger d ->
        let weaponStats = { d.WeaponStats with HitLimit = d.WeaponStats.HitLimit - 1<hl> }
        let weapon = Dagger ( {d with WeaponStats = weaponStats } )
       
        let weapon = { weapon with Name = "" }
        { Dagger d with Stats = weaponStats }
       

let processCommand
    (mailbox: Actor<CommandManagerProtocol>) =
    let rec loop (state: CommandManagerState) = actor { 
        let! message = mailbox.Receive ()
        match message with 
        | ReceiveActiveHumanCharacter ahc -> 
            let actionPoints = 
                match ahc.CombatStyle with 
                | Some style -> style.actionPoints
                | None -> 0

            return! loop { state with   ActiveMember = Some ahc; 
                                        RemainingActionPoint = Some actionPoints
                                        BattleSystem = Some (mailbox.Sender()) }

        | PerformAttackCommandOn target -> 
            let character = state.ActiveMember |> optionDefaultsToValue <| HumanCharacter.InitialGameCharacter
            let job = character.Job.Value
            let weapon = character.Equipment.Weapon |> optionDefaultsToValue <| Fist
            let weaponStat = weapon.Stats :?> WeaponStat
            let offensiveStrength = computeCharacterOverallOffensive weaponStat.Rank weapon job.Stats
            let lostHealth = (removeUnitFromFloat offensiveStrength)
            let tJob = target.Job.Value
            let tHealth = ((tJob.Stats.Health :> IEnergyPoint).reducePoints lostHealth ) :?> HealthPoint
            let tStats = { tJob.Stats with Health = tHealth }
            let job = findCharacterRoleType tJob (tJob.Role, tStats)
            let target = { target with Job = Some job }
            state.BattleSystem.Value <! UpdateWithInjuredBrainCharacter target
            experienceSystem <! ComputeGain(character, target, AttackedTarget)
            experienceSystem <! ProvideUpgradedCharacterWhenPossible
            
            return! loop state

        | ReceiveBetterCharacter betterCharacter -> 
            state.BattleSystem.Value <! UpdateCharacterWithExperienceIncreased betterCharacter 
            return! loop state

        | 

    }       
    loop CommandManagerState.Initial

let commandManagerRef = spawn system "Command system" <| processCommand 

