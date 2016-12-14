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

let equipPurchasedItem newItem (inventory,equipment) =
    let equipFunction =
        match newItem with
        | Protection(Helmet(_)) -> genericEquipFunction HelmetFun_
        | Protection(Gloves(_)) -> genericEquipFunction GlovesFun_
        | Protection(Legs(_))  -> genericEquipFunction LegsFun_
        | Protection(Armor(_)) -> genericEquipFunction ArmorFun_
        | Protection(Ring(_)) -> genericEquipFunction RingFun_
        | Protection(Shield(_)) -> genericEquipFunction ShieldFun_
        | Weapon _ -> genericEquipFunction WeaponFun_
        | Consumable HealthPotion -> genericEquipFunction LootFun_
        | Consumable HighHealthPotion -> genericEquipFunction LootFun_
        | Consumable MegaHealthPotion -> genericEquipFunction LootFun_
        | Consumable Elixir -> genericEquipFunction LootFun_
        | Consumable HighElixir -> genericEquipFunction LootFun_
        | Consumable MegaElixir -> genericEquipFunction LootFun_
        | Consumable PhoenixFeather -> genericEquipFunction LootFun_
        | Consumable MedicinalHerb -> genericEquipFunction  LootFun_

    let itemForInventory,newEquipment = equipFunction (Some newItem) equipment
    match itemForInventory with
    | None -> (inventory,newEquipment)
    | Some item ->
        let newInventory = inventory |> addToInventory { Item = item; Count = 1 }
        (newInventory,newEquipment)
