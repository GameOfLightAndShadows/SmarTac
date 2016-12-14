module GLSManager.EquipmentSystemManager

open System

open GLSCore.HelperFunctions
open GLSCore.GameItemsModel
open GLSCore.CharacterInformation
open GLSManager.Protocol
open GLSManager.InventoryManager

open Akka
open Akka.Actor
open Akka.FSharp

type EquipmentState = {
    Character : HumanCharacter
}
with 
    static member Initial = { Character = HumanCharacter.InitialGameCharacter }

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

        | UpdateCharacterWithLoot loot -> 
            let gcEquipment = state.Character.Equipment
            let equippedNewLoot = 
                getLootFun gcEquipment 
                |> equipLootFun <| gcEquipment 
            let gc = { state.Character with Equipment = equippedNewLoot }
            return! handleProtocol { state with Character = gc }

        | MoveBackHelmetToInventory ->
            let oCharacter = 
                state.Character.Equipment
                |> getHelmetFun
                |> Option.map(fun helmet -> 
                    inventoryManager <! AddSingleItem { Item = Protection(Helmet helmet) ; Count = 1 }
                    let gcEquipment = { state.Character.Equipment with Helmet = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state

        | MoveBackArmorToInventory -> 
            let oCharacter = 
                state.Character.Equipment
                |> getArmorFun
                |> Option.map(fun armor -> 
                    inventoryManager <! AddSingleItem { Item = Protection(Armor armor) ; Count = 1 }
                    let gcEquipment = { state.Character.Equipment with Armor = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state

        | MoveBackGlovesToInventory -> 
            let oCharacter = 
                state.Character.Equipment
                |> getGlovesFun
                |> Option.map(fun g -> 
                    inventoryManager <! AddSingleItem { Item = Protection(Gloves g) ; Count = 1 }
                    let gcEquipment = { state.Character.Equipment with Gloves = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state

        | MoveBackRingToInventory -> 
            let oCharacter = 
                state.Character.Equipment
                |> getRingFun
                |> Option.map(fun r -> 
                    inventoryManager <! AddSingleItem { Item = Protection(Ring r) ; Count = 1 }
                    let gcEquipment = { state.Character.Equipment with Armor = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state

        | MoveBackShieldToInventory -> 
            let oCharacter = 
                state.Character.Equipment
                |> getShieldFun
                |> Option.map(fun shield -> 
                    inventoryManager <! AddSingleItem { Item = Protection(Shield shield) ; Count = 1 }
                    let gcEquipment = { state.Character.Equipment with Shield = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state
            
            
        | MoveBackPantsToInventory -> 
            let oCharacter = 
                state.Character.Equipment
                |> getLegsFun
                |> Option.map(fun l-> 
                    inventoryManager <! AddSingleItem { Item = Protection(Legs l) ; Count = 1 }
                    let gcEquipment = { state.Character.Equipment with Armor = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state

        | MoveBackLootToInventory -> 
            let oCharacter = 
                state.Character.Equipment
                |> getLootFun
                |> Option.map(fun loot-> 
                    inventoryManager <! AddSingleItem { Item = Consumable loot ; Count = 1 }
                    let gcEquipment = { state.Character.Equipment with Armor = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state

        | MoveBackWeaponToInventory -> 
            let oCharacter = 
                state.Character.Equipment
                |> getWeaponFun
                |> Option.map(fun weapon -> 
                    match weapon with 
                    | Dagger d -> inventoryManager <! AddSingleItem { Item = Weapon(Dagger d) ; Count = 1 }
                    | Sword s -> inventoryManager <! AddSingleItem { Item = Weapon(Sword s) ; Count = 1 }
                    | Axe a -> inventoryManager <! AddSingleItem { Item = Weapon(Axe a) ; Count = 1 }
                    | Spear s -> inventoryManager <! AddSingleItem { Item = Weapon(Spear s) ; Count = 1 }
                    | Staff s -> inventoryManager <! AddSingleItem { Item = Weapon(Staff s) ; Count = 1 }
                    | LongBlade lb -> inventoryManager <! AddSingleItem { Item = Weapon(LongBlade lb) ; Count = 1 }
                    | Spellbook sb -> inventoryManager <! AddSingleItem { Item = Weapon(Spellbook sb) ; Count = 1 }

                    let gcEquipment = { state.Character.Equipment with Weapon = None }
                    let gc = { state.Character with Equipment = gcEquipment }
                    gc
                ) 
            match oCharacter with 
            | Some gc -> return! handleProtocol { state with Character = gc }
            | None -> return! handleProtocol state


    } 
    handleProtocol EquipmentState.Initial
