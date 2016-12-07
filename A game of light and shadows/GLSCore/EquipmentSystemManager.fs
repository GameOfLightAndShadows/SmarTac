module GLSManager.EquipmentSystemManager

open System

open GLSCore.HelperFunctions
open GLSCore.CharacterInformation
open GLSManager.Protocol

open Akka
open Akka.Actor
open Akka.FSharp

type EquipmentState = {
    Character : GameCharacter
}
with 
    static member Initial = { Character = GameCharacter.InitialGameCharacter }

let equipmentSystem (mailbox: Actor<EquipmentSystemProtocol>) =
    let rec handleProtocol (state: EquipmentState) = actor { 
        let! message = mailbox.Receive() 
        match message with 
        | UpdateWithCharacter gc -> 
            return! handleProtocol { state with Character = gc }

        | UpdateCharacterWithHelmet helmet -> 
            let gcEquipment = state.Character.Equipment
            let equippedNewHelmet =
                 getHelmetFun gcEquipment
                 |> equipHelmetFun <| gcEquipment
            let gc = { state.Character with Equipment = equippedNewHelmet }           
            return! handleProtocol { state with Character = gc } 

        | UpdateCharacterWithArmor armor -> 
            let gcEquipment = state.Character.Equipment
            let equippedNewArmor = 
                getArmorFun gcEquipment 
                |> equipArmorFun <| gcEquipment 
            let gc = { state.Character with Equipment = equippedNewArmor }
            return! handleProtocol { state with Character = gc }

        | UpdateCharacterWithWeapon weapon -> 
            let gcEquipment = state.Character.Equipment 
            let equippedNewWeapon = 
                getWeaponFun gcEquipment 
                |> equipWeaponFun <| gcEquipment 
            let gc = { state.Character with Equipment = equippedNewWeapon }
            return! handleProtocol { state with Character = gc }

        | UpdateCharacterWithGloves gloves -> 
            let gcEquipment = state.Character.Equipment
            let equippedNewGloves = 
                getGlovesFun gcEquipment 
                |> equipGlovesFun <| gcEquipment 
            let gc = { state.Character with Equipment = equippedNewGloves }
            return! handleProtocol { state with Character = gc }

        | UpdateCharacterWithRing ring -> 
            let gcEquipment = state.Character.Equipment
            let equippedNewRing = 
                getRingFun gcEquipment 
                |> equipRingFun <| gcEquipment 
            let gc = { state.Character with Equipment = equippedNewRing }
            return! handleProtocol { state with Character = gc }
        
        | UpdateCharacterWithPants pants -> 
            let gcEquipment = state.Character.Equipment 
            let equippedNewPants = 
                getLegsFun gcEquipment 
                |> equipLegsFun <| gcEquipment 
            let gc = { state.Character with Equipment = equippedNewPants }
            return! handleProtocol { state with Character = gc }

        | UpdateCharacterWithShield shield -> 
            let gcEquipment = state.Character.Equipment
            let equippedNewShield = 
                getShieldFun gcEquipment 
                |> equipShieldFun <| gcEquipment 
            let gc = { state.Character with Equipment = equippedNewShield }
            return! handleProtocol { state with Character = gc }

    } 
    handleProtocol EquipmentState.Initial
