module TeamInventory

open GLSCore.GameItemsModel
open GLSCore.CharacterInformation

open Akka
open Akka.Actor

let addToInventory newItem inventory =
    let newBag = inventory.Bag |> Array.append [|newItem|]
    { inventory with Bag = newBag }

let playerWantsToAutoEquip newItem =
    // In real game, you'd pop up a yes/no question for the player to click on
    printfn "Do you want to auto-equip your purchase?"
    printfn "Actually, don't bother answering; I'll just assume you said yes."
    true

let equipHelmet newHelm equipment = { equipment with Helmet = newHelm }
let getOldHelmet equipment = equipment.Helmet

let equipArmor newArmor equipment = { equipment with Armor = newArmor } 
let getOldArmor equipment = equipment.Armor 

let equipGloves newGloves equipment = { equipment with Gloves = newGloves }
let getOldGloves equipment = equipment.Gloves

let equipRing newRing equipment = { equipment with Ring = newRing } 
let getOldRing equipment = equipment.Ring 

let equipPants newPants equipment = { equipment with Legs = newPants }
let getOldPants equipment = equipment.Legs

let equipWeapon newWeapon equipment = { equipment with Weapon = newWeapon }
let getOldWeapon equipment = equipment.Weapon

let equipShield newShield equipment = { equipment with Shield = newShield } 
let getOldShield equipment = equipment.Shield 

let equipLoot newLoot equipment = { equipment with Loot = newLoot }
let getOldLoot equipment = equipment.Loot 
                        
let genericEquipFunction (getFunc,equipFunc) newItem equipment =
    let oldItem = equipment |> getFunc
    let newEquipment = equipment |> equipFunc newItem
    match oldItem with
    | None -> (None,newEquipment)
    | Some _ ->
        if playerWantsToAutoEquip newItem then
            (oldItem,newEquipment)
        else
            (newItem,equipment)

let equipPurchasedProtection newItem (inventory,equipment) =
    let equipFunction =
        match newItem with
        | Helmet(_)-> genericEquipFunction (getOldHelmet,equipHelmet)
        | Gloves(_) -> genericEquipFunction (getOldGloves,equipGloves)
        | Legs(_)  -> genericEquipFunction (getOldPants, equipPants)
        | Armor(_) -> genericEquipFunction (getOldArmor, equipArmor)
        | Ring(_) -> genericEquipFunction (getOldRing, equipRing)
        | Shield(_) -> genericEquipFunction (getOldShield, equipShield)

    let itemForInventory,newEquipment = equipFunction (Some (Protection(newItem))) equipment
    match itemForInventory with
    | None -> (inventory,newEquipment)
    | Some item ->
        let newInventory = inventory |> addToInventory { Item = item; Count = 1 }
        (newInventory,newEquipment)

let equipPurchasedWeapon newItem (inventory,equipment) =
    // Only one possible equipFunction for weapons
    let equipFunction = genericEquipFunction (getOldWeapon,equipWeapon)
    let itemForInventory,newEquipment = equipFunction newItem equipment
    match itemForInventory with
    | None -> (inventory,newEquipment)
    | Some item ->
        let newInventory = inventory |> addToInventory { Item = item; Count = 1 }
        (newInventory,newEquipment)

let equipPurchaseLoot newItem (inventory, equipment) = 
    let equipFunction = 
        match newItem with 
        | _ -> genericEquipFunction (getOldLoot, equipLoot)

    let itemForInventory, newEquipment = equipFunction newItem equipment
    match itemForInventory with 
    | None -> (inventory, newEquipment)
    | Some item -> 
        let inventory = inventory |> addToInventory { Item = item; Count = 1 }
        (inventory, newEquipment)

let equipPurchasedItem newItem (inventory,equipment) =
    let equipFunction =
        match newItem with
        | Protection(Helmet(_)) -> genericEquipFunction (getOldHelmet,equipHelmet)
        | Protection(Gloves(_)) -> genericEquipFunction (getOldGloves,equipGloves)
        | Protection(Legs(_))  -> genericEquipFunction (getOldPants, equipPants)
        | Protection(Armor(_)) -> genericEquipFunction (getOldArmor, equipArmor)
        | Protection(Ring(_)) -> genericEquipFunction (getOldRing, equipRing)
        | Protection(Shield(_)) -> genericEquipFunction (getOldShield, equipShield)
        | Weapon _ -> genericEquipFunction (getOldWeapon,equipWeapon)
        | Consumable HealthPotion -> genericEquipFunction (getOldLoot,equipLoot)
        | Consumable HighHealthPotion -> genericEquipFunction (getOldLoot,equipLoot)
        | Consumable MegaHealthPotion -> genericEquipFunction (getOldLoot,equipLoot)
        | Consumable Elixir -> genericEquipFunction (getOldLoot,equipLoot)
        | Consumable HighElixir -> genericEquipFunction (getOldLoot,equipLoot)
        | Consumable MegaElixir -> genericEquipFunction (getOldLoot,equipLoot)
        | Consumable PhoenixFeather -> genericEquipFunction (getOldLoot,equipLoot)
        | Consumable MedicinalHerb -> genericEquipFunction (getOldLoot,equipLoot)

    let itemForInventory,newEquipment = equipFunction (Some newItem) equipment
    match itemForInventory with
    | None -> (inventory,newEquipment)
    | Some item ->
        let newInventory = inventory |> addToInventory { Item = item; Count = 1 }
        (newInventory,newEquipment)
